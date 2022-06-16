using Mirror;
using UnityEngine;
using System.Collections.Generic;

// Agent script requires the player GameObject to have a CharacterController and mount point for camera. 
[RequireComponent(
    typeof(CharacterController), 
    typeof(GameObject))]
public class Agent : NetworkBehaviour
{
    public CharacterController Controller;
    public GameObject CameraMountPoint;
    public float Speed = 5f;
    public float Gravity;

    [SerializeField] internal List<string> _keys = new List<string>();
    public bool DisableLaserFlag { get; set; }
    public bool UseDoorFlag { get; set; }
    public bool KeyDoorFlag { get; set; }
    public bool AnyKeyPicked { get; private set; }

    private GameMenuController _gameMenuController;
    private Vector3 _velocity;
    private bool _isOpening;
    private Light _flashlight;
    private Animator _animator;

    private void Start()
    {
        if (isLocalPlayer)
        {
            _gameMenuController = GetComponentInChildren<GameMenuController>();
            _animator = GetComponent<Animator>();

            var skinnedMeshRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var meshRenderer in skinnedMeshRenderer)
                meshRenderer.enabled = false;
            
            var cameraTransform = Camera.main.gameObject.transform;
            cameraTransform.parent = CameraMountPoint.transform;
            cameraTransform.SetPositionAndRotation(CameraMountPoint.transform.position, CameraMountPoint.transform.rotation);
            cameraTransform.gameObject.AddComponent<MouseLook>();
            cameraTransform.gameObject.GetComponent<Camera>().cullingMask = ~(1 << 8 | 1 << 7);
            _flashlight = GetComponentInChildren<Light>();
        }
    }

    private void Update()
    {
        if (isLocalPlayer)
        {
            if (_velocity.y < 0)
                _velocity.y = -2f;

            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");
            Move(x, z);

            _velocity.y += Gravity * Time.deltaTime;
            Controller.Move(Speed * Time.deltaTime * _velocity);
            
            if (Input.GetButtonDown("Use") &&
                (UseDoorFlag || KeyDoorFlag || DisableLaserFlag))
            {
                _isOpening = !_isOpening;
            }

            if (Input.GetKeyDown(KeyCode.F))
                _flashlight.gameObject.SetActive(!_flashlight.gameObject.activeSelf);
        }
    }

    private void Move(float x, float z)
    {
        var objectTransform = gameObject.transform;
        var move = objectTransform.right * x + objectTransform.forward * z;
        Controller.Move(Speed * Time.deltaTime * move);
        _animator.SetFloat(ParameterIdLibrary.x, x); 
        _animator.SetFloat(ParameterIdLibrary.z, z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            var color = other.gameObject.GetComponent<Key>().GetKeyColor();
            _keys.Add(color);
            _gameMenuController.UpdateKeyAmountOnHUD(color, true);
            AnyKeyPicked = true;
            CmdDestroyObject(other.gameObject);
        }

        else if (other.CompareTag("PressurePlate"))
        {
            var openStatus = other.gameObject.GetComponent<PressurePlateTrigger>().isOpened;
            other.GetComponent<Collider>().enabled = false;
            if (openStatus == false)
                CmdPressureDoorOpen(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Open door on E button.
        if (_isOpening && UseDoorFlag)
        {
            _isOpening = !_isOpening;
            UseDoorFlag = !UseDoorFlag;
            var trigger = other.gameObject;
            if (!trigger.CompareTag("DoorTrigger")) return;
            CmdOpenDoor(trigger);
            CmdDestroyObject(trigger);
        }
        
        // Open door locked by key.
        else if (AnyKeyPicked && _isOpening && KeyDoorFlag)
        {
            _gameMenuController.UpdateKeyAmountOnHUD(other.gameObject.GetComponent<DoorTrigger>().Color.ToString(), false);
            _keys.Remove(other.GetComponent<DoorTrigger>().Color.ToString());
            _isOpening = !_isOpening;
            AnyKeyPicked = _keys.Count != 0;
            KeyDoorFlag = !KeyDoorFlag;
            var trigger = other.gameObject;
            if (trigger.CompareTag("KeyDoorTrigger"))
            {
                CmdOpenDoor(trigger);
                CmdDestroyObject(trigger);
            }
        }
        
        // Disable laser.
        else if (_isOpening && DisableLaserFlag)
        {
            _isOpening = !_isOpening;
            DisableLaserFlag = !DisableLaserFlag;
            CmdLaserToggleInitialize(other.gameObject);
        }
    }

    [Command]
    private void CmdLaserToggleInitialize(GameObject trigger) 
    {
        var laserTrigger = trigger.GetComponent<LaserTrigger>();
        foreach (var laser in laserTrigger._laserObjects)
            laser.GetComponent<Animator>().SetTrigger(ParameterIdLibrary.toggle);
        // Logic for single laser toggle.
        //Animator anim = laserObject.GetComponent<Animator>();
        //anim.SetTrigger("Toggle");
    }

    [Command]
    private void CmdPressureDoorOpen(GameObject trigger)
    {
        trigger.GetComponent<Collider>().enabled = false;
        
        var doorTrigger = trigger.GetComponent<PressurePlateTrigger>();
        var door = doorTrigger.GetDoorObject();
        var anim = door.GetComponent<Animator>();
        
        anim.SetTrigger(ParameterIdLibrary.openTrigger);
        doorTrigger.isOpened = true;
    }

    [Command]
    private void CmdOpenDoor(GameObject trigger)
    {
        var doorTrigger = trigger.GetComponent<DoorTrigger>();
        var door = doorTrigger.GetDoorObject();
        var anim = door.GetComponent<Animator>();
       
        anim.SetTrigger(ParameterIdLibrary.openTrigger);
        doorTrigger.isOpened = true;
        
        trigger.GetComponent<Message>().HasCollided = false;
    }

    [Command]
    private void CmdDestroyObject(GameObject obj) => NetworkServer.Destroy(obj);
}
