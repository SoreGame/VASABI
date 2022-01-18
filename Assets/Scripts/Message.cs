using Mirror;
using UnityEngine;

public class Message : NetworkBehaviour
{
    private readonly int _fontSize = 40;
    private string _labelText = "";
    private bool _isLocal;
    internal bool _hasCollided = false;
    
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
        if (other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer && other.CompareTag("Player"))
        {
            _isLocal = other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer;
            var agent = other.gameObject.GetComponent<Agent>();           
            if (gameObject.CompareTag("IncorrectPlate"))
            {
                _hasCollided = true;
                _labelText = "Ops! Wrong plate, try again";
            }

            else if (gameObject.CompareTag("CorrectPlate"))
            {
                _hasCollided = true;
                _labelText = "Plate pressed";
            }

            else
            {
                switch (gameObject.tag)
                {
                    case "DoorTrigger":
                        agent._useDoorFlag = true;
                        _hasCollided = true;
                        _labelText = "Press E to open";
                        break;

                    case "KeyDoorTrigger":
                        var keyColor = gameObject.GetComponent<DoorTrigger>().Color.ToString();
                        if (agent._anyKeyPicked && agent._keys.Contains(keyColor))
                        {
                            agent._keyDoorFlag = true;
                            _hasCollided = true;
                            _labelText = "Press E to open";
                            break;
                        }

                        else if (agent._anyKeyPicked && !agent._keys.Contains(keyColor) || !agent._anyKeyPicked)
                        {
                            _hasCollided = true;
                            _labelText = $"You need { keyColor } key to open this door.";
                            break;
                        }
                        break;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer && other.CompareTag("Player"))
        {
            _isLocal = other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer;
            var agent = other.gameObject.GetComponent<Agent>();

            if (gameObject.CompareTag("LaserTrigger"))
            {
                agent._offLaserFlag = true;
                _hasCollided = true;
                _labelText = "Press E to disable laser";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _hasCollided = false;
        other.gameObject.GetComponent<Agent>()._offLaserFlag = false;
        if (gameObject.CompareTag("KeyDoorTrigger"))
            other.gameObject.GetComponent<Agent>()._keyDoorFlag = false;
    }
}