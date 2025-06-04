using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Code.Systems.LoadingScene;

namespace NoFeedProtocol.Runtime.Logic.VFX
{
    [HideMonoScript]
    [RequireComponent(typeof(Camera))]
    public class UIStackOrderManager : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [SerializeField, Min(1)]
        private int m_priorityFromTop = 1;

        [BoxGroup("Settings")]
        [Tooltip("If true, this script bypass OnSceneLoadFinished event.")]
        [SerializeField]
        private bool m_override = false;

        private Camera m_uiCamera;
        private Camera m_baseCamera;

        #region Initialization and State Setup --------------------------

        private void Awake()
        {
            m_uiCamera = GetComponent<Camera>();

            if (m_override)
            {
                HandleSceneLoadFinished();
            }
        }

        private void OnEnable()
        {
            LoadSceneManager.OnSceneLoadFinished += HandleSceneLoadFinished;
        }

        private void OnDisable()
        {
            LoadSceneManager.OnSceneLoadFinished -= HandleSceneLoadFinished;
        
#if UNITY_EDITOR
            if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
                return;
#endif

            if (m_uiCamera == null)
                return;

            var baseCameraObj = Camera.main;
            if (baseCameraObj == null) return;

            var baseCamera = baseCameraObj.GetUniversalAdditionalCameraData();
            if (baseCamera == null) return;

            if (baseCamera.cameraStack.Contains(m_uiCamera))
            {
                baseCamera.cameraStack.Remove(m_uiCamera);
            }
        }

        #endregion

        #region Camera Stack Ordering -----------------------------------

        [Button]
        private void HandleSceneLoadFinished()
        {
            m_baseCamera = Camera.main;

            if (m_baseCamera == null || m_uiCamera == null)
                return;

            var baseData = m_baseCamera.GetUniversalAdditionalCameraData();
            var stack = baseData.cameraStack;

            stack.Remove(m_uiCamera);

            int insertIndex = Mathf.Clamp(stack.Count - m_priorityFromTop, 0, stack.Count);

            stack.Insert(insertIndex, m_uiCamera);

#if UNITY_EDITOR
            Debug.Log($"[UIStackOrderManager] Inserted {m_uiCamera.name} at stack index {insertIndex} (PriorityFromTop={m_priorityFromTop})");
#endif
        }

        #endregion
    }
}
