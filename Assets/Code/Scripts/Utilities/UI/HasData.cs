using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Data;
using UnityEngine.UI;

namespace NoFeedProtocol.Runtime.UI
{
    [HideMonoScript]
    [RequireComponent(typeof(Button))]
    public class HasData : MonoBehaviour
    {
        private void Start()
        {
            if (!ServiceLocator.Get<RuntimeDataStore>().HasRun)
            {
                this.GetComponent<Button>().interactable = false;
            }
        }
    }
}
