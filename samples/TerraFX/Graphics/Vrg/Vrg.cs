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
        private RenderParams _params;

        public Vrg(string name, params Assembly[] compositionAssemblies)
            : base(name, compositionAssemblies)
        {
            _params = new RenderParams("D:/ds/Navident/ds/cvcv/cipi/ss.cipi.ba.ba.161,161,132.1,1,1.linear");
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

            using var vertexStagingBuffer = graphicsDevice.MemoryAllocator.CreateBuffer(GraphicsBufferKind.Default, GraphicsResourceCpuAccess.CpuToGpu, 64 * 1024);
            using var indexStagingBuffer = graphicsDevice.MemoryAllocator.CreateBuffer(GraphicsBufferKind.Default, GraphicsResourceCpuAccess.CpuToGpu, 64 * 1024);
            using var textureStagingBuffer = graphicsDevice.MemoryAllocator.CreateBuffer(GraphicsBufferKind.Default, GraphicsResourceCpuAccess.CpuToGpu, 8 * _params.TexSize);

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
            var scaleX = (_params.TexDims[0] - 1f) / _params.TexDims[0];
            var scaleY = (_params.TexDims[1] - 1f) / _params.TexDims[1];
            var scaleZ = (_params.TexDims[2] - 1f) / _params.TexDims[2];

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

            var inputResourceRegions = new GraphicsMemoryRegion<GraphicsResource>[3] {
                CreateConstantBufferRegion(graphicsContext, constantBuffer),
                CreateConstantBufferRegion(graphicsContext, constantBuffer),
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

            GraphicsMemoryRegion<GraphicsResource> CreateTexture3DRegion(GraphicsContext graphicsContext, GraphicsBuffer textureStagingBuffer)
            {
                var texturePixels = _params.TexSize;
                var textureSize = texturePixels * 4;
                var textureWidth = _params.TexDims[0];
                var textureHeight = _params.TexDims[1];
                var textureDepth = _params.TexDims[2];

                var texture3D = graphicsContext.Device.MemoryAllocator.CreateTexture(GraphicsTextureKind.ThreeDimensional, GraphicsResourceCpuAccess.None, textureWidth, textureHeight, (ushort)textureDepth, texelFormat: TexelFormat.R8G8B8A8_UNORM);
                var texture3DRegion = texture3D.Allocate(texture3D.Size, alignment: 4);
                var pTextureData = textureStagingBuffer.Map<uint>(in texture3DRegion);

                for (uint n = 0; n < texturePixels; n++)
                {
                    var voxel = _params.Texels[n];
                    var value01 = voxel + 1000f;
                    if (value01 < 0)
                    {
                        value01 = 0;
                    }

                    value01 /= 3000;
                    value01 = Sigmoid0To1WithCenterAndWidth(value01, 0.5f, 0.5f);
                    var texel = (U32)(255 * value01);
                    pTextureData[n] = texel;
                }

                textureStagingBuffer.UnmapAndWrite(in texture3DRegion);
                graphicsContext.Copy(texture3D, textureStagingBuffer);

                return texture3DRegion;

                static F32 Sigmoid0To1WithCenterAndWidth(F32 x0to1, F32 center0to1, F32 width0to1)
                {
                    var mappedValue = 1.0f / (1 + MathF.Pow(1.5f / width0to1, -10 * (x0to1 - center0to1)));
                    return mappedValue;
                }
            }

            static GraphicsMemoryRegion<GraphicsResource> CreateVertexBufferRegion(GraphicsContext graphicsContext, GraphicsBuffer vertexBuffer, GraphicsBuffer vertexStagingBuffer, float aspectRatio)
            {
                var vertexBufferRegion = vertexBuffer.Allocate(SizeOf<Texture3DVertex>() * 4, alignment: 16);
                var pVertexBuffer = vertexStagingBuffer.Map<Texture3DVertex>(in vertexBufferRegion);

                var y = 1.0f;
                var x = y / aspectRatio;

                pVertexBuffer[0] = new Texture3DVertex {             //
                    Position = new Vector3(-x, y, 0.0f),             //   y          in this setup
                    UVW = new Vector3(0, 1, 0),                      //   ^     z    the origin o
                };                                                   //   |   /      is in the middle
                                                                     //   | /        of the rendered scene
                pVertexBuffer[1] = new Texture3DVertex {             //   o------>x
                    Position = new Vector3(x, y, 0.0f),              //
                    UVW = new Vector3(1, 1, 0),                      //   0 ----- 1
                };                                                   //   | \     |
                                                                     //   |   \   |
                pVertexBuffer[2] = new Texture3DVertex {             //   |     \ |
                    Position = new Vector3(x, -y, 0.0f),             //   3-------2
                    UVW = new Vector3(1, 0, 0),                      //
                };

                pVertexBuffer[3] = new Texture3DVertex {
                    Position = new Vector3(-x, -y, 0),
                    UVW = new Vector3(0, 0, 0),
                };

                vertexStagingBuffer.UnmapAndWrite(in vertexBufferRegion);
                return vertexBufferRegion;
            }

                //float r = 0.99f;
                //var a = new Texture3DVertex {                  //  
                //    Position = new Vector3(-r, r, 0.5f),       //   y          in this setup 
                //    UVW = new Vector3(0, 1, 0f),               //   ^     z    the origin o
                //};                                             //   |   /      is in the middle
                //                                               //   | /        of the rendered scene
                //var b = new Texture3DVertex {                  //   o------>x
                //    Position = new Vector3(r, r, 0.5f),        //  
                //    UVW = new Vector3(1, 1, 0f),               //   a ----- b
                //};                                             //   | \     |
                //                                               //   |   \   |
                //var c = new Texture3DVertex {                  //   |     \ |
                //    Position = new Vector3(r, -r, 0.5f),       //   d-------c
                //    UVW = new Vector3(1, 0, 0f),               //  
                //};                                             //   0 ----- 1  
                //                                               //   | \     |  
                //var d = new Texture3DVertex {                  //   |   \   |  
                //    Position = new Vector3(-r, -r, 0.5f),      //   |     \ |  
                //    UVW = new Vector3(0, 0, 0f),               //   3-------2  
                //};                                             //

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

                var resources = new GraphicsPipelineResource[3] {
                    new GraphicsPipelineResource(GraphicsPipelineResourceKind.ConstantBuffer, GraphicsShaderVisibility.Vertex),
                    new GraphicsPipelineResource(GraphicsPipelineResourceKind.ConstantBuffer, GraphicsShaderVisibility.Vertex),
                    new GraphicsPipelineResource(GraphicsPipelineResourceKind.Texture, GraphicsShaderVisibility.Pixel),
                };

                return graphicsDevice.CreatePipelineSignature(inputs, resources);
            }
        }
    }
}
