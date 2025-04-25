using UnityEngine;

namespace ParticleSystems {
    public class TriggerNozzleVfx : MonoBehaviour
    {
        [SerializeField] private ParticleSystem particleSystemToTrigger;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (particleSystemToTrigger != null)
                {
                    particleSystemToTrigger.Play();
                }
                else
                {
                    Debug.LogWarning("ParticleSystem reference not assigned!");
                }
            }
        }
    }
}
