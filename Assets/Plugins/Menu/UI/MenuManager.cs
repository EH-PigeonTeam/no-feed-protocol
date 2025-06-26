using System;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;

namespace PsychoGarden.UI
{
    [HideMonoScript]
    public class MenuManager : MonoBehaviour
    {
        #region Exposed Members

        [FoldoutGroup("Modules")]
        [FoldoutGroup("Modules/Audio")]
        [Tooltip("")]
        [SerializeField, HideLabel]
        private AudioController m_audioSettings;

        [FoldoutGroup("Modules")]
        [FoldoutGroup("Modules/Graphics")]
        [Tooltip("")]
        [SerializeField, HideLabel]
        private GraphicsController m_graphicsSettings;

        [FoldoutGroup("Modules")]
        [FoldoutGroup("Modules/Accessibility")]
        [Tooltip("")]
        [SerializeField, HideLabel]
        private AccessibilityController m_accessibilitySettings;

        #endregion

        #region Unity Callbacks

        private void Start()
        {
            this.m_audioSettings?.Initialize();
            this.m_graphicsSettings?.Initialize();
            this.m_accessibilitySettings?.Initialize();
        }

        private void OnEnable()
        {
            this.m_audioSettings?.OnEnable();
            this.m_graphicsSettings?.OnEnable();
            this.m_accessibilitySettings?.OnEnable();
        }

        private void OnDisable()
        {
            this.m_audioSettings?.OnDispose();
            this.m_graphicsSettings?.OnDispose();
            this.m_accessibilitySettings?.OnDispose();
        }

        #endregion
    }

    [Serializable]
    public class MenuModule
    {
        public virtual void Initialize() { }
        public virtual void OnEnable() { }
        public virtual void OnDispose() { }
    }

    public static class SceneObjectFinder
    {
        /// <summary>
        /// Finds a GameObject with the specified name in the specified scene.
        /// </summary>
        public static GameObject FindInScene(string sceneName, string objectName)
        {
            Scene scene = SceneManager.GetSceneByName(sceneName);
            if (!scene.isLoaded)
            {
                Debug.LogWarning($"Scene '{sceneName}' is not loaded.");
                return null;
            }

            GameObject[] rootObjects = scene.GetRootGameObjects();
            foreach (GameObject root in rootObjects)
            {
                GameObject found = FindInChildren(root.transform, objectName);
                if (found != null)
                    return found;
            }

            return null;
        }

        private static GameObject FindInChildren(Transform parent, string name)
        {
            if (parent.name == name)
                return parent.gameObject;

            foreach (Transform child in parent)
            {
                GameObject result = FindInChildren(child, name);
                if (result != null)
                    return result;
            }

            return null;
        }
    }
}
