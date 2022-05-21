using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Sabotris.Settings
{
    public class ScreenShader : ScriptableRendererFeature
    {
        class CustomRenderPass : ScriptableRenderPass
        {
            public RenderTargetIdentifier source;
            private Material material;
            private RenderTargetHandle tempRenderHandler;

            public CustomRenderPass(Material material)
            {
                this.material = material;
                tempRenderHandler.Init("_TemporaryColorTexture");
            }
            
            public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
            {
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var commandBuffer = CommandBufferPool.Get();
                
                commandBuffer.GetTemporaryRT(tempRenderHandler.id, renderingData.cameraData.cameraTargetDescriptor);
                Blit(commandBuffer, source, tempRenderHandler.Identifier(), material);
                Blit(commandBuffer, tempRenderHandler.Identifier(), source);
                
                context.ExecuteCommandBuffer(commandBuffer);
                CommandBufferPool.Release(commandBuffer);
            }

            public override void OnCameraCleanup(CommandBuffer cmd)
            {
            }
        }

        [System.Serializable]
        public class Settings
        {
            public Material material;
        }

        public Settings settings = new Settings();

        CustomRenderPass m_ScriptablePass;

        public override void Create()
        {
            m_ScriptablePass = new CustomRenderPass(settings.material);

            m_ScriptablePass.renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            m_ScriptablePass.source = renderer.cameraColorTarget;
            renderer.EnqueuePass(m_ScriptablePass);
        }
    }
}