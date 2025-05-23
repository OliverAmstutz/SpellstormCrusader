using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Controls
{
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] private float attackCooldown = 1f;
        [SerializeField] private GameObject attackPrefab;
        [SerializeField] private Transform referenceRotation;
        [SerializeField] private Transform wandPosition;
        [SerializeField] private AudioClip shootSfx;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private ParticleSystem muzzleFlash;

        private PlayerControls _inputActions;
        private bool _isAttackHeld;
        private float _lastAttackTime = Mathf.NegativeInfinity;

        private void Awake() => _inputActions = new PlayerControls();

        private void Update()
        {
            if (_isAttackHeld && Time.time >= _lastAttackTime + attackCooldown)
            {
                PerformAttack();
            }
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
            _inputActions.Player.BasicAttack.started += OnAttackStarted;
            _inputActions.Player.BasicAttack.canceled += OnAttackCanceled;
        }

        private void OnDisable()
        {
            _inputActions.Player.BasicAttack.started -= OnAttackStarted;
            _inputActions.Player.BasicAttack.canceled -= OnAttackCanceled;
            _inputActions.Player.Disable();
        }

        private void OnAttackStarted(InputAction.CallbackContext context)
        {
            _isAttackHeld = true;

            if (Time.time >= _lastAttackTime + attackCooldown)
            {
                PerformAttack();
            }
        }

        private void OnAttackCanceled(InputAction.CallbackContext context) => _isAttackHeld = false;

        private void PerformAttack()
        {
            _lastAttackTime = Time.time;

            if (attackPrefab && referenceRotation && wandPosition)
            {
                Instantiate(attackPrefab, wandPosition.position, referenceRotation.rotation);
                if (audioSource && shootSfx)
                {
                    audioSource.PlayOneShot(shootSfx);
                }

                if (muzzleFlash)
                {
                    muzzleFlash.Play();
                }
            }
            else
            {
                Debug.LogWarning("Attack prefab or spawn point not set.");
            }
        }
    }
}
