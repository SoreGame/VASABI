using UnityEngine;
using Mirror;

public class SimpleRotation : NetworkBehaviour
{
    [SerializeField] private float _speed = 200f;

    void Update() => transform.Rotate(Vector3.up, _speed * Time.deltaTime);
}