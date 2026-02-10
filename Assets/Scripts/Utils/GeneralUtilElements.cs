using UnityEngine;

namespace Util
{
    public class GeneralItilElements
    {
        public enum GameState
        {
            Betting,    // Betting phase
            Spinning,   // Closing bets and during roulette spin phase
            Result,     // Conclusion phase
            Payout      // Total calculation phase
        }
    }
}
