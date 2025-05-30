using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using DG.Tweening;
using Core.Gameplay.SlotMachine;
using Core.Gameplay.SlotMachine.Data;

namespace NoFeedProtocol.Runtime.Logic.Slot
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SlotMachineViewEnemy : SlotMachineView
    {
        [BoxGroup("Settings")]
        [Tooltip("The delay between each symbol's reveal animation."), Unit(Units.Second)]
        [SerializeField, MinValue(0f)]
        protected float m_slotMachineRevealTime = 1f;

        protected CanvasGroup m_slotMachineCanvasGroup;

        private void Start()
        {
            m_slotMachineCanvasGroup = GetComponent<CanvasGroup>();
            m_slotMachineCanvasGroup.alpha = 0f;
        }

        public override void DisplaySymbols(IReadOnlyList<SlotSymbolData> symbols, List<SlotWheel> slotWheels)
        {
            for (int i = 0; i < slotWheels.Count; i++)
            {
                if (i >= symbols.Count || symbols[i] == null)
                    continue;

                if (slotWheels[i].TryGetComponent<Image>(out var image))
                {
                    image.sprite = symbols[i].Sprite;
                }
            }

            DOTween.Sequence()
                .Append(m_slotMachineCanvasGroup.DOFade(1f, m_slotMachineRevealTime)
                .SetEase(m_symbolRevealEase))
                .AppendInterval(m_slotMachineRevealTime)
                .Append(m_slotMachineCanvasGroup.DOFade(0f, m_slotMachineRevealTime));
        }
    }
}
