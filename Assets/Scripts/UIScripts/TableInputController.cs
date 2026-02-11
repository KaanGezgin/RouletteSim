using Core;
using UIScripts;
using UnityEngine;
using static Core.Betting;
using static Util.GeneralItilElements;
using UnityEngine.EventSystems;

namespace UIScripts
{
    public class TableInputController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private LayerMask tableLayer;
        [SerializeField] private GameObject chipPrefab; 

        private int _selectedChipAmount = 10;

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;

            if (GameManager.Instance.CurrentState != GameState.Betting) return;

            if (Input.GetMouseButtonDown(0)) 
            {
                HandleClick();
            }
        }

        private void HandleClick()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, tableLayer))
            {
                BettingSpot spot = hit.collider.GetComponent<BettingSpot>();
                if (spot != null)
                {
                    PlaceBetOnSpot(spot);
                }
            }
        }

        private void PlaceBetOnSpot(BettingSpot spot)
        {
            BetBase newBet = spot.CreateBetData(_selectedChipAmount);

            bool success = BetManager.Instance.PlaceBet(newBet);

            if (success)
            {
                SpawnChipVisual(spot.chipSpawnPoint.position);
            }
        }

        private void SpawnChipVisual(Vector3 position)
        {
            Instantiate(chipPrefab, position, Quaternion.identity);
        }
        public void SetChipAmount(int amount)
        {
            _selectedChipAmount = amount;
        }
    }
}