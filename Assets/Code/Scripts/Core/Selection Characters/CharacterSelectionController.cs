using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using Core.Players;
using DG.Tweening;

namespace Core.Selection_Characters
{
    [HideMonoScript]
    public class CharacterSelectionController : MonoBehaviour
    {
        [BoxGroup("Character Selection")]
        [Tooltip("The list of characters")]
        [SerializeField, InlineEditor, HideLabel]
        private CharactersData m_charactersData;

        [BoxGroup("Character Selection")]
        [Tooltip("Top section container")]
        [SerializeField, Required, SceneObjectsOnly]
        private CharacterSection m_sectionTop;

        [BoxGroup("Character Selection")]
        [Tooltip("Bottom section container")]
        [SerializeField, Required, SceneObjectsOnly]
        private CharacterSection m_sectionBottom;

        [BoxGroup("Character Selection")]
        [Tooltip("Button prefab to instantiate")]
        [SerializeField, Required, AssetsOnly]
        private GameObject m_buttonPrefab;

        private readonly List<ButtonExtended> m_topButtons = new();
        private readonly List<ButtonExtended> m_bottomButtons = new();

        private ButtonExtended m_lastSelectedTop;
        private ButtonExtended m_lastSelectedBottom;

        private void Start()
        {
            GenerateButtons();
        }

        private void GenerateButtons()
        {
            for (int i = 0; i < this.m_charactersData.Characters.Length; i++)
            {
                var character = this.m_charactersData[i];

                // --- Top ---
                GameObject topGO = Instantiate(this.m_buttonPrefab, this.m_sectionTop.ButtonsParent);
                var topButton = topGO.GetComponent<ButtonExtended>();
                topGO.GetComponent<Image>().sprite = character.Icon;
                int topIndex = i; // necessary to avoid closure bugs
                topButton.onClick.AddListener(() => OnCharacterButtonClicked(topIndex, true));
                this.m_topButtons.Add(topButton);

                // --- Bottom ---
                GameObject bottomGO = Instantiate(this.m_buttonPrefab, this.m_sectionBottom.ButtonsParent);
                var bottomButton = bottomGO.GetComponent<ButtonExtended>();
                bottomGO.GetComponent<Image>().sprite = character.Icon;
                int bottomIndex = i;
                bottomButton.onClick.AddListener(() => OnCharacterButtonClicked(bottomIndex, false));
                this.m_bottomButtons.Add(bottomButton);

                // --- Set Opposite ---
                topButton.SetOppositeButton(bottomButton);
                bottomButton.SetOppositeButton(topButton);
            }
        }

        public void OnCharacterButtonClicked(int index, bool isTop)
        {
            var character = this.m_charactersData[index];

            if (isTop)
            {
                // Update UI
                this.m_sectionTop.CharacterManifest.Display(character);

                // Deselect previous
                if (this.m_lastSelectedTop != null)
                    this.m_lastSelectedTop.Deactivate();

                // Activate current
                ButtonExtended selected = this.m_topButtons[index] as ButtonExtended;
                selected.Activate();
                this.m_lastSelectedTop = selected;
            }
            else
            {
                // Update UI
                this.m_sectionBottom.CharacterManifest.Display(character);

                if (this.m_lastSelectedBottom != null)
                    this.m_lastSelectedBottom.Deactivate();

                ButtonExtended selected = this.m_bottomButtons[index] as ButtonExtended;
                selected.Activate();
                this.m_lastSelectedBottom = selected;
            }
        }
    }
}
