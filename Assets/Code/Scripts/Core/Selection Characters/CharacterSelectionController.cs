using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Core.Players;
using Code.Systems.Locator;

namespace Core.Selection_Characters
{
    [HideMonoScript]
    public class CharacterSelectionController : MonoBehaviour
    {
        #region Exposed Members ------------------------------------------

        [BoxGroup("Character Selection")]
        [Tooltip("The list of characters")]
        [SerializeField, InlineEditor, HideLabel]
        private CharactersData m_charactersData;

        [BoxGroup("Character Selection")]
        [Tooltip("The list of characters")]
        [SerializeField]
        private CharacterSection m_sectionTop;

        [BoxGroup("Character Selection")]
        [Tooltip("The list of characters")]
        [SerializeField]
        private CharacterSection m_sectionBottom;

        #endregion

        #region Private Members ------------------------------------------

        [FoldoutGroup("Character Selection Top")]
        [Tooltip("")]
        [SerializeField]
        private List<ButtonAudioToggle> m_topButtons = new();

        [FoldoutGroup("Character Selection Bottom")]
        [Tooltip("")]
        [SerializeField]
        private List<ButtonAudioToggle> m_bottomButtons = new();

        private ButtonAudioToggle m_lastSelectedTop;
        private ButtonAudioToggle m_lastSelectedBottom;

        #endregion

        #region Init ----------------------------------------------------

        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            ServiceLocator.Register<CharacterSelectionController>(this);
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<CharacterSelectionController>();
        }

        private void Init()
        {
            for (int i = 0; i < this.m_charactersData.Characters.Length; i++)
            {
                var character = this.m_charactersData[i];

                // set image
                m_topButtons[i].SetActiveSprite(character.Icon);
                m_bottomButtons[i].SetActiveSprite(character.Icon);

                m_topButtons[i].SetInactiveSprite(character.Icon2);
                m_bottomButtons[i].SetInactiveSprite(character.Icon2);

                m_topButtons[i].Init();
                m_bottomButtons[i].Init();

                int index = i;
                m_topButtons[i].onClick.AddListener(() => OnCharacterButtonClicked(index, true));
                m_bottomButtons[i].onClick.AddListener(() => OnCharacterButtonClicked(index, false));
            }
        }

        public void OnCharacterButtonClicked(int index, bool isTop)
        {
            Debug.Log("Character selected: " + index);

            var character = this.m_charactersData[index];

            if (isTop)
            {
                // Update UI
                this.m_sectionTop.CharacterManifest.Display(character);

                // Deselect previous
                if (this.m_lastSelectedTop != null)
                    this.m_lastSelectedTop.Deactivate();

                // Activate current
                ButtonAudioToggle selected = this.m_topButtons[index] as ButtonAudioToggle;
                selected.Activate();
                this.m_lastSelectedTop = selected;
            }
            else
            {
                // Update UI
                this.m_sectionBottom.CharacterManifest.Display(character);

                if (this.m_lastSelectedBottom != null)
                    this.m_lastSelectedBottom.Deactivate();

                ButtonAudioToggle selected = this.m_bottomButtons[index] as ButtonAudioToggle;
                selected.Activate();
                this.m_lastSelectedBottom = selected;
            }
        }

        #endregion

        #region Public Methods -------------------------------------------

        public bool HasTwoCharacterSelected() => this.m_lastSelectedTop != null && this.m_lastSelectedBottom != null;

        #endregion

    }
}
