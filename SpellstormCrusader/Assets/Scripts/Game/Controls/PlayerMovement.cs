using UnityEngine;

namespace Game.Controls
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        private Animator _animator;
        private PlayerControls _controls;
        private Vector2 _moveInput;
        private Rigidbody _rb;

        private void Awake()
        {
            _controls = new PlayerControls();
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
        }

        private void Update()
        {
            _animator.SetFloat("forwardBackward", _moveInput.y);
            _animator.SetFloat("leftRight", _moveInput.x);
            // animator.SetFloat("Speed", _moveInput.sqrMagnitude);
        }

        private void FixedUpdate()
        {
            Vector3 movement = new(_moveInput.x, 0, _moveInput.y);
            _rb.MovePosition(_rb.position + movement * (moveSpeed * Time.fixedDeltaTime));
        }

        private void OnEnable()
        {
            _controls.Player.Enable();
            _controls.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
            _controls.Player.Move.canceled += ctx => _moveInput = Vector2.zero;
        }

        private void OnDisable()
        {
            _controls.Player.Move.performed -= ctx => _moveInput = ctx.ReadValue<Vector2>();
            _controls.Player.Move.canceled -= ctx => _moveInput = Vector2.zero;
            _controls.Player.Disable();
        }
    }
}
