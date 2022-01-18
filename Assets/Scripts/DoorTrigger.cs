using UnityEngine;
using Mirror;

public class DoorTrigger : NetworkBehaviour
{
    [SerializeField] internal GameObject _doorObject;
    internal bool _isOpened = false;

    // Key door has color of it's key.
    public enum KeyColor { Red, Green, Blue, Yellow, Pink, White }
    public KeyColor Color;
}
