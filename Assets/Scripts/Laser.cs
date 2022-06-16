using UnityEngine;
using Mirror;

public class Laser : NetworkBehaviour 
{
	[SerializeField] private Transform _startPoint;
	[SerializeField] private Transform _endPoint;
	[SerializeField] private Transform _retryPointTransform;
	[SerializeField] private ParticleSystem _laserParticleSystem;
	private LineRenderer _laserLine;

	private void Start() 
	{
		_laserLine = GetComponentInChildren<LineRenderer>();
		_laserLine.startWidth = .2f;
		_laserLine.endWidth = .2f;
		_laserLine.SetPosition(0, _startPoint.position);
		_laserLine.SetPosition(1, _endPoint.position);
	}

    private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player") && other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
		{
			GameObject player;
			var characterController = (player = other.gameObject).GetComponent<CharacterController>();
			characterController.enabled = false;
			player.transform.position = _retryPointTransform.position;
			characterController.enabled = true;
		}
	}
}
