using UnityEngine;

namespace NoFeedProtocol.Runtime.Logic.Slot
{
    /// <summary>
    /// Represents the outcome of a slot machine spin.
    /// Contains energy gains and shield recovery values.
    /// </summary>
    public readonly struct SpinResult
    {
        public int EnergyTop { get; }
        public int EnergyBottom { get; }
        public int ShieldRecovery { get; }

        /// <summary>
        /// Constructs a new SpinResult, normalizing each value.
        /// </summary>
        /// <param name="energyTop">Raw energy for top character.</param>
        /// <param name="energyBottom">Raw energy for bottom character.</param>
        /// <param name="shieldRecovery">Raw shield recovery value.</param>
        public SpinResult(int energyTop, int energyBottom, int shieldRecovery)
        {
            EnergyTop = NormalizeValue(energyTop);
            EnergyBottom = NormalizeValue(energyBottom);
            ShieldRecovery = NormalizeValue(shieldRecovery);
        }

        /// <summary>
        /// Indicates whether the spin result has no useful values.
        /// </summary>
        public bool IsEmpty =>
            EnergyTop == 0 &&
            EnergyBottom == 0 &&
            ShieldRecovery == 0;

        public override string ToString()
        {
            return $"[Top: {EnergyTop}, Bottom: {EnergyBottom}, Shield: {ShieldRecovery}]";
        }

        /// <summary>
        /// Applies a fixed penalty to values and clamps negative results to zero.
        /// </summary>
        private static int NormalizeValue(int value)
        {
            const int NormalizationPenalty = 2;
            return Mathf.Max(0, value - NormalizationPenalty);
        }
    }
}
