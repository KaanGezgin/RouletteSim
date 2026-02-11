using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Core
{
    public class StatisticsManager : MonoBehaviour
    {
        public static StatisticsManager Instance { get; private set; }

        // --- Data Model ---
        [Header("Read Only Stats")]
        public int TotalSpins;
        public int TotalWins;
        public int TotalLosses;
        public int NetProfit;

        public List<int> MatchHistory = new List<int>();

        public System.Action OnStatsUpdated;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadStats(); 
        }

        private void Start()
        {
            // Eventleri Dinle
            if (GameManager.Instance != null)
                GameManager.Instance.OnSpinResultDetermined += RecordSpin;

            if (BetManager.Instance != null)
                BetManager.Instance.OnWinAmountCalculated += RecordFinancials;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnSpinResultDetermined -= RecordSpin;

            if (BetManager.Instance != null)
                BetManager.Instance.OnWinAmountCalculated -= RecordFinancials;
        }

        private void RecordSpin(int number)
        {
            TotalSpins++;

            MatchHistory.Insert(0, number);
            if (MatchHistory.Count > 10)
            {
                MatchHistory.RemoveAt(MatchHistory.Count - 1);
            }

            SaveStats();
            OnStatsUpdated?.Invoke();
        }

        private void RecordFinancials(int winAmount)
        {
            int currentBet = BetManager.Instance.TotalBetOnTable; 

            int profit = winAmount - currentBet;
            NetProfit += profit;

            if (profit > 0)
                TotalWins++;
            else
                TotalLosses++;

            SaveStats();
            OnStatsUpdated?.Invoke();
        }

        // --- Persistence (Save/Load) ---
        private void SaveStats()
        {
            PlayerPrefs.SetInt("Stats_Spins", TotalSpins);
            PlayerPrefs.SetInt("Stats_Wins", TotalWins);
            PlayerPrefs.SetInt("Stats_Losses", TotalLosses);
            PlayerPrefs.SetInt("Stats_Profit", NetProfit);

            string historyStr = string.Join(",", MatchHistory);
            PlayerPrefs.SetString("Stats_History", historyStr);

            PlayerPrefs.Save();
        }

        private void LoadStats()
        {
            TotalSpins = PlayerPrefs.GetInt("Stats_Spins", 0);
            TotalWins = PlayerPrefs.GetInt("Stats_Wins", 0);
            TotalLosses = PlayerPrefs.GetInt("Stats_Losses", 0);
            NetProfit = PlayerPrefs.GetInt("Stats_Profit", 0);

            string historyStr = PlayerPrefs.GetString("Stats_History", "");
            if (!string.IsNullOrEmpty(historyStr))
            {
                MatchHistory = historyStr.Split(',')
                    .Select(s => int.Parse(s))
                    .ToList();
            }
        }
    }
}