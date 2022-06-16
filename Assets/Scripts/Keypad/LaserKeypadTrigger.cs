using Mirror;
using UnityEngine;

public class LaserKeypadTrigger : NetworkBehaviour
{
    [SerializeField] private LaserKeypadHandler[] _laserObjects;
    [SerializeField] private Keypad _keypad;
    [SerializeField] private Outline _panelOutline;
    [SerializeField] private Collider _triggerCollider;
    
    private const int FontSize = 40;
    private string _labelText = "";
    private bool _hasCollided;
    private bool _triggerEntered;
    
    private bool _isServer;
    private bool _isLocal;

    public delegate void ButtonUnpressHandler(bool isServerConn);
    public event ButtonUnpressHandler ButtonUnpressed;
    
    private void Start()
    {
        _triggerCollider.enabled = false;
        _keypad.CodeEntered += BeginActivateTrigger;
    }

    private void Update()
    {
        if (!_triggerEntered) return;

        if (Input.GetButton("Use") && _isLocal)
        {
            if (_isServer)
                RpcSwitchLaserDownState();
            else
                CmdSwitchLaserDownState();
        }
        else
            ButtonUnpressed?.Invoke(_isServer);
    }

    private void BeginActivateTrigger()
    {
        if (_isServer)
            RpcActivateTrigger();
        else
            CmdActivateTrigger();
    }

    [Command(requiresAuthority = false)]
    private void CmdActivateTrigger() => RpcActivateTrigger();

    [ClientRpc]
    private void RpcActivateTrigger()
    {
        _panelOutline.OutlineColor = Color.green;
        _triggerCollider.enabled = true;
    }

    [Command(requiresAuthority = false)]
    private void CmdSwitchLaserDownState() => RpcSwitchLaserDownState();
    
    [ClientRpc]
    private void RpcSwitchLaserDownState()
    {
        foreach (var laser in _laserObjects)
            laser.SwitchDown();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (_isLocal = other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isServer)
                _isServer = true;
            _triggerEntered = true;
            _hasCollided = true;
            _labelText = "Зажмите E для отлючения лазеров";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _triggerEntered = false;
        _hasCollided = false;
    }

    private void OnGUI()
    {
        if (!_hasCollided) return;
        
        var messageStyle = new GUIStyle("box");
        messageStyle.fontSize = FontSize;
        messageStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Box(new Rect(350, Screen.height - 125, Screen.width - 300, 140), _labelText, messageStyle);
    }

    private void OnDestroy()
    {
        _keypad.CodeEntered -= BeginActivateTrigger;
    }
}
