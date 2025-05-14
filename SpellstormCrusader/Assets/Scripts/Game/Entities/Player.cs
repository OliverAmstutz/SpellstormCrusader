using Bootstrap;
using UnityEngine;

namespace Game.Entities
{
    public class Player : MonoBehaviour
    {
        private CameraFollower _cameraFollower;

        private void Awake() => _cameraFollower = FindAnyObjectByType<CameraFollower>();

        private void Start() => _cameraFollower.SetPlayer(transform);
    }
}
