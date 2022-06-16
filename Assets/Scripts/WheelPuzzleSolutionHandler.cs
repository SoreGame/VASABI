using System.Collections.Generic;
using UnityEngine;

public class WheelPuzzleSolutionHandler : MonoBehaviour
{
    [SerializeField] private List<WheelPuzzleSector> _sectors;
    [SerializeField] private GameObject _door;
    private double _collisionCounter;  

    private void Start()
    {
        foreach (var sector in _sectors)
        {
            sector.Connected += OnConnectedSector;
            sector.Disconnected += OnDisconnectedSector;
        }
    }

    private void OnDestroy()
    {
        foreach (var sector in _sectors)
        {
            sector.Connected -= OnConnectedSector;
            sector.Disconnected -= OnDisconnectedSector;
        }
    }

    private void OnConnectedSector()
    {
        _collisionCounter += 0.5;
        if (_collisionCounter == 3)
            HandleComplete();
    }

    private void HandleComplete()
    {
        var anim = _door.GetComponent<Animator>();
        anim.SetTrigger(ParameterIdLibrary.openTrigger);

        foreach (var sector in _sectors)
            sector.DisableSectorRotation();
    }

    private void OnDisconnectedSector()
    {
        _collisionCounter -= 0.5;
    }
}