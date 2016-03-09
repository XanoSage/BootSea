﻿using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Weapons;

namespace Aratog.NavyFight.Models.Unity3D.Ship
{
   public class ComandBase : Ship
    {
       public ComandBase(){
           Type = ShipType.ComandBase;
           Speed = 0.07f;
           MaxSpeed = 1.5f;
           RotationSpeedCoeff = 1.75f;

           BasicHealthPoint = 3;
           BasicBombCount = 5;

           HealthPoint = BasicHealthPoint;
           BombCount = BasicBombCount;

           RotationSpeed = 2.2f;

           Size = 2;

           CameraFOVFrom = 54;
           CameraFOVTo = 75;
       }

       public override void OnMove()
       {
         //  throw new NotImplementedException();
       }
       public override void OnRespawn(bool invulnerability = false)
       {
           base.OnRespawn(invulnerability);

           HealthPoint = BasicHealthPoint;
       }
		
    }
}