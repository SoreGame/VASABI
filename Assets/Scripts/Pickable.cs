using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Pickable : NetworkBehaviour
{
    public Rigidbody Rigidbody { get; private set; }

    private void Awake() => Rigidbody = GetComponent<Rigidbody>();
}
