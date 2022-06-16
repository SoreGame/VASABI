using UnityEngine;
using Mirror;

public class PickupSystem : NetworkBehaviour
{
    [SerializeField] private Transform _itemSlot;
    [SerializeField] private float _dropForce;
    private Pickable _pickedItem;
    private Camera _playerCamera;

    private void Start()
    {
        _playerCamera = Camera.main;
    } 

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (!Input.GetButtonDown("Use")) return;

            if (_pickedItem)
            {
                if (isServer)
                    RpcDropItem(_pickedItem);
                else
                    CmdDropItem(_pickedItem);
            }
            
            else
            {
                var ray = _playerCamera.ViewportPointToRay(Vector3.one * 0.5f);
                if (!Physics.Raycast(ray, out var hit, 2f)) return;
                var pickable = hit.transform.GetComponent<Pickable>();

                if (!pickable) return;
                
                if (isServer)
                    RpcPickItem(pickable);
                else
                    CmdPickItem(pickable);

            }
        }
    }

    [Command]
    private void CmdPickItem(Pickable item) => RpcPickItem(item);

        [Command]
    private void CmdDropItem(Pickable item) => RpcDropItem(item);

    [ClientRpc]
    private void RpcPickItem(Pickable item)
    {
        _pickedItem = item;
        item.Rigidbody.isKinematic = true;
        item.Rigidbody.velocity = Vector3.zero;
        item.Rigidbody.angularVelocity = Vector3.zero;
        
        Transform transform1;
        (transform1 = item.transform).SetParent(_itemSlot);
        transform1.localPosition = Vector3.zero;
        transform1.localEulerAngles = Vector3.zero;
    }

    [ClientRpc]
    private void RpcDropItem(Pickable item)
    {
        _pickedItem = null;
        Transform transform1;
        
        (transform1 = item.transform).SetParent(null);
        item.Rigidbody.isKinematic = false;
        item.Rigidbody.AddForce(transform1.forward * _dropForce, ForceMode.VelocityChange);
    }
}
