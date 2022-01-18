using Mirror;
using UnityEngine;

public class SceneChangeTrigger : NetworkBehaviour
{
    [SerializeField] private string _scene;
    
    private NetworkIdentity _netId;
    private SceneChanger _sceneChanger;
    
    private readonly int _fontSize = 40;
    private string _labelText = "";
    private bool _isLocal;
    private bool _hasCollided = false;

    private void Start()
    {
        _sceneChanger = GameObject.FindGameObjectWithTag("SceneChanger").GetComponent<SceneChanger>();
    }

    private void OnGUI()
    {
        if (_hasCollided == true && _isLocal)
        {
            GUIStyle messageStyle = new GUIStyle("box");
            messageStyle.fontSize = _fontSize;
            messageStyle.alignment = TextAnchor.UpperCenter;
            GUI.Box(new Rect(150, Screen.height - 50, Screen.width - 300, 140), (_labelText), messageStyle);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            _netId = other.gameObject.GetComponent<NetworkIdentity>();
            _isLocal = _netId.isLocalPlayer;
            if (_netId.isServer && NetworkClient.ready)
            {
                _sceneChanger.FadeToLevel(_scene);
            }
           
            else
            {
                _hasCollided = true;
                _labelText = "Wait for the other player";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _hasCollided = false;
    }
}
