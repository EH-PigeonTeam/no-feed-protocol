using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Canvas))]
public class CanvasSceneCameraLinker : MonoBehaviour
{
    void Start()
    {
        var canvas = GetComponent<Canvas>();
        if (canvas.renderMode != RenderMode.WorldSpace && canvas.renderMode != RenderMode.ScreenSpaceCamera)
            return;

        canvas.worldCamera = gameObject.scene.GetRootGameObjects()
            .SelectMany(root => root.GetComponentsInChildren<Camera>(true))
            .FirstOrDefault();
    }
}
