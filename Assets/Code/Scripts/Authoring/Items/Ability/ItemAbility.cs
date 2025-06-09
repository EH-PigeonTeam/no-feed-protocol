using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace NoFeedProtocol.Authoring.Items
{
    /// <summary>
    /// Describes the ability embedded in an item, with trigger, conditions, and effects.
    /// </summary>
    [Serializable]
    public class ItemAbility
    {
        [FoldoutGroup("Ability")]
        [Tooltip("The trigger moment of the ability (e.g. always active, when used).")]
        [SerializeField]
        private AbilityTriggerMoment m_trigger;

        [FoldoutGroup("Ability")]
        [Tooltip("Effects that the ability applies when triggered.")]
        [SerializeField]
        private List<AbilityEffect> m_effects;

        public AbilityTriggerMoment Trigger => m_trigger;
        public List<AbilityEffect> Effects => m_effects;
    }

    /// <summary>
    /// Represents an action applied by an ability.
    /// </summary>
    [Serializable]
    public class AbilityEffect
    {
        [SerializeField]
        private StatType m_statType;

        [SerializeField]
        private int m_value;

        public StatType Stat => m_statType;
        public int Value => m_value;
    }

    /// <summary>
    /// Defines the rarity tier of an item.
    /// </summary>
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Leggendary,
        OnlyInShop
    }

    /// <summary>
    /// The trigger moment for the item's ability.
    /// </summary>
    public enum AbilityTriggerMoment
    {
        Always,
        //OnUse,
        //OnCharacterDeath
    }

    /// <summary>
    /// The type of stat that is affected.
    /// </summary>
    public enum StatType
    {
        Hp,
        Shield,
        HpDamage,
        ShieldDamage,
        //Coins,
        EnergyRequired,
        WheelCount,
    }
}
