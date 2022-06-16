using UnityEngine;

namespace Mirror
{
    /// <summary>Shows NetworkManager controls in a GUI at runtime.</summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/NetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-manager-hud")]
    public class NetworkManagerHUD : MonoBehaviour
    {
        NetworkManager manager;
        GUIStyle customButton;
        GUIStyle customTextField;

        public int offsetX;
        public int offsetY;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
            customButton = new GUIStyle("button");
            customButton.fontSize = 25;
            customTextField = new GUIStyle("textfield");
            customTextField.fontSize = 25;
        }

        void OnGUI()
        {
            GUILayout.BeginArea(new Rect(Screen.width / 2 - 200, Screen.height / 2, 400, 10000));
            if (!NetworkClient.isConnected && !NetworkServer.active)
            {
                StartButtons();
            }
            /*else
            {
                StatusLabels();
            }*/

            // client ready
            if (NetworkClient.isConnected && !NetworkClient.ready)
            {
                if (GUILayout.Button("Клиент готов", customButton))
                {
                    NetworkClient.Ready();
                    if (NetworkClient.localPlayer == null)
                    {
                        NetworkClient.AddPlayer();
                    }
                }
            }

            //StopButtons();

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            if (!NetworkClient.active)
            {
                // Server + Client
                if (Application.platform != RuntimePlatform.WebGLPlayer)
                {
                    if (GUILayout.Button("Хост (Сервер + Клиент)", customButton))
                    {
                        manager.StartHost();
                    }
                }

                // Client + IP
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Клиент", customButton))
                {
                    manager.StartClient();
                }
                manager.networkAddress = GUILayout.TextField(manager.networkAddress, customTextField);
                GUILayout.EndHorizontal();

                /*// Server Only
                if (Application.platform == RuntimePlatform.WebGLPlayer)
                {
                    // cant be a server in webgl build
                    GUILayout.Box("(  WebGL cannot be server  )");
                }
                else
                {
                    if (GUILayout.Button("Server Only")) manager.StartServer();
                }*/
            }
            else
            {
                // Connecting
                GUILayout.Label($"Подключение к {manager.networkAddress}..");
                if (GUILayout.Button("Отменить попытку подключения", customButton))
                {
                    manager.StopClient();
                }
            }
        }

        void StatusLabels()
        {
            // host mode
            // display separately because this always confused people:
            //   Server: ...
            //   Client: ...
            if (NetworkServer.active && NetworkClient.active)
            {
                GUILayout.Label($"<b>Хост</b>: запущен через {Transport.activeTransport}");
            }
            /*// server only
            else if (NetworkServer.active)
            {
                GUILayout.Label($"<b>Сервер</b>: запущен через {Transport.activeTransport}");
            }*/
            // client only
            else if (NetworkClient.isConnected)
            {
                GUILayout.Label($"<b>Клиент</b>: подключен к {manager.networkAddress} через {Transport.activeTransport}");
            }
        }

        void StopButtons()
        {
            // stop host if host mode
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                if (GUILayout.Button("Остановить хостинг"))
                {
                    manager.StopHost();
                }
            }
            // stop client if client-only
            else if (NetworkClient.isConnected)
            {
                if (GUILayout.Button("Остановить подключение"))
                {
                    manager.StopClient();
                }
            }
            /*// stop server if server-only
            else if (NetworkServer.active)
            {
                if (GUILayout.Button("Остановить сервер"))
                {
                    manager.StopServer();
                }
            }*/
        }
    }
}
