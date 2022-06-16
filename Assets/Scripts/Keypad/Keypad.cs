using Mirror;
using TMPro;
using UnityEngine;

public class Keypad : MonoBehaviour
{
    [SerializeField] private NumberKey[] _numberKeys;
    [SerializeField] private ClearKey _clearKey;
    [SerializeField] private EnterKey _enterKey;
    
    [SerializeField] private TextMeshProUGUI _codeField;
    [SerializeField] private KeypadBehaviourHandler _keypadCanvas;

    public delegate void CodeEnteredHandler();
    public event CodeEnteredHandler CodeEntered;
    
    private void Start()
    {
        _codeField.text = "";
        foreach (var key in _numberKeys)
            key.Interacted += Add;
        _clearKey.Interacted += Clear;
        _enterKey.Interacted += Enter;
    }

    private void Add(string value)
    {
        if (_codeField.text.Length >= 6)
        {
            _enterKey.GetFailedClip().Play();
            return;
        }
        _codeField.text += value;
    }

    private void Clear()
    {
        _codeField.text = "";
    }

    private void Enter()
    {
        if (_codeField.text != _keypadCanvas.GetKeypadCode())
        {
            Clear();
            _enterKey.GetFailedClip().Play();
        }
        else
        {
            CodeEntered?.Invoke();
        }
    }

    private void OnDestroy()
    {
        foreach (var key in _numberKeys)
            key.Interacted -= Add;
        _clearKey.Interacted -= Clear;
        _enterKey.Interacted -= Enter;
    }
}
