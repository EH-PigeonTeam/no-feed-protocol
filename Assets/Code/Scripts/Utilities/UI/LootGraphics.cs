using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    public class LootGraphics : MonoBehaviour
    {
        [BoxGroup("References")]
        [SerializeField] private Image m_item;

        [BoxGroup("References")]
        [SerializeField] private TMP_Text m_text;

        public void SetItem(Sprite sprite, string text)
        {
            if (m_item != null)
            {
                m_item.sprite = sprite;
            }

            if (m_text != null)
            {
                m_text.text = text;
            }
        }
    }
}
