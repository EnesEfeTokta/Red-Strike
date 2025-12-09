using UnityEngine;

namespace PlanetAtmosphereSystem
{
    public class PlanetAtmosphereSystem : MonoBehaviour
    {
        [Header("Sun Settings")]
        public Light sunLight;
        public float dayDurationInSeconds = 120f;
        private float timeElapsed = 0f;

        [Header("Materials")]
        public Material atmosphereMaterial;
        public Material cloudMaterial;
        public Material fogMaterial;

        private void Update()
        {
            if (sunLight != null)
            {
                Vector3 sunDirection = -sunLight.transform.forward;
                atmosphereMaterial.SetVector("_SunDirection", sunDirection);
                cloudMaterial.SetVector("_SunDirection", sunDirection);
                fogMaterial.SetVector("_SunDirection", sunDirection);

                timeElapsed += Time.deltaTime;
                float normalizedTime = (timeElapsed % dayDurationInSeconds) / dayDurationInSeconds;

                float sunAngle = normalizedTime * 360f - 90f;
                sunLight.transform.rotation = Quaternion.Euler(new Vector3(sunAngle, 170f, 0f));
            }
        }
    }
}
