using UnityEngine;

public abstract class SelectableObject : MonoBehaviour
{
    public virtual void OnSelect()
    {
        Debug.Log(gameObject.name + " selected!");
    }

    public virtual void OnDeselect()
    {
        Debug.Log(gameObject.name + " deselected!");
    }
}