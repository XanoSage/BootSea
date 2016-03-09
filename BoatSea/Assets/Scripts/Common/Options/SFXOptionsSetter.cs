using UnityEngine;
using System.Collections;

public class SFXOptionsSetter : OptionsSetter
{

    public UICellSlider UiCellSlider;

	// Use this for initialization
    private void Start()
    {
        SetValue();
    }

    private void SetValue()
    {
        if (UiCellSlider != null)
        {
            float sfxVolumeVal = Options.SFXVolume;

            int numberOfToggles = System.Convert.ToInt32(UiCellSlider._toggles.Count*sfxVolumeVal);

            UiCellSlider.Value = numberOfToggles;
        }
    }

    public override void SetOptionValue(float value)
    {
        Options.SFXVolume = value;
    }
}
