using System;
using System.Reflection;
using TerraFX.ApplicationModel;
using TerraFX.Graphics;
using TerraFX.Numerics;
using static TerraFX.Utilities.InteropUtilities;

using Ptr = System.IntPtr;
using U16 = System.UInt16;
using U32 = System.UInt32;
using I64 = System.Int64;
using I16 = System.Int16;
using F32 = System.Single;
using U8 = System.Byte;
using U64 = System.UInt64;
using I8 = System.Char;
using I32 = System.Int32;
using F64 = System.Double;

namespace TerraFX.Samples.Graphics
{
    public sealed class Vrg : HelloWindow
    {
        private GraphicsPrimitive _quadPrimitive = null!;
        private GraphicsBuffer _constantBuffer = null!;
        private GraphicsBuffer _indexBuffer = null!;
        private GraphicsBuffer _vertexBuffer = null!;
        private float _texturePosition;
        private readonly RenderParams _params;

        public Vrg(string name, params Assembly[] compositionAssemblies)
            : base(name, compositionAssemblies)
        {
            _params = new RenderParams(
                "D:/ds/Navident/ds/cvcv/cipi/ss.cipi.ba.ba.322,322,264.0.5,0.5,0.5.linear",
                "D:/projects/ClaroNav/Vrg/argbLut.CT_Bones_Dental.rgba");
        }

        public override void Cleanup()
        {
            _quadPrimitive?.Dispose();
            _constantBuffer?.Dispose();
            _indexBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            base.Cleanup();
        }

        /// <summary>Initializes the GUI for this sample.</summary>
        /// <param name="application">The hosting <see cref="Application" />.</param>
        /// <param name="timeout">The <see cref="TimeSpan" /> after which this sample should stop running.</param>
        /// <param name="windowLocation">The <see cref="Vector2" /> that defines the initial window location.</param>
        /// <param name="windowSize">The <see cref="Vector2" /> that defines the initial window client rectangle size.</param>
        public override void Initialize(Application application, TimeSpan timeout, Vector2? windowLocation, Vector2? windowSize)
        {
            base.Initialize(application, timeout, windowLocation, windowSize);

            var graphicsDevice = GraphicsDevice;
            var currentGraphicsContext = graphicsDevice.CurrentContext;
            var textureSize = (4 * (U64)256) + (4 * (U64)_params.IntensityTexture3d.Length);

            using var vertexStagingBuffer = graphicsDevice.MemoryAllocator.CreateBuffer(GraphicsBufferKind.Default, GraphicsResourceCpuAccess.CpuToGpu, 64 * 1024);
            using var indexStagingBuffer = graphicsDevice.MemoryAllocator.CreateBuffer(GraphicsBufferKind.Default, GraphicsResourceCpuAccess.CpuToGpu, 64 * 1024);
            using var textureStagingBuffer = graphicsDevice.MemoryAllocator.CreateBuffer(GraphicsBufferKind.Default, GraphicsResourceCpuAccess.CpuToGpu, textureSize);

            _constantBuffer = graphicsDevice.MemoryAllocator.CreateBuffer(GraphicsBufferKind.Constant, GraphicsResourceCpuAccess.CpuToGpu, 64 * 1024);
            _indexBuffer = graphicsDevice.MemoryAllocator.CreateBuffer(GraphicsBufferKind.Index, GraphicsResourceCpuAccess.GpuOnly, 64 * 1024);
            _vertexBuffer = graphicsDevice.MemoryAllocator.CreateBuffer(GraphicsBufferKind.Vertex, GraphicsResourceCpuAccess.GpuOnly, 64 * 1024);

            currentGraphicsContext.BeginFrame();
            _quadPrimitive = CreateQuadPrimitive(currentGraphicsContext, vertexStagingBuffer, indexStagingBuffer, textureStagingBuffer);
            currentGraphicsContext.EndFrame();

            graphicsDevice.Signal(currentGraphicsContext.Fence);
            graphicsDevice.WaitForIdle();
        }

        protected override unsafe void Update(TimeSpan delta)
        {
            var scaleX = (_params.IntensityTexDims[0] - 1f) / _params.IntensityTexDims[0];
            var scaleY = (_params.IntensityTexDims[1] - 1f) / _params.IntensityTexDims[1];
            var scaleZ = (_params.IntensityTexDims[2] - 1f) / _params.IntensityTexDims[2];

            var translationSpeed = 0.1f;

            var dz = _texturePosition;
            {
                dz += (float)(translationSpeed * delta.TotalSeconds);
                dz %= 1.0f;
            }
            _texturePosition = dz;

            var constantBufferRegion = _quadPrimitive.InputResourceRegions[1];
            var constantBuffer = _constantBuffer;
            var pConstantBuffer = constantBuffer.Map<Matrix4x4>(in constantBufferRegion);

            // Shaders take transposed matrices, so we want to set X.W
            pConstantBuffer[0] = new Matrix4x4(
                new Vector4(scaleX, 0.0f, 0.0f, 0f),
                new Vector4(0.0f, scaleY, 0.0f, 0f),
                new Vector4(0.0f, 0.0f, scaleZ, dz),
                new Vector4(0.0f, 0.0f, 0.0f, 1.0f)
            );

            constantBuffer.UnmapAndWrite(in constantBufferRegion);
        }

