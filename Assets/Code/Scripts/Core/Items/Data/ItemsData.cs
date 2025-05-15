using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.Reflection;

namespace Core.Gameplay.Items.Data
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "ItemsData", menuName = "No Feed Protocol/ItemsData")]
    public class ItemsData : ScriptableObject
    {
        [BoxGroup("Items")]
        [Tooltip("Items in the game")]
        [SerializeField]
        private Item[] m_items;

        public Item[] Items => m_items;
        public Item GetItem(int index) => m_items[index];
    }

    [System.Serializable]
    public class Item
    {
        [FoldoutGroup("@m_name")]
        [Tooltip("The name of the item")]
        [SerializeField]
        private string m_name;

        [FoldoutGroup("@m_name")]
        [Tooltip("The icon of the item")]
        [SerializeField, PreviewField(100)]
        private Sprite m_icon;

        [FoldoutGroup("@m_name")]
        [Tooltip("The description of the item")]
        [SerializeField, TextArea(4, 10)]
        private string m_description;

        [FoldoutGroup("@m_name")]
        [Tooltip("The percentage of the item can appear")]
        [SerializeField, Range(0, 1)]
        private float m_Percent = 1f;

        // Logic
        [FoldoutGroup("@m_name")]
        [Tooltip("The percentage of the item can appear")]
        [SerializeField, InlineProperty, HideLabel]
        private Ability ability;

        public string Name => m_name;
        public Sprite Icon => m_icon;
        public string Description => m_description;
        public float Percent => m_Percent;
        public Ability GetAbility => ability;
    }

    [System.Serializable]
    public class Ability
    {
        [FoldoutGroup("Ability")]
        [Tooltip("")]
        [SerializeField]
        public AbilityTriggerMoment TriggerMoment;

        [FoldoutGroup("Ability")]
        [Tooltip("")]
        [SerializeField, InlineProperty, HideLabel]
        public AbilityCondition Condition;

        [FoldoutGroup("Ability")]
        [Tooltip("")]
        [SerializeField]
        public List<AbilityEffect> Effects;
    }

    [System.Serializable]
    public class AbilityCondition
    {
        [FoldoutGroup("AbilityCondition")]
        [Tooltip("")]
        [SerializeField]
        public AbilityConditionType Type;

        [FoldoutGroup("AbilityCondition")]
        [Tooltip("")]
        [SerializeField]
        public AbilityTargetType Target; 
        
        [FoldoutGroup("AbilityCondition")]
        [Tooltip("")]
        [SerializeField]
        public int Value;
    }

    [System.Serializable]
    public class AbilityEffect
    {
        [FoldoutGroup("$Type")]
        [Tooltip("")]
        [SerializeField]
        public AbilityEffectType Type;

        [FoldoutGroup("$Type")]
        [Tooltip("")]
        [SerializeField]
        public AbilityEffectAction Action;

        [FoldoutGroup("$Type")]
        [Tooltip("")]
        [SerializeField]
        public int Value;

        [FoldoutGroup("$Type")]
        [Tooltip("")]
        [SerializeField]
        public AbilityTargetType Target;
    }

    public enum AbilityTriggerMoment
    {
        Always,
        //OnUse
    }

    public enum AbilityConditionType
    {
        OwnsItem,
        //TurnsPassed,
        //OnDeathCharacter,
        //OnDeathEnemy,
        //OnHpBelowCharacter,
        //OnHpBelowEnemy,
        //OnShieldBelowCharacters,
        //OnShieldBelowEnemies,
        //WheelSpin,
        //WheelResultsProvided,
        //WheelLocked,
        //WheelRotationsRemaining,
        //WheelPointsProvided,
        //OnActCharacter,
        //OnActEnemy,
        //ConsumableUsed,
        //OnGain,
        //OnLoss
    }

    public enum AbilityEffectType
    {
        Health,
        Shield,
        ActionPoints,
        DamageHealth,
        DamageShield,
        Coins,
        //Movement,
        Wheels,
        //HealHealth,
        //HealShield
    }

    public enum AbilityEffectAction
    {
        Gain,
        Remove
    }

    public enum AbilityTargetType
    {
        Self,
        //Ally,
        //Enemy,
        //AllAllies,
        //AllEnemies,
        //AllCharacters,
        //SpecificCharacter,
        //SpecificEnemy,
        SlotMachine
    }
}
