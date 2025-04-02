using UnityEngine;

public class Ornithopter : BaseUnit
{
    public OrnithopterState currentState;
    public OrnithopterType ornithopterType;

    public float resilience = 1.0f;

    private void Start()
    {
        switch (ornithopterType)
        {
            case OrnithopterType.A:
                resilience = 0.8f;
                break;
            case OrnithopterType.B:
                resilience = 1.2f;
                break;
            default:
                resilience = 1.0f;
                break;
        }
    }

    public override void TakeDamage(float damageAmount)
    {
        damageAmount = damageAmount * resilience; // Ornithopter has variable damage reduction based on type.
        base.TakeDamage(damageAmount);
    }

    public enum OrnithopterState { Idle, Flying, Attacking, Landing }

    public enum OrnithopterType { A, B }
}
