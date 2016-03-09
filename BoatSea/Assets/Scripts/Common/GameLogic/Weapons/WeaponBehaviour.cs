using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Ship;
using Aratog.NavyFight.Models.Unity3D.Weapons;
using Assets.Scripts.Common.GameLogic.Multiplayer;
using UnityEngine;
using System.Collections;

public class WeaponBehaviour: PoolItem {

	#region Constants

	[HideInInspector]
	public const string PrefabPath = "Prefabs/Weapons/Basic/Base_";

	#endregion

	#region Variables

	[HideInInspector]
	public Weapon weaponShell;

	public WeaponsType type;

	[SerializeField]
	public TeamColor Team;


	/// <summary>
	/// for mortal
	public bool isMoratalStart = false;
	public Vector3 enemieTarget;
	
	public float gravity = 9.8f;
	
	public float firingAngle = 45;
	/// </summary>

	protected bool isSet = false;

	private Vector3 currentPosition;
	[HideInInspector] public Vector3 direction;

	[HideInInspector] public float StartTime;
	[HideInInspector] public float EndTime;
	[HideInInspector] public float CurrentTime;

	[HideInInspector] public int nearestX;
	[HideInInspector] public int nearestY;

	private Point nearestPoint;

	[HideInInspector]
	public int viewID;

	public Transform target;



	public bool IsRicoshet {
		set
		{

				Debug.Log("Ricoshet");
				int random = Random.Range(1,2);
				if(random ==1)
				{
				direction.x *=-1;
				}
				else 
				{
					direction.y *=-1;
				}

			
			canRickoshet--;
			if(canRickoshet<0)
			{
				BlowUp();
			}
		}
		get
		{
			return IsRicoshet;
		}
	}

	public int canRickoshet;
	

	#endregion

	#region MonoBehaviour events

	// Use this for initialization
	private void Awake () {

	}

	private void Start () {
		if (weaponShell != null || GameSetObserver.Instance.CurrentGameType != GameType.Multiplayer) {
				
			return;
				}
		PhotonView photonView = PhotonView.Get(this);

		int playerId = (int) photonView.instantiationData[1];

		Player player = GameController.Instance.GetPlayer(playerId);
		if (player == null) {
			Debug.LogError(string.Format("WeaponsBeaviour - Start: Can't find player with id:{0}", playerId));
			Debug.Log("check1");	
			return;
		}
		Debug.Log (type);
		switch (type) {
			case WeaponsType.BasicProjectile :
				weaponShell = player.MyShip.BasicWeapon;
				break;
			case WeaponsType.BasicTorpedo :
				weaponShell = player.MyShip.BasicWeapon;
				break;
			case WeaponsType.BasicBomb :
				weaponShell = player.MyShip.BasicBomb;
				break;
			case WeaponsType.Missile :
				weaponShell = player.MyShip.AdvanceWeapon;
				break;
		}

		Debug.Log (weaponShell);

		if (weaponShell != null)
			weaponShell.Owner = player;

		//Team = player.Team;
	}

