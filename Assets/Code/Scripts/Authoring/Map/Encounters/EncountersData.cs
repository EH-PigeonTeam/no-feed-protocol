using UnityEngine;
using Sirenix.OdinInspector;
using System.Linq;

namespace NoFeedProtocol.Authoring.Map
{
    [HideMonoScript]
    [CreateAssetMenu(fileName = "NewEncounter", menuName = "No Feed Protocol/Encounters Data")]
    public class EncountersData : ScriptableObject
    {
        [SerializeField]
        private EncounterData[] m_encounters;

#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var encounter in m_encounters)
                encounter.OnValidate();
        }
#endif

        public EncounterData[] Encounters => m_encounters;
        public EncounterData GetEncounter(string id) => m_encounters.FirstOrDefault(x => x.Id == id);
    }
}
