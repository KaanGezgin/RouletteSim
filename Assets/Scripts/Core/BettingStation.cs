using UnityEngine;
using TMPro;

namespace Core
{
    public class BettingStation : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject interactionText;
        [SerializeField] private GameObject betWindowPanel;  

        private bool _isPlayerInZone = false;

        private void Start()
        {
            interactionText.SetActive(false);
            betWindowPanel.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInZone = true;
                interactionText.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _isPlayerInZone = false;
                interactionText.SetActive(false);
                betWindowPanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (_isPlayerInZone && Input.GetKeyDown(KeyCode.E))
            {
                ToggleBetWindow();
            }
        }

        private void ToggleBetWindow()
        {
            bool isActive = !betWindowPanel.activeSelf;
            betWindowPanel.SetActive(isActive);

            Cursor.visible = isActive;
            Cursor.lockState = isActive ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}