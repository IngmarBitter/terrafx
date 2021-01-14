// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using TerraFX.Interop;
using TerraFX.Threading;
using TerraFX.Utilities;
using static TerraFX.Interop.Vulkan;
using static TerraFX.Threading.VolatileState;
using static TerraFX.Utilities.AssertionUtilities;
using static TerraFX.Utilities.MarshalUtilities;

namespace TerraFX.Graphics.Providers.Vulkan
{
    /// <inheritdoc />
    public sealed unsafe class VulkanGraphicsAdapter : GraphicsAdapter
    {
        private readonly VkPhysicalDevice _vulkanPhysicalDevice;

        private ValueLazy<VkPhysicalDeviceProperties> _vulkanPhysicalDeviceProperties;
        private ValueLazy<VkPhysicalDeviceMemoryProperties> _vulkanPhysicalDeviceMemoryProperties;
        private ValueLazy<string> _name;

        private VolatileState _state;

        internal VulkanGraphicsAdapter(VulkanGraphicsProvider provider, VkPhysicalDevice vulkanPhysicalDevice)
            : base(provider)
        {
            AssertNotNull(vulkanPhysicalDevice, nameof(vulkanPhysicalDevice));

            _vulkanPhysicalDevice = vulkanPhysicalDevice;

            _vulkanPhysicalDeviceMemoryProperties = new ValueLazy<VkPhysicalDeviceMemoryProperties>(GetVulkanPhysicalDeviceMemoryProperties);
            _vulkanPhysicalDeviceProperties = new ValueLazy<VkPhysicalDeviceProperties>(GetVulkanPhysicalDeviceProperties);
            _name = new ValueLazy<string>(GetName);

            _ = _state.Transition(to: Initialized);
        }

        /// <inheritdoc />
        public override uint DeviceId => VulkanPhysicalDeviceProperties.deviceID;

        /// <inheritdoc />
        public override string Name => _name.Value;

        /// <inheritdoc cref="GraphicsAdapter.Provider" />
        public new VulkanGraphicsProvider Provider => (VulkanGraphicsProvider)base.Provider;

        /// <inheritdoc />
        public override uint VendorId => VulkanPhysicalDeviceProperties.vendorID;

        /// <summary>Gets the underlying <see cref="VkPhysicalDevice" /> for the adapter.</summary>
        /// <exception cref="ObjectDisposedException">The adapter has been disposed.</exception>
        public VkPhysicalDevice VulkanPhysicalDevice
        {
            get
            {
                _state.ThrowIfDisposedOrDisposing();
                return _vulkanPhysicalDevice;
            }
        }

        /// <summary>Gets the <see cref="VkPhysicalDeviceProperties" /> for <see cref="VulkanPhysicalDevice" />.</summary>
        /// <exception cref="ObjectDisposedException">The adapter has been disposed and the value was not otherwise cached.</exception>
        public ref readonly VkPhysicalDeviceMemoryProperties VulkanPhysicalDeviceMemoryProperties => ref _vulkanPhysicalDeviceMemoryProperties.ValueRef;

        /// <summary>Gets the <see cref="VkPhysicalDeviceProperties" /> for <see cref="VulkanPhysicalDevice" />.</summary>
        /// <exception cref="ObjectDisposedException">The adapter has been disposed and the value was not otherwise cached.</exception>
        public ref readonly VkPhysicalDeviceProperties VulkanPhysicalDeviceProperties => ref _vulkanPhysicalDeviceProperties.ValueRef;

        /// <inheritdoc />
        public override GraphicsDevice CreateDevice(IGraphicsSurface surface, int contextCount) => CreateVulkanGraphicsDevice(surface, contextCount);

        /// <inheritdoc cref="CreateDevice(IGraphicsSurface, int)" />
        public VulkanGraphicsDevice CreateVulkanGraphicsDevice(IGraphicsSurface surface, int contextCount)
        {
            _state.ThrowIfDisposedOrDisposing();
            return new VulkanGraphicsDevice(this, surface, contextCount);
        }

        /// <inheritdoc />
        /// <remarks>While there are no unmanaged resources to cleanup, we still want to mark the instance as disposed if, for example, <see cref="GraphicsAdapter.Provider" /> was disposed.</remarks>
        protected override void Dispose(bool isDisposing)
        {
            _ = _state.BeginDispose();
            _state.EndDispose();
        }

        private string GetName()
        {
            _state.ThrowIfDisposedOrDisposing();
            return MarshalUtf8ToReadOnlySpan(in VulkanPhysicalDeviceProperties.deviceName[0], 256).AsString() ?? string.Empty;
        }

        private VkPhysicalDeviceMemoryProperties GetVulkanPhysicalDeviceMemoryProperties()
        {
            _state.ThrowIfDisposedOrDisposing();

            VkPhysicalDeviceMemoryProperties physicalDeviceMemoryProperties;
            vkGetPhysicalDeviceMemoryProperties(VulkanPhysicalDevice, &physicalDeviceMemoryProperties);
            return physicalDeviceMemoryProperties;
        }

        private VkPhysicalDeviceProperties GetVulkanPhysicalDeviceProperties()
        {
            _state.ThrowIfDisposedOrDisposing();

            VkPhysicalDeviceProperties physicalDeviceProperties;
            vkGetPhysicalDeviceProperties(VulkanPhysicalDevice, &physicalDeviceProperties);
            return physicalDeviceProperties;
        }
    }
}
