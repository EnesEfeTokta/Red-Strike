using UnityEngine;

namespace AmmunitionSystem.Ammunitions.BasicBullet
{
    [RequireComponent(typeof(Rigidbody))]
    public class BasicBullet : Ammunition
    {
        private Rigidbody rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        protected override void Start()
        {
            base.Start();
            rb.linearVelocity = transform.forward * ammunitionData.speed;
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (ownerVehicle != null && collision.gameObject == ownerVehicle.gameObject)
                return;

            var unit = collision.gameObject.GetComponent<Unit.Unit>();
            if (unit == null)
                return;

            if (unit.teamId == ownerVehicle.teamId)
                return;

            unit.TakeDamage(ammunitionData.damage);

            OnDestroy();
        }
    }
}