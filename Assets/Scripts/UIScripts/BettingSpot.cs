using UnityEngine;
using static Core.Betting.BetBase;
using static Core.Betting;

namespace UIScripts
{
    public class BettingSpot : MonoBehaviour
    {
        [Header("Bet Definition")]
        public BetType betType; 

        public int valueParam;
        public NumberColor colorParam; 
        public BetParity parityParam; 

        [Header("Visuals")]
        public Transform chipSpawnPoint;
        public BetBase CreateBetData(int amount)
        {
            switch (betType)
            {
                case BetType.Straight:
                    return new StraightBet(amount, valueParam);
                case BetType.Color:
                    return new ColorBet(amount, colorParam);
                case BetType.Parity:
                    return new ParityBet(amount, parityParam);
                default:
                    return null;
            }
        }
    }
}