using UnityEngine;
using System.Collections;

public class MusicOptionsSetter : OptionsSetter {

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
            float volumeVal = Options.MusicVolume;

            int numberOfToggles = System.Convert.ToInt32(UiCellSlider._toggles.Count * volumeVal);

            UiCellSlider.Value = numberOfToggles;
        }
    }

    public override void SetOptionValue(float value)
    {
        Options.MusicVolume = value;
    }
}
