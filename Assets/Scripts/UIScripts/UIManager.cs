using UnityEngine;
using TMPro;
using UnityEngine.UI;
using static Util.GeneralItilElements;
using Core;
namespace UIScripts
{
    public class UIManager : MonoBehaviour
    {
        [Header("HUD References")]
        [SerializeField] private TextMeshProUGUI balanceText;
        [SerializeField] private TextMeshProUGUI currentBetText;
        [SerializeField] private TextMeshProUGUI resultText;

        private void Start()
        {
            BetManager.Instance.OnBalanceChanged += UpdateBalance;
            BetManager.Instance.OnCurrentBetChanged += UpdateCurrentBet;
            GameManager.Instance.OnSpinResultDetermined += ShowThinking; 

        }

        private void OnDestroy()
        {
            if (BetManager.Instance != null)
            {
                BetManager.Instance.OnBalanceChanged -= UpdateBalance;
                BetManager.Instance.OnCurrentBetChanged -= UpdateCurrentBet;
            }
            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnSpinResultDetermined -= ShowThinking;
            }
        }


        private void UpdateBalance(int newBalance)
        {
            balanceText.text = $"Balance: {newBalance}";
        }

        private void UpdateCurrentBet(int newBet)
        {
            currentBetText.text = $"Bet: {newBet}";
        }

        private void ShowThinking(int targetNumber)
        {
            resultText.text = "Spinning...";
        }


    }
}

