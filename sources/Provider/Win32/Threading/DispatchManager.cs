// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Concurrent;
using System.Composition;
using System.Runtime.InteropServices;
using System.Threading;
using TerraFX.Interop;
using TerraFX.Threading;
using static TerraFX.Interop.Kernel32;
using static TerraFX.Interop.Windows;
using static TerraFX.Utilities.ExceptionUtilities;

namespace TerraFX.Provider.Win32.Threading
{
    /// <summary>Provides a means of managing the message dispatch objects for an application.</summary>
    [Export(typeof(IDispatchManager))]
    [Export(typeof(DispatchManager))]
    [Shared]
    public sealed unsafe class DispatchManager : IDispatchManager
    {
        #region Fields
        /// <summary>The tick frequency for the system's monotonic timer.</summary>
        internal readonly double _tickFrequency;

        /// <summary>The <see cref="IDispatcher" /> instances that have been created by the instance.</summary>
        internal readonly ConcurrentDictionary<Thread, IDispatcher> _dispatchers;
        #endregion

        #region Constructors
        /// <summary>Initializes a new instance of the <see cref="DispatchManager" /> class.</summary>
        /// <exception cref="ExternalException">The call to <see cref="QueryPerformanceFrequency(out LARGE_INTEGER)" /> failed.</exception>
        public DispatchManager()
        {
            _tickFrequency = GetTickFrequency();
            _dispatchers = new ConcurrentDictionary<Thread, IDispatcher>();
        }
        #endregion

        #region Static Methods
        /// <summary>Gets the tick frequency for the system's monotonic timer.</summary>
        /// <returns>The tick frequency for the system's monotonic timer.</returns>
        /// <exception cref="ExternalException">The call to <see cref="QueryPerformanceFrequency(out LARGE_INTEGER)" /> failed.</exception>
        internal static double GetTickFrequency()
        {
            var succeeded = QueryPerformanceFrequency(out var frequency);

            if (succeeded == FALSE)
            {
                ThrowExternalExceptionForLastError(nameof(QueryPerformanceFrequency));
            }

            const double ticksPerSecond = Timestamp.TicksPerSecond;
            return (ticksPerSecond / frequency.QuadPart);
        }
        #endregion

        #region TerraFX.Threading.IDispatchManager Properties
        /// <summary>Gets the current <see cref="Timestamp" /> for the instance.</summary>
        /// <exception cref="ExternalException">The call to <see cref="QueryPerformanceCounter(out LARGE_INTEGER)" /> failed.</exception>
        public Timestamp CurrentTimestamp
        {
            get
            {
                var succeeded = QueryPerformanceCounter(out var performanceCount);

                if (succeeded == FALSE)
                {
                    ThrowExternalExceptionForLastError(nameof(QueryPerformanceCounter));
                }

                var ticks = (long)(performanceCount.QuadPart * _tickFrequency);
                return new Timestamp(ticks);
            }
        }

        /// <summary>Gets the <see cref="IDispatcher" /> instance for <see cref="Thread.CurrentThread" />.</summary>
        /// <returns>The <see cref="IDispatcher" /> instance for <see cref="Thread.CurrentThread" />.</returns>
        /// <remarks>This will create a new <see cref="IDispatcher" /> instance if one does not already exist.</remarks>
        public IDispatcher DispatcherForCurrentThread
        {
            get
            {
                return GetDispatcher(Thread.CurrentThread);
            }
        }
        #endregion

        #region TerraFX.Threading.IDispatchManager Methods
        /// <summary>Gets the <see cref="IDispatcher" /> instance associated with a <see cref="Thread" />, creating one if it does not exist.</summary>
        /// <param name="thread">The <see cref="Thread" /> for which the <see cref="IDispatcher" /> instance should be retrieved.</param>
        /// <returns>The <see cref="IDispatcher" /> instance associated with <paramref name="thread" />.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="thread" /> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">A <see cref="IDispatcher" /> instance for <paramref name="thread" /> could not be found.</exception>
        public IDispatcher GetDispatcher(Thread thread)
        {
            if (thread is null)
            {
                ThrowArgumentNullException(nameof(thread));
            }

            return _dispatchers.GetOrAdd(thread, (parentThread) => new Dispatcher(this, parentThread));
        }

        /// <summary>Gets the <see cref="IDispatcher" /> instance associated with a <see cref="Thread" /> or <c>null</c> if one does not exist.</summary>
        /// <param name="thread">The <see cref="Thread" /> for which the <see cref="IDispatcher" /> instance should be retrieved.</param>
        /// <param name="dispatcher">The <see cref="IDispatcher" /> instance associated with <paramref name="thread" />.</param>
        /// <returns><c>true</c> if a <see cref="IDispatcher" /> instance was found for <paramref name="thread" />; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="thread" /> is <c>null</c>.</exception>
        public bool TryGetDispatcher(Thread thread, out IDispatcher dispatcher)
        {
            if (thread is null)
            {
                ThrowArgumentNullException(nameof(thread));
            }

            return _dispatchers.TryGetValue(thread, out dispatcher);
        }
        #endregion
    }
}