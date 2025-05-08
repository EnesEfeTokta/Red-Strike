using UnityEngine;

public abstract class VehicleControlUI : MonoBehaviour
{
    public virtual void UpdateUIProperties(float health, float energy, float attackCooldown, float maxHealth, float maxEnergy, float maxAttackCooldown)
    {
        // Update UI properties here, if needed
        // For example, update health bar, energy bar, etc.
    }

    public virtual void OnSelect()
    {
        Debug.Log(gameObject.name + " selected!");
    }
    public virtual void OnDeselect()
    {
        Debug.Log(gameObject.name + " deselected!");
    }
    public virtual void OnClick()
    {
        Debug.Log(gameObject.name + " clicked!");
    }
}
