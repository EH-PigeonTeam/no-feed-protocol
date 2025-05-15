using UnityEngine;
using Sirenix.OdinInspector;
using Core.Gameplay.SlotMachine.Data;
using UnityEngine.UI;
using DG.Tweening;

namespace Core.Gameplay.SlotMachine
{
    [HideMonoScript]
    public class SlotMachine : MonoBehaviour, IResettable
    {
        #region Exposed Members ---------------------------------------------------

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField, InlineEditor, HideLabel, Required]
        private SlotMachineData m_slotMachine;

        [FoldoutGroup("Settings/Costruction")]
        [Tooltip("")]
        [SerializeField, AssetsOnly, Required]
        private GameObject m_slotWheelPrefab;

        [FoldoutGroup("Settings/Costruction")]
        [Tooltip("")]
        [SerializeField, AssetsOnly, Required]
        private GameObject m_indicatorPrefab;

        [FoldoutGroup("Settings/References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly, Required]
        private Transform m_slotWheelContainer;

        [FoldoutGroup("Settings/References")]
        [Tooltip("")]
        [SerializeField, ChildGameObjectsOnly, Required]
        private Transform m_indicatorContainer;

        [Button("Spin", ButtonStyle.FoldoutButton)]
        public void SpinButton() => Interact();

        [Button("Reset", ButtonStyle.FoldoutButton)]
        public void ResetButton() => Restore();

        #endregion

        #region Private Members ---------------------------------------------------

        [FoldoutGroup("Debug")]
        [Tooltip("")]
        [ShowInInspector, ReadOnly]
        private SlotWheel[] m_slotWheels;

        [FoldoutGroup("Debug")]
        [Tooltip("")]
        [ShowInInspector, ReadOnly]
        private Indicator[] m_indicators;

        private float m_lastSpinTime;
        private int m_spinCount;
        private bool m_isLocked;

        #endregion

        #region Unity Callbacks ---------------------------------------------------

        private void Awake()
        {
            if (m_slotMachine == null
                || m_slotWheelPrefab == null
                || m_indicatorPrefab == null
                || m_slotWheelContainer == null
                || m_indicatorContainer == null)
            {
                return;
            }

            GenerateSlotWheels(this.m_slotMachine.SlotWheelCount);
            GenerateIndicators(this.m_slotMachine.SpinCount);
        }

        #endregion

        #region Private Methods ---------------------------------------------------

        private void GenerateSlotWheels(int count)
        {
            this.m_slotWheels = new SlotWheel[count];

            for (int i = 0; i < count; i++)
            {
                var wheel = Instantiate(this.m_slotWheelPrefab, this.m_slotWheelContainer);
                this.m_slotWheels[i] = wheel.GetComponent<SlotWheel>();
            }
        }

        private void GenerateIndicators(int count)
        {
            this.m_indicators = new Indicator[count];

            for (int i = 0; i < count; i++)
            {
                var indicator = Instantiate(this.m_indicatorPrefab, this.m_indicatorContainer);
                this.m_indicators[i] = indicator.GetComponent<Indicator>();
            }
        }

        private void Spin()
        {
            this.m_lastSpinTime = Time.time + this.m_slotMachine.Delay;

            var symbols = this.m_slotMachine.Symbols;

            if (symbols == null || symbols.Length == 0)
            {
                Debug.LogError("No symbols defined in SlotMachineData!");
                return;
            }

            for (int i = 0; i < this.m_slotWheels.Length; i++)
            {
                if (this.m_slotWheels[i].IsLocked)
                    continue;

                int index = i; // Fix closure
                float roll = Random.Range(0f, 1f);
                float cumulative = 0f;
                int chosenIndex = symbols.Length - 1;

                for (int j = 0; j < symbols.Length; j++)
                {
                    float normalized = this.m_slotMachine.GetNormalizedPercent(j);
                    if (float.IsNaN(normalized) || normalized <= 0f)
                        continue;

                    cumulative += normalized;

                    if (roll <= cumulative)
                    {
                        chosenIndex = j;
                        break;
                    }
                }

                int finalChosenIndex = chosenIndex; // Fix closure

                float delay = index * this.m_slotMachine.Delay / this.m_slotWheels.Length;

                DOVirtual.DelayedCall(delay, () =>
                {
                    var image = this.m_slotWheels[index].GetComponent<Image>();
                    if (finalChosenIndex >= 0 && finalChosenIndex < symbols.Length)
                        image.sprite = symbols[finalChosenIndex].Sprite;
                    else
                        Debug.LogError("Invalid symbol index: " + finalChosenIndex);
                });
            }
        }

        private void ResetWheels()
        {
            for (int i = 0; i < this.m_slotWheels.Length; i++)
            {
                this.m_slotWheels[i].Restore();
            }
        }

        private void UpdateIndicators()
        {
            this.m_indicators[this.m_spinCount - 1].Activate();
        }

        private void ResetInidicators()
        {
            for (int i = 0; i < this.m_indicators.Length; i++)
            {
                this.m_indicators[i].Restore();
            }
        }

        #endregion

        #region Public Methods ----------------------------------------------------

        /// <summary>
        /// Spin the slot machine
        /// </summary>
        public void Interact()
        {
            if (this.m_isLocked
                || Time.time - this.m_lastSpinTime < this.m_slotMachine.Delay
                || this.m_spinCount >= this.m_slotMachine.SpinCount)
                return;

            Spin();
            this.m_spinCount++;
            UpdateIndicators();
            this.m_isLocked = this.m_spinCount >= this.m_slotMachine.SpinCount;
        }

        /// <summary>
        /// Reset the slot machine
        /// </summary>
        public void Restore()
        {
            this.m_spinCount = 0;
            this.m_lastSpinTime = 0f;
            this.m_isLocked = false;

            ResetWheels();
            ResetInidicators();
        }

        #endregion

    }
}
