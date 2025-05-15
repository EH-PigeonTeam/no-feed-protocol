using UnityEngine;
using Sirenix.OdinInspector;

namespace Core.Gameplay.SlotMachine.Data
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "SlotSymbol", menuName = "No Feed Protocol/SlotSymbol")]
    public class SlotMachineData : ScriptableObject
    {
        [BoxGroup("Data")]
        [Tooltip("The symbols of the slot machine")]
        [SerializeField]
        private SlotSymbolData[] m_symbols;

        [BoxGroup("Data")]
        [Tooltip("The base number of wheels of the slot machine")]
        [SerializeField, Range(1, 10)]
        private int m_slotWheelsCount = 6;

        [BoxGroup("Data")]
        [Tooltip("The base number of wheels of the slot machine")]
        [SerializeField, Range(1, 5)]
        private int m_chances = 3;

        [BoxGroup("Data")]
        [Tooltip("The delay of the slot machine"), Unit(Units.Second)]
        [SerializeField, MinValue(0)]
        private float m_delay = 1f;

        public SlotSymbolData[] Symbols => m_symbols;
        public float GetNormalizedPercent(int index)
        {
            float total = 0f;
            foreach (var symbol in this.m_symbols)
            {
                total += symbol.Percent;
            }

            if (total == 0f)
                return 0f;

            return this.m_symbols[index].Percent / total;
        }
        public int SlotWheelCount => m_slotWheelsCount;
        public int SpinCount => m_chances;
        public float Delay => m_delay;
    }

    [System.Serializable]
    public class SlotSymbolData
    {
        [FoldoutGroup("@m_name")]
        [Tooltip("The name of the symbol")]
        [SerializeField]
        private string m_name;

        [FoldoutGroup("@m_name")]
        [Tooltip("The sprite of the symbol")]
        [SerializeField, PreviewField(100)]
        private Sprite m_sprite;

        [FoldoutGroup("@m_name")]
        [Tooltip("The multiplier of the symbol")]
        [SerializeField, Range(1, 10)]
        private float m_multiplier = 1f;

        [FoldoutGroup("@m_name")]
        [Tooltip("The target of the symbol")]
        [SerializeField]
        private SymbolTarget m_target = SymbolTarget.Middle;

        [FoldoutGroup("@m_name")]
        [Tooltip("The percentage of the symbol can appear")]
        [SerializeField, Range(0, 1)]
        private float m_percent = 1f;

        public string Name => m_name;
        public Sprite Sprite => m_sprite;
        public float Multiplier => m_multiplier;
        public SymbolTarget Target => m_target;
        public float Percent => m_percent;
    }

    public enum SymbolTarget
    {
        Top,
        Middle,
        Bottom
    }
}