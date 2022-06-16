using UnityEngine;

public class Key : MonoBehaviour
{
    // Every key has specific color.
    public enum KeyColor { Red, Green, Blue, Yellow, Pink, White }
    public KeyColor Color;

    public string GetKeyColor() => Color.ToString();
}