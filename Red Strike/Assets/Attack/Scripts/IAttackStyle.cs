using UnityEngine;

public interface IAttackStyle
{
    void HandleSingleTargetInput(Transform target);
    void HandleMultiTargetInput(Transform[] targets);
    void ProcessMultiTargetAttack();
    void HandleFPSAimingInput();
}
