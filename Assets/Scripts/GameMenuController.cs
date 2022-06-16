using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuController : MonoBehaviour
{
    public bool IsPaused = false;
    public GameObject PauseMenuUI;
    public GameObject PlayerHUD; 
    public Text RedKey;
    public Text GreenKey;
    public Text BlueKey;
    public Text YellowKey;
    public Text PinkKey;
    public Text WhiteKey;

    private static readonly int _redKeysAmount;
    private static readonly int _greenKeysAmount;
    private static readonly int _blueKeysAmount;
    private static readonly int _yellowKeysAmount;
    private static readonly int _pinkKeysAmount;
    private static readonly int _whiteKeysAmount;
    private static readonly Dictionary<string, int> _keysDictionary = new Dictionary<string, int>()
    {
        { "Red", _redKeysAmount },
        {"Green", _greenKeysAmount },
        { "Blue", _blueKeysAmount },
        { "Yellow", _yellowKeysAmount },
        { "Pink", _pinkKeysAmount },
        { "White", _whiteKeysAmount }
    };

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            if (IsPaused) 
                Resume();
            else 
                Pause();
        }

        RedKey.text = "" + _keysDictionary["Red"];
        GreenKey.text = "" + _keysDictionary["Green"];
        BlueKey.text = "" + _keysDictionary["Blue"];
        YellowKey.text = "" + _keysDictionary["Yellow"];
        PinkKey.text = "" + _keysDictionary["Pink"];
        WhiteKey.text = "" + _keysDictionary["White"];
    }   

    public void UpdateKeyAmountOnHUD(string color, bool isIncrement)
    {
        if (isIncrement)
            _keysDictionary[color] += 1;
        else
            _keysDictionary[color] -= 1;
    }

    public void Resume()
    {
        PauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        IsPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Pause()
    {
        PauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        IsPaused = true;
    }

    public void Disconnect()
    {   
        // Disconnect Host.
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }

        // Disconnect Client.
        else if (NetworkClient.isConnected)
        {
            NetworkServer.RemoveConnection(NetworkClient.connection.connectionId);
            NetworkServer.RemovePlayerForConnection(NetworkClient.connection, true);
            
            NetworkManager.singleton.StopClient();
        }
    }
}
