using UnityEngine;

namespace Game.Controls
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private AudioClip[] footstepClips;
        [SerializeField] private float footstepInterval = 0.5f;
        [SerializeField] private float moveSpeed = 5f;
        private Animator _animator;
        private PlayerControls _controls;
        private AudioSource _footstepSource;
        private float _footstepTimer;
        private Vector2 _moveInput;
        private Rigidbody _rb;

        private void Awake()
        {
            _controls = new PlayerControls();
            _rb = GetComponent<Rigidbody>();
            _animator = GetComponent<Animator>();
            _footstepSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            _animator.SetFloat("forwardBackward", _moveInput.y);
            _animator.SetFloat("leftRight", _moveInput.x);
            // animator.SetFloat("Speed", _moveInput.sqrMagnitude);
            HandleFootsteps();
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

        private void HandleFootsteps()
        {
            if (_moveInput.sqrMagnitude > 0.01f)
            {
                _footstepTimer -= Time.deltaTime;
                if (_footstepTimer <= 0f)
                {
                    PlayFootstep();
                    _footstepTimer = footstepInterval;
                }
            }
            else
            {
                _footstepTimer = 0f;
            }
        }

        private void PlayFootstep()
        {
            if (footstepClips.Length == 0)
            {
                return;
            }

            int index = Random.Range(0, footstepClips.Length);
            _footstepSource.pitch = Random.Range(0.95f, 1.2f);
            _footstepSource.PlayOneShot(footstepClips[index]);
        }
    }
}
