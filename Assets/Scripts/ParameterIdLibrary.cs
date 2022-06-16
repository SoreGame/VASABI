using UnityEngine;

// Library of all Id of animator parameters. 
public class ParameterIdLibrary : MonoBehaviour
{
    // Player animation
    public static readonly int x = Animator.StringToHash("X");
    public static readonly int z = Animator.StringToHash("Z");
    // Laser
    public static readonly int toggle = Animator.StringToHash("Toggle");
    // Door
    public static readonly int openTrigger = Animator.StringToHash("Open");
    public static readonly int closeTrigger = Animator.StringToHash("Close");
}
