using Mirror;
using UnityEngine;

public class OpenCloseTrigger : NetworkBehaviour
{
    [SerializeField] private DoorObject _door;
    private Animator _animator;
    
    private const int FontSize = 40;
    private string _labelText = "";
    private bool _hasCollided;
    
    private bool _isLocal;
    private bool _isServer;
    private bool _triggerEntered;

    private void Start()
    {
        _animator = _door.GetAnimator();
    }

    private void Update()
    {
        if (!_isLocal && !_triggerEntered) return;

        if (!Input.GetButtonDown("Use")) return;

        if (!_door.OpenState)
            BeginOpen();
        else
            BeginClose();
    }

    private void BeginOpen()
    {
        if (_isServer)
            RpcOpen();
        else
            CmdOpen();
    }
    
    private void BeginClose()
    {
        if (_isServer) 
            RpcClose();
        else
            CmdClose();
    }
    
    
    [Command(requiresAuthority = false)]
    private void CmdClose() => RpcClose();

    [Command(requiresAuthority = false)]
    private void CmdOpen() => RpcOpen();

    [ClientRpc]
    private void RpcOpen()
    {
        _animator.SetTrigger(ParameterIdLibrary.closeTrigger);
        _animator.ResetTrigger(ParameterIdLibrary.openTrigger);
        _door.OpenState = !_door.OpenState;
    }
    
    [ClientRpc]
    private void RpcClose()
    {
        _animator.SetTrigger(ParameterIdLibrary.openTrigger);
        _animator.ResetTrigger(ParameterIdLibrary.closeTrigger);
        _door.OpenState = !_door.OpenState;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (_isLocal = other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isServer)
                _isServer = true;
            
            _triggerEntered = true;
            _hasCollided = true;
            _labelText = "Нажмите E для\nвзаимодействия";
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        _triggerEntered = false;
        _hasCollided = false;
        _isLocal = false;
    }
    
    private void OnGUI()
    {
        if (!_hasCollided) return;
        
        var messageStyle = new GUIStyle("box");
        messageStyle.fontSize = FontSize;
        messageStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Box(new Rect(350, Screen.height - 125, Screen.width - 300, 140), _labelText, messageStyle);
    }
}
