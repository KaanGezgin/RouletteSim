using UnityEngine;
using System;
using System.Linq;
using static Util.GeneralItilElements;


namespace Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private RouletteDataSO rouletteData;

        // State Events
        public event Action<GameState> OnStateChanged;
        public event Action<int> OnSpinResultDetermined;

        // Internal State
        public GameState CurrentState { get; private set; }
        private int _forcedResult = -1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            ChangeState(GameState.Betting);
        }

        // State machine
        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            Debug.Log($"[GameManager] State Changed: {newState}");
            OnStateChanged?.Invoke(newState);

            switch (newState)
            {
                case GameState.Betting:
                    // Cleaning the previous games data
                    _forcedResult = -1;
                    break;

                case GameState.Spinning:
                    DetermineResult();
                    break;

                case GameState.Result:
                    ChangeState(GameState.Payout);
                    break;

                case GameState.Payout:
                    // Bet calculation
                    Invoke(nameof(ResetRound), 3.0f);
                    break;
            }
        }

        // Logic
        public void SetDeterministicResult(int targetNumber)
        {
            // The number is in wheel control
            if (rouletteData.wheelSlots.Any(x => x.number == targetNumber))
            {
                _forcedResult = targetNumber;
                Debug.Log($"[GameManager] Next Result Forced to: {targetNumber}");
            }
            else
            {
                Debug.LogError($"[GameManager] Invalid number selected: {targetNumber}");
            }
        }

        private void DetermineResult()
        {
            int finalNumber;

            if (_forcedResult != -1)
            {
                // If the player choose the number
                finalNumber = _forcedResult;
            }
            else
            {
                // If the player not choose the number
                int randomIndex = UnityEngine.Random.Range(0, rouletteData.wheelSlots.Count);
                finalNumber = rouletteData.wheelSlots[randomIndex].number;
            }

            Debug.Log($"[GameManager] Result Determined: {finalNumber}");

            OnSpinResultDetermined?.Invoke(finalNumber);
        }

        // Spin button UI method
        public void StartSpin()
        {
            if (CurrentState != GameState.Betting) return;
            ChangeState(GameState.Spinning);
        }

        private void ResetRound()
        {
            ChangeState(GameState.Betting);
        }

        // Getter for roulette data 
        public RouletteDataSO GetRouletteData() => rouletteData;
    }
}