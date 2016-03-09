using Aratog.NavyFight.Models.Ships;
using UnityEngine;
using System.Collections;

public static class SoundController
{

    #region Shooting

    private const string ShootBoat = "ShootSound_4";
    private const string ShootSubmarine = "BombSplashSound_2";
    private const string ShootBigShip = "ShootSound_6";

    public static void PlaySalvoShoot(GameObject sourceGO, ShipType shipType, bool isPlayer)
    {
        string shootSound = "";
        
        switch (shipType)
        {
            case ShipType.Boat:
            {
                shootSound = ShootBoat;
                break;
            }

            case ShipType.Submarine:
            {
                shootSound = ShootSubmarine;
                break;
            }

            case ShipType.BigShip:
            {
                shootSound = ShootBigShip;
                break;
            }

            default:
            {
                shootSound = ShootBoat;
                break;
            }
        }
        AudioSource audioSource = SoundManager.PlaySFX(sourceGO,SoundManager.Load(shootSound));

        if (isPlayer)
        {
            //SoundManagerTools.make2D(ref audioSource);
        }
    }

    #endregion

    #region Engine

    private const string EngineBoat = "BoatEngine_1(loop)";
    private const string EngineSubmarine = "BoatEngine_3(loop)";
    private const string EngineBigShip = "BoatEngine_1(loop)";
    private const string EngineIdling = "BoatEngine_4(loop)";
    

    public static void PlayShipEngine(GameObject sourceGO, bool isHuman, ShipType shipType)
    {
        string engineSound = "";

        switch (shipType)
        {
            case ShipType.Boat:
                {
                    engineSound = EngineBoat;
                    break;
                }

            case ShipType.Submarine:
                {
                    engineSound = EngineSubmarine;
                    break;
                }

            case ShipType.BigShip:
                {
                    engineSound = EngineBigShip;
                    break;
                }

            default:
                {
                    engineSound = EngineBoat;
                    break;
                }
        }
        

        float volume = Options.SFXVolume * 1.0f;

        if (isHuman == false)
        {
            volume = Options.SFXVolume * 0.25f;
        }

        SoundManager.PlaySFX(sourceGO, SoundManager.Load(engineSound), true, volume);
    }

    public static void StopPlayShipEngine(GameObject sourceGO, bool isHuman)
    {

        float volume = Options.SFXVolume*1.0f;

        if (isHuman == false)
        {
            volume = Options.SFXVolume*0.25f;
        }

        AudioSource audioSource = SoundManager.PlaySFX(sourceGO, SoundManager.Load(EngineIdling), true, volume);

        if (isHuman == true)
        {
            //SoundManagerTools.make2D(ref audioSource);
        }
    }

    #endregion

    #region Explosion

    private const string ShipExplosion = "ExplosionSound_1";
    private const string BuildingExplosion = "ExplosionSound_2";

    public static void PlayShipExplosion(GameObject sourceGO)
    {
        SoundManager.PlaySFX(sourceGO, SoundManager.Load(ShipExplosion));
    }

    public static void PlaySBuildingExplosion(GameObject sourceGO)
    {
        SoundManager.PlaySFX(sourceGO, SoundManager.Load(BuildingExplosion));
    }

    public static void PlayGroundExplosion(GameObject sourceGO)
    {
        SoundManager.PlaySFX(sourceGO, SoundManager.Load(BuildingExplosion));
    }
    
    #endregion

    #region Fanfare 

    private const string FanfarePossitiveName = "FanfarePossitive";
    private const string FanfareNegativeName = "FanfareNegative";


    public static void PlayPossitiveFanfare()
    {
        SoundManager.PlaySFX(SoundManager.LoadFromGroup(FanfarePossitiveName));
    }

    public static void PlayNegativeFanfare()
    {
        SoundManager.PlaySFX(SoundManager.LoadFromGroup(FanfareNegativeName));
    }

    #endregion

    #region Splash

    private const string SplashGroupName = "BombSplash";

    public static void PlayRandomBombSplashSound(GameObject sourceGO)
    {
        SoundManager.PlaySFX(sourceGO, SoundManager.LoadFromGroup(SplashGroupName));
    }

    #endregion

    #region Ship Sunk

    private const string ShipSunkName = "ShipSunkSound_1";

    public static void PlayShipSunk(GameObject sourceGO)
    {
        //Debug.Log("Play shipsunk");
        SoundManager.PlaySFX(sourceGO, SoundManager.Load(ShipSunkName));
    }
    
    #endregion

    #region Metal Scratches

    private const string MetalScratchGroupName = "MetalScratch";

    public static void PlayRandomMetalScratchSound(GameObject sourceGO)
    {
        SoundManager.PlaySFX(sourceGO, SoundManager.LoadFromGroup(MetalScratchGroupName));
    }
    
    #endregion

    #region Menu

    private const string MenuClick = "MenuClick";

    public static void PlayMenuClick()
    {
        SoundManager.PlaySFX(SoundManager.Load(MenuClick));
    }

    private const string MenuOpen = "MenuOpenNewWindow";

    public static void PlayMenuOpen()
    {
        SoundManager.PlaySFX(SoundManager.Load(MenuOpen));
    }

    private const string SerYesSerGroup = "SerYes";

    public static void PlaySerYesSer()
    {
        SoundManager.PlaySFX(SoundManager.LoadFromGroup(SerYesSerGroup));
    }

    #endregion

    #region Misc

    private const string AlarmSoundName = "AlarmSound_1";

    public static AudioSource PlayAlarmSound()
    {
        return SoundManager.PlaySFX(SoundManager.Load(AlarmSoundName));
    }

    public static void StopAlarmSound(AudioSource audioSource)
    {
        SoundManager.StopSFXObject(audioSource);
    }

    #endregion
}
