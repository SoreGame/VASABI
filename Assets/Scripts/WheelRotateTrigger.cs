using UnityEngine;

public class WheelRotateTrigger : MonoBehaviour
{
    public delegate void TriggerHandler();
    public event TriggerHandler Activated;
    public event TriggerHandler Exited;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Activated?.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Exited?.Invoke();
        }
    }
}