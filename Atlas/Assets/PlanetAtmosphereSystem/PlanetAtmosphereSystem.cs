using UnityEngine;

namespace PlanetAtmosphereSystem
{
    public class PlanetAtmosphereSystem : MonoBehaviour
    {
        [Header("Sun Settings")]
        public Light sunLight;
        [Range(10f, 600f)] public float dayDurationInSeconds = 120f;
        private float timeElapsed = 0f;

        [Header("Materials")]
        public Material atmosphereMaterial;
        public Material cloudMaterial;
        public Material fogMaterial;

        [Header("Particle Systems")]
        public ParticleSystem firefliesParticleSystem;

        [Header("Firefly Settings")]
        [Range(0f, 1f)] public float nightStartTime = 0.75f;
        [Range(0f, 1f)] public float nightEndTime = 0.25f;

        private bool firefliesActive = false;

        private void Update()
        {
            Vector3 sunDirection = -sunLight.transform.forward;
            atmosphereMaterial.SetVector("_SunDirection", sunDirection);
            cloudMaterial.SetVector("_SunDirection", sunDirection);
            fogMaterial.SetVector("_SunDirection", sunDirection);

            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed % dayDurationInSeconds / dayDurationInSeconds;

            float sunAngle = normalizedTime * 360f - 90f;
            sunLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle, 170f, 0f));

            UpdateFireflies(normalizedTime);
        }

        private void UpdateFireflies(float normalizedTime)
        {
            if (firefliesParticleSystem == null) return;

            bool isNight = normalizedTime >= nightStartTime || normalizedTime <= nightEndTime;

            if (isNight && !firefliesActive)
            {
                firefliesParticleSystem.gameObject.SetActive(true);
                firefliesActive = true;
            }
            else if (!isNight && firefliesActive)
            {
                firefliesParticleSystem.gameObject.SetActive(false);
                firefliesActive = false;
            }
        }
    }
}
