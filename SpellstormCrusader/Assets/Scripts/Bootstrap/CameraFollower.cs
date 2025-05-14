using UnityEngine;

namespace Bootstrap
{
    public class CameraFollower : MonoBehaviour
    {
        [SerializeField] [Header("Distance in Z before camera starts moving")]
        private float followThreshold = 5f;

        [SerializeField] [Header("How fast the camera catches up")]
        private float cameraFollowSpeed = 5f;

        private float _currentZOffset;

        private float _initialZOffset;
        private Transform _player;

        private void LateUpdate()
        {
            if (!_player)
            {
                return;
            }

            // Calculate current Z offset
            float currentZOffset = transform.position.z - _player.position.z;

            // If character is too far "up" (towards +Z), move the camera up
            if (currentZOffset < -followThreshold)
            {
                // Target Z position for the camera
                float targetZ = _player.position.z + _initialZOffset;

                // Only move positively in Z (never backwards)
                if (targetZ > transform.position.z)
                {
                    Vector3 newPos = new(
                        transform.position.x,
                        transform.position.y,
                        Mathf.Lerp(transform.position.z, targetZ, Time.deltaTime * cameraFollowSpeed)
                    );

                    transform.position = newPos;
                }
            }
        }

        public void SetPlayer(Transform player)
        {
            _player = player;
            _initialZOffset = transform.position.z - _player.position.z;
        }
    }
}
