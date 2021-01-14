// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using TerraFX.Interop;
using TerraFX.Threading;
using TerraFX.Utilities;
using static TerraFX.Interop.Libc;
using static TerraFX.Interop.Xlib;
using static TerraFX.Threading.VolatileState;
using static TerraFX.UI.Providers.Xlib.XlibAtomId;
using static TerraFX.Utilities.AssertionUtilities;
using static TerraFX.Utilities.ExceptionUtilities;
using static TerraFX.Utilities.MathUtilities;
using static TerraFX.Utilities.UnsafeUtilities;

namespace TerraFX.UI.Providers.Xlib
{
    /// <summary>Provides access to an X11 based dispatch subsystem.</summary>
    public sealed unsafe class XlibDispatchProvider : DispatchProvider
    {
        private const uint AtomIdCount = (uint)ATOM_ID_COUNT;

        private static readonly XlibDispatchProvider s_instance = new XlibDispatchProvider();

        private readonly ConcurrentDictionary<Thread, XlibDispatcher> _dispatchers;

        private ValueLazy<nuint[]> _atoms;
        private ValueLazy<nuint> _defaultRootWindow;
        private ValueLazy<Pointer<Screen>> _defaultScreen;
        private ValueLazy<IntPtr> _display;
        private ValueLazy<nuint[]> _supportedAtoms;

        private VolatileState _state;

        private XlibDispatchProvider()
        {
            _dispatchers = new ConcurrentDictionary<Thread, XlibDispatcher>();

            _display = new ValueLazy<IntPtr>(CreateDisplayHandle);
            _defaultRootWindow = new ValueLazy<nuint>(GetDefaultRootWindow);
            _defaultScreen = new ValueLazy<Pointer<Screen>>(GetDefaultScreen);
            _atoms = new ValueLazy<nuint[]>(CreateAtoms);
            _supportedAtoms = new ValueLazy<nuint[]>(GetSupportedAtoms);

            _ = _state.Transition(to: Initialized);
        }

        /// <summary>Finalizes an instance of the <see cref="XlibDispatchProvider" /> class.</summary>
        ~XlibDispatchProvider() => Dispose(isDisposing: false);

        /// <summary>Gets the <see cref="XlibDispatchProvider" /> instance for the current program.</summary>
        public static XlibDispatchProvider Instance => s_instance;

        /// <inheritdoc />
        /// <exception cref="ExternalException">The call to <see cref="clock_gettime(int, timespec*)" /> failed.</exception>
        public override Timestamp CurrentTimestamp
        {
            get
            {
                timespec timespec;
                ThrowExternalExceptionIf(clock_gettime(CLOCK_MONOTONIC, &timespec) != 0, nameof(clock_gettime));

                const long NanosecondsPerSecond = TimeSpan.TicksPerSecond * 100;
                Assert(NanosecondsPerSecond == 1000000000, Resources.ArgumentOutOfRangeExceptionMessage, nameof(NanosecondsPerSecond), NanosecondsPerSecond);

                var ticks = (long)timespec.tv_sec;
                {
                    ticks *= NanosecondsPerSecond;
                    ticks += timespec.tv_nsec;
                    ticks /= 100;
                }
                return new Timestamp(ticks);
            }
        }

        /// <inheritdoc />
        public override XlibDispatcher DispatcherForCurrentThread => GetDispatcher(Thread.CurrentThread);

        /// <summary>Gets the <c>Display</c> that was created for the instance.</summary>
        public IntPtr Display => _display.Value;

        /// <summary>Gets the default root window associated with <see cref="Display" />.</summary>
        public nuint DefaultRootWindow => _defaultRootWindow.Value;

        /// <summary>Gets the default screen associated with <see cref="Display" />.</summary>
        public Screen* DefaultScreen => _defaultScreen.Value;

        internal nuint GetAtom(XlibAtomId id) => _atoms.Value[(nuint)id];

        internal bool GetAtomIsSupported(XlibAtomId id)
        {
            var (supportedAtomIndex, supportedAtomBitIndex) = DivRem((nuint)id, SizeOf<nuint>() * 8);
            return (_supportedAtoms.Value[supportedAtomIndex] & ((nuint)1 << (int)supportedAtomBitIndex)) != 0;
        }

