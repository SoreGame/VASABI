using UnityEngine;
using Mirror;

public class NumberPuzzleSector : NetworkBehaviour
{
    private Vector3 _boundsCenterPosition;
    public Vector3 DefaultRotation { get; } = Vector3.zero;
    
    public delegate void RotationHandler();
    public event RotationHandler Rotated;

    private void Start()
    {
        _boundsCenterPosition = gameObject.GetComponent<Renderer>().bounds.center;
    }

    [Command(requiresAuthority = false)]
    public void CmdRotate(string axis) => RpcRotate(axis);

    [ClientRpc]
    public void RpcRotate(string axis)
    {
        switch (axis)
        {
            case "X":
                gameObject.transform.RotateAround(_boundsCenterPosition, Vector3.right, 90.0f);
                break;
            case "Y":
                gameObject.transform.RotateAround(_boundsCenterPosition, Vector3.up, 90.0f);
                break;
            case "Z":
                gameObject.transform.RotateAround(_boundsCenterPosition, Vector3.forward, 90.0f);
                break;
        }
        
        Rotated?.Invoke();
    }
}
