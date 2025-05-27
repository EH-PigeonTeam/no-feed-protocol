using System;
using System.Collections.Generic;
using System.Linq;
using Core.Gameplay.SlotMachine.Data;
using NoFeedProtocol.Authoring.Items;

namespace NoFeedProtocol.Runtime.Logic.Slot
{
    /// <summary>
    /// Core logic for spinning slot machines, extracting results, and applying item modifiers.
    /// This class is UI-agnostic and suitable for both player and AI use.
    /// </summary>
    public class SlotMachineLogic
    {
        public delegate void SpinCompletedHandler(SlotResult result);
        public event SpinCompletedHandler OnSpinCompleted;

        private List<SlotSymbolData> m_symbolPool;
        private int m_wheelCount;
        private int m_spinLimit;

        private List<int> m_lockedIndexes;
        private List<SlotSymbolData> m_currentSymbols;

        private Random m_rng;

        public IReadOnlyList<SlotSymbolData> CurrentSymbols => m_currentSymbols;
        public bool IsSpinLimitReached => m_spinCount >= m_spinLimit;
        public int Count => m_spinCount;

        private int m_spinCount;

        /// <summary>
        /// Initializes the slot machine logic with data and optional item modifiers.
        /// </summary>
        public void Setup(SlotMachineData baseData, List<Item> items)
        {
            m_symbolPool = new List<SlotSymbolData>(baseData.Symbols);
            m_wheelCount = baseData.SlotWheelCount;
            m_spinLimit = baseData.SpinCount;

            m_lockedIndexes = new List<int>();
            m_currentSymbols = new List<SlotSymbolData>(new SlotSymbolData[m_wheelCount]);
            m_spinCount = 0;
            m_rng = new Random();

            ApplyItemModifiers(items);
        }

        /// <summary>
        /// Spins all unlocked wheels and stores the resulting symbols.
        /// </summary>
        public void Spin(List<int> lockedIndexes = null)
        {
            m_lockedIndexes = lockedIndexes;

            if (IsSpinLimitReached || AllWheelsLocked())
            {
                m_spinCount = m_spinLimit;
                return;
            }

            for (int i = 0; i < m_wheelCount; i++)
            {
                if (m_lockedIndexes.Contains(i))
                    continue;

                m_currentSymbols[i] = PickRandomSymbol();
            }

            m_spinCount++;

            if (IsSpinLimitReached || AllWheelsLocked())
            {
                var result = CalculateResult();
                OnSpinCompleted?.Invoke(result);
            }
        }

        /// <summary>
        /// Locks a wheel at the given index, preventing it from spinning again.
        /// </summary>
        public void LockWheel(int index)
        {
            if (!m_lockedIndexes.Contains(index) && index >= 0 && index < m_wheelCount)
                m_lockedIndexes.Add(index);
        }

        /// <summary>
        /// Resets spin counters and unlocks all wheels.
        /// </summary>
        public void Reset()
        {
            m_spinCount = 0;
            m_lockedIndexes.Clear();
            m_currentSymbols = new List<SlotSymbolData>(new SlotSymbolData[m_wheelCount]);
        }

        /// <summary>
        /// Analyzes the current symbol configuration and returns the outcome.
        /// </summary>
        public SlotResult CalculateResult()
        {
            int energyTop = 0;
            int energyBottom = 0;
            int shieldRecovery = 0;

            foreach (var symbol in m_currentSymbols)
            {
                if (symbol == null)
                    continue;

                int value = (int)MathF.Round(symbol.Multiplier);

                switch (symbol.Target)
                {
                    case SymbolTarget.Top:
                        energyTop += value;
                        break;

                    case SymbolTarget.Bottom:
                        energyBottom += value;
                        break;

                    case SymbolTarget.Middle:
                        shieldRecovery += value;
                        break;
                }
            }

            return new SlotResult(energyTop, energyBottom, shieldRecovery);
        }

        private bool AllWheelsLocked()
        {
            return m_lockedIndexes.Count >= m_wheelCount;
        }

        private SlotSymbolData PickRandomSymbol()
        {
            float total = m_symbolPool.Sum(s => s.Percent);
            float roll = (float)m_rng.NextDouble() * total;

            float cumulative = 0f;
            foreach (var symbol in m_symbolPool)
            {
                cumulative += symbol.Percent;
                if (roll <= cumulative)
                    return symbol;
            }

            return m_symbolPool.Last(); // fallback
        }

        /// <summary>
        /// Applies item-based effects to modify the slot machine configuration.
        /// </summary>
        private void ApplyItemModifiers(List<Item> items)
        {
            // TODO: Implement logic to affect spin limit, symbol pool, wheel count, etc.
            // For example:
            // - Add extra spins
            // - Add new symbols
            // - Modify symbol weights

            // This is where Ability.Condition / AbilityEffect could be checked
        }
    }
}
