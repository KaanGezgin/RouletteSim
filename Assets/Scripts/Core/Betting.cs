using UnityEngine;

namespace Core
{
    public class Betting
    {
        public enum BetType { Straight, Color, Parity, Range, Dozen, Column }

        // Bet Color 
        public enum BetColor { Red, Black }

        // Odd or even choice
        public enum BetParity { Odd, Even }

        // --- Abstract Base Class ---
        //All bets are derived from this class.
        [System.Serializable]
        public abstract class BetBase
        {
            public int Amount { get; protected set; }
            public BetType Type { get; protected set; }

            public BetBase(int amount, BetType type)
            {
                this.Amount = amount;
                this.Type = type;
            }

            // Bets win con control
            public abstract bool IsWin(int winningNumber, RouletteDataSO data);

            // Gain multiplier
            public abstract int GetMultiplier();

            // FÝnal calculation
            public int CalculatePayout()
            {
                // Standart roulette rule: money on table + (money on table * multiplier)
                return Amount + (Amount * GetMultiplier());
            }

            // 1. Straight Bet
            public class StraightBet : BetBase
            {
                private int _targetNumber;

                public StraightBet(int amount, int targetNumber) : base(amount, BetType.Straight)
                {
                    _targetNumber = targetNumber;
                }

                public override bool IsWin(int winningNumber, RouletteDataSO data)
                {
                    return winningNumber == _targetNumber;
                }

                public override int GetMultiplier() => 35; // 35:1
            }

            // 2. Color Bet
            public class ColorBet : BetBase
            {
                private NumberColor _targetColor;

                public ColorBet(int amount, NumberColor targetColor) : base(amount, BetType.Color)
                {
                    _targetColor = targetColor;
                }

                public override bool IsWin(int winningNumber, RouletteDataSO data)
                {
                    // 0 generally is green. Line below is green control
                    return data.GetColorOfNumber(winningNumber) == _targetColor;
                }

                public override int GetMultiplier() => 1; // 1:1
            }

            // 3. Parity Bet (Odd/Even)
            public class ParityBet : BetBase
            {
                private BetParity _targetParity;

                public ParityBet(int amount, BetParity parity) : base(amount, BetType.Parity)
                {
                    _targetParity = parity;
                }

                public override bool IsWin(int winningNumber, RouletteDataSO data)
                {
                    if (winningNumber == 0) return false; // 0 is either odd or even (Roulette rules)

                    bool isEven = (winningNumber % 2 == 0);
                    return (_targetParity == BetParity.Even && isEven) ||
                           (_targetParity == BetParity.Odd && !isEven);
                }

                public override int GetMultiplier() => 1; // 1:1
            }
        }


    }
}
