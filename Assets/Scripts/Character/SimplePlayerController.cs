using UnityEngine;
using UnityEngine.InputSystem;

namespace Character
{
    public class SimplePlayerController : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float lookSensitivity = 0.5f;

        private Rigidbody _rb;
        private float _rotationX = 0f;

        private Vector2 _moveInput;
        private Vector2 _lookInput;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Cursor.visible) return;

            _moveInput = Keyboard.current != null ?
                new Vector2(
                    (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0),
                    (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0)
                ) : Vector2.zero;

            _lookInput = Mouse.current != null ? Mouse.current.delta.ReadValue() : Vector2.zero;

            HandleRotation();
        }

        private void FixedUpdate()
        {
            if (Cursor.visible)
            {
                _rb.linearVelocity = new Vector3(0, _rb.linearVelocity.y, 0);
                return;
            }

            HandleMovement();
        }

        private void HandleMovement()
        {
            Vector3 moveDir = (transform.forward * _moveInput.y + transform.right * _moveInput.x).normalized;
            _rb.linearVelocity = new Vector3(moveDir.x * moveSpeed, _rb.linearVelocity.y, moveDir.z * moveSpeed);
        }

        private void HandleRotation()
        {
            float mouseX = _lookInput.x * lookSensitivity;
            float mouseY = _lookInput.y * lookSensitivity;

            transform.Rotate(Vector3.up * mouseX);

            _rotationX -= mouseY;
            _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);
            Camera.main.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        }
    }
}