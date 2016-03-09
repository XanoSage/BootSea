using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Flags;
using Aratog.NavyFight.Models.Unity3D.Weapons;
using Assets.Scripts.Common.GameLogic.Multiplayer;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Ship;
using Aratog.NavyFight.Models.Maps;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Extensions;

[RequireComponent(typeof (PhotonView))]
public class ShipBehaviour : PoolItem
{

	#region Variables

	//Параметры для однозначного определения корабля в пуле объектов
	public ShipType shipType;
	public TeamColor color;
	public EnvironmentType environmentType;
	public int health = 0;

	public int AdwanceWeapon;
	//Параметры для задания статовой точки появления снарядов
	[SerializeField] private Transform FirstShootPosition;
	private Transform SecondShootPosition;
	private Transform BombShootPosition;

	//Счётчики таймеров для различных состояний корабля
	private float FireCooldownCounter;
	private float ShipCooldownCounter;
	private float ShipSunkTimerCount;
	private float _submarineDiveStartTime;

	private float _afterHitBeforeSunkDelay;

	private float InvulnerabilityCounter;

	public Transform SmokeHitEffectsTransform;

	private EffectsBehaviour _smokeHitEffects;

	//Ссылка на игрока, кто управляет данным плавсредством
//	[HideInInspector]
	public Player Player;

	//Вращающиеся элементы плавсредства
	[SerializeField] private RotationPart[] _rotationParts;

	public TrailRenderer Trail;

	//Различные флаги состояний плавсредства
	private bool IsCanShoot;
	private bool IsPauseAfterShooting;
	//private bool IsShipSunk;
	private bool IsTweaking;

	private bool IsNeedUseTransorm;

	private ShipsHealthBar _HealtBar;

	public int iArmor;

	//для улучшений
	private float bulletSpeed;
	private bool isTral = false;

	private bool isTwoShoot = false;

	private bool isDestroeyr { get; set; }

	private List<Vector3> _previousPosition;
	private Vector3 tweakingDirection;

	[SerializeField] private Vector3 nearestPointPosition;

	//Вражеский флаг, если мы его взяли
	[HideInInspector] public FlagsBehaviour enemyFlag;

	private int id;

	//
	private Renderer[] _meshRenderer;

	[SerializeField] private GameObject acelerator;
	[SerializeField] private GameObject armor;
	[SerializeField] private GameObject acelerator2;
	[SerializeField] private GameObject fastProgectail;
	[SerializeField] private GameObject armor2;
	[SerializeField] private GameObject armor3;
	[SerializeField] private GameObject rapidFire;
	[SerializeField] private GameObject trawl;

	private Rigidbody _rigidbody ;

	public Rigidbody ShipRigidbody
	{
		get { return _rigidbody; }
	}

	private ShipMovement _shipMovement;

	#endregion

	#region MonoBehaviour Events

	private void Awake()
	{
		id = 0;
		enemyFlag = null;
		_previousPosition = new List<Vector3>();

		_rotationParts = GetComponentsInChildren<RotationPart>();

		ShootTransform[] shootTransform = GetComponentsInChildren<ShootTransform>();

		//Определяем место вылета снарядов и бомб
		if (shootTransform.Length > 1)
		{
			float local = 1/transform.localScale.x;

			ShootTransform shoot =
				shootTransform.FirstOrDefault(shTransform => shTransform.PositionType == ShootTransform.ShootPositionType.FirstShoot);
			if (shoot != null)
			{
				FirstShootPosition = shoot.transform;
			}

			shoot =
				shootTransform.FirstOrDefault(shTransform => shTransform.PositionType == ShootTransform.ShootPositionType.BombShoot);

			if (shoot != null)
			{
				BombShootPosition = shoot.transform;
			}

			shoot =
				shootTransform.FirstOrDefault(
					shTransform => shTransform.PositionType == ShootTransform.ShootPositionType.SecondShoot);

			if (shoot != null)
			{
				SecondShootPosition = shoot.transform;
			}
		}
		else
		{
			//	Debug.LogError("Can't find 'ShootTransform' component on this prefab");
		}


		IsCanShoot = true;

		IsTweaking = false;

		IsNeedUseTransorm = false;

		tweakingDirection = Vector3.zero;

		_meshRenderer = GetComponentsInChildren<MeshRenderer>();

		if (_meshRenderer == null)
		{
			//	Debug.LogError("Can't find mesh renderer in the game object");
		}

		_rigidbody = GetComponent<Rigidbody>();
	}


	private void Start()
	{
		if (Player == null && GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer)
		{

			PhotonView photonView = PhotonView.Get(this);
			if (photonView != null)
			{
				//		Debug.Log(string.Format("PhotonView: {0}", photonView));

				int playerId = (int) photonView.instantiationData[0];
				Player player = GameSetObserver.Instance.GetPlayer(playerId);
				if (player != null)
				{
					Player = player;
					id = Player.Id;
					SetEventHandler();
					BattleController.Instance.ships.Add(this);
					BattleController.Instance.PositionablesObject.Add(player);
				}
				else
				{
					//		Debug.LogError(string.Format("Player with id {0}, not found", playerId));
				}
			}
		}

		if (Player == null)
		{
			//		Debug.LogError("ShipBeaviour: Player is not setted");
			return;
		}


		Player.OnMovementStateChanged += OnMovementStateChanged;

		Player.Forward = transform.forward;

		if (Player is AIPlayer)
		{
			PathFindingHelper.PathReturn += (Player as AIPlayer).PathSet;

			AIShipBehaviourHelper aiShipBehaviourHelper = gameObject.AddComponent<AIShipBehaviourHelper>();

			if (null != aiShipBehaviourHelper)
			{
				aiShipBehaviourHelper.InitAiPlayer(Player as AIPlayer);
			}

			if (Player.Team == TeamColor.BlueTeam)
			{
				AIPlayer aiPlayer = Player as AIPlayer;

			}
			Player.IsEventWasInitiated = true;

			if (Player == null)
			{
				return;
			}
//			Debug.Log(string.Format("Ai Player was init id: {0}", Player.Id));
		}

		//передаем параметры улучшеного оружия
		Player.MyShip.AdvanceWeapon = Weapon.CreateWeapon(Player.AdvanceWeapon);
		//Отображаем и задаем параметры улучшений

		if (Player.upgrades != null)

			if (Player.upgrades.Length >= 1)
			{
				for (int i = 0; i < Player.upgrades.Length; i++)
				{
					if (Player.upgrades[i] == UpgradesType.Accelerator)
					{
						Player.MyShip.MaxSpeed += (Player.MyShip.MaxSpeed*ConfigUpgrades.Upgrades[UpgradesType.Accelerator].value);
						acelerator.SetActive(true);
					}
					else if (Player.upgrades[i] == UpgradesType.AcceleratorInf)
					{
						Player.MyShip.MaxSpeed += ConfigUpgrades.Upgrades[UpgradesType.AcceleratorInf].value;
						acelerator2.SetActive(true);
					}
					else if (Player.upgrades[i] == UpgradesType.Armor)
					{
						Player.MyShip.BaseArmor += (int) ConfigUpgrades.Upgrades[UpgradesType.Armor].value;
						armor2.SetActive(true);
					}
					else if (Player.upgrades[i] == UpgradesType.ArmorAdvance)
					{
						Player.MyShip.BaseArmor += (int) ConfigUpgrades.Upgrades[UpgradesType.ArmorAdvance].value;
						armor3.SetActive(true);
					}
					else if (Player.upgrades[i] == UpgradesType.FastShell)
					{
						bulletSpeed = ConfigUpgrades.Upgrades[UpgradesType.FastShell].value;
						fastProgectail.SetActive(true);
					}
					else if (Player.upgrades[i] == UpgradesType.IceHouseDestroy)
					{
						isTral = true;
						trawl.SetActive(true);
					}
					else if (Player.upgrades[i] == UpgradesType.RapidShot)
					{
						float value = Player.MyShip.BasicWeapon.FireCooldownCount;
						Player.MyShip.BasicWeapon.FireCooldown -= ConfigUpgrades.Upgrades[UpgradesType.RapidShot].value;
						if (Player.MyShip.BasicWeapon.FireCooldown <= 0)
						{
							Player.MyShip.BasicWeapon.FireCooldown = value;
							Debug.LogError("Wrong param");
						}
						rapidFire.SetActive(true);
					}
				}
			}

		if (PlayerInfo.Instance != null)

			if (!PlayerInfo.Instance.HealthBar)
			{
				Transform healthBar = transform.FindChild("HealthBar") ;

				if (healthBar != null)
				{
					_HealtBar = healthBar.GetComponent<ShipsHealthBar>();
					_HealtBar.Init();
				}
			}
			else
			{
				Transform healthBar = transform.FindChild("HealthBar") ;

				if (healthBar != null)
				{
					_HealtBar = healthBar.GetComponent<ShipsHealthBar>();
					_HealtBar.gameObject.SetActive(false);
				}
			}

		Player.MyShip.Armor = Player.MyShip.BaseArmor;

		if (Player.Team == TeamColor.BlueTeam && Player != GameSetObserver.Instance.Human)
		{
			id = Player.Id;
		}

		_shipMovement = GetComponent<ShipMovement>();

		//Destroy(this);

		if (Player != GameSetObserver.Instance.Human)
		{
			return;
		}

		HUDButtons.Instance.SetAdvanceWeaponIcon(Player.AdvanceWeapon);
		HUDButtons.Instance.SetSpecialCountLabel(Player.AdvanceWeaponNumber);

		HUDJoystick.Instance.OnDirectionChangeEvent += OnDirectionChange;
		HUDJoystick.Instance.On90DegreeRotationEvent += On90DegreeRotation;

		CameraFollowsShip.Instance.FovFrom = Player.CameraFOVFrom;
		CameraFollowsShip.Instance.FovTo = Player.CameraFOVTo;
	}

