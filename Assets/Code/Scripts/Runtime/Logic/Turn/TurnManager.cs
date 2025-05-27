using System;
using Sirenix.OdinInspector;

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
        public TeamSide CurrentTurn => this.m_currentTurn;

        /// <summary>
        /// Starts the battle with the player's turn by default.
        /// </summary>
        public void StartFirstTurn()
        {
            this.m_currentTurn = TeamSide.Player;
        }

        /// <summary>
        /// Ends the current turn and passes control to the other team.
        /// </summary>
        public void NextTurn()
        {
            this.m_currentTurn = this.m_currentTurn == TeamSide.Player ? TeamSide.Enemy : TeamSide.Player;
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
