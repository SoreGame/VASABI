using Mirror;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class LeverTrigger : MonoBehaviour
{
    [SerializeField] private Collider _triggerCollider;
    private const int FontSize = 40;
    private string _labelText = "";
    private bool _isServer;
    private bool _isLocal;
    private bool _triggerEntered;
    
    public bool HasCollided { get; set; }

    public delegate void EnterHandler();
    public delegate void ExitHandler();
    public delegate void PressedHandler(bool isServerConn);

    public event EnterHandler Entered;
    public event ExitHandler Exited;
    public event PressedHandler Pressed;

    private void Start()
    {
        Entered += () =>
        {
            _labelText = "Нажмите E для активации";
            HasCollided = true;
        };
        
        Exited += () =>
        {
            _labelText = "";
            HasCollided = false;
        };
    }

    private void Update()
    {
        
        if (!_isLocal && !_triggerEntered) return;

        if (Input.GetKeyDown(KeyCode.E) && _isLocal)
        {
            Pressed?.Invoke(_isServer);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (_isLocal = other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isServer)
                _isServer = true;
            
            Entered?.Invoke();
            _triggerEntered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Exited?.Invoke();
        _triggerEntered = false;
        _isLocal = false;
        _isServer = false;
        HasCollided = false;
    }

    private void OnGUI()
    {
        if (!HasCollided) return;
        
        var messageStyle = new GUIStyle("box");
        messageStyle.fontSize = FontSize;
        messageStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Box(new Rect(350, Screen.height - 125, Screen.width - 300, 140), _labelText, messageStyle);
    }

    public void ToDisableState()
    {
        _triggerEntered = false;
        _isLocal = false;
        _isServer = false;
        _triggerCollider.enabled = false;
    }
}
