using UnityEngine;

namespace NoFeedProtocol.Runtime.Logic.Slot
{
    /// <summary>
    /// Represents the outcome of a slot machine spin.
    /// Contains energy gains and shield recovery values.
    /// </summary>
    public class SpinResult
    {
        public int EnergyTop { get; set; }
        public int EnergyBottom { get; set; }
        public int ShieldRecovery { get; set; }

        public SpinResult(int energyTop, int energyBottom, int shieldRecovery)
        {
            EnergyTop = NormalizeValue(energyTop);
            EnergyBottom = NormalizeValue(energyBottom);
            ShieldRecovery = NormalizeValue(shieldRecovery);
        }

        public bool IsEmpty =>
            EnergyTop == 0 &&
            EnergyBottom == 0 &&
            ShieldRecovery == 0;

        public override string ToString()
        {
            return $"[Top: {EnergyTop}, Bottom: {EnergyBottom}, Shield: {ShieldRecovery}]";
        }

        private static int NormalizeValue(int value)
        {
            const int NormalizationPenalty = 2;
            return Mathf.Max(0, value - NormalizationPenalty);
        }
    }

}
