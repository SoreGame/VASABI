using UnityEngine;
using Mirror;

public class SequencePlate : NetworkBehaviour
{
    [SerializeField]
    GameObject doorObject;
    
    [SerializeField]
    [Header("MARK AS TRUE FOR PART OF SEQUENCE")]
    internal bool isCorrect;

    [SerializeField]
    [Header("MARK AS TRUE FOR INCORRECT PART OF SEQUENCE")]
    internal bool incorrect;
    [SerializeField]
    internal bool isOpened = false;

    private void OnTriggerEnter(Collider playerCollider)
    {
        if (playerCollider.CompareTag("Player"))
        {
            switch(isCorrect)
            {
                case true:
                    isOpened = true;
                    break;
                case false:
                    doorObject.GetComponent<Door_>().ResetTriggers();
                    break;
            }
        }
    }
}