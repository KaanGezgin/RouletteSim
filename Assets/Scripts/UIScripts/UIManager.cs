using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Core;
using static Util.GeneralItilElements;

namespace UIScripts
{
    public class UIManager : MonoBehaviour
    {
        [Header("HUD References")]
        [SerializeField] private TextMeshProUGUI balanceText;
        [SerializeField] private TextMeshProUGUI currentBetText;
        [SerializeField] private TextMeshProUGUI resultText;

        private int _lastWinningNumber;

        private void Start()
        {
            BetManager.Instance.OnBalanceChanged += UpdateBalance;
            BetManager.Instance.OnCurrentBetChanged += UpdateCurrentBet;

            GameManager.Instance.OnStateChanged += HandleStateChange;
            GameManager.Instance.OnSpinResultDetermined += SetPendingResult;

             resultText.text = "Place Your Bets";
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
                GameManager.Instance.OnStateChanged -= HandleStateChange;
                GameManager.Instance.OnSpinResultDetermined -= SetPendingResult;
            }
        }

        private void HandleStateChange(GameState newState)
        {
            switch (newState)
            {
                case GameState.Betting:
                     resultText.text = "Place Your Bets"; 
                    break;

                case GameState.Spinning:
                     resultText.text = "Spinning..."; 
                    break;

                case GameState.Result:
                    var color = GameManager.Instance.GetRouletteData().GetColorOfNumber(_lastWinningNumber);
                    resultText.text = $"Result: <color={color.ToString().ToLower()}>{_lastWinningNumber}</color>";
                    break;

                case GameState.Payout:
                    break;
            }
        }

        private void SetPendingResult(int winningNumber)
        {
            _lastWinningNumber = winningNumber;
        }

        private void UpdateBalance(int newBalance)
        {
            balanceText.text = $"{newBalance}"; 
        }

        private void UpdateCurrentBet(int newBet)
        {
             currentBetText.text = $"{newBet}"; 
        }
    }
}