        /// <inheritdoc />
        public override XlibDispatcher GetDispatcher(Thread thread)
        {
            ThrowIfNull(thread, nameof(thread));
            return _dispatchers.GetOrAdd(thread, (parentThread) => new XlibDispatcher(this, parentThread));
        }

        /// <inheritdoc />
        public override bool TryGetDispatcher(Thread thread, [MaybeNullWhen(false)] out Dispatcher dispatcher)
        {
            ThrowIfNull(thread, nameof(thread));
            Unsafe.SkipInit(out dispatcher);
            return _dispatchers.TryGetValue(thread, out Unsafe.As<Dispatcher, XlibDispatcher>(ref dispatcher)!);
        }

        private static IntPtr CreateDisplayHandle()
        {
            var display = XOpenDisplay(null);
            ThrowExternalExceptionIf(display == (nint)0, nameof(XOpenDisplay));

            _ = XSetErrorHandler(&HandleXlibError);
            _ = XSetIOErrorHandler(&HandleXlibIOError);

            return display;
        }

        [UnmanagedCallersOnly]
        private static int HandleXlibError(IntPtr display, XErrorEvent* errorEvent)
        {
            // Due to the asynchronous nature of Xlib, there can be a race between
            // the window being deleted and it being unmapped. This ignores the warning
            // raised by the unmap event in that scenario, as the call to XGetWindowProperty
            // will fail.

            var errorCode = (XlibErrorCode)errorEvent->error_code;
            var requestCode = (XlibRequestCode)errorEvent->request_code;

            if ((errorCode != XlibErrorCode.BadWindow) || (requestCode != XlibRequestCode.GetProperty))
            {
                ThrowExternalException((int)errorCode, requestCode.ToString());
            }

            return 0;
        }

        [UnmanagedCallersOnly]
        private static int HandleXlibIOError(IntPtr display) => 0;

        private nuint[] CreateAtoms()
        {
            var atoms = new nuint[AtomIdCount];

            fixed (nuint* pAtoms = atoms)
            {
                var atomNames = stackalloc sbyte*[(int)AtomIdCount] {
                    (sbyte*)XlibAtomName._NET_ACTIVE_WINDOW.AsPointer(),
                    (sbyte*)XlibAtomName._NET_CLIENT_LIST.AsPointer(),
                    (sbyte*)XlibAtomName._NET_CLIENT_LIST_STACKING.AsPointer(),
                    (sbyte*)XlibAtomName._NET_CLOSE_WINDOW.AsPointer(),
                    (sbyte*)XlibAtomName._NET_CURRENT_DESKTOP.AsPointer(),
                    (sbyte*)XlibAtomName._NET_DESKTOP_GEOMETRY.AsPointer(),
                    (sbyte*)XlibAtomName._NET_DESKTOP_LAYOUT.AsPointer(),
                    (sbyte*)XlibAtomName._NET_DESKTOP_NAMES.AsPointer(),
                    (sbyte*)XlibAtomName._NET_DESKTOP_VIEWPORT.AsPointer(),
                    (sbyte*)XlibAtomName._NET_FRAME_EXTENTS.AsPointer(),
                    (sbyte*)XlibAtomName._NET_MOVERESIZE_WINDOW.AsPointer(),
                    (sbyte*)XlibAtomName._NET_NUMBER_OF_DESKTOPS.AsPointer(),
                    (sbyte*)XlibAtomName._NET_REQUEST_FRAME_EXTENTS.AsPointer(),
                    (sbyte*)XlibAtomName._NET_RESTACK_WINDOW.AsPointer(),
                    (sbyte*)XlibAtomName._NET_SHOWING_DESKTOP.AsPointer(),
                    (sbyte*)XlibAtomName._NET_SUPPORTED.AsPointer(),
                    (sbyte*)XlibAtomName._NET_SUPPORTING_WM_CHECK.AsPointer(),
                    (sbyte*)XlibAtomName._NET_VIRTUAL_ROOTS.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_ABOVE.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_BELOW.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_CHANGE_DESKTOP.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_CLOSE.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_FULLSCREEN.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_MAXIMIZE_HORZ.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_MAXIMIZE_VERT.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_MINIMIZE.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_MOVE.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_RESIZE.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_SHADE.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ACTION_STICK.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ALLOWED_ACTIONS.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_BYPASS_COMPOSITOR.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_DESKTOP.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_FULL_PLACEMENT.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_FULLSCREEN_MONITORS.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_HANDLED_ICONS.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ICON.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ICON_GEOMETRY.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_ICON_NAME.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_MOVERESIZE.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_NAME.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_OPAQUE_REGION.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_PID.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_PING.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_ABOVE.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_BELOW.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_DEMANDS_ATTENTION.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_FOCUSED.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_FULLSCREEN.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_HIDDEN.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_MAXIMIZED_HORZ.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_MAXIMIZED_VERT.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_MODAL.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_SHADED.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_SKIP_PAGER.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_SKIP_TASKBAR.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STATE_STICKY.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STRUT.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_STRUT_PARTIAL.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_SYNC_REQUEST.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_USER_TIME.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_USER_TIME_WINDOW.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_VISIBLE_NAME.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_VISIBLE_ICON_NAME.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_COMBO.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_DESKTOP.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_DIALOG.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_DND.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_DOCK.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_DROPDOWN_MENU.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_MENU.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_NORMAL.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_NOTIFICATION.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_POPUP_MENU.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_SPLASH.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_TOOLBAR.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_TOOLTIP.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WM_WINDOW_TYPE_UTILITY.AsPointer(),
                    (sbyte*)XlibAtomName._NET_WORKAREA.AsPointer(),
                    (sbyte*)XlibAtomName._TERRAFX_CREATE_WINDOW.AsPointer(),
                    (sbyte*)XlibAtomName._TERRAFX_DISPOSE_WINDOW.AsPointer(),
                    (sbyte*)XlibAtomName._TERRAFX_NATIVE_INT.AsPointer(),
                    (sbyte*)XlibAtomName._TERRAFX_WINDOWPROVIDER.AsPointer(),
                    (sbyte*)XlibAtomName.UTF8_STRING.AsPointer(),
                    (sbyte*)XlibAtomName.WM_DELETE_WINDOW.AsPointer(),
                    (sbyte*)XlibAtomName.WM_PROTOCOLS.AsPointer(),
                    (sbyte*)XlibAtomName.WM_STATE.AsPointer(),
                };

                var status = XInternAtoms(
                    Display,
                    atomNames,
                    (int)AtomIdCount,
                    False,
                    pAtoms
                );

                ThrowExternalExceptionIf(status == 0, nameof(XInternAtoms));
            }

            return atoms;
        }

