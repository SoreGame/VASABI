using Mirror;
using UnityEngine;

public class PressureLaser : NetworkBehaviour
{
    [SerializeField] private Collider _laserCollider;
    [SerializeField] private ParticleSystem _laserParticleSystem;
    [SerializeField] private LineRenderer _laserLineRenderer;
    private bool _isServer;

    private void HandleLaserDown(bool isServerConn)
    {
        if (isServerConn)
            RpcSwitchDown();
        else
            CmdSwitchDown();
    }

    private void HandleLaserUp(bool isServerConn)
    {
        if (isServerConn)
            RpcSwitchUp();
        else
            CmdSwitchUp();
    }

    [Command(requiresAuthority = false)]
    private void CmdSwitchDown() => RpcSwitchDown();

    [Command(requiresAuthority = false)]
    private void CmdSwitchUp() => RpcSwitchUp();

    [ClientRpc]
    private void RpcSwitchUp()
    {
        _laserCollider.enabled = true;
        _laserLineRenderer.enabled = true;
        _laserParticleSystem.Play();
    }

    [ClientRpc]
    private void RpcSwitchDown()
    {
        _laserCollider.enabled = false;
        _laserLineRenderer.enabled = false;
        _laserParticleSystem.Stop();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && (other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer))
        {
            if (other.gameObject.GetComponent<NetworkIdentity>().isServer)
                _isServer = true;
        }

        HandleLaserDown(_isServer);
    }

    private void OnTriggerExit(Collider other)
    {
        HandleLaserUp(_isServer);
        _isServer = false;
    }
}
