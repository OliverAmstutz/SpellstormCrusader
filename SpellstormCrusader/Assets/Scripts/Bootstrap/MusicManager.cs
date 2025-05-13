using System.Collections;
using UnityEngine;

namespace Bootstrap
{
    public class MusicManager : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private float fadeDuration = 2f;
        public static MusicManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PlayMusic(AudioClip newClip)
        {
            if (audioSource.isPlaying && audioSource.clip == newClip)
            {
                return;
            }

            StopAllCoroutines();
            StartCoroutine(FadeToNewClip(newClip));
        }

        private IEnumerator FadeToNewClip(AudioClip newClip)
        {
            if (audioSource.isPlaying)
            {
                // Fade out
                for (float t = 0; t < fadeDuration; t += Time.deltaTime)
                {
                    audioSource.volume = 1 - t / fadeDuration;
                    yield return null;
                }

                audioSource.Stop();
            }

            audioSource.clip = newClip;
            audioSource.Play();

            // Fade in
            for (float t = 0; t < fadeDuration; t += Time.deltaTime)
            {
                audioSource.volume = t / fadeDuration;
                yield return null;
            }

            audioSource.volume = 1f;
        }
    }
}
