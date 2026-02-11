using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Core;

namespace UIScripts
{
    public class StatisticsUI : MonoBehaviour
    {
        [Header("Text References")]
        [SerializeField] private TextMeshProUGUI statsText;

        private void Start()
        {
            StatisticsManager.Instance.OnStatsUpdated += UpdateUI;

            UpdateUI();
        }

        private void OnDestroy()
        {
            if (StatisticsManager.Instance != null)
                StatisticsManager.Instance.OnStatsUpdated -= UpdateUI;
        }

        private void UpdateUI()
        {
            var stats = StatisticsManager.Instance;
            statsText.text = $"Spins: {stats.TotalSpins}\nWins: {stats.TotalWins} | Losses: {stats.TotalLosses}\nNet: {stats.NetProfit}";

            statsText.color = stats.NetProfit >= 0 ? Color.green : Color.red;
        }

   
    }
}