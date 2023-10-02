using System.Collections;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.Variables;
using TMPro;
using UnityEngine;

public class UiTextIntInjector : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;
    [SerializeField] private string _replaceString = "<var>";
    [SerializeField] private IntVariable _variable;

    private void Awake()
    {
        UpdateText();
        _variable.Subscribe(UpdateText);
    }

    private void OnDestroy()
    {
        _variable.Unsubscribe(UpdateText);
    }

    protected void UpdateText()
    {
        _text.text = _text.text.Replace(_replaceString, _variable.Value.ToString());
    }
}
