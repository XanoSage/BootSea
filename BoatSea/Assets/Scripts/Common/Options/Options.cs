using UnityEngine;
using Aratog.NavyFight.Models.Games;
using ViageSoft.Parsers.Common;

public static class Options
{
	const string IsFirstLaunchID = "IsFirstLaunch";
	public static bool IsFirstLaunch
	{
		get
		{
			bool firstLaunch = (PlayerPrefs.GetInt(IsFirstLaunchID) == 0) ? false : true;
			if (firstLaunch)
				PlayerPrefs.SetInt(IsFirstLaunchID, 1);
			return firstLaunch;
		}
		set
		{
			PlayerPrefs.SetInt(IsFirstLaunchID, (value == true) ? 1 : 0);
		}
	}

	const string MechanicsID = "Options_Mechanics";
	public static MechanicsType Mechanics
	{
		get
		{
			return (MechanicsType)PlayerPrefs.GetInt(MechanicsID);
		}
		set
		{
			PlayerPrefs.SetInt(MechanicsID, (int)value);
		}
	}
	
	public static void ResetAll()
	{
		IsFirstLaunch = false;
	}

    const string MusicVolumeId = "MusicVolume";
    public static float MusicVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(MusicVolumeId,0.5f);
        }

        set
        {
            PlayerPrefs.SetFloat(MusicVolumeId,value);

            SoundManager.SetVolumeMusic(value);
        }
    }

    const string SFXVolumeId = "SFXVolume";
    public static float SFXVolume
    {
        get
        {
            return PlayerPrefs.GetFloat(SFXVolumeId, 1.0f);
        }

        set
        {
            PlayerPrefs.SetFloat(SFXVolumeId, value);

            SoundManager.SetVolumeSFX(value);
        }
    }
}
