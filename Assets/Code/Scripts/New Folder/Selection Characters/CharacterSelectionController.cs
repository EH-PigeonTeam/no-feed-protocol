using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using NoFeedProtocol.Authoring.Characters;

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
        [SerializeField]
        private CharacterSection m_sectionTop;

        [BoxGroup("Character Selection")]
        [SerializeField]
        private CharacterSection m_sectionBottom;

        #endregion

        #region Private Members ------------------------------------------

        [SerializeField]
        private List<ButtonAudioToggle> m_topButtons = new();

        [SerializeField]
        private List<ButtonAudioToggle> m_bottomButtons = new();

        private int m_selectedTopIndex = -1;
        private int m_selectedBottomIndex = -1;

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
            for (int i = 0; i < m_charactersData.Characters.Length; i++)
            {
                var character = m_charactersData[i];

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
            var character = m_charactersData[index];

            if (isTop)
            {
                m_sectionTop.CharacterManifest.Display(character);
                DeactivatePrevious(m_topButtons, ref m_selectedTopIndex, index);
            }
            else
            {
                m_sectionBottom.CharacterManifest.Display(character);
                DeactivatePrevious(m_bottomButtons, ref m_selectedBottomIndex, index);
            }
        }

        private void DeactivatePrevious(List<ButtonAudioToggle> buttons, ref int lastIndex, int currentIndex)
        {
            if (lastIndex >= 0 && lastIndex != currentIndex)
                buttons[lastIndex].Deactivate();

            buttons[currentIndex].Activate();
            lastIndex = currentIndex;
        }

        #endregion

        #region Public API ----------------------------------------------

        public bool HasTwoCharacterSelected() =>
            m_selectedTopIndex >= 0 && m_selectedBottomIndex >= 0;

        public CharacterData SelectedTopCharacter =>
            m_selectedTopIndex >= 0 ? m_charactersData[m_selectedTopIndex] : null;

        public CharacterData SelectedBottomCharacter =>
            m_selectedBottomIndex >= 0 ? m_charactersData[m_selectedBottomIndex] : null;

        #endregion
    }
}
