using UnityEngine;
using UnityEngine.VFX;

namespace Game.Entities
{
    public class BasicAttackProjectile : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float maxDistance = 20f;
        [SerializeField] private float lifetime = 10f;

        private Vector3 _startPosition;
        private float _traveledDistance;
        private VisualEffect[] _vfxs;
        private TrailRenderer[] _trails;

        private void Start()
        {
            _startPosition = transform.position;

            _vfxs = GetComponentsInChildren<VisualEffect>(true);
            foreach (var vfx in _vfxs)
            {
                vfx.enabled = true;
            }

            _trails = GetComponentsInChildren<TrailRenderer>(true);
            foreach (var trail in _trails)
            {
                trail.Clear();
                trail.enabled = true;
            }

            Destroy(gameObject, lifetime);
        }

        private void Update()
        {
            float distanceToMove = speed * Time.deltaTime;
            transform.Translate(Vector3.up * distanceToMove);
            _traveledDistance += distanceToMove;

            if (_traveledDistance >= maxDistance)
            {
                Destroy(gameObject);
            }
        }
    }
}
