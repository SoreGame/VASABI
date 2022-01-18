using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class Agent : NetworkBehaviour
{
    public CharacterController Controller;
    public GameObject ÑameraMountPoint;
    public float Speed = 5f;
    public float Gravity;

    [SerializeField] internal List<string> _keys = new List<string>();
    internal bool _offLaserFlag = false;
    internal bool _useDoorFlag = false;
    internal bool _keyDoorFlag = false;
    internal bool _anyKeyPicked = false;

    private GameMenuController _gameMenuController;
    private Vector3 _velocity;
    private bool _isOpening = false;
    private Light _flashlight;
    private Animator _animator;

    private void Start()
    {
        if (isLocalPlayer)
        {
            _gameMenuController = GetComponentInChildren<GameMenuController>();
            _animator = GetComponent<Animator>();
            SkinnedMeshRenderer[] skinnedMeshRenderer = gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer renderer in skinnedMeshRenderer)
                renderer.enabled = false;
            Transform cameraTransform = Camera.main.gameObject.transform;
            cameraTransform.parent = ÑameraMountPoint.transform;
            cameraTransform.SetPositionAndRotation(ÑameraMountPoint.transform.position, ÑameraMountPoint.transform.rotation);
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

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");
            Move(x, z);

            _velocity.y += Gravity * Time.deltaTime;
            Controller.Move(Speed * Time.deltaTime * _velocity);
            
            if (Input.GetButtonDown("Use") &&
                (_useDoorFlag || _keyDoorFlag || _offLaserFlag))
            {
                _isOpening = !_isOpening;
            }

            if (Input.GetKeyDown(KeyCode.F))
                _flashlight.gameObject.SetActive(!_flashlight.gameObject.activeSelf);
        }
    }

    private void Move(float x, float z)
    {
        Vector3 move = transform.right * x + transform.forward * z;
        Controller.Move(Speed * Time.deltaTime * move);
        _animator.SetFloat("X", x); 
        _animator.SetFloat("Z", z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Key"))
        {
            string color = other.gameObject.GetComponent<Key>().Color.ToString();
            _keys.Add(color);
            _gameMenuController.UpdateKeyAmountOnHUD(color, true);
            _anyKeyPicked = true;
            CmdDestroyObject(other.gameObject);
        }

        else if (other.CompareTag("PressurePlate"))
        {
            bool openStatus = other.gameObject.GetComponent<PressurePlateTrigger>()._isOpened;
            other.GetComponent<Collider>().enabled = false;
            if (openStatus == false)
                CmdPressureDoorOpen(other.gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Open door on E button.
        if (_isOpening && _useDoorFlag)
        {
            _isOpening = !_isOpening;
            _useDoorFlag = !_useDoorFlag;
            GameObject trigger = other.gameObject;
            if (trigger.CompareTag("DoorTrigger"))
            {
                CmdOpenDoor(trigger);
                CmdDestroyObject(trigger);
            }
        }
        
        // Open door locked by key.
        else if (_anyKeyPicked && _isOpening && _keyDoorFlag)
        {
            _gameMenuController.UpdateKeyAmountOnHUD(other.gameObject.GetComponent<DoorTrigger>().Color.ToString(), false);
            _keys.Remove(other.GetComponent<DoorTrigger>().Color.ToString());
            _isOpening = !_isOpening;
            _anyKeyPicked = _keys.Count != 0;
            _keyDoorFlag = !_keyDoorFlag;
            GameObject trigger = other.gameObject;
            if (trigger.CompareTag("KeyDoorTrigger"))
            {
                CmdOpenDoor(trigger);
                CmdDestroyObject(trigger);
            }
        }
        
        // Disable laser.
        else if (_isOpening && _offLaserFlag)
        {
            _isOpening = !_isOpening;
            _offLaserFlag = !_offLaserFlag;
            CmdLaserToggleInitialize(other.gameObject);
        }
    }

    [Command]
    private void CmdLaserToggleInitialize(GameObject trigger) 
    {
        LaserTrigger laserTrigger = trigger.GetComponent<LaserTrigger>();
        foreach (var laser in laserTrigger._laserObjects)
            laser.GetComponent<Animator>().SetTrigger("Toggle");
        // Logic for single laser toggle.
        //Animator anim = laserObject.GetComponent<Animator>();
        //anim.SetTrigger("Toggle");
    }

    [Command]
    private void CmdPressureDoorOpen(GameObject trigger)
    {
        trigger.GetComponent<Collider>().enabled = false;
        
        PressurePlateTrigger doorTrigger = trigger.GetComponent<PressurePlateTrigger>();
        GameObject door = doorTrigger._doorObject;
        Animator anim = door.GetComponent<Animator>();
        
        anim.SetTrigger("Open");
        doorTrigger._isOpened = true;
    }

    [Command]
    private void CmdOpenDoor(GameObject trigger)
    {
        DoorTrigger doorTrigger = trigger.GetComponent<DoorTrigger>();
        GameObject door = doorTrigger._doorObject;
        Animator anim = door.GetComponent<Animator>();
       
        anim.SetTrigger("Open");
        doorTrigger._isOpened = true;
        
        trigger.GetComponent<Message>()._hasCollided = false;
    }

    [Command]
    private void CmdDestroyObject(GameObject obj)
    {
        NetworkServer.Destroy(obj);
    }
}
