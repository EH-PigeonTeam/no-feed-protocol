using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace NoFeelProtocol.Runtime.Data.Items
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

        /// <summary>
        /// Finds an item by its unique ID.
        /// </summary>
        public Item GetById(string id)
        {
            foreach (var item in m_items)
            {
                if (item.Id == id)
                    return item;
            }

            Debug.LogWarning($"Item ID '{id}' not found.");
            return null;
        }
    }

    [System.Serializable]
    public class Item : ISerializationCallbackReceiver
    {
        #region Unique ID ---------------------------------------------------

        [HideInInspector]
        [SerializeField]
        private string m_id;

        /// <summary>
        /// Unique, non-editable ID used for lookup and save data.
        /// </summary>
        public string Id => m_id;

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (string.IsNullOrEmpty(m_id))
            {
                m_id = Guid.NewGuid().ToString();
                EditorUtility.SetDirty(Selection.activeObject);
            }
        }
#endif

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() { }

        #endregion

        #region Basic Info --------------------------------------------------

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

        #endregion

        #region Public Properties -------------------------------------------

        public string Name => m_name;
        public Sprite Icon => m_icon;
        public string Description => m_description;
        public float Percent => m_Percent;
        public Ability GetAbility => ability;

        #endregion
    }

    [System.Serializable]
    public class Ability
    {
        [FoldoutGroup("Ability")]
        [SerializeField]
        public AbilityTriggerMoment TriggerMoment;

        [FoldoutGroup("Ability")]
        [SerializeField, InlineProperty, HideLabel]
        public AbilityCondition Condition;

        [FoldoutGroup("Ability")]
        [SerializeField]
        public List<AbilityEffect> Effects;
    }

    [System.Serializable]
    public class AbilityCondition
    {
        [FoldoutGroup("AbilityCondition")]
        [SerializeField]
        public AbilityConditionType Type;

        [FoldoutGroup("AbilityCondition")]
        [SerializeField]
        public AbilityTargetType Target;

        [FoldoutGroup("AbilityCondition")]
        [SerializeField]
        public int Value;
    }

    [System.Serializable]
    public class AbilityEffect
    {
        [FoldoutGroup("$Type")]
        [SerializeField]
        public AbilityEffectType Type;

        [FoldoutGroup("$Type")]
        [SerializeField]
        public AbilityEffectAction Action;

        [FoldoutGroup("$Type")]
        [SerializeField]
        public int Value;

        [FoldoutGroup("$Type")]
        [SerializeField]
        public AbilityTargetType Target;
    }

    public enum AbilityTriggerMoment
    {
        Always
    }

    public enum AbilityConditionType
    {
        OwnsItem,
        TurnsPassed,
        OnDeathCharacter,
        OnDeathEnemy,
        OnHpBelowCharacter,
        OnHpBelowEnemy,
        OnShieldBelowCharacters,
        OnShieldBelowEnemies,
        WheelSpin,
        WheelResultsProvided,
        WheelLocked,
        WheelRotationsRemaining,
        WheelPointsProvided,
        OnActCharacter,
        OnActEnemy,
        ConsumableUsed,
        OnGain,
        OnLoss
    }

    public enum AbilityEffectType
    {
        Health,
        Shield,
        ActionPoints,
        DamageHealth,
        DamageShield,
        Coins,
        Wheels,
        HealHealth,
        HealShield
    }

    public enum AbilityEffectAction
    {
        Gain,
        Remove
    }

    public enum AbilityTargetType
    {
        Self,
        Ally,
        Enemy,
        AllAllies,
        AllEnemies,
        AllCharacters,
        SpecificCharacter,
        SpecificEnemy,
        SlotMachine
    }
}
