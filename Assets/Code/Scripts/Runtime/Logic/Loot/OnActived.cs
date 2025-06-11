using UnityEngine;
using Sirenix.OdinInspector;
using PsychoGarden.TriggerEvents;

namespace NoFeedProtocol.Runtime.Logic.Loot
{
    [HideMonoScript]
    public class OnActived : MonoBehaviour
    {
        [BoxGroup("References", showLabel: false)]
        [SerializeField] TriggerEvent triggerEvent;

        private void OnEnable()
        {
            triggerEvent?.Invoke(this.transform);
        }
    }
}