	IEnumerator SimulateProjectile()
	{
		// Short delay added before Projectile is thrown
		yield return new WaitForSeconds (.1f);
		
		
		
		// Calculate distance to target
		float target_Distance = Vector3.Distance (transform.position, enemieTarget);
		
		// Calculate the velocity needed to throw the object to the target at specified angle.
		float projectile_Velocity = target_Distance / (Mathf.Sin (2 * firingAngle * Mathf.Deg2Rad) / gravity);
		
		// Extract the X  Y componenent of the velocity
		float Vx = Mathf.Sqrt (projectile_Velocity) * Mathf.Cos (firingAngle * Mathf.Deg2Rad);
		float Vy = Mathf.Sqrt (projectile_Velocity) * Mathf.Sin (firingAngle * Mathf.Deg2Rad);
		
		// Calculate flight time.
		float flightDuration = target_Distance / Vx;
		
		// Rotate projectile to face the target.
		transform.rotation = Quaternion.LookRotation (enemieTarget - transform.position);
		
		float elapse_time = 0;
		
		while (elapse_time < flightDuration) {
			transform.Translate (0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
			
			if (Vector3.Distance (transform.position, enemieTarget) <= 0.5f) {
				ShowExplosionFx(transform.position);
			}
			elapse_time += Time.deltaTime;
			
			yield return null;
		}
	}

	// Update is called once per frame
	private void Update () {

		if (!GameSetObserver.Instance.IsBattleStarted || GameSetObserver.Instance.IsPause)
			return;



		if (!isSet)
			return;

		if(type == WeaponsType.MortalProjectile)
		{

			if(!isMoratalStart)
			{
				isMoratalStart = true;
				StartCoroutine("SimulateProjectile");
			}
		}
		else if (weaponShell.Type == WeaponsType.HomingMissile)
		{
			if(!target)
			{
					if (CurrentTime > EndTime)
						{TimeToBlowUp ();}
						currentPosition += direction * weaponShell.Speed * Time.deltaTime;
						transform.position = currentPosition;
						transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (direction), Time.deltaTime * 200);
						}
					else
					{
				//поворачиваемся к цели
				transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(
					new Vector3(target.position.x,
				    0.0f, target.position.z) - new Vector3(
					transform.position.x, 0.0f, transform.position.z)),
				                                     1);
				// идем туда куда смотрим
				transform.position += transform.forward * weaponShell.Speed * Time.deltaTime;
					}

			} 
		else
		{
			if (CurrentTime > EndTime)
			{
				TimeToBlowUp ();
			}
		
				currentPosition += direction * weaponShell.Speed * Time.deltaTime;
		

				transform.position = currentPosition;
				transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (direction), Time.deltaTime * 200);
		
		}
		CurrentTime += Time.deltaTime;
	}



	private void OnCollisionEnter(Collision other)
	{

		

	}


	private void OnTriggerEnter (Collider other) {



		if (!target) {
			ShipBehaviour _ship = other.gameObject.GetComponent<ShipBehaviour>();
			if(_ship)
			{
				target = other.transform;
			}
			
		}
		
		//damage base
		if (other.gameObject.tag == "BaseDefense") {
			other.gameObject.SendMessageUpwards("AddDamage",this.weaponShell.Damage);
			ShowExplosionFx(transform.position);
			if(type != WeaponsType.OneRicochet){
			Deactivate();
			}
			else {
				IsRicoshet = true;
			}
		}
		
		
		if (other.transform.parent == null || (other.transform.parent.parent == null))
			return;
		
		WeaponBehaviour weapon = null;
		weapon = other.transform.parent.parent.GetComponent<WeaponBehaviour>();
		
		if (weapon != null)
		{
			if (this.weaponShell.Owner.Team != weapon.weaponShell.Owner.Team)
			{
				

				if(type != WeaponsType.OneRicochet){
					weapon.BlowUp(true);
					BlowUp(true);
				}
				else {
					IsRicoshet = true;
				}
				Debug.Log("Explosives");
				ShowExplosionFx(transform.position);
				ShowExplosionFx(weapon.transform.position);
			}
			return;
		}
		
		ShipBehaviour ship = null;
		ship = other.transform.parent.parent.GetComponent<ShipBehaviour>();
		
		if (ship != null)
			return;
		
		FlagsBehaviour flags = null;
		flags = other.transform.parent.parent.GetComponent<FlagsBehaviour>();
		
		if (flags != null)
			return;
		
		FlagSpotBehaviour flagSpot = null;
		flagSpot = other.transform.parent.parent.GetComponent<FlagSpotBehaviour>();
		
		if (flagSpot != null)
			return;
		if (other.gameObject.CompareTag ("Cannon"))
		{
			other.gameObject.transform.parent.parent.gameObject.SendMessageUpwards("GiveDamage",weaponShell.Owner.Team);
			if(type != WeaponsType.OneRicochet){
				BlowUp(false);
			}
			else {
				IsRicoshet = true;
			}
		}

		if (other.gameObject.CompareTag("Destructable"))
		{
			Destructable destructable = null;
			destructable = other.transform.parent.parent.GetComponent<Destructable>();
			
			if (destructable != null)
			{
				Vector3 correctPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 3);
				destructable.Hit(weaponShell.Damage, correctPos);
				if(type != WeaponsType.OneRicochet){
					BlowUp(false);
				}
				else {
					IsRicoshet = true;
				}

			}
			return;
		}
		
		SpawnPointBehaviour spawnPoint = null;
		spawnPoint = other.transform.parent.parent.GetComponent<SpawnPointBehaviour>();
		
		if (spawnPoint != null)
			return;
		
		ShowGroundExplosion(transform.position);
		
		if(type != WeaponsType.OneRicochet){
			BlowUp(false);
		}
		else {
			IsRicoshet = true;
		}

	}

	void OnTriggerStay(Collider c)
	{
		OnTriggerEnter(c);
	}



	#endregion

	#region Common events

	private void SetConfigParam(WeaponsType type,float speedChanger = 0)
	{
		weaponShell.Damage = ConfigWeapons.Weapon [type].Damage;
		weaponShell.Speed = ConfigWeapons.Weapon [type].Speed;
		if (weaponShell.Speed > 0) {
			weaponShell.Speed += speedChanger;
		}
	}
	public void SetBasicData (Vector3 startPosition, Transform parent, TeamColor Team,Player player, Vector3 dir,float bulletSpeedUpgrade) {
		//	transform.parent = parent;
		//	transform.localPosition = Vector3.zero;
		//	transform.localPosition = startPosition;
		//	transform.parent = null;
		
		transform.position = startPosition;
		currentPosition = transform.position;
		
		Team = Team;
	//	weapon.Owner = player;
		
		weaponShell = player.MyShip.BasicWeapon;
		
		StartTime = Time.time;
		CurrentTime = Time.time;
		
		
		Vector3 curPos = new Vector3(currentPosition.x, 0, currentPosition.z);
		Vector3 shipPos = new Vector3(parent.position.x, 0, parent.position.z);
		
		direction = parent.forward;

		weaponShell.BlowUpTimer = Time.time + weaponShell.Flight;
		
		EndTime = weaponShell.BlowUpTimer;
		
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), 0f);

	

		isSet = true;
		
		BattleController.Instance.AddShell(this);
		SetConfigParam (type,bulletSpeedUpgrade);
	}

	public void SetBasicData (Vector3 startPosition, Transform parent, Player player, Weapon weapon, Vector3 dir,float bulletSpeedUpgrade) {
	//	transform.parent = parent;
	//	transform.localPosition = Vector3.zero;
	//	transform.localPosition = startPosition;
	//	transform.parent = null;

		transform.position = startPosition;
		currentPosition = transform.position;

		Team = player.Team;
		weapon.Owner = player;

		weaponShell = weapon;

		StartTime = Time.time;
		CurrentTime = Time.time;


		Vector3 curPos = new Vector3(currentPosition.x, 0, currentPosition.z);
		Vector3 shipPos = new Vector3(player.Position.x, 0, player.Position.z);

		direction = parent.forward;


		weaponShell.BlowUpTimer = Time.time + weaponShell.Flight;

		EndTime = weaponShell.BlowUpTimer;

		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), 0f);

		isSet = true;

		BattleController.Instance.AddShell(this);
		SetConfigParam (type,bulletSpeedUpgrade);
	}

	public void SetBasicData (Vector3 startPosition, Transform parent, Player player, Vector3 dir,float bulletSpeedUpgrade) {
	//	transform.parent = parent;
	//	transform.localPosition = Vector3.zero;
	//	transform.localPosition = startPosition;
	//	transform.parent = null;



		Debug.Log ("Advance weapon create");

		transform.position = startPosition;

		currentPosition = transform.position;
		
		Team = player.Team;
	//	weapon.Owner = player;
		
	//	weaponShell = weapon;

		weaponShell = player.MyShip.AdvanceWeapon;


		weaponShell.Owner = player;

		StartTime = Time.time;
		CurrentTime = Time.time;
		
		
		Vector3 curPos = new Vector3(currentPosition.x, 0, currentPosition.z);
		Vector3 shipPos = new Vector3(player.Position.x, 0, player.Position.z);
		
		direction = parent.forward;


		weaponShell.BlowUpTimer = Time.time + weaponShell.Flight;
		
		EndTime = weaponShell.BlowUpTimer;
		
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction), 0f);
		
		isSet = true;
		
		BattleController.Instance.AddShell(this);
		SetConfigParam (type,bulletSpeedUpgrade);


	}

	private Point GetNearestPoint (Vector3 currentVector, Vector3 dir) {
		Point nearestPoint = null;
		Point currentPoint = Map.GetMapPosition(GameSetObserver.Instance.CurrentBattle.Map, currentVector);
		nearestPoint = new Point(currentPoint.X + Mathf.CeilToInt(dir.x), currentPoint.Y + Mathf.CeilToInt(dir.z));
		return nearestPoint;
	}

	protected virtual void TimeToBlowUp ()
	{
		Debug.Log ("Explosive");
		EffectsBehaviour.EffectsType explosionType = EffectsBehaviour.EffectsType.HitExplosion;

		switch (weaponShell.Type)
		{
			case WeaponsType.BasicBomb:
				explosionType = EffectsBehaviour.EffectsType.HitExplosion;
				break;

			case WeaponsType.BasicProjectile:
				explosionType = EffectsBehaviour.EffectsType.SplashForProjectile;
				break;

			case WeaponsType.BasicTorpedo:
				explosionType = EffectsBehaviour.EffectsType.HitExplosion;
				break;
			default:
				explosionType = EffectsBehaviour.EffectsType.SplashForProjectile;
				break;
		}

		EffectsBehaviour explosion =
			ResourceBehaviourController.Instance.GetEffectsFromPool(explosionType);

		if (explosion != null)
		{
			Vector3 pos = explosionType == EffectsBehaviour.EffectsType.SplashForProjectile
				              ? new Vector3(transform.position.x, 0, transform.position.z)
				              : transform.position;
			explosion.SetBasicData(pos);

            SoundController.PlayShipExplosion(explosion.gameObject);
		}

		BlowUp();
	}

	IEnumerator MortarExplosive()
	{
		//TODO: проверкa  команды 

		Debug.Log ("Corutine Started");
		ShipBehaviour[] AllShips;
		
		int count = BattleController.Instance.ships.Count;
		AllShips = new ShipBehaviour[count];
		AllShips = BattleController.Instance.ships.ToArray();
		
		int distance = 4;
//		TeamColor color = weaponShell.Owner.Team;
		Debug.Log(AllShips.Length);
		for (int i=0; i<AllShips.Length; i++)
		{
			Debug.Log(AllShips[i].Player.Team);
//			if(AllShips[i].Player.Team != color)
//			{
				

				if(Vector3.Distance(transform.position,AllShips[i].transform.position)<=distance)
				{
					
					AllShips[i].DealShipDamage(0,1);
				}
//			}	
		}
		BlowUp ();
		yield return new WaitForSeconds(1f);

	}

	public void ShowExplosionFx(Vector3 pos)
	{
		EffectsBehaviour explosion;
		if (type != WeaponsType.MortalProjectile) {
			explosion =
			ResourceBehaviourController.Instance.GetEffectsFromPool (EffectsBehaviour.EffectsType.HitExplosion);

		}
		else
		{
			 explosion =
				ResourceBehaviourController.Instance.GetEffectsFromPool (EffectsBehaviour.EffectsType.MortarExplosive);

			StartCoroutine("MortarExplosive");
		}

		if (explosion != null)
		{
			explosion.SetBasicData(pos);
            SoundController.PlaySBuildingExplosion(explosion.gameObject);
		}
	}

	private void ShowGroundExplosion(Vector3 pos)
	{
		EffectsBehaviour explosion =
			ResourceBehaviourController.Instance.GetEffectsFromPool(EffectsBehaviour.EffectsType.GroundExplosion);
		
		if (explosion != null)
		{
			explosion.SetBasicData(pos);
            SoundController.PlayGroundExplosion(explosion.gameObject);
		}
	}

	public virtual void BlowUp (bool force = false) {

		BattleController.Instance.RemoveShell(this);

		if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer) {
			if (force)
				MultiplayerManager.Instance.NeedBlowUpShell(MultiplayerManager.MyMultiplayerEntity.photonPlayer, viewID);
		}

		Pool.Push(this);

	}

	#endregion

	#region Ovveride events

	public override bool EqualsTo (PoolItem item) {
		if (!(item is WeaponBehaviour))
			return false;

		WeaponBehaviour weaponBehaviour = item as WeaponBehaviour;

		if (weaponBehaviour.type != type || weaponBehaviour.Team != Team)
			return false;

		return true;
	}

	public override void Activate () {
		base.Activate();
		gameObject.SetActive(true);
	}

	public override void Deactivate () {
		//Debug.Log ("Deactivate");
		base.Deactivate();
		gameObject.SetActive(false);
	}

	#endregion
}
