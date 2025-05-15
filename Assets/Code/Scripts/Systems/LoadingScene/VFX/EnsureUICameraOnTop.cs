using Code.Systems.LoadingScene;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Code.Systems.LoadingScene.VFX
{
    [HideMonoScript]
    public class EnsureUICameraOnTop : MonoBehaviour
    {
        private Camera m_baseCamera;
        private Camera m_uiCamera;

        private UniversalAdditionalCameraData m_baseCamData;

        private void Awake()
        {
            this.m_uiCamera = this.GetComponent<Camera>();
            this.m_baseCamera = Camera.main;

            if (this.m_baseCamera == null)
                this.m_baseCamera = Camera.main;

            if (this.m_baseCamera != null)
                this.m_baseCamData = this.m_baseCamera.GetUniversalAdditionalCameraData();
        }

        private void LateUpdate()
        {
            if (this.m_baseCamData == null || this.m_uiCamera == null)
                return;

            var stack = this.m_baseCamData.cameraStack;

            if (stack.Contains(this.m_uiCamera))
            {
                if (stack[^1] != this.m_uiCamera)
                {
                    stack.Remove(this.m_uiCamera);
                    stack.Add(this.m_uiCamera);
                }
            }
            else
            {
                stack.Add(this.m_uiCamera);
            }
        }
    }
}
