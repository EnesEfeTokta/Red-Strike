using UnityEngine;

public abstract class BaseUnit : MonoBehaviour
{
    #region  Base unit properties
    private float health { get; set; }
    private float speed { get; set; }
    private float armor { get; set; }
    private float damage { get; set; }
    private float attackCooldown { get; set; }

    private int repeatShot { get; set; }

    private float range { get; set; }
    private int density { get; set; }
    private bool recreationStatus { get; set; }
    private int recreation { get; set; }
    #endregion

    #region Properties Set/Get
    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    public float Speed
    {
        get { return speed; }
        set { speed = value; }
    }

    public float Armor
    {
        get { return armor; }
        set { armor = value; }
    }

    public float Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public float AttackCooldown
    {
        get { return attackCooldown; }
        set { attackCooldown = value; }
    }

    public int RepeatShot
    {
        get { return repeatShot; }
        set { repeatShot = value; }
    }

    public float Range
    {
        get { return range; }
        set { range = value; }
    }

    public int Density
    {
        get { return density; }
        set { density = value; }
    }

    public bool RecreationStatus
    {
        get { return recreationStatus; }
        set { recreationStatus = value; }
    }

    public int Recreation
    {
        get { return recreation; }
        set { recreation = value; }
    }
    #endregion

    public virtual void TakeDamage(float damageAmount)
    {
        float effectiveDamage = damageAmount - armor;
        Health -= Mathf.Max(effectiveDamage, 0);
        Debug.Log($"{gameObject.name} took {effectiveDamage} damage. Remaining health: {Health}");
        if (Health <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }

    public virtual void Move(Vector3 targetPosition)
    {
        Debug.Log($"{gameObject.name} is moving to {targetPosition}.");
    }

    public virtual void StopEngine()
    {
        Debug.Log($"{gameObject.name} engine stopped.");
    }

    public virtual void StartEngine()
    {
        Debug.Log($"{gameObject.name} engine started.");
    }
}
