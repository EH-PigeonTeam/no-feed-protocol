using Code.Systems.Locator;
using NoFeedProtocol.Runtime.Logic.Data;
using UnityEngine;

public class FinishGame : MonoBehaviour
{
    public void Finish()
    {
        ServiceLocator.Get<RuntimeDataStore>().GameData.Run = null;
    }
}
