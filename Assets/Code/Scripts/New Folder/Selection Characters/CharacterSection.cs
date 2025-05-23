using UnityEngine;
using Sirenix.OdinInspector;

namespace Core.Selection_Characters
{
    [HideMonoScript]
    public class CharacterSection : MonoBehaviour
    {
        [BoxGroup("Character Selection")]
        [Tooltip("The parent transform of the character selection buttons")]
        [SerializeField, Required, ChildGameObjectsOnly]
        private Transform m_buttonSection;

        [BoxGroup("Character Selection")]
        [Tooltip("The character data exposer")]
        [SerializeField, Required]
        private CharacterDataExposer m_characterDataExposer;

        public Transform ButtonsParent => this.m_buttonSection;
        public CharacterDataExposer CharacterManifest => this.m_characterDataExposer;
    }
}