using UnityEngine;

internal abstract class KeypadKey : MonoBehaviour
{
    [SerializeField] internal string _value;
    public abstract void Interact();
}
