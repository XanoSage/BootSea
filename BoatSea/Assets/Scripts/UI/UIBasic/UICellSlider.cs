using System;
using System.Collections.Generic;

using UnityEngine;

public class UICellSlider : MonoBehaviour
{
    public List<GameObject> _toggles;
   

    [SerializeField]
    private OptionsSetter _optionsSetter;

    private int _value;
    public int Value
    {
        get { return _value; }
        set
        {
            _value = value;
            Toggle(_toggles[_value - 1]);

            _optionsSetter.SetOptionValue((float)_value / (float)_toggles.Count);
        }
    }

    private float _valueInPercent;
    public float ValueInpercent
    {
        get { return _value; }
        set
        {
            _valueInPercent = value;

            int numberOfTogglesToShow = System.Convert.ToInt32(_toggles.Count * _valueInPercent);

            Toggle(_toggles[numberOfTogglesToShow - 1]);
        }
    }

    private void Start()
    {
        foreach (GameObject toggle in _toggles)
        {
            UIButtonMessage message = toggle.AddComponent<UIButtonMessage>();
            message.target = gameObject;
            message.functionName = "SetValue";
            message.trigger = UIButtonMessage.Trigger.OnClick;
        }
    }

    public void SetValue(GameObject toggle)
    {
        int value = _toggles.IndexOf(toggle) + 1;

        Value = value;
        
    }

    private void HideAllToggles()
    {
        foreach (var t in _toggles)
        {
            t.GetComponent<UIToggle>().isChecked = false;
        }
    }

    private void Toggle(GameObject toggle)
    {
        foreach (var t in _toggles)
        {
            if (_toggles.IndexOf(t) <= _toggles.IndexOf(toggle))
            {
                t.GetComponent<UIToggle>().isChecked = true;
            }
            else
            {
                t.GetComponent<UIToggle>().isChecked = false;
            }
        }
    }

}