using System.Collections.Generic;
using Mirror;
using UnityEngine;

// Z - RotateZ; X - RotateX; C - RotateY
// B - Previous; N - Next;
public class NumberPuzzleLever : NetworkBehaviour
{
    [SerializeField] private List<NumberPuzzleSector> _sectors;
    private bool _triggerEntered;
    private int _sectorsCount;
    private int _currentSectorIndex;

    private bool _isLocal;
    private bool _isServer;

    private const int FontSize = 40;
    private string _labelText = "";
    private bool _hasCollided;
    
    private void OnGUI()
    {
        if (!_hasCollided) return;
        
        var messageStyle = new GUIStyle("box");
        messageStyle.fontSize = FontSize;
        messageStyle.alignment = TextAnchor.UpperCenter;
        GUI.Box(new Rect(350, Screen.height - 125, Screen.width - 300, 140), _labelText, messageStyle);
    }

    private void Start()
    {
        _currentSectorIndex = 0;
        _sectorsCount = _sectors.Count;
    }

    private void Update()
    {
        if (!_triggerEntered || !_isLocal) return;
        
        if (Input.GetKeyDown(KeyCode.X))
            HandleRotateSector(KeyCode.X, _isServer);

        else if (Input.GetKeyDown(KeyCode.C))
            HandleRotateSector(KeyCode.C, _isServer);

        else if (Input.GetKeyDown(KeyCode.Z))
            HandleRotateSector(KeyCode.Z, _isServer);

        
        else if (Input.GetKeyDown(KeyCode.B))
            GetPreviousSector();
            
        else if (Input.GetKeyDown(KeyCode.N))
            GetNextSector();

    }
    
    private void HandleRotateSector(KeyCode keyCode, bool isServerConn)
    {
        switch (keyCode, isServerConn)
        {
            case (KeyCode.X, true):
                _sectors[GetCurrentSector()].RpcRotate("X");
                break;
            case (KeyCode.X, false):
                _sectors[GetCurrentSector()].CmdRotate("X");
                break;
            
            case (KeyCode.C, true):
                _sectors[GetCurrentSector()].RpcRotate("Y");
                break;
            case (KeyCode.C, false):
                _sectors[GetCurrentSector()].CmdRotate("Y");
                break;
            
            case (KeyCode.Z, true):
                _sectors[GetCurrentSector()].RpcRotate("Z");
                break;
            case (KeyCode.Z, false):
                _sectors[GetCurrentSector()].CmdRotate("Z");
                break;
        }
    }

    private int GetCurrentSector()
    {
        return _currentSectorIndex;
    }

    private void GetNextSector()
    {
        if (_currentSectorIndex + 1 > _sectorsCount - 1)
        {
            _currentSectorIndex = 0;
            OutlineUpdate(_currentSectorIndex, _sectorsCount - 1);
        }
        
        else
        {
            _currentSectorIndex++;
            OutlineUpdate(_currentSectorIndex, _currentSectorIndex - 1);
        }
    }

    private void GetPreviousSector()
    {
        if (_currentSectorIndex - 1 < 0)
        {
            _currentSectorIndex = _sectorsCount - 1;
            OutlineUpdate(_currentSectorIndex, 0);
        }

        else
        {
            _currentSectorIndex--;
            OutlineUpdate(_currentSectorIndex, _currentSectorIndex + 1);
        }

    }

    private void OutlineUpdate(int newIndex, int oldIndex)
    {
        _sectors[newIndex].GetComponent<Outline>().enabled = true;
        _sectors[oldIndex].GetComponent<Outline>().enabled = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
        {
            _sectors[GetCurrentSector()].GetComponent<Outline>().enabled = true;
            if (other.gameObject.GetComponent<NetworkIdentity>().isServer)
                _isServer = true;

            _isLocal = true;
            _triggerEntered = true;
            _hasCollided = true;
            _labelText = "Используйте Z, X, C для вращения по оси Z, X, Y соответственно.\nB - предыдущая деталь, N - следующая";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _sectors[GetCurrentSector()].GetComponent<Outline>().enabled = false;
        _triggerEntered = false;
        _hasCollided = false;
        _isLocal = false;
    }
}
