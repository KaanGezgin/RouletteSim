using System.Collections.Generic;
using System;
using UnityEngine;
using static Core.Betting.BetBase;
using static Core.Betting;
using static Util.GeneralItilElements;

namespace Core
{
    public class BetManager : MonoBehaviour
    {
        public static BetManager Instance { get; private set; }

        [Header("Wallet Settings")]
        [SerializeField] private int startingBalance = 1000;

        // UI Update Events
        public event Action<int> OnBalanceChanged;
        public event Action<int> OnCurrentBetChanged;
        public event Action<int> OnWinAmountCalculated;

        private int _currentBalance;
        private int _totalBetOnTable;
        private int _lastWinningNumber = -1; // Variable to hold the winning number
        private List<BetBase> _activeBets;

        public int TotalBetOnTable => _totalBetOnTable;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            _activeBets = new List<BetBase>();
            _currentBalance = PlayerPrefs.GetInt("PlayerBalance", startingBalance); // Load saved balance, otherwise use starting balance
        }

        private void Start()
        {
            // Event subscriptions
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinResultDetermined += HandleSpinResult;
            }

            UpdateUI();
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinResultDetermined -= HandleSpinResult;
            }
        }

        // --- Public API ---

        public bool PlaceBet(BetBase newBet)
        {
            if (GameManager.Instance.CurrentState != GameState.Betting)
            {
                Debug.LogWarning("[BetManager] Betting is closed!");
                return false;
            }

            if (_currentBalance >= newBet.Amount)
            {
                _currentBalance -= newBet.Amount;
                _totalBetOnTable += newBet.Amount;

                _activeBets.Add(newBet);

                UpdateUI();
                Debug.Log($"[BetManager] Bet Placed: {newBet.Type}, Amount: {newBet.Amount}");
                return true;
            }
            else
            {
                Debug.LogWarning("[BetManager] Insufficient Balance!");
                return false;
            }
        }

        public void ClearAllBets()
        {
            if (GameManager.Instance.CurrentState != GameState.Betting) return;

            foreach (var bet in _activeBets)
            {
                _currentBalance += bet.Amount;
            }

            _activeBets.Clear();
            _totalBetOnTable = 0;
            UpdateUI();
            Debug.Log("[BetManager] All bets on the table cleared.");
        }

        // --- Internal Logic ---

        // Triggered when the wheel stops, saves the result
        private void HandleSpinResult(int winningNumber)
        {
            _lastWinningNumber = winningNumber;
            Debug.Log($"[BetManager] Winning Number Recorded: {_lastWinningNumber}");
        }

        // Called by GameManager during Payout State
        public void ResolvePayouts()
        {
            if (_lastWinningNumber == -1)
            {
                Debug.LogError("[BetManager] Error: Proceeded to payout without a determined winning number!");
                return;
            }

            int totalWin = 0;
            RouletteDataSO data = GameManager.Instance.GetRouletteData();

            foreach (var bet in _activeBets)
            {
                if (bet.IsWin(_lastWinningNumber, data))
                {
                    int payout = bet.CalculatePayout();
                    totalWin += payout;
                    Debug.Log($"[BetManager] YOU WON! Bet: {bet.Type}, Payout: {payout}");
                }
            }

            _currentBalance += totalWin;
            OnWinAmountCalculated?.Invoke(totalWin);

            // Clear the table for the next round
            _activeBets.Clear();
            _totalBetOnTable = 0;
            _lastWinningNumber = -1; // Reset state

            UpdateUI();
            SaveGame();
        }

        private void UpdateUI()
        {
            OnBalanceChanged?.Invoke(_currentBalance);
            OnCurrentBetChanged?.Invoke(_totalBetOnTable);
        }

        private void SaveGame()
        {
            PlayerPrefs.SetInt("PlayerBalance", _currentBalance);
            PlayerPrefs.Save();
        }
    }
}