using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class SceneReference
{
#if UNITY_EDITOR
    [SerializeField] private SceneAsset sceneAsset;
#endif

    [SerializeField, HideInInspector] private string sceneName;

    public string SceneName => sceneName;

#if UNITY_EDITOR
    public SceneAsset SceneAsset => sceneAsset;

    public void Validate()
    {
        if (sceneAsset != null)
        {
            sceneName = sceneAsset.name;
        }
    }
#endif
}