	private void OnMovementStateChanged()
	{
		_submarineDiveStartTime = Time.time;
	}

	private void Update()
	{
		if (!GameSetObserver.Instance.IsBattleStarted || IsPauseAfterShooting || GameSetObserver.Instance.IsPause)
			return;

		if (Player == null)
		{
			Debug.LogError("ShipBeaviour: Player is null");
			this.Deactivate();
			return;
		}

		AdwanceWeapon = Player.AdvanceWeaponNumber;
		iArmor = Player.MyShip.Armor;
		health = Player.MyShip.HealthPoint;

		if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer)
		{
			PhotonView photonView = PhotonView.Get(this);
			if (!photonView.isMine)
				return;
		}

		if (Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.Sunk)
		{
			OnSunkAction();
			return;
		}

		if (Player.MyShip.StateOfShipBehaviour != Ship.ShipBehaviourState.Normal &&
		    Player.MyShip.StateOfShipBehaviour != Ship.ShipBehaviourState.RespawnInvulnerability)
			return;

		//ShipRotation();

		// Управление джойстиком
		Player.SetIsInMovementFree(false);
		if (PlayerInfo.Instance != null)
			if (!PlayerInfo.Instance.HealthBar)
			{
				if (_HealtBar)
				{
					_HealtBar.UpdateHealthBar();
				}
			}

		if (_shipMovement != null)
			Player.IsInMovement = _shipMovement.IsInMovement;
		
		AiPlayerUpdate();

		CheckIsShipMoving();

		if (Player.IsMoving)
		{

			foreach (RotationPart rotPart in _rotationParts)
			{
				rotPart.Rotate(Player.MyShip.Speed);
			}
		}

		if (IsThisSubmarine())
		{
			SubmarineDivingUpdate();
		}

		UpdateEngineSound();

