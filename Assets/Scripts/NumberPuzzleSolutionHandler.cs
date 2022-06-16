using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NumberPuzzleSolutionHandler : MonoBehaviour
{
    [SerializeField] private List<NumberPuzzleSector> _sectors;

    private void Start()
    {
        foreach (var sector in _sectors)
        {
            sector.Rotated += CheckForCompletion;
        }
    }

    private void CheckForCompletion()
    {
        if (_sectors.All(sector => sector.transform.localEulerAngles == sector.DefaultRotation))
            Debug.Log("!!!!!!!!!FINISHED!!!!!!!!!");
    }

    private void OnDestroy()
    {
        foreach (var sector in _sectors)
        {
            sector.Rotated -= CheckForCompletion;
        }
    }
}
