using System;
using UnityEngine;
using Sirenix.OdinInspector;
using Code.Systems.Locator;
using NoFeedProtocol.Runtime.UI;
using NoFeedProtocol.Runtime.Entities;
using NoFeedProtocol.Authoring.Characters;
using NoFeedProtocol.Runtime.Logic.Data;

namespace NoFeedProtocol.Runtime.Logic
{
    [HideMonoScript]
    public class MakePlayers : MonoBehaviour
    {
        [SerializeReference, InlineProperty, HideLabel]
        [TypeFilter("GetPlayerTypes")]
        private MakePlayer m_playerLeft;

        [SerializeReference, InlineProperty, HideLabel]
        [TypeFilter("GetPlayerTypes")]
        private MakePlayer m_playerRight;

        private static Type[] GetPlayerTypes()
        {
            return new[]
            {
                typeof(MakeHumanPlayer),
                typeof(MakeAIPlayer),
                typeof(MakeTestPlayer)
            };
        }
        public RunRuntimeData RunData => /*ServiceLocator.Get<>().RunRuntimeData*/null;

        private void Start()
        {
            m_playerLeft.Generate(this.transform);
            m_playerRight.Generate(this.transform);
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

        [BoxGroup("Characters Prefab")]
        [SerializeField, AssetsOnly]
        protected CharactersData m_charactersData;

        protected Transform Transform;

        public virtual void Generate(Transform transform)
        {
            Transform = transform;
        }

        protected GameObject Make(GameObject prefab, TransformData transform, CharacterRuntimeData data)
        {
            GameObject character = GameObject.Instantiate(prefab);

            transform.ApplyTo(character.transform);
            character.transform.parent = Transform;

            if (character.TryGetComponent(out CharacterInterface characterInterface) && data != null)
            {
                characterInterface.Init(data, m_charactersData);
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
        PlayerRuntimeData m_playerData;

        public override void Generate(Transform transform)
        {
            base.Generate(transform);

            this.m_playerData = ServiceLocator.Get<RuntimeDataStore>().GameData.Run.Player;

            Make(this.m_characterPrefab, this.m_topTransform, this.m_playerData.CharacterTop);
            Make(this.m_characterPrefab, this.m_bottomTransform, this.m_playerData.CharacterBottom);
        }
    }

    public class MakeAIPlayer : MakePlayer
    {
        [SerializeField, InlineProperty, HideLabel]
        private CharactersData m_enemies;

        public override void Generate(Transform transform)
        {
            base.Generate(transform);

            Make(this.m_characterPrefab, this.m_topTransform, GetRandomEnemy());
            Make(this.m_characterPrefab, this.m_bottomTransform, GetRandomEnemy());
        }

        private CharacterRuntimeData GetRandomEnemy()
        {
            CharacterData randomData = m_enemies.Characters[UnityEngine.Random.Range(0, m_enemies.Characters.Length)];

            return new CharacterRuntimeData
            {
                Id = randomData.Id,
                Health = randomData.MaxHealth,
                Energy = randomData.EnergyRequired
            };
        }

    }

    public class MakeTestPlayer : MakePlayer
    {
        [FoldoutGroup("Player")]
        [SerializeField, InlineProperty, HideLabel]
        private PlayerRuntimeData m_playerData;

        public override void Generate(Transform transform)
        {
            base.Generate(transform);

            Make(this.m_characterPrefab, this.m_topTransform, m_playerData.CharacterTop);
            Make(this.m_characterPrefab, this.m_bottomTransform, m_playerData.CharacterBottom);
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