		//Player.IsInMovement = false;
		
	}

	private void OnSunkAction()
	{
		Vector3 curPos = _rigidbody.position;
		curPos += Vector3.down*Ship.SunkSpeed*Time.deltaTime;
		//Player.Position += Vector3.down*Ship.SunkSpeed*Time.deltaTime;
		
		_rigidbody.MovePosition(curPos);
		//transform.position = Player.Position;

		if (IsTweaking)
		{
			IsTweaking = false;
			nearestPointPosition = Vector3.zero;
			tweakingDirection = Vector3.zero;
		}
	}

	private void ShipRotation()
	{
		if (Player.Direction != Vector3.zero)
		{
			if (Player.Mechanics == MechanicsType.Classic)
			{
				Player.RotationTo = Quaternion.LookRotation(Player.Direction);
				Player.RotationFrom =
					Player.Rotation =
					transform.rotation =
					Quaternion.Slerp(Player.RotationFrom,
					                 Player.RotationTo,
					                 Player.MyShip.RotationSpeed*Time.deltaTime*Player.coeffRotationSpeed);
			}
			else
			{
				Player.RotationTo = Quaternion.LookRotation(Player.Direction);

				if (Player is AIPlayer)
				{
					Player.Rotation =
						transform.rotation =
						Quaternion.Slerp(transform.rotation,
						                 Player.RotationTo,
						                 Player.MyShip.RotationSpeed*Time.deltaTime*Player.coeffRotationSpeed);
				}
				else
				{


					Player.RotationFrom =
						Player.Rotation =
						transform.rotation =
						Quaternion.Slerp(Player.RotationFrom,
						                 Player.RotationTo,
						                 Player.MyShip.RotationSpeed*Time.deltaTime*Player.coeffRotationSpeed*0.25f);

				}
			}
		}
	}

	private void CheckIsShipMoving()
	{
		if (_previousPosition.Count >= Player.PreviousCount)
			_previousPosition.RemoveAt(0);

		_previousPosition.Add(Player.Position);

		Player.Forward = transform.forward;

		Player.IsMoving = !IsPositionTheSame();
	}

	private void AiPlayerUpdate()
	{
		//AIUpdate
		if (Player is AIPlayer /*&& !Player.IsRotated*/&& !IsPauseAfterShooting &&
		    (Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.Normal ||
		     Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.RespawnInvulnerability))
		{
			Player.SetIsInMovementFree(false);

			if ((Player as AIPlayer).FSM != null)
			{
				(Player as AIPlayer).FSM.Update();
			}

			//transform.position = Player.Position;
			_rigidbody.MovePosition(Player.Position);
		}

	}

	private Vector3 _divingVector;

	private void SubmarineDivingUpdate()
	{
		Submarine submarine = Player.MyShip as Submarine;

		if (submarine == null)
			return;

		bool isNeedDiving = true;

		_divingVector = Vector3.zero;

		Submarine.DivingDirection divingDirection = Submarine.DivingDirection.Down;

		switch (submarine.StateOfMotion)
		{
			case Submarine.MotionState.Stop:
				if (Player.IsMoving)
				{
					submarine.StateOfMotion = Submarine.MotionState.EmmersionAndMove;
					isNeedDiving = false;
					_submarineDiveStartTime = Time.time;
				}

				break;

			case Submarine.MotionState.EmmersionAndMove:

				divingDirection = Submarine.DivingDirection.Up;

				if (Player.IsMoving &&
					Math.Abs(submarine.Position.y - ((float) Submarine.DivingDirection.Up)*Ship.SubmarineDiveY) <= 0f)
				{
					submarine.StateOfMotion = Submarine.MotionState.Move;
					isNeedDiving = false;
				}

				if (!_shipMovement.IsInMovement)
				{
					submarine.StateOfMotion = Submarine.MotionState.AfterMoveImmersion;
					isNeedDiving = false;
					_submarineDiveStartTime = Time.time;
				}

				break;

			case Submarine.MotionState.Move:

				isNeedDiving = false;
				divingDirection = Submarine.DivingDirection.Up;

				if (!Player.IsMoving || !_shipMovement.IsInMovement)
				{
					submarine.StateOfMotion = Submarine.MotionState.AfterMoveImmersion;
					_submarineDiveStartTime = Time.time;
				}
				break;

			case Submarine.MotionState.AfterMoveImmersion:

				divingDirection = Submarine.DivingDirection.Down;

				if (!Player.IsMoving &&
					Math.Abs(submarine.Position.y - ((float) Submarine.DivingDirection.Down)*Ship.SubmarineDiveY) <= 0f)
				{
					submarine.StateOfMotion = Submarine.MotionState.Stop;
				}

				if (_shipMovement.IsInMovement)
				{
					submarine.StateOfMotion = Submarine.MotionState.EmmersionAndMove;
					isNeedDiving = false;
					_submarineDiveStartTime = Time.time;
				}

				break;
		}

		if (isNeedDiving)
		{

			//Vector3 divingVector = _divingVector;

			//_divingVector = new Vector3(Player.Position.x,
			//							  Mathf.Lerp(Player.Position.y, ((float) divingDirection*Ship.SubmarineDiveY),
			//										 (Time.time - _submarineDiveStartTime)*Ship.SubmarineDiveSpeed),
			//							  Player.Position.z);

			_divingVector = new Vector3(0, (float) divingDirection*Ship.SubmarineDiveY * 3f, 0);
		}

		//_rigidbody.velocity += _divingVector;
		//transform.position = _divingVector;

		//Debug.Log(
		//	string.Format("ShipBehaviour.SubmarineDivingUpdate - OK, _divingVector: {0}, submarine state: {1}, Player.IsInMovement: {2}, PlayerPos: {3}",
		//				  _divingVector, submarine.StateOfMotion, _shipMovement.IsInMovement, Player.Position));
	}

	private void FixedUpdate()
	{
		if (!GameSetObserver.Instance.IsBattleStarted || GameSetObserver.Instance.IsPause || Player == null)
			return;

		if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer)
		{
			PhotonView photonView = PhotonView.Get(this);
			if (!photonView.isMine)
				return;
		}

		CheckIsShipCanBombShoot();

		CheckIsCanShipBasicShoot();
		
		CheckIsNeedPauseAfterShooting();

		StateOfShipBehavoiurUpdate();
	}

	private void CheckIsNeedPauseAfterShooting()
	{
		if (IsPauseAfterShooting)
		{
			if (ShipCooldownCounter < Player.MyShip.BasicWeapon.AfterFireCooldown)
				ShipCooldownCounter += Time.deltaTime;

			if (ShipCooldownCounter >= Player.MyShip.BasicWeapon.AfterFireCooldown)
				IsPauseAfterShooting = false;
		}
	}

	private void CheckIsCanShipBasicShoot()
	{
		if (!IsCanShoot)
		{
			if (FireCooldownCounter < Player.MyShip.BasicWeapon.FireCooldown)
				FireCooldownCounter += Time.deltaTime;

			if (FireCooldownCounter >= Player.MyShip.BasicWeapon.FireCooldown)
				IsCanShoot = true;
		}
	}

	private void CheckIsShipCanBombShoot()
	{
		if (!Player.MyShip.BasicBomb.IsCanShoot)
		{
			if (Player.MyShip.BasicBomb.FireCooldownCount < Player.MyShip.BasicBomb.FireCooldown)
			{
				Player.MyShip.BasicBomb.FireCooldownCount += Time.deltaTime;
			}

			if (Player.MyShip.BasicBomb.FireCooldownCount >= Player.MyShip.BasicBomb.FireCooldown)
			{
				Player.MyShip.BasicBomb.Reload();
			}
		}
	}

	private void StateOfShipBehavoiurUpdate()
	{
		switch (Player.MyShip.StateOfShipBehaviour)
		{
			case Ship.ShipBehaviourState.RespawnInvulnerability:
				ShipInvulnerability();
				if (InvulnerabilityCounter < Ship.InvulnerabilityTime)
				{
					InvulnerabilityCounter += Time.deltaTime;
				}

				if (InvulnerabilityCounter >= Ship.InvulnerabilityTime)
				{
					InvulnerabilityCounter = 0f;
					Player.MyShip.IsInvulnerability = false;
					Player.MyShip.StateOfShipBehaviour = Ship.ShipBehaviourState.Normal;
					SetShipMeshVisible(true);
				}
				break;

			case Ship.ShipBehaviourState.Normal:
				break;

			case Ship.ShipBehaviourState.Destroyed:
				if (_afterHitBeforeSunkDelay < Ship.AfterHitBeforeSunkDelay)
				{
					_afterHitBeforeSunkDelay += Time.deltaTime;
				}

				if (_afterHitBeforeSunkDelay >= Ship.AfterHitBeforeSunkDelay)
				{
					OnShipSunk();
					_afterHitBeforeSunkDelay = 0f;
					blinkTimer = 0f;
				}
				break;

			case Ship.ShipBehaviourState.Sunk:
				if (ShipSunkTimerCount < Ship.BeforeRespawnDelay)
				{
					ShipSunkTimerCount += Time.deltaTime;
				}

				if (ShipSunkTimerCount >= Ship.BeforeRespawnDelay)
				{
					ShipSunkTimerCount = 0f;
					Player.MyShip.IsShipSunk = false;

					FireTimerReset();

					Player.MyShip.OnShipSunk();
				}
				break;

		}
	}

	private const float BlinkingTime = 0.2f;
	private float blinkTimer = 0f;
	private bool isShipVisible = true;

	private void ShipInvulnerability()
	{
		if (blinkTimer < BlinkingTime)
		{
			blinkTimer += Time.deltaTime;
		}

		if (blinkTimer >= BlinkingTime)
		{
			blinkTimer = 0f;
			isShipVisible = !isShipVisible;
			SetShipMeshVisible(isShipVisible);
		}
	}

	private void SetShipMeshVisible(bool isVisible)
	{
		if (_meshRenderer == null)
		{
			Debug.LogError("Can't set ship visible because meshRenderer is null");
			return;
		}

		foreach (Renderer renderer1 in _meshRenderer)
		{
			renderer1.enabled = isVisible;
		}

		isShipVisible = isVisible;
	}


	private void AddHealth()
	{
		Player.MyShip.HealthPoint = Player.MyShip.getBasicHealthPoint();
	}

	private IEnumerator AddShield()
	{
		int time = (int) ConfigBonuses.Bonuses[BonusesType.Shield].time;
		for (int i = 0; i <= time; i++)
		{
			if (i == 0)
			{
				int bonus = (int) ConfigBonuses.Bonuses[BonusesType.Shield].value;
				int _health = Player.MyShip.getBasicHealthPoint() + bonus;
				Player.MyShip.setBasicHealthPoint(_health);
				Player.MyShip.HealthPoint += bonus;
			}
			if (i == time)
			{
				int bonus = (int) ConfigBonuses.Bonuses[BonusesType.Shield].value;
				int _health = Player.MyShip.getBasicHealthPoint();
				Player.MyShip.setBasicHealthPoint(_health);
				Player.MyShip.HealthPoint -= bonus;
				StopCoroutine("AddShield");
			}
			yield return new WaitForSeconds(1);
		}
	}

	private IEnumerator Invisible()
	{
		int time = (int) ConfigBonuses.Bonuses[BonusesType.Invisible].time;

		for (int i = 0; i <= time; i++)
		{
			if (i == 0)
			{
				transform.FindChild("ModelHolder").gameObject.SetActive(false);
			}
			if (i == time)
			{
				transform.FindChild("ModelHolder").gameObject.SetActive(true);
				Debug.Log("TwoSpeed CAncel");
				StopCoroutine("Invisible");
			}
			yield return new WaitForSeconds(1);
		}
	}

	private IEnumerator Destroyer()
	{
		int time = (int) ConfigBonuses.Bonuses[BonusesType.Destroyer].time;

		for (int i = 0; i <= time; i++)
		{
			if (i == 0)
			{
				isDestroeyr = true;
			}
			if (i == time)
			{
				isDestroeyr = false;
				Debug.Log("Destroyer Cancel");
				StopCoroutine("Destroyer");
			}
			yield return new WaitForSeconds(1);
		}
	}


	private IEnumerator TwoShootActivate()
	{
		int time = (int) ConfigBonuses.Bonuses[BonusesType.TwoShoot].time;

		for (int i = 0; i <= time; i++)
		{
			if (i == 0)
			{
				isTwoShoot = true;
			}

			if (i == time)
			{
				isTwoShoot = false;
				Debug.Log("TwoShoot Cancel");
				StopCoroutine("TwoShootActivate");
			}
			yield return new WaitForSeconds(1);
		}
	}

	private IEnumerator TwoSpeed()
	{
		int time = (int) ConfigBonuses.Bonuses[BonusesType.TwoSpeed].time;

		for (int i = 0; i <= time; i++)
		{
			if (i == 0)
				Player.MyShip.MaxSpeed = Player.MyShip.MaxSpeed*2;

			if (i == time)
			{
				Player.MyShip.MaxSpeed = Player.MyShip.MaxSpeed/2;
				Debug.Log("TwoSpeed CAncel");
				StopCoroutine("TwoSpeed");
			}
			yield return new WaitForSeconds(1);
		}
	}

	private IEnumerator Immortal()
	{
		int time = (int) ConfigBonuses.Bonuses[BonusesType.Immortal].time;

		for (int i = 0; i <= time; i++)
		{
			if (i == 0)
			{
				Player.MyShip.isImortal = true;
			}

			if (i == time)
			{
				Player.MyShip.isImortal = false;
				Debug.Log("Immortal CAncel");
				StopCoroutine("Immortal");
			}
			yield return new WaitForSeconds(1);
		}
	}


	private void AddBonus(BonusesType _type)
	{

		Debug.Log(_type);
		switch (_type)
		{
			case BonusesType.Immortal:
				StartCoroutine("Immortal");
				break;
			case BonusesType.Health:
				AddHealth();
				break;
			case BonusesType.Invisible:
				StartCoroutine("Invisible");
				break;
			case BonusesType.Shield:
				StartCoroutine("AddShield");
				break;
			case BonusesType.TwoShoot:
				StartCoroutine("TwoShootActivate");
				break;
			case BonusesType.TwoSpeed:
				StartCoroutine("TwoSpeed");
				break;
			case BonusesType.Destroyer:
				StartCoroutine("Destroyer");
				break;
		}
	}


	private void OnTriggerEnter(Collider other)
	{

		if (other.tag == "Bonus")
		{
			BonusBehavior _bonusType = other.gameObject.GetComponent<BonusBehavior>();
			AddBonus(_bonusType.Type);
			_bonusType.Deactivate();
		}

		if (Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.Destroyed ||
		    Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.RespawnInvulnerability)
			return;

		if (Player.MyShip.IsShipSunk || Player.MyShip.IsInvulnerability)
			return;

		WeaponBehaviour weapon = null;
		if (other.transform.parent != null)
			if (other.transform.parent.parent != null)
				weapon = other.transform.parent.parent.GetComponent<WeaponBehaviour>();

		if (weapon == null)
			return;

		if (weapon.Team == Player.Team)
			return;

		// Чтобы события попадания обрабатывались только на стороне корабля, в кого мы попадаем
		if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer)
		{
			if (MultiplayerManager.MyMultiplayerEntity.player.PlayersFleet.Any(player => player == weapon.weaponShell.Owner))
				return;
		}


		if (weapon.type == WeaponsType.BasicBomb) {
			Debug.Log("EXPLOSIVE Init");
			weapon.ShowExplosionFx (other.transform.position);
		}
		
		else if(weapon.type == WeaponsType.MortalProjectile)
		{
			weapon.ShowExplosionFx (other.transform.position);

		}
		weapon.BlowUp (true);

		ShowHitFx(weapon.transform.position);
		if (weapon.weaponShell.Type == WeaponsType.BasicBomb)
		{
			if (!isTral)
			{
				DealShipDamage(weapon.weaponShell.Owner.Id, weapon.weaponShell.Damage);
			}

		}
		else if(weapon.type != WeaponsType.MortalProjectile)
		{
			Debug.Log(weapon.weaponShell.Type);

			//DealShipDamage (weapon.weaponShell.Owner.Id, weapon.weaponShell.Damage);
			//Костыль
			
			DealShipDamage (-1, weapon.weaponShell.Damage);

		}
	}

	public void GunAddDamage(int damage)
	{

		ShowHitFx(transform.position);
		ShowHitFx(transform.position);
		DealShipDamage(0, damage);
	}

	private void OnWeaponCollisin(Collision other)
	{

		if (Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.Destroyed ||
		    Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.RespawnInvulnerability)
			return;

		if (Player.MyShip.IsShipSunk || Player.MyShip.IsInvulnerability)
			return;

		WeaponBehaviour weapon = null;
		if (other.gameObject.transform.parent != null)
			if (other.gameObject.transform.parent.parent != null)
				weapon = other.gameObject.transform.parent.parent.GetComponent<WeaponBehaviour>();

		if (weapon == null)
		{

			IsNeedUseTransorm = true;

			ShipBehaviour shipBehaviour = other.gameObject.GetComponent<ShipBehaviour>();

			if (shipBehaviour != null && shipBehaviour.shipType == ShipType.Submarine)
			{
				if (shipBehaviour.gameObject.transform.position.y <= -Ship.SubmarineDiveY*0.5f)
				{
					IsNeedUseTransorm = false;
					return;
				}
			}

			if (shipType == ShipType.BigShip)
			{
				BigShipEnterCollision(other);
			}

			//Player.Position = _rigidbody.position;
			//Player.Position = transform.position;

			PlayScratchSound(other);

			return;
		}

		if (weapon.Team == Player.Team)
			return;

		// Чтобы события попадания обрабатывались только на стороне корабля, в кого мы попадаем
		if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer)
		{
			if (MultiplayerManager.MyMultiplayerEntity.player.PlayersFleet.Any(player => player == weapon.weaponShell.Owner))
				return;
		}


		if (weapon.type == WeaponsType.BasicBomb)
		{
			weapon.ShowExplosionFx(other.transform.position);
		}
		weapon.BlowUp (true);

			ShowHitFx (weapon.transform.position);
		
		if (weapon.weaponShell.Type == WeaponsType.BasicBomb) {
			if(!isTral)
			{
				DealShipDamage(weapon.weaponShell.Owner.Id, weapon.weaponShell.Damage);
			}

		}
		else
		{
			DealShipDamage(weapon.weaponShell.Owner.Id, weapon.weaponShell.Damage);
		}


	}


	private void OnCollisionEnter(Collision collision)
	{
		IsNeedUseTransorm = true;

		if (shipType == ShipType.BigShip)
		{
			BigShipEnterCollision(collision);
		}

		if (Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.RespawnInvulnerability &&
			(collision.collider.CompareTag("Mine") || collision.collider.CompareTag("Shell") ||
			 collision.collider.CompareTag("Untouchable")))
			return;

		if (collision.collider.CompareTag("Mine"))
			return;

		ShipBehaviour shipBehaviour = collision.gameObject.GetComponent<ShipBehaviour>();

		if (shipBehaviour != null && shipBehaviour.shipType == ShipType.Submarine)
		{
			if (shipBehaviour.gameObject.transform.position.y <= -Ship.SubmarineDiveY*0.5f)
			{
				IsNeedUseTransorm = false;
				return;
			}
		}

		if (collision.collider.name == "Collider01")
			return;

		//Spring(CalculateSpringVector(collision));

		PlayScratchSound(collision);

		//Debug.Log(string.Format("ShipBeahaviour.OnCollisionEnter - collision name: {0} ", collision.collider.name));
	}

	private void OnCollisionStay(Collision collision)
	{
		if (shipType == ShipType.BigShip)
		{
			BigShipStayInCollision();
		}

		if (Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.RespawnInvulnerability &&
		    (collision.collider.CompareTag("Mine") || collision.collider.CompareTag("Shell") ||
		     collision.collider.CompareTag("Untouchable")))
			return;

		if (collision.collider.CompareTag("Mine"))
			return;

		if (collision.collider.CompareTag("UiRadar"))
		{
			return;
		}

		if (collision.collider.name == "Collider01")
			return;

		if (Player is HumanPlayer)
			HUDJoystick.Instance.CancelAutoMovement();

		//Debug.Log("ShipBehaviour.OnCollisionStay - collision name:" + other.transform.tag);

		if (collision.transform.GetComponentInChildren<FlagsBehaviour>() != null ||
		    collision.transform.GetComponentInChildren<WeaponBehaviour>() != null)
		{
			return;
		}

		Player.IsPlayerIsInCollision = true;

		IsNeedUseTransorm = true;
		IsTweaking = false;

		//Debug.Log("TestSpring.OnCollisionStay :" + other.relativeVelocity);

		//Spring(CalculateSpringVector(collision));

		//Player.Position = transform.position;

		//Debug.Log(string.Format("ShipBeahaviour.OnCollisionStay - collision name: {0} ", collision.collider.name));
	}

	private void OnCollisionExit(Collision collision)
	{
		if (shipType == ShipType.BigShip)
		{
			BigShipExitCollision(collision);
		}

		if (Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.RespawnInvulnerability &&
		    (collision.collider.CompareTag("Mine") || collision.collider.CompareTag("Shell") ||
		     collision.collider.CompareTag("Untouchable")))
			return;

		if (collision.collider.CompareTag("Mine"))
			return;

		if (collision.collider.name == "Collider01")
			return;

		Player.IsPlayerIsInCollision = false;

		//Spring(CalculateSpringVector(collision));

		IsNeedUseTransorm = false;

		//Debug.Log(string.Format("ShipBeahaviour.OnCollisionExit - collision name: {0} ", collision.collider.name));
	}

	private const float _timeCoeff = 9f;
	private const float DemperCoeff = 0.65f;

	private void Spring(Vector3 springVector)
	{
		if (springVector.magnitude > 0.8)
		{
			//Vector3 pos = Player.Position;
			//Vector3 newPos = pos - springVector*DemperCoeff;

			//Player.Position = Vector3.Lerp(pos, newPos, Time.deltaTime*_timeCoeff);

			if (HUDJoystick.Instance != null)
				HUDJoystick.Instance.CancelAutoMovement();
		}

	}

	private const float MinSpringVector = -1.25f;
	private const float MaxSpringVector = 1.25f;

	private Vector3 CalculateSpringVector(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Destructable") && IsThisBigShip())
		{
			Destructable destructable = null;

			if (collision.gameObject.transform.parent != null && collision.gameObject.transform.parent.parent != null)
			{
				destructable = collision.gameObject.transform.parent.parent.GetComponent<Destructable>();
				if (destructable != null && destructable.HitPoint < 2)
				{
					return Vector3.zero;
				}
			}

			
		}

		Vector3 springVector = Vector3.zero;

		foreach (ContactPoint contact in collision.contacts)
		{
			Vector3 pos = contact.point - Player.Position;
			springVector += pos;
		}

		springVector = new Vector3(Mathf.Clamp(springVector.x, MinSpringVector, MaxSpringVector), 0,
		                           Mathf.Clamp(springVector.z, MinSpringVector, MaxSpringVector));
		

		return springVector;
	}

	private bool IsThisBigShip()
	{
		return Player.MyShip.Type == ShipType.BigAtlant || Player.MyShip.Type == ShipType.BigMetal ||
		       Player.MyShip.Type == ShipType.BigDark || Player.MyShip.Type == ShipType.BigShip;
	}

	private bool IsThisSubmarine()
	{
		return Player.MyShip.Type == ShipType.Submarine || Player.MyShip.Type == ShipType.MiddleDark;
	}

	private void OnTriggerStay(Collider other)
	{
		//if (Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.RespawnInvulnerability &&
		//	(other.collider.CompareTag("Mine") || other.collider.CompareTag("Shell") ||
		//	 other.collider.CompareTag("Untouchable")))
		//	return;

		//if (other.transform.parent != null && other.transform.parent.parent != null)
		//{
		//	FlagSpotBehaviour flagSpotBehaviour =
		//		other.transform.parent.parent.transform.GetComponentInChildren<FlagSpotBehaviour>();
		//	if (flagSpotBehaviour != null)
		//	{
		//		if (flagSpotBehaviour.Color != color && flagSpotBehaviour.Base.IsFlagOnSpot)
		//		{
		//			if (GameSetObserver.Instance.CurrentBattle.Mode == GameMode.CaptureTheFlag)
		//			{
		//				flagSpotBehaviour.TakeFlag(this);
		//				Player.Statistic_flag++;
		//			}
		//		}
		//	}
		//}
	}

	private void OnDestroy()
	{
		if (Player is AIPlayer)
		{
			PathFindingHelper.PathReturn -= (Player as AIPlayer).PathSet;
		}
	}

	#region Big ship smash ability

	private class BigShipCollision
	{
		public Destructable Destructable;
		public float TimeInCollision = 0f;
		public float MaxTimeInCollision = 0.1f;


		public bool IsDestroyed;

		public BigShipCollision(Destructable destructable)
		{
			Destructable = destructable;
		}

		public BigShipCollision(Destructable destructable, float maxTimeIncollision)
		{
			Destructable = destructable;
			MaxTimeInCollision = maxTimeIncollision;
		}

	}

	private List<BigShipCollision> _bigShipCollisions;

	private void BigShipEnterCollision(Collision other)
	{
		if (_bigShipCollisions == null)
		{
			_bigShipCollisions = new List<BigShipCollision>();
		}

		if (other.gameObject.transform.parent == null)
		{
			return;
		}

		if (other.gameObject.transform.parent.parent == null)
		{
			return;
		}

		Destructable destructable =
			other.gameObject.transform.parent.parent.GetComponent<Destructable>();

		if (destructable != null)
		{
			BigShipCollision bigShipCollision = new BigShipCollision(destructable);

			_bigShipCollisions.Add(bigShipCollision);
		}


	}

	private void BigShipStayInCollision()
	{
		if (_bigShipCollisions == null)
		{
			_bigShipCollisions = new List<BigShipCollision>();
			return;
		}

		for (int i = _bigShipCollisions.Count - 1; i >= 0; i--)
		{
			BigShipCollision bigShipCollision = _bigShipCollisions[i];

			// Increase time in collision
			bigShipCollision.TimeInCollision += Time.deltaTime;

			// Check if time is more than max
			if (bigShipCollision.TimeInCollision >= bigShipCollision.MaxTimeInCollision)
			{

				if (bigShipCollision.IsDestroyed)
				{
					_bigShipCollisions.RemoveAt(i);
					return;
				}

				bigShipCollision.IsDestroyed = true;

				if (bigShipCollision == null)
				{
					_bigShipCollisions.RemoveAt(i);
					return;
				}

				if (bigShipCollision.Destructable == null)
				{
					_bigShipCollisions.RemoveAt(i);
					return;
				}


				if (bigShipCollision.Destructable.HitPoint < 2)
				{
					_bigShipCollisions.RemoveAt(i);
					bigShipCollision.Destructable.Hit(1);
					return;
				}
			}
		}

	}

	private void BigShipExitCollision(Collision other)
	{
		if (_bigShipCollisions == null)
		{
			_bigShipCollisions = new List<BigShipCollision>();
			return;
		}

		if (other.gameObject.transform.parent == null)
		{
			return;
		}

		if (other.gameObject.transform.parent.parent == null)
		{
			return;
		}

		Destructable destructable =
			other.gameObject.transform.parent.parent.GetComponent<Destructable>();

		for (int i = _bigShipCollisions.Count - 1; i >= 0; i--)
		{
			BigShipCollision bigShipCollision = _bigShipCollisions[i];

			if (bigShipCollision.Destructable == destructable)
			{
				bigShipCollision.IsDestroyed = true;
				_bigShipCollisions.RemoveAt(i);
			}

		}
	}

	#endregion

	#endregion

	#region Events

	private void OnDamageTaken()
	{
		ShowAfterHitEffects();
	}

	public void DealShipDamage(int shellOwnerId , int damage, bool fromServer = false)
	{
		Debug.Log("DAmage");
		Player.MyShip.SetShellOwnerId(shellOwnerId);


		Player.MyShip.TakeDamage(damage);

		if (fromServer)
			return;

		if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer)
		{
			MultiplayerManager.Instance.NeedShipDealDamage(Player, damage, shellOwnerId);
		}

	}

	private void OnCriticalDamageTaken()
	{
		Debug.Log("CriticalDAmage");
		if (Player.MyShip.HealthPoint <= 0)
		{
			if (Trail != null)
			{
				Trail.time = 1f;
			}

			ShowShipExplosion(transform.position);

			OnShipDestroyed();

			Player player = GameSetObserver.Instance.GetPlayer(Player.MyShip.shellOwnerplayerID);
			//Ошибка вылазит из за костыля на 978 строке
			player.MyShip.OnShipDestroyed();

			Player.MyShip.SetShellOwnerId(-1);
		}
		else
		{
			Player.MyShip.HealthPoint -= 5;
			if (Player.MyShip.HealthPoint <= 0)
			{
				if (Trail != null)
				{
					Trail.time = 1f;
				}

				ShowShipExplosion(transform.position);

				OnShipDestroyed();

				Player player = GameSetObserver.Instance.GetPlayer(Player.MyShip.shellOwnerplayerID);

				player.MyShip.OnShipDestroyed();

				Player.MyShip.SetShellOwnerId(-1);
			}

		}
	}

	private void OnShipDestroyed()
	{
		Player.MyShip.StateOfShipBehaviour = Ship.ShipBehaviourState.Destroyed;
		_afterHitBeforeSunkDelay = 0;
		Player.Statistic_death++;

		if (GameController.Instance != null)
			if (GameController.Instance.AdmiralTarget == Player.MyShip.Type.ToString())
			{
				GameController.Instance.AdmiralTargetCheck();
			}

		if (BattleController.Instance.ActiveBattle.Mode == GameMode.Deathmatch)
		{
			BattleController.Instance.OnShipDestroyed(this.color);
		}
	}

	private void OnShipSunk()
	{
		RemoveAfterHitEffects();

		Player.MyShip.StateOfShipBehaviour = Ship.ShipBehaviourState.Sunk;

		ShipSunkTimerCount = 0f;

		SoundController.PlayShipSunk(gameObject);

		OnShipSunkAction();
	}

	public void OnShipSunkAction(bool fromServer = false)
	{
		BattleController.Instance.UpdateBombCount(Player);

		if (!Player.MyShip.IsFlagTaken)
			return;

		Player.MyShip.OnFlagDropped(enemyFlag.Color);

		if (enemyFlag == null)
			return;

		enemyFlag.SetParent(null);
		enemyFlag = null;

		if (fromServer)
			return;
	}

	public void OnRespawn(bool invulnerability = false)
	{
		//transform.position = Player.Position;

		Vector3 pos = BattleController.Instance.GetRandomSpawnPointVector(Player.Team);
		_rigidbody.position = (pos);

		if (Player == GameSetObserver.Instance.Human)
		{
			
			if (CameraFollowsShip.Instance != null)
				CameraFollowsShip.Instance.Target = this;

			if (transform.FindChild("Blue") != null)
			{
				transform.FindChild("Blue").gameObject.SetActive(false);
			}
		}

		if (invulnerability)
		{
			Player.MyShip.StateOfShipBehaviour = Ship.ShipBehaviourState.RespawnInvulnerability;
			InvulnerabilityCounter = 0f;
		}

		BattleController.Instance.UpdateBombCount(Player);

		if (Trail != null)
		{
			Trail.time = 3f;
		}
	}


	private void OnFireAdwance(bool fromServer)
	{
		Debug.Log("Shoot Adwance Weapon");
		if (Player.AdvanceWeaponNumber > 0)
		{

			// Проверяем возможность стрелять, если нет задержки и мы не подбиты и не тоним
			if (!IsCanShoot || Player.MyShip.StateOfShipBehaviour != Ship.ShipBehaviourState.Normal)
				return;

			// Сбрасываем таймер перезагрузки основного оружия
			if (GameSetObserver.Instance.CurrentGameType != GameType.Multiplayer || !fromServer)
				FireTimerReset();

			Player.Statistic_shootAdvance++;
			WeaponBehaviour weapon = ResourceBehaviourController.Instance.GetWeaponsFromPool(Player.AdvanceWeapon,
			                                                                                 Player);

			if (weapon == null)
			{
				Debug.LogError("ShipBeaviour: OnFireBasic - can't get weapons behavoiur");
				return;
			}

			if (Player.AdvanceWeapon != WeaponsType.DeepBomb)
			{
				// Задаем местоположение старта стрельбы и направление
				weapon.SetBasicData(FirstShootPosition.position, transform, Player, Vector3.left, bulletSpeed);
				// Отображаем эффекты выстрела основного оружия
				ShowFireBasicFx(FirstShootPosition.position);

			}
			else
			{
				// Задаем местоположение старта стрельбы и направление
				weapon.SetBasicData(BombShootPosition.position, transform, Player, Vector3.left, bulletSpeed);
				// Отображаем эффекты выстрела основного оружия
				EffectsBehaviour splashBomb =
					ResourceBehaviourController.Instance.GetEffectsFromPool(EffectsBehaviour.EffectsType.SplashForBomb);

				if (splashBomb != null)
				{
					splashBomb.SetBasicData(weapon.transform.position);

					SoundController.PlayRandomBombSplashSound(splashBomb.gameObject);
				}
			}

			//если стрeльба прошла успешно отнимаем один патрон
			Player.AdvanceWeaponNumber--;

			//отнимаем патрон из сейва корабля
			if (Player == GameSetObserver.Instance.Human ||
			    (Player != GameSetObserver.Instance.Human && Player.Team == TeamColor.BlueTeam))
			{
				PlayerInfo.Instance.ShipSave[id].weaponCount--;
			}


			HUDButtons.Instance.SetSpecialCountLabel(Player.AdvanceWeaponNumber);
			// Посылаем запрос на отображение снаряда другим клиентам игры
			if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer && !fromServer)
			{
				MultiplayerManager.Instance.NeedPlayerFireBasic(Player.Id);
			}
		}
		else
		{
			Debug.Log("Not Ammo");
		}
	}


	private IEnumerator TwoShoot()
	{
		int time = 1;
		Debug.Log("OnDublFire CorutineStart");
		for (int i = 0; i <= 1; i++)
		{

			if (i == time)
			{
				Debug.Log("OnDublFire Corutine");
				WeaponBehaviour weapon = ResourceBehaviourController.Instance.GetWeaponsFromPool(Player.MyShip.BasicWeapon.Type,
				                                                                                 Player);

				Player.Statistic_shoot++;
				// Задаем местоположение старта стрельбы и направление
				weapon.SetBasicData(FirstShootPosition.position, transform, Player, Player.MyShip.BasicWeapon, Vector3.left,
				                    bulletSpeed);

				// Отображаем эффекты выстрела основного оружия
				ShowFireBasicFx(FirstShootPosition.position);

				// Если количество пушек у нас больше 1, то инициализируем снаряд и эффекты для второй пушки
				if (Player.MyShip.BasicWeaponShellCount > 1)
				{
					weapon = ResourceBehaviourController.Instance.GetWeaponsFromPool(Player.MyShip.BasicWeapon.Type, Player);
					weapon.SetBasicData(SecondShootPosition.position, transform, Player, Player.MyShip.BasicWeapon, Vector3.left,
					                    bulletSpeed);

					ShowFireBasicFx(SecondShootPosition.position);
				}

				Debug.Log("Two Fire");

				//TODO// Посылаем запрос на отображение снаряда другим клиентам игры
				StopCoroutine("TwoShoot");
			}
			yield return new WaitForSeconds(0.3f);
		}

	}


	private void OnFireBasic(bool fromServer)
	{
		Debug.Log("Shoot Basic Weapon");
		// Проверяем возможность стрелять, если нет задержки и мы не подбиты и не тоним
		if (!IsCanShoot || Player.MyShip.StateOfShipBehaviour != Ship.ShipBehaviourState.Normal)
			return;

		// Сбрасываем таймер перезагрузки основного оружия
		if (GameSetObserver.Instance.CurrentGameType != GameType.Multiplayer || !fromServer)
			FireTimerReset();

		WeaponBehaviour weapon = ResourceBehaviourController.Instance.GetWeaponsFromPool(Player.MyShip.BasicWeapon.Type,
		                                                                                 Player);
		if (weapon == null)
		{
			//		Debug.LogError("ShipBeaviour: OnFireBasic - can't get weapons behavoiur");
			return;
		}

		Player.Statistic_shoot++;
		// Задаем местоположение старта стрельбы и направление
		weapon.SetBasicData(FirstShootPosition.position, transform, Player, Player.MyShip.BasicWeapon, Vector3.left,
		                    bulletSpeed);

		// Отображаем эффекты выстрела основного оружия
		ShowFireBasicFx(FirstShootPosition.position);

		// Если количество пушек у нас больше 1, то инициализируем снаряд и эффекты для второй пушки
		if (Player.MyShip.BasicWeaponShellCount > 1)
		{
			weapon = ResourceBehaviourController.Instance.GetWeaponsFromPool(Player.MyShip.BasicWeapon.Type, Player);
			weapon.SetBasicData(SecondShootPosition.position, transform, Player, Player.MyShip.BasicWeapon, Vector3.left,
			                    bulletSpeed);

			ShowFireBasicFx(SecondShootPosition.position);
		}

		if (isTwoShoot)
		{
			Debug.Log("OnDublFire");
			StartCoroutine("TwoShoot");
		}
		// Посылаем запрос на отображение снаряда другим клиентам игры
		if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer && !fromServer)
		{
			MultiplayerManager.Instance.NeedPlayerFireBasic(Player.Id);
		}

	}

	private void OnFireBomb(bool fromServer)
	{

		if (Player.MyShip.StateOfShipBehaviour != Ship.ShipBehaviourState.Normal || !Player.MyShip.BasicBomb.IsCanShoot)
			return;

//		if (fromServer)
		//	Debug.Log(string.Format("Player {0} trying fired bomb, bomb count: {1}", Player.Id, Player.MyShip.BombCount));

		WeaponBehaviour weapon = ResourceBehaviourController.Instance.GetWeaponsFromPool(Player.MyShip.BasicBomb.Type,
		                                                                                 Player);

		if (weapon == null)
		{
			//	Debug.LogError("ShipBeaviour: OnFireBasic - can't get weapons behavoiur");
			return;
		}
		Player.Statistic_mines++;

		Debug.Log(Player.MyShip.BasicBomb.Type);
		weapon.SetBasicData(BombShootPosition.position, transform, Player, Player.MyShip.BasicBomb, Vector3.left, bulletSpeed);

		EffectsBehaviour splashBomb =
			ResourceBehaviourController.Instance.GetEffectsFromPool(EffectsBehaviour.EffectsType.SplashForBomb);

		if (splashBomb != null)
		{
			splashBomb.SetBasicData(BombShootPosition.position);

			SoundController.PlayRandomBombSplashSound(splashBomb.gameObject);
		}


		BattleController.Instance.UpdateBombCount(Player);

		if (fromServer)
			//	Debug.Log(string.Format("Player {0} fired bomb, bomb count: {1}", Player.Id, Player.MyShip.BombCount));

			if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer && !fromServer)
			{
				MultiplayerManager.Instance.NeedPlayerFireBomb(Player.Id);
			}
	}

	public bool IsCanTakeFlag
	{
		get { return Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.Normal; }
	}

	private void FireTimerReset()
	{
		IsCanShoot = false;

		if (Player.Mechanics == MechanicsType.Classic)
			IsPauseAfterShooting = true;

		FireCooldownCounter = 0f;
		ShipCooldownCounter = 0f;
	}

	public void SetEventHandler()
	{
		//	Debug.Log(string.Format("Player:{0} - set event handler", Player.Id));

		Player.MyShip.OnFireBasicEvent += OnFireBasic;
		Player.MyShip.OnFireBombEvent += OnFireBomb;
		Player.MyShip.OnFireAdvanceEvent += OnFireAdwance;

		Player.MyShip.OnRespawnEvent += OnRespawn;

		Player.MyShip.OnDamageTakenEvent += OnDamageTaken;
		Player.MyShip.OnCriticalDamageTakenEvent += OnCriticalDamageTaken;

		Player.MyShip.OnFlagTakenEvent += BattleController.Instance.OnFlagTaken;
		Player.MyShip.OnFlagDeliveredEvent += BattleController.Instance.OnFlagDelivered;
		Player.MyShip.OnFlagDroppedEvent += BattleController.Instance.OnFlagDropped;
		Player.MyShip.OnFlagReturnedEvent += BattleController.Instance.OnFlagReturned;

		//if (Player is HumanPlayer || Player is MultiplayerPlayer)
		//	//transform.position = Player.Position;
		//	_rigidbody.MovePosition(Player.Position);
	}

	public void RemoveEventHandler()
	{
		Player.MyShip.OnFireBasicEvent -= OnFireBasic;
		Player.MyShip.OnFireBombEvent -= OnFireBomb;
		Player.MyShip.OnFireAdvanceEvent -= OnFireAdwance;

		Player.MyShip.OnRespawnEvent -= OnRespawn;

		Player.MyShip.OnDamageTakenEvent -= OnDamageTaken;
		Player.MyShip.OnCriticalDamageTakenEvent -= OnCriticalDamageTaken;

		Player.MyShip.OnFlagTakenEvent -= BattleController.Instance.OnFlagTaken;
		Player.MyShip.OnFlagDeliveredEvent -= BattleController.Instance.OnFlagDelivered;
		Player.MyShip.OnFlagDroppedEvent -= BattleController.Instance.OnFlagDropped;
		Player.MyShip.OnFlagReturnedEvent -= BattleController.Instance.OnFlagReturned;

		HUDJoystick.Instance.OnDirectionChangeEvent -= OnDirectionChange;
		HUDJoystick.Instance.On90DegreeRotationEvent -= On90DegreeRotation;
	}

	public void Reset()
	{
		RemoveEventHandler();
	}

	private bool IsPositionTheSame()
	{
		foreach (Vector3 pos in _previousPosition)
		{
			if (Math.Abs(pos.x - Player.Position.x) > Player.eps*0.5f || Math.Abs(pos.z - Player.Position.z) > Player.eps*0.5f)
				return false;
		}

		return true;
	}

	private void On90DegreeRotation()
	{
		//Debug.Log("On90DegreeRotation");
	}

	private void OnDirectionChange()
	{
		Map currentMap = BattleController.Instance.ActiveBattle.Map;

		nearestPointPosition = Map.GetWorldPosition(currentMap,
		                                            Map.GetNearestPoint(currentMap, Player.Position, /*Player.Direction*/
		                                                                Vector3.zero));

		tweakingDirection = (nearestPointPosition - Player.Position).normalized;
	}


	#region Show effect FX

	private void ShowFireBasicFx(Vector3 pos)
	{
		if (Player.MyShip.Type != ShipType.Submarine)
		{
			EffectsBehaviour.EffectsType effectsType = Player.Team == TeamColor.BlueTeam
				                                           ? EffectsBehaviour.EffectsType.SparksBlue
				                                           : EffectsBehaviour.EffectsType.SparksRed;

			EffectsBehaviour fireFx =
				ResourceBehaviourController.Instance.GetEffectsFromPool(effectsType);

			if (fireFx != null)
			{
				fireFx.transform.position = pos;
				//	fireFx.transform.parent = transform;
				//	fireFx.transform.localPosition = Vector3.zero;
				//	fireFx.transform.localPosition = pos;

				fireFx.SetBasicData(fireFx.transform.position);
				SoundController.PlaySalvoShoot(fireFx.gameObject, shipType, Player is HumanPlayer);
			}


		}
	}

	private void ShowHitFx(Vector3 pos)
	{


		EffectsBehaviour.EffectsType effectsType = Player.Team == TeamColor.OrangeTeam
			                                           ? EffectsBehaviour.EffectsType.SparksBlue
			                                           : EffectsBehaviour.EffectsType.SparksRed;



		EffectsBehaviour hitFx =
			ResourceBehaviourController.Instance.GetEffectsFromPool(effectsType);

		if (hitFx != null)
		{
			hitFx.SetBasicData(pos);
		}
	}

	private void ShowShipExplosion(Vector3 pos)
	{
		EffectsBehaviour shipExplosion =
			ResourceBehaviourController.Instance.GetEffectsFromPool(EffectsBehaviour.EffectsType.ShipExplosion);

		if (shipExplosion != null)
		{
			shipExplosion.SetBasicData(pos);

			SoundController.PlayShipExplosion(shipExplosion.gameObject);
		}
	}

	private void ShowAfterHitEffects()
	{
		if (SmokeHitEffectsTransform == null)
			return;

		EffectsBehaviour.EffectsType effectsType = EffectsBehaviour.EffectsType.Damage;

		if (_smokeHitEffects == null)
			_smokeHitEffects = ResourceBehaviourController.Instance.GetEffectsFromPool(effectsType);

		if (_smokeHitEffects != null)
			_smokeHitEffects.SetBasicData(SmokeHitEffectsTransform);
	}

	public void RemoveAfterHitEffects()
	{
		if (_smokeHitEffects == null)
			return;

		_smokeHitEffects.Remove();
		_smokeHitEffects = null;
	}

	#endregion

	#endregion

	#region Ovveride functions

	public override bool EqualsTo(PoolItem item)
	{
		if (!(item is ShipBehaviour))
			return false;

		ShipBehaviour ship = item as ShipBehaviour;

		return ship.shipType == shipType && ship.color == color && ship.environmentType == environmentType;
	}

	public override void Activate()
	{
		base.Activate();
		gameObject.SetActive(true);
	}

	public override void Deactivate()
	{
		base.Deactivate();
		gameObject.SetActive(false);
	}

	#endregion

	#region Sound

	private bool _isEngineSoundPlaying;

	private void UpdateEngineSound()
	{
		bool isHuman = Player is HumanPlayer;

		if (Player.IsMoving)
		{
			if (_isEngineSoundPlaying == false)
			{


				SoundController.PlayShipEngine(gameObject, isHuman, shipType);
				_isEngineSoundPlaying = true;
			}
		}
		else
		{
			if (_isEngineSoundPlaying)
			{
				SoundController.StopPlayShipEngine(gameObject, isHuman);
				_isEngineSoundPlaying = false;
			}
		}
	}

	private void PlayScratchSound(Collision other)
	{
		bool isHuman = Player is HumanPlayer;

		if (isHuman)
		{
			SoundController.PlayRandomMetalScratchSound(other.gameObject);
		}
	}

	#endregion

}