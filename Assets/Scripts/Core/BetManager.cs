using System.Collections.Generic;
using System;
using UnityEngine;
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
        public event Action<int> OnBalanceChanged; // Current wallet
        public event Action<int> OnCurrentBetChanged; // On table bets
        public event Action<int> OnWinAmountCalculated; // final calculated amount

        private int _currentBalance;
        private int _totalBetOnTable;
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
            _currentBalance = startingBalance;
        }

        private void Start()
        {
            GameManager.Instance.OnSpinResultDetermined += HandleSpinResult;
            GameManager.Instance.OnStateChanged += HandleStateChange;

            UpdateUI(); // UI setup
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinResultDetermined -= HandleSpinResult;
                GameManager.Instance.OnStateChanged -= HandleStateChange;
            }
        }

        // --- Public API (For UI) ---

        // Bet method
        public bool PlaceBet(BetBase newBet)
        {
            if (GameManager.Instance.CurrentState != GameState.Betting)
            {
                return false;
            }

            if (_currentBalance >= newBet.Amount)
            {
                _currentBalance -= newBet.Amount;
                _totalBetOnTable += newBet.Amount;

                _activeBets.Add(newBet);

                UpdateUI();
                Debug.Log($"Bet Made: {newBet.Type}, Amount: {newBet.Amount}");
                return true;
            }
            else
            {
                Debug.LogWarning("Insufficient Balance!");
                return false;
            }
        }

        // Clear Table
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
        }

        // --- Internal Logic ---

        // When ball choose a number
        private void HandleSpinResult(int winningNumber)
        {

        }

        private void HandleStateChange(GameState newState)
        {
            if (newState == GameState.Payout)
            {
                CalculateWinnings(GameManager.Instance.GetRouletteData());
            }
        }

        private void CalculateWinnings(RouletteDataSO data)
        {
            int winningNumber = -1;
        
        }

        private int _lastWinningNumber = -1;

        private void HandleSpinResult_Revised(int winningNumber)
        {
            _lastWinningNumber = winningNumber;
        }

        public void ResolvePayouts()
        {
            if (_lastWinningNumber == -1) return;

            int totalWin = 0;
            RouletteDataSO data = GameManager.Instance.GetRouletteData();

            foreach (var bet in _activeBets)
            {
                if (bet.IsWin(_lastWinningNumber, data))
                {
                    int payout = bet.CalculatePayout();
                    totalWin += payout;
                    Debug.Log($"KAZANDIN! Bahis: {bet.Type}, Ödeme: {payout}");
                }
            }

            _currentBalance += totalWin;
            OnWinAmountCalculated?.Invoke(totalWin);

            _activeBets.Clear();
            _totalBetOnTable = 0;

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