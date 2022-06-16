using Mirror;
using TMPro;
using UnityEngine;

public class GuardScreen : NetworkBehaviour
{
    [SerializeField] private KeypadBehaviourHandler _keypad;
    [SerializeField] private TextMeshProUGUI _text;

    private void Start()
    {
        _keypad.CodeChanged += EventHandle;
    }

    private void EventHandle(string code, bool isServerConn)
    {
        if (isServerConn)
            RpcDisplayCode(code);
        else
            CmdDisplayCode(code);
    }

    [Command(requiresAuthority = false)]
    private void CmdDisplayCode(string code) => RpcDisplayCode(code);
    
    [ClientRpc]  
    private void RpcDisplayCode(string code)
    {
        _text.text = $"Код:\n{code}";
    }

    private void OnDestroy()
    {
        _keypad.CodeChanged -= EventHandle;
    }
}
