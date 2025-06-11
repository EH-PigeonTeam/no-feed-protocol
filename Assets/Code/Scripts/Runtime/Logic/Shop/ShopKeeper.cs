using Code.Systems.Locator;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace NoFeedProtocol.Runtime.Logic.Shop
{
    [HideMonoScript]
    public class ShopKeeper : MonoBehaviour
    {
        #region Fields -----------

        public List<string> ItemsSelected;
        public int Cash;
        public int Price;

        #endregion

        #region Init -------------

        private void Awake()
        {
            ServiceLocator.Register(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Unregister<ShopKeeper>();
        }

        #endregion

        #region Methods -----------

        public bool HasCash => (Cash - Price) > 0f;

        public void AddItem(string item) => ItemsSelected.Add(item);

        //add price?

        //public void Buy()
        //{
        //    Cash -= Price;
        //}

        #endregion

    }
}

