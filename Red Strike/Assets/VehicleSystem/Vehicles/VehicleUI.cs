using UnityEngine;
using UnityEngine.UI;

namespace VehicleSystem.Vehicles
{
    public class VehicleUI : MonoBehaviour
    {
        [Header("Icon Settings")]
        public Sprite warningIcon;
        public Sprite refuelingIcon;

        [Header("UI References")]
        public Image vehicleStatusIcon;

        private void Start()
        {
            vehicleStatusIcon.enabled = false;
        }

        public void SetVehicleStatusIconToWarning()
        {
            vehicleStatusIcon.sprite = warningIcon;
            vehicleStatusIcon.enabled = true;
        }

        public void SetVehicleStatusIconToRefueling()
        {
            vehicleStatusIcon.sprite = refuelingIcon;
            vehicleStatusIcon.enabled = true;
        }

        public void ClearVehicleStatusIcon()
        {
            vehicleStatusIcon.sprite = null;
            vehicleStatusIcon.enabled = false;
        }
    }
}
