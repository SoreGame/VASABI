using Mirror;
using UnityEngine;

public class KeypadBehaviourHandler : NetworkBehaviour
{
    [SerializeField] private GameObject _canvasToToggle;
    [SerializeField] private Keypad _keypad;
    private bool _triggerEntered;

    private bool _isLocal;
    private bool _isServer;

    private const int FontSize = 40;
    private string _labelText = "";
    private bool _hasCollided;
    
    [SerializeField] private float _cooldown;
    private float _cooldownStart;

    [SyncVar] private string _keypadCode = "000000";
    public string GetKeypadCode() => _keypadCode;
    private const string Numbers = "0123456789";
    public delegate void CodeChangeHandler(string code, bool isServer);
    public event CodeChangeHandler CodeChanged;

    private void Start()
    {
        _keypad.CodeEntered += BeginDisableCanvas;
    }
    
    private void Update()
    {
        if (Time.time > _cooldownStart && _keypad.gameObject.activeInHierarchy)
        {
            _cooldownStart = Time.time + _cooldown;
            GenerateCode();
        }
        
        if (!_triggerEntered || !_isLocal) return;
        if (Input.GetKeyDown(KeyCode.E))
        {
            _canvasToToggle.SetActive(!_canvasToToggle.activeSelf);
            Cursor.lockState = CursorLockMode.None;
        }
    }
    
    private void GenerateCode()
    {
        const int length = 6;
        string code = "";
        for (var i = 0; i < length; i++)
        {
            code += Numbers[Random.Range(0, Numbers.Length)];
        }

        _keypadCode = code;
        CodeChanged?.Invoke(code, _isServer);
    }

    private void BeginDisableCanvas()
    {
        if (_isServer)
            RpcDisableCanvas();
        else
            CmdDisableCanvas();
    }


    [Command(requiresAuthority = false)]
    private void CmdDisableCanvas() => RpcDisableCanvas();
    
    [ClientRpc]
    private void RpcDisableCanvas()
    {
        _canvasToToggle.SetActive(false);
        gameObject.GetComponent<Collider>().enabled = false;
        _triggerEntered = false;
        _hasCollided = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void OnGUI()
    {
        if (!_hasCollided) return;
        
        var messageStyle = new GUIStyle("box");
        messageStyle.fontSize = FontSize;
        messageStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Box(new Rect(350, Screen.height - 125, Screen.width - 300, 140), _labelText, messageStyle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer) return;
        if (other.gameObject.GetComponent<NetworkIdentity>().isServer)
            _isServer = true;

        _isLocal = true;
        _triggerEntered = true;
        _hasCollided = true;
        _labelText = "Нажмите E для взаимодействия";
    }

    private void OnTriggerExit(Collider other)
    {
        _triggerEntered = false;
        _hasCollided = false;
        _isLocal = false;
        _canvasToToggle.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDestroy()
    {
        _keypad.CodeEntered -= BeginDisableCanvas;
    }
}
