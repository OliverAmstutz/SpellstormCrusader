using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

namespace Abiogenesis3d
{
    [Serializable]
    public class PixelArtEdgeHighlightsSettings
    {
        [Range(0, 1)] public float convexHighlight = 0.5f;
        [Range(0, 1)] public float outlineShadow = 0.5f;
        [Range(0, 1)] public float concaveShadow = 1.0f;
        [Range(0.001f, 0.03f)] public float depthSensitivity = 0.002f;
        public PixelArtEdgeHighlightsDebugEffect debugEffect;
        [HideInInspector] public RenderPassEvent injectionPoint = RenderPassEvent.BeforeRenderingTransparents;
    }

    public class PixelArtEdgeHighlightsFeature : ScriptableRendererFeature
    {
        public PixelArtEdgeHighlightsSettings settings = new PixelArtEdgeHighlightsSettings();
        [HideInInspector] public Shader shader; // NOTE: public needed for builds or Shader.Find cannot find the file
        [HideInInspector] public Material material;

        PixelArtEdgeHighlightsPass pixelArtEdgeHighlightsPass;

        void OnValidate() => UpdateMaterialProperties();

        public override void Create()
        {
            pixelArtEdgeHighlightsPass = new PixelArtEdgeHighlightsPass();
            var passInput = ScriptableRenderPassInput.Color | ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal;
            pixelArtEdgeHighlightsPass.ConfigureInput(passInput);
            UpdateMaterialProperties();
        }

        public void UpdateMaterialProperties()
        {
            if (!shader) shader = Shader.Find("Abiogenesis3d/PixelArtEdgeHighlights");
            if (!material) material = new Material(shader);

            var _ConvexHighlight = settings.convexHighlight;
            var _OutlineShadow = settings.outlineShadow;
            if (QualitySettings.activeColorSpace == ColorSpace.Gamma)
            {
                // TODO: this is an approximation, what is precise way to convert from gamma?
                _ConvexHighlight *= 0.25f;
                _OutlineShadow *= 0.75f;
            }
            material.SetFloat("_ConvexHighlight", _ConvexHighlight);
            material.SetFloat("_OutlineShadow", _OutlineShadow);
            material.SetFloat("_ConcaveShadow", settings.concaveShadow);
            material.SetFloat("_DepthSensitivity", settings.depthSensitivity);
            material.SetInt("_DebugEffect", (int)settings.debugEffect);
            if (settings.debugEffect != 0)
                material.EnableKeyword("DEBUG_EFFECT");
            else material.DisableKeyword("DEBUG_EFFECT");
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
#if UNITY_EDITOR
            // not needed for build, only in editor as prefab/scene/script/properties lose material state
            UpdateMaterialProperties();
#endif
            pixelArtEdgeHighlightsPass.Setup(material, settings.injectionPoint);
            renderer.EnqueuePass(pixelArtEdgeHighlightsPass);
        }
    }

    class PixelArtEdgeHighlightsPass: ScriptableRenderPass
    {
        new string passName = "Pixel Art Edge Highlights Pass";
        Material material;

        public void Setup(Material m, RenderPassEvent injectionPoint)
        {
            material = m;
            renderPassEvent = injectionPoint;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData = frameData.Get<UniversalCameraData>();
            if (cameraData.isSceneViewCamera) return;
            if (cameraData.isPreviewCamera) return;

            var resourceData = frameData.Get<UniversalResourceData>();
            var source = resourceData.activeColorTexture;
            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = passName;
            destinationDesc.clearBuffer = false;
            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);
            var parameters = new RenderGraphUtils.BlitMaterialParameters(source, destination, material, 0);
            renderGraph.AddBlitPass(parameters, passName);
            resourceData.cameraColor = destination;
        }
    }

    public enum PixelArtEdgeHighlightsDebugEffect
    {
        None,
        Highlights,
        DepthMap,
        NormalsMap,
        Sobel,
        DepthDiff,
        NormalsDiff,
        ConcaveNormalsDiff,
    }
}
