internal class NumberKey : KeypadKey
{
    public delegate void InteractionHandler(string value);
    public event InteractionHandler Interacted;
    
    public override void Interact()
    {
        Interacted?.Invoke(_value);
    }
}
