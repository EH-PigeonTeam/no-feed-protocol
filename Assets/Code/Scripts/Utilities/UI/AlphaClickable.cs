using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AlphaClickable : MonoBehaviour
{
    private void Awake()
    {
        var image = GetComponent<Image>();
        image.alphaHitTestMinimumThreshold = 0.1f; // Set the threshold to 0.1
    }
}