        protected override void Draw(GraphicsContext graphicsContext)
        {
            graphicsContext.Draw(_quadPrimitive);
            base.Draw(graphicsContext);
        }

        private unsafe GraphicsPrimitive CreateQuadPrimitive(GraphicsContext graphicsContext, GraphicsBuffer vertexStagingBuffer, GraphicsBuffer indexStagingBuffer, GraphicsBuffer textureStagingBuffer)
        {
            var graphicsDevice = GraphicsDevice;
            var graphicsSurface = graphicsDevice.Surface;

            var graphicsPipeline = CreateGraphicsPipeline(graphicsDevice, "Vrg", "main", "main");

            var constantBuffer = _constantBuffer;
            var indexBuffer = _indexBuffer;
            var vertexBuffer = _vertexBuffer;

            var vertexBufferRegion = CreateVertexBufferRegion(graphicsContext, vertexBuffer, vertexStagingBuffer, aspectRatio: graphicsSurface.Width / graphicsSurface.Height);
            graphicsContext.Copy(vertexBuffer, vertexStagingBuffer);

            var indexBufferRegion = CreateIndexBufferRegion(graphicsContext, indexBuffer, indexStagingBuffer);
            graphicsContext.Copy(indexBuffer, indexStagingBuffer);

            var inputResourceRegions = new GraphicsMemoryRegion<GraphicsResource>[4] {
                CreateConstantBufferRegion(graphicsContext, constantBuffer),
                CreateConstantBufferRegion(graphicsContext, constantBuffer),
                CreateTexture1DRegion(graphicsContext, textureStagingBuffer),
                CreateTexture3DRegion(graphicsContext, textureStagingBuffer),
            };

            return graphicsDevice.CreatePrimitive(graphicsPipeline, vertexBufferRegion, SizeOf<Texture3DVertex>(), indexBufferRegion, SizeOf<ushort>(), inputResourceRegions);

            static GraphicsMemoryRegion<GraphicsResource> CreateConstantBufferRegion(GraphicsContext graphicsContext, GraphicsBuffer constantBuffer)
            {
                var constantBufferRegion = constantBuffer.Allocate(SizeOf<Matrix4x4>(), alignment: 256);
                var pConstantBuffer = constantBuffer.Map<Matrix4x4>(in constantBufferRegion);

                pConstantBuffer[0] = Matrix4x4.Identity;

                constantBuffer.UnmapAndWrite(in constantBufferRegion);
                return constantBufferRegion;
            }

            static GraphicsMemoryRegion<GraphicsResource> CreateIndexBufferRegion(GraphicsContext graphicsContext, GraphicsBuffer indexBuffer, GraphicsBuffer indexStagingBuffer)
            {
                var indexBufferRegion = indexBuffer.Allocate(SizeOf<ushort>() * 6, alignment: 2);
                var pIndexBuffer = indexStagingBuffer.Map<ushort>(in indexBufferRegion);

                // clockwise when looking at the triangle from the outside

                pIndexBuffer[0] = 0;
                pIndexBuffer[1] = 1;
                pIndexBuffer[2] = 2;

                pIndexBuffer[3] = 0;
                pIndexBuffer[4] = 2;
                pIndexBuffer[5] = 3;

                indexStagingBuffer.UnmapAndWrite(in indexBufferRegion);
                return indexBufferRegion;
            }

            GraphicsMemoryRegion<GraphicsResource> CreateTexture1DRegion(GraphicsContext graphicsContext, GraphicsBuffer textureStagingBuffer)
            {
                var textureWidth = 256u; //_params.IntensityToRgba.Length;
                var texturePixels = textureWidth;
                var textureSize = texturePixels * 4;

                var texture1D = graphicsContext.Device.MemoryAllocator.CreateTexture(GraphicsTextureKind.OneDimensional, GraphicsResourceCpuAccess.None, textureWidth, texelFormat: TexelFormat.R8G8B8A8_UNORM);
                var texture1DRegion = texture1D.Allocate(texture1D.Size, alignment: 4);
                var pTextureData = textureStagingBuffer.Map<uint>(in texture1DRegion);

                for (uint n = 0; n < texturePixels; n++)
                {
                    var texel = _params.IntensityToRgba[n];
                    pTextureData[n] = texel; // RGBA as least significant byte to most significant byte, i.e. r<<0 ... a<<24
                }
                textureStagingBuffer.UnmapAndWrite(in texture1DRegion);
                graphicsContext.Copy(texture1D, textureStagingBuffer);

                return texture1DRegion;
            }

            GraphicsMemoryRegion<GraphicsResource> CreateTexture3DRegion(GraphicsContext graphicsContext, GraphicsBuffer textureStagingBuffer)
            {
                var texturePixels = (U32)_params.IntensityTexture3d.Length;
                var textureSize = texturePixels * 4;
                var textureWidth = _params.IntensityTexDims[0];
                var textureHeight = _params.IntensityTexDims[1];
                var textureDepth = _params.IntensityTexDims[2];

                var texelFormat = TexelFormat.R8G8B8A8_UNORM;
                var texture3D = graphicsContext.Device.MemoryAllocator.CreateTexture(GraphicsTextureKind.ThreeDimensional, GraphicsResourceCpuAccess.None, textureWidth, textureHeight, (ushort)textureDepth, texelFormat: texelFormat);
                var texture3DRegion = texture3D.Allocate(texture3D.Size, alignment: 4);
                var pTextureData = textureStagingBuffer.Map<uint>(in texture3DRegion);

                for (uint n = 0; n < texturePixels; n++)
                {
                    U32 texel = _params.IntensityTexture3d[n];
                    pTextureData[n] = texel;
                }

                textureStagingBuffer.UnmapAndWrite(in texture3DRegion);
                graphicsContext.Copy(texture3D, textureStagingBuffer);

                return texture3DRegion;
            }

            static GraphicsMemoryRegion<GraphicsResource> CreateVertexBufferRegion(GraphicsContext graphicsContext, GraphicsBuffer vertexBuffer, GraphicsBuffer vertexStagingBuffer, float aspectRatio)
            {
                var vertexBufferRegion = vertexBuffer.Allocate(SizeOf<Texture3DVertex>() * 4, alignment: 16);
                var pVertexBuffer = vertexStagingBuffer.Map<Texture3DVertex>(in vertexBufferRegion);

                var r = 0.99f;
                var a = r / aspectRatio;
                var t = 1f;
                pVertexBuffer[0] = new Texture3DVertex {       //  
                    Position = new Vector3(-a, r, 0.5f),       //   y          Vertex position space: 
                    UVW = new Vector3(0, 0, 0),                //   ^     z    the origin o is
                };                                             //   |   /      in the middle
                                                               //   | /        of the rendered scene
                pVertexBuffer[1] = new Texture3DVertex {       //   o------>x
                    Position = new Vector3(a, r, 0.5f),        //  
                    UVW = new Vector3(t, 0, 0),                //   o------>x  Texture coordinate space:
                };                                             //   | \        the origin o is 
                                                               //   |   \      at the top left corner
                pVertexBuffer[2] = new Texture3DVertex {       //   v     z    and at the beginning of the texture memory
                    Position = new Vector3(a, -r, 0.5f),       //   y          
                    UVW = new Vector3(t, t, 0),                //  
                };                                             //   0 ----- 1  
                                                               //   | \     |  
                pVertexBuffer[3] = new Texture3DVertex {       //   |   \   |  
                    Position = new Vector3(-a, -r, 0.5f),      //   |     \ |  
                    UVW = new Vector3(0, t, 0),                //   3-------2  
                };                                             //

                vertexStagingBuffer.UnmapAndWrite(in vertexBufferRegion);
                return vertexBufferRegion;
            }

            GraphicsPipeline CreateGraphicsPipeline(GraphicsDevice graphicsDevice, string shaderName, string vertexShaderEntryPoint, string pixelShaderEntryPoint)
            {
                var signature = CreateGraphicsPipelineSignature(graphicsDevice);
                var vertexShader = CompileShader(graphicsDevice, GraphicsShaderKind.Vertex, shaderName, vertexShaderEntryPoint);
                var pixelShader = CompileShader(graphicsDevice, GraphicsShaderKind.Pixel, shaderName, pixelShaderEntryPoint);

                return graphicsDevice.CreatePipeline(signature, vertexShader, pixelShader);
            }

            static GraphicsPipelineSignature CreateGraphicsPipelineSignature(GraphicsDevice graphicsDevice)
            {
                var inputs = new GraphicsPipelineInput[1] {
                    new GraphicsPipelineInput(
                        new GraphicsPipelineInputElement[2] {
                            new GraphicsPipelineInputElement(typeof(Vector3), GraphicsPipelineInputElementKind.Position, size: 12),
                            new GraphicsPipelineInputElement(typeof(Vector3), GraphicsPipelineInputElementKind.TextureCoordinate, size: 12),
                        }
                    ),
                };

                var resources = new GraphicsPipelineResource[4] {
                    new GraphicsPipelineResource(GraphicsPipelineResourceKind.ConstantBuffer, GraphicsShaderVisibility.Vertex),
                    new GraphicsPipelineResource(GraphicsPipelineResourceKind.ConstantBuffer, GraphicsShaderVisibility.Vertex),
                    new GraphicsPipelineResource(GraphicsPipelineResourceKind.Texture, GraphicsShaderVisibility.Pixel),
                    new GraphicsPipelineResource(GraphicsPipelineResourceKind.Texture, GraphicsShaderVisibility.Pixel),
                };

                return graphicsDevice.CreatePipelineSignature(inputs, resources);
            }
        }
    }
}
