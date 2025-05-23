using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.Systems.LoadingScene.VFX
{
    [HideMonoScript]
    [RequireComponent(typeof(Camera))]
    public class CameraStackConnector : MonoBehaviour
    {
        private Camera overlayCamera;

        private void OnEnable()
        {
            overlayCamera = GetComponent<Camera>();
            var cameraData = overlayCamera.GetUniversalAdditionalCameraData();

            if (cameraData.renderType != CameraRenderType.Overlay)
            {
                Debug.LogWarning($"Camera '{overlayCamera.name}' is not set to Overlay render type.");
                return;
            }

            var baseCameraObj = Camera.main;
            if (baseCameraObj == null)
            {
                Debug.LogError("No camera with tag 'MainCamera' found in scene.");
                return;
            }

            var baseCamera = baseCameraObj.GetUniversalAdditionalCameraData();
            if (baseCamera == null)
            {
                Debug.LogError("Main camera does not have UniversalAdditionalCameraData.");
                return;
            }

            if (!baseCamera.cameraStack.Contains(overlayCamera))
            {
                baseCamera.cameraStack.Add(overlayCamera);
            }
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                return;
#endif

            if (overlayCamera == null)
                return;

            var baseCameraObj = Camera.main;
            if (baseCameraObj == null) return;

            var baseCamera = baseCameraObj.GetUniversalAdditionalCameraData();
            if (baseCamera == null) return;

            if (baseCamera.cameraStack.Contains(overlayCamera))
            {
                baseCamera.cameraStack.Remove(overlayCamera);
            }
        }
    }
}
