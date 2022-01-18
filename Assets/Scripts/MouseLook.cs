using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity = 150f;
    private Transform _agentBody;
    private float _xRotation = 0f;

    void Start()
    {
        _agentBody = gameObject.transform.parent.parent;
        Cursor.lockState = CursorLockMode.Locked;        
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity * Time.deltaTime;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        _agentBody.Rotate(Vector3.up * mouseX);
    }
}
