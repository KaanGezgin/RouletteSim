using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using Core;
using static Core.Betting.BetBase;
using static Core.Betting;

public class BetWindowUI : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private TMP_InputField amountInput;
    [SerializeField] private TMP_Dropdown betTypeDropdown;
    [SerializeField] private TMP_Dropdown colorDropdown;
    [SerializeField] private TMP_Dropdown parityDropdown;
    [SerializeField] private TMP_InputField straightNumberInput; 

    [Header("Action Buttons")]
    [SerializeField] private Button placeBetButton;
    [SerializeField] private Button spinButton;

    private void Start()
    {
        SetupDropdowns();

        placeBetButton.onClick.AddListener(OnPlaceBetClicked);
        spinButton.onClick.AddListener(() => GameManager.Instance.StartSpin());

        betTypeDropdown.onValueChanged.AddListener(OnBetTypeChanged);
        OnBetTypeChanged(0); 
    }

    private void SetupDropdowns()
    {
        // Bet Types
        betTypeDropdown.ClearOptions();
        betTypeDropdown.AddOptions(new List<string> { "Straight", "Color", "Parity" });

        // Colors
        colorDropdown.ClearOptions();
        colorDropdown.AddOptions(new List<string> { "Red", "Black" });

        // Parity
        parityDropdown.ClearOptions();
        parityDropdown.AddOptions(new List<string> { "Odd", "Even" });
    }

    private void OnBetTypeChanged(int index)
    {
        straightNumberInput.gameObject.SetActive(index == 0);
        colorDropdown.gameObject.SetActive(index == 1);
        parityDropdown.gameObject.SetActive(index == 2);
    }

    private void OnPlaceBetClicked()
    {
        if (!int.TryParse(amountInput.text, out int amount) || amount <= 0)
        {
            Debug.LogWarning("Invalid Amount!");
            return;
        }

        BetBase newBet = null;
        int typeIndex = betTypeDropdown.value;

        switch (typeIndex)
        {
            case 0: // Straight
                if (int.TryParse(straightNumberInput.text, out int targetNum))
                {
                    newBet = new StraightBet(amount, targetNum);
                }
                break;

            case 1: // Color
                NumberColor color = (colorDropdown.value == 0) ? NumberColor.Red : NumberColor.Black; // 0=Red
                newBet = new ColorBet(amount, color);
                break;

            case 2: // Parity
                BetParity parity = (parityDropdown.value == 0) ? BetParity.Odd : BetParity.Even; // 0=Odd
                newBet = new ParityBet(amount, parity);
                break;
        }

        if (newBet != null)
        {
            bool success = BetManager.Instance.PlaceBet(newBet);
            if (success) Debug.Log("The bet was placed successfully!");
        }
    }
}