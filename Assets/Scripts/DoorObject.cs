using UnityEngine;

public class DoorObject : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    public bool OpenState { get; set; }
    public Animator GetAnimator() => _animator;
}
