using UnityEngine;
using System.Collections;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Weapons;

public class CannonMortalController : CannonBasic {


	// Use this for initialization
	void Start () {
		base.Start ();
	}

	public override void Shoot ()
	{
		//TODO: IN multiplayer need change INSTANCE (from pool items) Boolet and Shoot Particle
		if (ShootCurTime <= 0) {
			
			
			for (int b =0; b<ShootPos.Length; b++) {
				
			
				
						Debug.Log ("Shoot");
					//	bulets [i].SetActive (true);
					//	bulets [i].transform.position = ShootSparks [b].transform.position;
					//	bulets [i].transform.rotation = ShootSparks[b].transform.rotation;


						WeaponBehaviour weapon = ResourceBehaviourController.Instance.GetWeaponsFromPool(WeaponsType.MortalProjectile,TColor);

						ShootCurTime = ShootWaitTime;
						weapon.isMoratalStart = false;
						// Задаем местоположение старта стрельбы и направление
						weapon.SetBasicData(ShootSparks [b].transform.position, transform, TColor,AllShips[0].Player, Vector3.left,5);
						weapon.enemieTarget = CurrTarget.transform.position;
					

						//активируем ефект выстрела из пулла
						EffectsBehaviour.EffectsType effectsType = EffectsBehaviour.EffectsType.SparksRed;
						EffectsBehaviour fireFx =
						ResourceBehaviourController.Instance.GetEffectsFromPool(effectsType);
						if (fireFx != null)
						{
							fireFx.transform.position = ShootSparks [b].transform.position;
							fireFx.SetBasicData(fireFx.transform.position);
							//	SoundController.PlaySalvoShoot(fireFx.gameObject, ShipType.Boat, Player is HumanPlayer);
						}
					//	break;
					

			}
		}

	}




	// Update is called once per frame
	public override	void Update () {
		//поворачиваем пушку
		if (CurrTarget) {
			Quaternion rotation = Quaternion.LookRotation(CurrTarget.transform.position-transform.position,Vector3.back);
			rotation.x = 0;
			rotation.z = 0;
			transform.rotation = Quaternion.Slerp(transform.rotation,rotation,Time.deltaTime*RotationSpeed);
		}
		base.Update ();
	}
}
