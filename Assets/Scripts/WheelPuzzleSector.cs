using UnityEngine;

public class WheelPuzzleSector : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private WheelRotateTrigger _circleTrigger;
    private Vector3 _boundsCenterPosition;
    private readonly Vector3 _zAxis = new Vector3(0, 0, 1);
    private readonly Vector3 _xAxis = new Vector3(1, 0, 0);
    private bool _isTriggered;
    private bool _puzzleCompleted;

    public delegate void CollisionHandler();

    public event CollisionHandler Connected;
    public event CollisionHandler Disconnected;

    private void Start()
    {
        _boundsCenterPosition = gameObject.GetComponent<Renderer>().bounds.center;
        _circleTrigger.Activated += SetActive;
        _circleTrigger.Exited += SetDeactive;
    }

    private void Update()
    {
        if (_isTriggered)
        {
            gameObject.transform.RotateAround(_boundsCenterPosition, _zAxis, _speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sector"))
            Connected?.Invoke();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sector"))
            Disconnected?.Invoke();
    }

    private void SetActive()
    {
        if (_puzzleCompleted) return;
        _isTriggered = true;
    }
    
    private void SetDeactive()
    {
        if (_puzzleCompleted) return;
        _isTriggered = false;
    }

    private void OnDestroy()
    {
        _circleTrigger.Activated -= SetActive;
        _circleTrigger.Exited -= SetDeactive;
    }

    public void DisableSectorRotation()
    {
        _puzzleCompleted = true;
        _isTriggered = false;
    }
}