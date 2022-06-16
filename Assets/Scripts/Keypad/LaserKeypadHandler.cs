using UnityEngine;
using Mirror;

[RequireComponent(typeof(Collider))]
public class LaserKeypadHandler : NetworkBehaviour
{
    [SerializeField] private Collider _laserCollider;
    [SerializeField] private ParticleSystem _laserParticleSystem;
    [SerializeField] private LineRenderer _laserLineRenderer;
    [SerializeField] private LaserKeypadTrigger _laserKeypadTrigger;

    private void Start()
    {
        _laserKeypadTrigger.ButtonUnpressed += BeginSwitchToUpState;
    }

    private void BeginSwitchToUpState(bool isServerConn)
    {
        if (isServerConn)
            RpcSwitchUp();
        else
            CmdSwitchUp();

    }

    [Command(requiresAuthority = false)]
    private void CmdSwitchUp() => RpcSwitchUp();
    
    [ClientRpc]
    private void RpcSwitchUp()
    {
        _laserCollider.enabled = true;
        _laserLineRenderer.enabled = true;
        _laserParticleSystem.Play();
    }
    
    public void SwitchDown()
    {
        _laserCollider.enabled = false;
        _laserLineRenderer.enabled = false;
        _laserParticleSystem.Stop();
    }
}
