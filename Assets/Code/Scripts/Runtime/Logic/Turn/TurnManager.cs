using System;
using UnityEngine;
using Sirenix.OdinInspector;
using NoFeedProtocol.Runtime.Logic.Data;
//using NoFeedProtocol.Runtime.Logic.Events;

namespace NoFeedProtocol.Runtime.Logic.Turns
{
    /// <summary>
    /// Manages turn flow between the player and the enemy.
    /// </summary>
    [Serializable]
    public class TurnManager
    {
        [ShowInInspector, ReadOnly]
        private TeamSide m_currentTurn;

        /// <summary>
        /// Current active team.
        /// </summary>
        public TeamSide CurrentTurn => m_currentTurn;

        /// <summary>
        /// Starts the battle with the player's turn by default.
        /// </summary>
        public void StartFirstTurn()
        {
            m_currentTurn = TeamSide.Player;
            //BattleEventSystem.Invoke(BattleEventType.OnTurnStart, new BattleContext(m_currentTurn));
        }

        /// <summary>
        /// Ends the current turn and passes control to the other team.
        /// </summary>
        public void NextTurn()
        {
            m_currentTurn = m_currentTurn == TeamSide.Player ? TeamSide.Enemy : TeamSide.Player;
            //BattleEventSystem.Invoke(BattleEventType.OnTurnStart, new BattleContext(m_currentTurn));
        }
    }

    /// <summary>
    /// Indicates which team currently has control.
    /// </summary>
    public enum TeamSide
    {
        Player,
        Enemy
    }
}
