using UnityEngine;

namespace PlanetAtmosphereSystem
{
    public class PlanetAtmosphereSystem : MonoBehaviour
    {
        public Light sunLight;
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
            }
        } 
    }
}
