using UnityEngine;

public class TankAttack : MonoBehaviour, IAttackStyle
{
    // Free Look modunda tek hedef seçilirse.
    public void HandleSingleTargetInput(Transform target)
    {
        // Implement single target attack logic here
    }

    // Free Look modunda birden fazla hedef seçilirse.
    public void HandleMultiTargetInput(Transform[] targets)
    {
        // Implement multi-target attack logic here
    }

    // TPS modunda ateş etme için input alır.
    public void HandleTPSAimingInput()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Implement FPS aiming input logic here
        }
    }
}