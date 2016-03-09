using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Weapons;


[RequireComponent(typeof (PhotonView))]
public class CannonBasic : PoolItem {
	public CannonType Type;

	public int Health;
	public float RotationSpeed;

	public float ShootWaitTime;
	public float ShootCurTime;

	public float ShootDistance;

	public TeamColor TColor;

	public GameObject[] bulets;

	public Transform [] ShootPos;
	public GameObject [] ShootSparks;

	public ShipBehaviour CurrTarget;


	public ShipBehaviour[] AllShips;

	public List<ShipBehaviour> TargetShips;

	public Player player;


	// True if objects is deactivated and in pool.
	public bool IsInPool { get; private set; }	

	// Use this for initialization
	public virtual void Start () {

		player = Player.CreatePlayer (PlayerType.AIPlayer,false,TColor);

		//load cannon config
		Health = ConfigCannons.Cannons [Type].health;
		RotationSpeed = ConfigCannons.Cannons [Type].RotationSpeed;
		ShootDistance = ConfigCannons.Cannons [Type].Distance;
		ShootWaitTime = ConfigCannons.Cannons [Type].ShootSpeed;

		// init enemie List
		int count = BattleController.Instance.ships.Count;
		AllShips = new ShipBehaviour[count];
		AllShips = BattleController.Instance.ships.ToArray();
		TargetShips = new List<ShipBehaviour>();
		for (int i=0; i<AllShips.Length; i++)
		{
			if(AllShips[i].Player.Team != TColor)
			{
				TargetShips.Add(AllShips[i]);
			}	
		}

	}


	// Activating happens on removing object from pool.
	public override void Activate() {
		base.Activate ();
	}
	
	// Deactivating happens on pushing object to pool.
	public override void Deactivate() {
		base.Deactivate ();
	}
	
	// Items can have their specific ways to recognize equal ones.
	public override bool EqualsTo(PoolItem item)
	{
		return true;
	}


	public void GiveDamage(TeamColor color)
	{
		if (color != TColor) {
			Health -= 1;
			if (Health <= 0) {
				Die ();
			}
		}
	}




	public void Die ()
	{
		Debug.Log ("Die");

		EffectsBehaviour.EffectsType effectsType = EffectsBehaviour.EffectsType.ExplosionWithSmoke;
		EffectsBehaviour fireFx =
			ResourceBehaviourController.Instance.GetEffectsFromPool(effectsType);
		if (fireFx != null)
		{
			fireFx.transform.position = transform.position;
			fireFx.SetBasicData(fireFx.transform.position);
			//	SoundController.PlaySalvoShoot(fireFx.gameObject, ShipType.Boat, Player is HumanPlayer);
		}
		gameObject.SetActive (false);
	}



	public virtual void  Shoot()
	{
		//TODO: IN multiplayer need change INSTANCE (from pool items) Boolet and Shoot Particle
		if (ShootCurTime <= 0) {
	

				for (int b =0; b<ShootPos.Length; b++) {


							Debug.Log ("Shoot");
						//	bulets [i].SetActive (true);
						//	bulets [i].transform.position = ShootPos [b].position;
						//	bulets [i].transform.rotation = ShootPos[b].rotation;

						WeaponBehaviour weapon = ResourceBehaviourController.Instance.GetWeaponsFromPool(WeaponsType.BasicProjectile,
						                                                                                 TColor);
						// Задаем местоположение старта стрельбы и направление
						weapon.SetBasicData(ShootPos [b].position, transform, TColor,AllShips[0].Player, Vector3.left,5);


							ShootCurTime = ShootWaitTime;
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

					
				}
		}
	}

	public virtual void Update () {
		if (ShootCurTime > 0) {
			ShootCurTime-=Time.deltaTime;
		}



		//List search target
	
		if (!CurrTarget)
		{
			for (int i=0; i<TargetShips.Count; i++) 
			{
				if(Vector3.Distance(transform.position,TargetShips[i].transform.position)<=ShootDistance)
				{
					CurrTarget = TargetShips[i];
					break;
				}
			}
		}
		else 
		{
			if(Vector3.Distance(transform.position,CurrTarget.transform.position)>ShootDistance)
			{
				CurrTarget = null;
				return;
			}

			//Raycast Checker
			for (int i=0; i<ShootPos.Length; i++) {
				Vector3 forward = ShootPos [i].transform.TransformDirection (Vector3.forward);
				RaycastHit hit;
				Ray ray = new Ray (ShootPos [i].position, forward);
				if (Physics.Raycast (ray, out(hit), ShootDistance)) {
					if(hit.collider.gameObject.layer ==12)
					{
						if (hit.collider.transform.parent.parent.gameObject == CurrTarget.gameObject)
						{
							Shoot ();
						}
					}
				}
				Debug.DrawRay (ShootPos [i].position, ShootPos [i].transform.TransformDirection (Vector3.forward) * ShootDistance);
			}
		}




	}



}