        private nuint GetDefaultRootWindow() => XDefaultRootWindow(Display);

        private Pointer<Screen> GetDefaultScreen() => XDefaultScreenOfDisplay(Display);

        private nuint[] GetSupportedAtoms()
        {
            var supportedAtoms = new nuint[DivideRoundingUp(AtomIdCount, SizeOf<nuint>() * 8)];

            nuint actualType;
            int actualFormat;
            nuint itemCount;
            nuint bytesRemaining;
            nuint* pSupportedAtoms;

            _ = XGetWindowProperty(
                Display,
                DefaultRootWindow,
                GetAtom(_NET_SUPPORTED),
                0,
                nint.MaxValue,
                False,
                XA_ATOM,
                &actualType,
                &actualFormat,
                &itemCount,
                &bytesRemaining,
                (byte**)&pSupportedAtoms
            );

            if ((actualType == XA_ATOM) && (actualFormat == 32) && (bytesRemaining == 0))
            {
                for (nuint i = 0; i < itemCount; i++)
                {
                    var supportedAtom = pSupportedAtoms[i];

                    for (nuint n = 0; n < AtomIdCount; n++)
                    {
                        if (_atoms.Value[n] != supportedAtom)
                        {
                            continue;
                        }

                        var (supportedAtomIndex, supportedAtomBitIndex) = DivRem(n, SizeOf<nuint>() * 8);
                        supportedAtoms[supportedAtomIndex] |= (nuint)1 << (int)supportedAtomBitIndex;
                        break;
                    }
                }
            }

            return supportedAtoms;
        }

        /// <inheritdoc />
        protected override void Dispose(bool isDisposing)
        {
            var priorState = _state.BeginDispose();

            if (priorState < Disposing)
            {
                DisposeDisplay();
            }

            _state.EndDispose();
        }

        private void DisposeDisplay()
        {
            _state.AssertDisposing();

            if (_display.IsCreated)
            {
                _ = XSetIOErrorHandler(null);
                _ = XSetErrorHandler(null);
                _ = XCloseDisplay(_display.Value);
            }
        }
    }
}
