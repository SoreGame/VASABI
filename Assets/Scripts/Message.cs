using Mirror;
using UnityEngine;

public class Message : NetworkBehaviour
{
    private const int FontSize = 40;
    private string _labelText = "";
    private bool _isLocal;
    public bool HasCollided { get; set; }
    
    private void OnGUI()
    {
        if (!HasCollided || !_isLocal) return;
        
        var messageStyle = new GUIStyle("box");
        messageStyle.fontSize = FontSize;
        messageStyle.alignment = TextAnchor.MiddleCenter;
        GUI.Box(new Rect(350, Screen.height - 125, Screen.width - 300, 140), _labelText, messageStyle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer && other.CompareTag("Player"))
        {
            _isLocal = other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer;
            var agent = other.gameObject.GetComponent<Agent>();           
            if (gameObject.CompareTag("IncorrectPlate"))
            {
                HasCollided = true;
                _labelText = "Упс! Неверная плита, начните заново.";
            }

            else if (gameObject.CompareTag("CorrectPlate"))
            {
                HasCollided = true;
                _labelText = "Плита нажата";
            }

            else
            {
                switch (gameObject.tag)
                {
                    case "DoorTrigger":
                        agent.UseDoorFlag = true;
                        HasCollided = true;
                        _labelText = "Нажмите \"Е\" чтобы открыть";
                        break;

                    case "KeyDoorTrigger":
                        var keyColor = gameObject.GetComponent<DoorTrigger>().GetKeyColor();
                        if (agent.AnyKeyPicked && agent._keys.Contains(keyColor))
                        {
                            agent.KeyDoorFlag = true;
                            HasCollided = true;
                            _labelText = "Нажмите \"Е\" чтобы открыть";
                        }

                        else if (agent.AnyKeyPicked && !agent._keys.Contains(keyColor) || !agent.AnyKeyPicked)
                        {
                            HasCollided = true;
                            _labelText = $"Вам нужен { keyColor } ключ чтобы открыть эту дверь";
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
                agent.DisableLaserFlag = true;
                HasCollided = true;
                _labelText = "Нажмите \"Е\" чтобы переключить лазеры";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        HasCollided = false;
        other.gameObject.GetComponent<Agent>().DisableLaserFlag = false;
        if (gameObject.CompareTag("KeyDoorTrigger"))
            other.gameObject.GetComponent<Agent>().KeyDoorFlag = false;
    }
}