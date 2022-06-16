using UnityEngine;

internal class EnterKey : KeypadKey
{
    public delegate void InteractionHandler();
    public event InteractionHandler Interacted;
    
    [SerializeField] private AudioSource _failedClip;
    public AudioSource GetFailedClip() => _failedClip;

    public override void Interact()
    {
        Interacted?.Invoke();
    }
}