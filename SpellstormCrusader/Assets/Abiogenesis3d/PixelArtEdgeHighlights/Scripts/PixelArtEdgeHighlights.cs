using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Abiogenesis3d
{
    [Serializable]
    public class PixelArtEdgeHighlightsCameraInfo
    {
        public Camera cam;
    }

    [ExecuteInEditMode]
    public class PixelArtEdgeHighlights : MonoBehaviour
    {
        // NOTE: this reference is set to the renderer features
        public PixelArtEdgeHighlightsSettings settings;

        [Header("Experimental")]
        public bool clampCameraFarClipPlane = true;

        [Header("To ignore a camera add PixelArtEdgeHighlightsIgnore component to it.")]
        public bool autoDetectCameras = true;
        public List<PixelArtEdgeHighlightsCameraInfo> cameraInfos = new List<PixelArtEdgeHighlightsCameraInfo>();

        public List<PixelArtEdgeHighlightsFeature> rendererFeatures;

        public void HandleSettings()
        {
            foreach (var feature in rendererFeatures)
            {
                if (!feature) continue;
                feature.settings = settings;
                feature.UpdateMaterialProperties();
            }
        }

        float GetCamDepthOr0(Camera cam)
        {
            if (cam != null) return cam.depth;
            return 0;
        }

        Type GetIgnoredType()
        {
            return typeof(PixelArtEdgeHighlightsIgnore);
        }

        // TODO: export this into helper file
        void AutoDetectCameras()
        {
            var allCameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);

            foreach(var cam in allCameras)
            {
                var ignoreTag = cam.GetComponent(GetIgnoredType());
                var camInfo = cameraInfos.FirstOrDefault(c => c.cam == cam);

                if (camInfo == null)
                {
                    if (ignoreTag == null)
                    {
                        camInfo = new PixelArtEdgeHighlightsCameraInfo {cam = cam};
                        cameraInfos = cameraInfos.Concat(new[] {camInfo}).ToList();
                    }
                }
                else
                {
                    if (ignoreTag != null)
                        cameraInfos = cameraInfos.Where(c => c.cam != cam).ToList();
                }
            }
            cameraInfos = cameraInfos.OrderBy(c => GetCamDepthOr0(c.cam)).ToList();
        }

        void CheckForInstances()
        {
            var existingInstances = FindObjectsByType<PixelArtEdgeHighlights>(FindObjectsSortMode.None);
            if (existingInstances.Length > 1)
            {
                Debug.Log($"PixelArtEdgeHighlights: There should only be one active instance in the scene. Deactivating: {name}");
                enabled = false;
                return;
            }

#if UNITY_EDITOR
            // TODO: call less frequently
            var urpAssets = PipelineAssets.GetUrpAssets();
            foreach(var urpAsset in urpAssets)
                SetupRenderFeatures.SetDownsamplingToNone(urpAsset);
            rendererFeatures = SetupRenderFeatures.AddAndGetRendererFeatures<PixelArtEdgeHighlightsFeature>(urpAssets);
#endif

            if (rendererFeatures.Count == 0)
            {
                Debug.Log("Renderer Features could not be added.");
                return;
            }

            foreach (var feature in rendererFeatures)
            {
                if (!feature) continue;
                feature.SetActive(true);
            }
        }

        void OnEnable()
        {
            CheckForInstances();
            HandleSettings();
        }

        void Update()
        {
            if (autoDetectCameras)
                AutoDetectCameras();

            foreach (var camInfo in cameraInfos)
            {
                if (!camInfo.cam) continue;
                HandleCam(camInfo);
            }
        }

        void HandleCam(PixelArtEdgeHighlightsCameraInfo camInfo)
        {
            camInfo.cam.depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.DepthNormals;
            camInfo.cam.allowMSAA = false;

            if (clampCameraFarClipPlane)
            {
                var maxFarClipPlane = 100;
                if (camInfo.cam.farClipPlane > maxFarClipPlane)
                {
                    camInfo.cam.farClipPlane = maxFarClipPlane;
                    Debug.Log($"PixelArtEdgeHighlights: Clamping camera.farClipPlane to {maxFarClipPlane}. Shader values are adjusted for that depthmap precision. To go beyond please uncheck the clampCameraFarClipPlane option and adjust depthSensitivity. With larger farClipPlane the depthmap becomes less precise.");
                }
            }
        }

        void OnDisable()
        {
            foreach (var feature in rendererFeatures)
            {
                if (!feature) continue;
                feature.SetActive(false);
            }
        }
    }
}
