using UnityEngine;
using Mirror;

public class PressurePlateTrigger : NetworkBehaviour
{
    [SerializeField] private GameObject _doorObject;
    public bool isOpened { get; set; }
    public GameObject GetDoorObject() => _doorObject;
}