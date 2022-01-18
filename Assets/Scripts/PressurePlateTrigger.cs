using UnityEngine;
using Mirror;

public class PressurePlateTrigger : NetworkBehaviour
{
    [SerializeField] internal GameObject _doorObject;
    internal bool _isOpened = false;
}
