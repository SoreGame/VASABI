internal class ClearKey : KeypadKey
{
    public delegate void InteractionHandler();
    public event InteractionHandler Interacted;
    
    public override void Interact()
    {
        Interacted?.Invoke();
    }
}
