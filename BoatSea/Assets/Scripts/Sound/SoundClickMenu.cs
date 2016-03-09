using UnityEngine;
using System.Collections;

public class SoundClickMenu : MonoBehaviour {

    void OnClick()
    {
        SoundController.PlayMenuClick();
    }
}
