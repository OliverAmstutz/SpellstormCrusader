using Bootstrap;
using UnityEngine;

namespace Game
{
    public class GameSceneController : MonoBehaviour
    {
        [SerializeField] private AudioClip gameMusic;

        private void Start() => MusicManager.Instance?.PlayMusic(gameMusic);
    }
}
