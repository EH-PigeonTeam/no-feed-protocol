using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.UI;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Authoring.Characters;
using NoFeedProtocol.Runtime.Logic.Data;
using NoFeedProtocol.Runtime.Logic.Battle;
using System.Collections.Generic;
using NoFeedProtocol.Runtime.Services.Characters;

namespace NoFeedProtocol.Runtime.Logic
{
    [HideMonoScript]
    public class MakePlayers : MonoBehaviour
    {
        [SerializeField, SceneObjectsOnly]
        private BattleManager m_battleData;

        [SerializeReference, InlineProperty, HideLabel]
        [TypeFilter("GetPlayerTypes")]
        private MakePlayer m_playerLeft;

        [SerializeReference, InlineProperty, HideLabel]
        [TypeFilter("GetPlayerTypes")]
        private MakePlayer m_playerRight;

#if UNITY_EDITOR
        private static Type[] GetPlayerTypes()
        {
            return new[]
            {
                typeof(MakeHumanPlayer),
                typeof(MakeAIPlayer),
                typeof(MakeTestPlayer)
            };
        }
#endif
        public RunRuntimeData RunData => ServiceLocator.Get<RuntimeDataStore>().GameData.Run;

        private void Start()
        {
            m_playerLeft.Generate(this.transform);
            m_playerRight.Generate(this.transform);

            m_battleData.BattleRuntimeData.Set(m_playerLeft.PlayerData, m_playerRight.PlayerData);
        }

        private void OnDrawGizmos()
        {
            this.m_playerLeft.OnDrawGizmos();
            this.m_playerRight.OnDrawGizmos();
        }
    }

    public class MakePlayer
    {
        [FoldoutGroup("Top Character Transform")]
        [SerializeField, InlineProperty, HideLabel]
        protected TransformData m_topTransform;

        [FoldoutGroup("Bottom Character Transform")]
        [SerializeField, InlineProperty, HideLabel]
        protected TransformData m_bottomTransform;

        [BoxGroup("Characters Prefab")]
        [SerializeField, AssetsOnly]
        protected GameObject m_characterPrefab;

        protected Transform Transform;

        public PlayerRuntimeData PlayerData { get; protected set; }

        public virtual void Generate(Transform transform)
        {
            Transform = transform;
        }

        protected GameObject Make(GameObject prefab, TransformData transform, CharacterWrapper data, CharacterRuntimeData characterRuntimeData = null)
        {
            GameObject character = GameObject.Instantiate(prefab);

            transform.ApplyTo(character.transform);
            character.transform.parent = Transform;

            if (character.TryGetComponent(out CharacterInterface characterInterface) && data != null)
            {
                switch (data)
                {
                    case CharacterData characterData:
                        characterInterface.Init(characterData, characterRuntimeData);
                        break;
                    case CharacterEnemyData enemyData:
                        characterInterface.Init(enemyData, characterRuntimeData);
                        break;
                    default:
                        Debug.LogWarning($"[MakePlayer] Unsupported character config type: {data.GetType()}");
                        break;
                }
            }

            return character;
        }

        public virtual void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.m_topTransform.Position, 0.25f);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(this.m_bottomTransform.Position, 0.25f);
        }
    }

    public class MakeHumanPlayer : MakePlayer
    {
        public override void Generate(Transform transform)
        {
            base.Generate(transform);

            var runData = ServiceLocator.Get<RuntimeDataStore>().GameData.Run.Player;

            CharacterResolver resolver = ServiceLocator.Get<CharacterResolver>();

            Make(m_characterPrefab, m_topTransform, resolver.GetById(runData.CharacterTop.Id), runData.CharacterTop);
            Make(m_characterPrefab, m_bottomTransform, resolver.GetById(runData.CharacterBottom.Id), runData.CharacterBottom);

            PlayerData = new PlayerRuntimeData
            {
                CharacterTop = runData.CharacterTop,
                CharacterBottom = runData.CharacterBottom,
                CurrentShield = runData.CurrentShield,
                Coins = runData.Coins,
                Items = new List<string>(runData.Items)
            };
        }
    }

    public class MakeAIPlayer : MakePlayer
    {
        public override void Generate(Transform transform)
        {
            base.Generate(transform);

            var enemy1 = GetRandomEnemy();
            var enemy2 = GetRandomEnemy();

            CharacterEnemyResolver resolver = ServiceLocator.Get<CharacterEnemyResolver>();

            Make(m_characterPrefab, m_topTransform, resolver.GetById(enemy1.Id), enemy1);
            Make(m_characterPrefab, m_bottomTransform, resolver.GetById(enemy2.Id), enemy2);

            PlayerData = new PlayerRuntimeData
            {
                CharacterTop = enemy1,
                CharacterBottom = enemy2,
                CurrentShield = 10,
                Coins = 0,
                Items = new List<string>() // empty -> needs to be implemented
            };
        }

        private CharacterRuntimeData GetRandomEnemy()
        {
            var resolver = ServiceLocator.Get<CharacterEnemyResolver>();
            var enemies = resolver.GetAll();
            var random = enemies[UnityEngine.Random.Range(0, enemies.Count)];

            return new CharacterRuntimeData
            {
                Id = random.Id,
                Health = random.MaxHealth,
                Energy = 0
            };
        }
    }

    public class MakeTestPlayer : MakePlayer
    {
        [FoldoutGroup("Player")]
        [SerializeField, InlineProperty, HideLabel]
        private PlayerRuntimeData m_playerData;

        CharacterResolver resolver = ServiceLocator.Get<CharacterResolver>();

        public override void Generate(Transform transform)
        {
            base.Generate(transform);

            Make(this.m_characterPrefab, this.m_topTransform, resolver.GetById(m_playerData.CharacterTop.Id));
            Make(this.m_characterPrefab, this.m_bottomTransform, resolver.GetById(m_playerData.CharacterBottom.Id));
        }
    }

    [Serializable]
    public struct TransformData
    {
        public Vector3 Position;
        public Vector3 Rotation; // Euler angles
        public Vector3 Scale;

        public static TransformData FromTransform(Transform t)
        {
            return new TransformData
            {
                Position = t.position,
                Rotation = t.eulerAngles,
                Scale = t.localScale
            };
        }

        public void ApplyTo(Transform t)
        {
            t.position = Position;
            t.eulerAngles = Rotation;
            t.localScale = Scale;
        }
    }

}