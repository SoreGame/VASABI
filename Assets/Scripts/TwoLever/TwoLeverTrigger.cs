using System.Collections;
using Mirror; 
using UnityEngine;

public class TwoLeverTrigger : NetworkBehaviour
{
    [SerializeField] private Animator _doorAnimator;
    [SerializeField] private Outline[] _panelOutlines;
    [SerializeField] private LeverTrigger _firstLever;
    [SerializeField] private LeverTrigger _secondLever;
    [SerializeField] private float _openTimeWindow;

    [SyncVar] private bool _firstChecked;
    [SyncVar] private bool _secondChecked;
    [SyncVar] private bool _completed;

    private void Start()
    {
        _firstLever.Pressed += BeginFirstCheck;
        _secondLever.Pressed += BeginSecondCheck;
    }

    private void BeginFirstCheck(bool isServerConn)
    {
        if (isServerConn)
            RpcFirstCheck();
        else
            CmdFirstCheck();
    }

    private void BeginSecondCheck(bool isServerConn)
    {
        if (isServerConn)
            RpcSecondCheck();
        else
            CmdSecondCheck();
    }

    [Command(requiresAuthority = false)]
    private void CmdFirstCheck() => RpcFirstCheck();

    [ClientRpc]
    private void RpcFirstCheck()
    {
        _firstChecked = true;
        _panelOutlines[0].OutlineColor = Color.green;
        _firstLever.HasCollided = false;
        OpenDoor();
        StartCoroutine(WaitForTimeWindow(0));
    }

    [Command(requiresAuthority = false)]
    private void CmdSecondCheck() => RpcSecondCheck();

    [ClientRpc]
    private void RpcSecondCheck()
    {
        _secondChecked = true;
        _panelOutlines[1].OutlineColor = Color.green;
        _secondLever.HasCollided = false;
        OpenDoor();
        StartCoroutine(WaitForTimeWindow(1));
    }

    private void OpenDoor()
    {
        if (_firstChecked && _secondChecked)
        {
            _doorAnimator.SetTrigger(ParameterIdLibrary.openTrigger);
            _completed = true;
            SwitchToCompletedState();
        }
    }

    private IEnumerator WaitForTimeWindow(int index)
    {
        yield return new WaitForSeconds(_openTimeWindow);
        _firstChecked = false;
        _secondChecked = false;
        ReturnToFailedState(index);
    }

    private void ReturnToFailedState(int index)
    {
        if (_completed) return;
        foreach (var outline in _panelOutlines)
        {
            outline.OutlineColor = Color.red;
        }

        if (index == 0)
            _firstChecked = false;
        else
            _secondChecked = false;
    }


    private void SwitchToCompletedState()
    {
        _firstLever.ToDisableState();
        _secondLever.ToDisableState();
        
        _firstLever.HasCollided = false;
        _secondLever.HasCollided = false;

        foreach (var outline in _panelOutlines)
        {
            outline.OutlineColor = Color.green;
        }
        
        _firstLever.Pressed -= BeginFirstCheck;
        _secondLever.Pressed -= BeginSecondCheck;
    }
}
