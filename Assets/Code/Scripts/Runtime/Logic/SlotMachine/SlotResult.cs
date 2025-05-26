namespace NoFeedProtocol.Runtime.Logic.Slot
{
    /// <summary>
    /// Represents the outcome of a slot machine spin.
    /// Contains energy gains and shield recovery values.
    /// </summary>
    public readonly struct SlotResult
    {
        public int EnergyTop { get; }
        public int EnergyBottom { get; }
        public int ShieldRecovery { get; }

        public SlotResult(int energyTop, int energyBottom, int shieldRecovery)
        {
            EnergyTop = energyTop;
            EnergyBottom = energyBottom;
            ShieldRecovery = shieldRecovery;
        }

        /// <summary>
        /// Checks if this result provides any usable value.
        /// </summary>
        public bool IsEmpty =>
            EnergyTop == 0 &&
            EnergyBottom == 0 &&
            ShieldRecovery == 0;

        public override string ToString()
        {
            return $"[Top: {EnergyTop}, Bottom: {EnergyBottom}, Shield: {ShieldRecovery}]";
        }
    }
}
