using NoFeelProtocol.Runtime.Data.Characters;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Core.Selection_Characters
{
    [HideMonoScript]
    public class CharacterDataExposer : MonoBehaviour
    {
        [BoxGroup("Settings")]
        [FoldoutGroup("Settings/References")]
        [Tooltip("The name of the character")]
        [SerializeField, Required]
        private TMP_Text m_name;

        [FoldoutGroup("Settings/References")]
        [Tooltip("The description of the character")]
        [SerializeField, Required]
        private TMP_Text m_description;

        [FoldoutGroup("Settings/References/Stats")]
        [Tooltip("The health of the character")]
        [SerializeField, Required]
        private StatSliderController m_health;

        [FoldoutGroup("Settings/References/Stats")]
        [Tooltip("The shield of the character")]
        [SerializeField, Required]
        private StatSliderController m_shield;

        [FoldoutGroup("Settings/References/Stats")]
        [Tooltip("The attack shield of the character")]
        [SerializeField, Required]
        private StatSliderController m_attackShield;

        [FoldoutGroup("Settings/References/Stats")]
        [Tooltip("The attack of the character")]
        [SerializeField, Required]
        private StatSliderController m_attack;

        [FoldoutGroup("Settings/References/Stats")]
        [Tooltip("The energy required to charge the attack of the character")]
        [SerializeField, Required]
        private StatSliderController m_chargePoints;

        public void Display(string name, string description, int health, int shield, int attackShield, int attack, int chargePoints)
        {
            this.m_name.text = name;
            this.m_description.text = description;
            this.m_health.SetValue(health, 100);
            this.m_shield.SetValue(shield, 100);
            this.m_attackShield.SetValue(attackShield, 100);
            this.m_attack.SetValue(attack, 100);
            this.m_chargePoints.SetValue(chargePoints, 10);
        }

        public void Display(CharacterData character)
        {
            this.m_name.text = character.Name;
            this.m_description.text = character.Description;
            this.m_health.SetValue(character.MaxHealth, 200);
            this.m_shield.SetValue(character.Shield, 200);
            this.m_attackShield.SetValue(character.AttackPointsShield, 200);
            this.m_attack.SetValue(character.AttackPoints, 200);
            this.m_chargePoints.SetValue(character.ActionPoints, 10);
        }
    }
}
