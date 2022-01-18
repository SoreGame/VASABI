using System.Collections;
using UnityEngine;
using System.Linq;

public class Door_ : MonoBehaviour
{
    [SerializeField] private GameObject[] _triggers;
    private Animator _animator;

    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        if (IsCorrectSequencePressed(_triggers) == true)
            StartCoroutine(AnimationPlayCoroutine());
    }

    private IEnumerator AnimationPlayCoroutine()
    {
        _animator.Play("OpenDoor");
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
        DeactivateTriggers(_triggers);
    }

    private void DeactivateTriggers(GameObject[] triggers)
    {
        foreach (var trigger in triggers)
            trigger.SetActive(false);
    }

    internal bool IsCorrectSequencePressed(GameObject[] plates)
    {
        for (int i = 0; i < plates.Length; i++)
        {
            if (plates[i].GetComponent<SequencePlate>().isOpened == false)
                return false;
        }
        return true;
    }

    internal void ResetTriggers()
    {
        foreach (var sp in from trigger in _triggers
                           let sp = trigger.GetComponent<SequencePlate>()
                           select sp)
        {
            if (sp.incorrect)
                sp.isOpened = true;
            else
                sp.isOpened = false;
        }
    }
}
