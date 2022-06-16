using UnityEngine;
using Mirror;

public class DoorTrigger : NetworkBehaviour
{
    [SerializeField] private GameObject _doorObject;
    public bool isOpened { get; set; }
    // Key door has color of it's key.
    public enum KeyColor { Red, Green, Blue, Yellow, Pink, White }
    public KeyColor Color;
    public string GetKeyColor() => Color.ToString();
    public GameObject GetDoorObject() => _doorObject;
}