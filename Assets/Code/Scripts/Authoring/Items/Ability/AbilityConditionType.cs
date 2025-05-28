namespace NoFeedProtocol.Authoring.Items.Abilities
{
    /// <summary>
    /// Represents all the possible conditions under which an ability can be triggered or evaluated.
    /// </summary>
    public enum AbilityConditionType
    {
        /// <summary>Triggered if the character owns a specific item.</summary>
        OwnsItem,

        /// <summary>Triggered after a specific number of turns have passed.</summary>
        TurnsPassed,

        /// <summary>Triggered when a player-controlled character dies.</summary>
        OnDeathCharacter,

        /// <summary>Triggered when an enemy dies.</summary>
        OnDeathEnemy,

        /// <summary>Triggered if a character's HP falls below a threshold.</summary>
        OnHpBelowCharacter,

        /// <summary>Triggered if an enemy's HP falls below a threshold.</summary>
        OnHpBelowEnemy,

        /// <summary>Triggered if the total shield of characters is below a threshold.</summary>
        OnShieldBelowCharacters,

        /// <summary>Triggered if the total shield of enemies is below a threshold.</summary>
        OnShieldBelowEnemies,

        /// <summary>Triggered when the wheel is spun.</summary>
        WheelSpin,

        /// <summary>Triggered when results from the wheel spin are provided.</summary>
        WheelResultsProvided,

        /// <summary>Triggered when the wheel is locked and cannot be spun.</summary>
        WheelLocked,

        /// <summary>Triggered if the wheel has remaining rotations.</summary>
        WheelRotationsRemaining,

        /// <summary>Triggered when points are provided as a result of a wheel interaction.</summary>
        WheelPointsProvided,

        /// <summary>Triggered when a character takes an action.</summary>
        OnActCharacter,

        /// <summary>Triggered when an enemy takes an action.</summary>
        OnActEnemy,

        /// <summary>Triggered when a consumable item is used.</summary>
        ConsumableUsed,

        /// <summary>Triggered when a resource or value is gained.</summary>
        OnGain,

        /// <summary>Triggered when a resource or value is lost.</summary>
        OnLoss
    }
}
