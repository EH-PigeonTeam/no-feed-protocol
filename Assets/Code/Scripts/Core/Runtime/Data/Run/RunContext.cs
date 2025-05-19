using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using PsychoGarden.Systems.Save;
using NoFeedProtocol.Runtime.Data.Items;
using NoFeedProtocol.Runtime.Data.Characters;

namespace NoFeedProtocol.Runtime.Data.Run
{
    [HideMonoScript]
    public class RunContext : MonoBehaviour
    {
        #region Fields ------------------------------------------------------

        [BoxGroup("Settings")]
        [Tooltip("The file name used to save the run")]
        [SerializeField]
        private string m_saveFileName = "run_data";

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField]
        private CharactersData m_charactersData;

        [BoxGroup("Settings")]
        [Tooltip("")]
        [SerializeField]
        private ItemsData m_itemsData;

        [ShowInInspector, ReadOnly]
        private RunData m_runData;

        #endregion

        #region Properties --------------------------------------------------

        public RunData RunData => m_runData;
        public bool HasValidRun => m_runData != null;

        #endregion

        #region Unity Methods -----------------------------------------------

        private void OnEnable()
        {
            ServiceLocator.Register<RunContext>(this);
            LoadData();
        }

        private void OnDisable()
        {
            ServiceLocator.Unregister<RunContext>();
        }

        #endregion

        #region Public Methods ----------------------------------------------

        public void Initialize(RunData runData)
        {
            m_runData = runData;
            SaveData();
        }

        public void SetRunData(RunData runData)
        {
            m_runData = runData;
            SaveData();
        }

        #endregion

        #region Save/Load ---------------------------------------------------

        private void LoadData()
        {
            RunSaveData saveData = SaveSystem.Load<RunSaveData>(m_saveFileName);
            m_runData = saveData != null ? RunSaveData.ToRuntime(saveData, this.m_charactersData, this.m_itemsData) : null;
        }

        private void SaveData()
        {
            if (m_runData == null) return;

            RunSaveData saveData = RunSaveData.FromRuntime(m_runData);
            SaveSystem.Save(saveData, m_saveFileName);
        }

        #endregion
    }
}
