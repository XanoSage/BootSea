using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Maps;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Base;
using Aratog.NavyFight.Models.Unity3D.Flags;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Weapons;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ResourceBehaviourController : MonoBehaviour
{
	
	#region Constants
	
	private string ShipPath = "Prefabs/Ships/{0}";
	private string WeaponPath = "Prefabs/Weapons/Basic";
	
	private string AdwanceWeapon = "Prefabs/Weapons/Adwance";
	
	private string FlagsPath = "Prefabs/Flags/{0}";
	private string FlagSpotsPath = "Prefabs/FlagSpots/{0}";
	private string SpawnPointPath = "Prefabs/SpawnPoints/{0}";
	
	private string EffectsPath = "Prefabs/Effects/";
	
	private string LevelPath = "{0}/{1}"; //0 - Environment, 1 - level name
	
	private string AdwanceRocketName = "Rocket";
	
	private string BasicProjectilleName = "Bullet";
	private string BasicTorpedoName = "Torpeda";
	private string BasicBombName = "Mine";
	
	//Ships sprite name by it color
	
	public const string BigShipBlueIcon = "Battleship_ico_b";
	public const string SubmarineBlueIcon = "Middle_ico";
	public const string BoatBlueIcon = "Destroyer_ico_b";
	
	public const string BigShipRedIcon = "Big_ico";
	public const string SubmarineRedIcon = "Submarine_ico_b";
	public const string BoatRedIcon = "Small_ico";
	
	public const string MainAtlasName = "mainGUI";
	
	
	#endregion
	
	#region Variables
	
	
	public static ResourceBehaviourController Instance { get; private set; }
	
	/// <summary>
	/// Массив префабов готовых корабликов, загружаемых по необходимости
	/// </summary>
	[HideInInspector] private Object[] ShipSystemPrefabs;
	
	/// <summary>
	/// Массив префабов готового оружия, загружаемых по необходимости
	/// </summary>
	
	[HideInInspector] private Object[] WeaponsPrefabs;
	
	/// <summary>
	/// Массив префабов готовых флагов, загружаемых по необходимости
	/// </summary>
	/// 
	[HideInInspector] private Object[] FlagsPrefabs;
	
	/// <summary>
	/// Массив префабов флагшотов, загружаемых по необходимости
	/// </summary>
	[HideInInspector] private Object[] FlagSpotPrefabs;
	
	
	private Object[] SpawnPointPrefabs;
	
	private Object[] EffectPrefabs;
	
	#endregion
	
	
	public static string GetShipIcon(TeamColor team, ShipType shipType)
	{

		string shipIconString = BigShipBlueIcon;
		
		switch (shipType)
		{
		case ShipType.BigShip:
			shipIconString = team == TeamColor.BlueTeam ? BigShipBlueIcon : BigShipRedIcon;
			break;
		case ShipType.Boat:
			shipIconString = team == TeamColor.BlueTeam ? BoatBlueIcon : BoatRedIcon;
			break;
		case ShipType.Submarine:
			shipIconString = team == TeamColor.BlueTeam ? SubmarineBlueIcon : SubmarineRedIcon;
			break;
		}
		
		return shipIconString;
	}
	
	
	#region MonoBehaviour events
	
	private void Awake()
	{
		Instance = this;
	}
	
	/// <summary>
	/// Задаём порядок вызова событий при старте игры, чтобы всё инициализировалось в нужном поядке без ошибок
	/// </summary>
	private void Start()
	{
		if (GameController.Instance != null)
		{
			GameController.Instance.OnStartBattle += UIController.Instance.OnBattleStart;
			GameController.Instance.OnStartBattle += OnStartBattle;
			GameController.Instance.OnStartBattle += BattleController.Instance.OnBattleStart;
			GameController.Instance.OnStartBattle += UIManager.OnBattleStart;
			
			GameController.Instance.OnEndBattle += OnEndBattle;
			GameController.Instance.OnEndBattle += UIController.Instance.OnBattleEnd;
		}
		else if (AITestController.Instance != null)
		{
			AITestController.Instance.OnStartBattle += OnStartBattle;
			AITestController.Instance.OnStartBattle += BattleController.Instance.OnBattleStart;
			
			AITestController.Instance.OnEndBattle += OnEndBattle;
		}
	}
	
	// Update is called once per frame
	private void Update()
	{
		
	}
	
	#endregion
	
	#region events
	
	/// <summary>
	/// Загружаем необходимые ресурсы в зависимости от используемого окружения
	/// </summary>
	/// <param name="type"></param>
	private void LoadResources(EnvironmentType type)
	{
	//	
		string loadingPath = string.Format(ShipPath, type.ToString());
		ShipSystemPrefabs = Resources.LoadAll(loadingPath);
		Debug.Log(string.Format("Was loaded {0} ships from path:{1}", ShipSystemPrefabs.Length, loadingPath));
		
		loadingPath = WeaponPath;
		WeaponsPrefabs = Resources.LoadAll(WeaponPath);
		Debug.Log(string.Format("Was loaded {0} weapons from path:{1}", WeaponsPrefabs.Length, loadingPath));
		
		loadingPath = string.Format(FlagsPath, type);
		FlagsPrefabs = Resources.LoadAll(loadingPath);
		Debug.Log(string.Format("Was loaded {0} flags from path:{1}", FlagsPrefabs.Length, loadingPath));
		
		loadingPath = string.Format(FlagSpotsPath, type);
		FlagSpotPrefabs = Resources.LoadAll(loadingPath);
		Debug.Log(string.Format("Was loaded {0} flagSpot from path:{1}", FlagSpotPrefabs.Length, loadingPath));
		
		loadingPath = string.Format(SpawnPointPath, type);
		SpawnPointPrefabs = Resources.LoadAll(loadingPath);
		Debug.Log(string.Format("Was loaded {0} spawn points from path:{1}", SpawnPointPrefabs.Length, loadingPath));
		
		loadingPath = string.Format(EffectsPath);
		EffectPrefabs = Resources.LoadAll(loadingPath);
		Debug.Log(string.Format("Was loaded {0} effects from path:{1}", EffectPrefabs.Length, loadingPath));
	}
	
	
	private void OnStartBattle()
	{
		Debug.Log("startBattle");
		
		if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer)
			Pool.IsMultiplayer = true;
		else
			Pool.IsMultiplayer = false;

		string levelName = GameSetObserver.Instance.CurrentBattle.Map.Name;

		//if (AITestController.Instance != null)
		{
			levelName += " 1";
		}
		
		LoadLevel(GameSetObserver.Instance.CurrentBattle.Map.Environment.ToString(),
		          levelName);
		
		LoadResources(GameSetObserver.Instance.CurrentBattle.Map.Environment);
		
		PushLoadedShipsToPool(GameSetObserver.Instance.CurrentGameType);
		
		InitShips(GameSetObserver.Instance.Players);


		
		PushLoadedWeaponsToPool();
		
		PushLoadedSpawnPointsToPool();
		
		PushLoadedFlagSpotsToPool();
		
		PushLoadedFlagsToPool();
		
		PushLoadedEffectsToPool();
		
		
	}
	
	private void OnEndBattle()
	{
		RemoveAllEffect();
		Pool.UnloadItem();
		Resources.UnloadUnusedAssets();
		isTryingGetExplosion = false;
	}
	
	private void LoadLevel(string environment, string levelName)
	{
		Debug.Log ("Load Level");
		string levelPath = string.Format (LevelPath, environment, levelName);
		Debug.Log (string.Format ("Current scene is {0}", Application.loadedLevelName));
		if (UIControllerForNGUI.Instance != null)
		{
			if (!UIControllerForNGUI.Instance.IsTutorial)
			{
				Application.LoadLevelAdditive(levelName);
			}
			else
			{
				Application.LoadLevelAdditive("Tutorial");
			}
		}
		else
		{
			Application.LoadLevelAdditive(levelName);
		}

	}
	
	#region Ships Resources
	
	private void PushLoadedShipsToPool(GameType game)
	{
		foreach (Object shipSystemPrefab in ShipSystemPrefabs)
		{
			if (game == GameType.Multiplayer)
				continue;
			
			GameObject shipSystem = null;
			shipSystem = Instantiate(shipSystemPrefab) as GameObject;
			
			if (shipSystem == null)
			{
				Debug.LogError(string.Format("Can't instatiate shipPrefab: {0}", shipSystemPrefab.name));
				continue;
			}
			
			ShipBehaviour shipBehaviour = shipSystem.GetComponent<ShipBehaviour>();
			Pool.PushShip(shipBehaviour);
		}
	}
	
	private void InitShips(List<Player> players)
	{
		if (players == null)
		{
			Debug.LogError("ResourceBehaviourController.InitShips: players not initialized");
			return;
		}
		
		foreach (Player player in players)
		{
			InitShip(player);
		}
		
		Debug.Log(string.Format("ResourceBehaviourController: ship was initialized"));
	}
	
	public void InitShip(Player player)
	{
		Debug.Log ("start Loading SHIPs from Resurses");
		
		Debug.Log ("Need Change loading patch");


		string loadingPath = string.Format(ShipPath + "/{1}/{1}_{2}",
		                                EnvironmentType.MachineCity,	//   GameSetObserver.Instance.CurrentBattle.Map.Environment,
		                                   player.MyShip.Type,
		                                   player.Team == TeamColor.BlueTeam ? "Blue" : "Orange");

		Debug.Log ("SHIPS "+player.MyShip.Type + "/Shiptype/ "+loadingPath);

		GameObject shipSystem = null;
		
		if (GameSetObserver.Instance.CurrentGameType == GameType.Multiplayer)
		{
			if (GameController.Instance.IsShipBelongHumanFleet(player))
			{
				object[] args = new object[1];
				args[0] = player.Id;
				shipSystem = PhotonNetwork.Instantiate(loadingPath, Vector3.zero, Quaternion.identity, 0, args) as GameObject;
				Debug.Log(string.Format("Instatiate ship for player: {0}, ship system is {1}", player.Id, shipSystem));
			}
		}
		else
		{
			shipSystem = Resources.Load(loadingPath) as GameObject;
		}
		
		if (shipSystem == null)
		{
			if (GameSetObserver.Instance.CurrentGameType != GameType.Multiplayer)
				Debug.LogError(string.Format("Error when loading {0}, for player: {1}", loadingPath, player.Id));
			
			return;
		}
		
		ShipBehaviour shipBehaviour = shipSystem.GetComponent<ShipBehaviour>();
		if (shipBehaviour == null)
		{
			Debug.LogError(string.Format("Error: selected game object no shipBehaviour"));
			return;
		}
		
		if (GameSetObserver.Instance.CurrentGameType != GameType.Multiplayer) {
		
			shipBehaviour = (ShipBehaviour)Pool.ShipPop (shipBehaviour);
		}
		if (shipBehaviour == null)
		{
			Debug.LogError(string.Format("Error: Cannot load shipBehaviour object from pool"));
			return;
		}
		
		shipBehaviour.Player = player;
		
		shipBehaviour.SetEventHandler();
		LoadShipParamFromConfig (player.MyShip);
		
		BattleController.Instance.ships.Add(shipBehaviour);
		BattleController.Instance.PositionablesObject.Add(player);
	}
	
	private void LoadShipParamFromConfig(Aratog.NavyFight.Models.Unity3D.Ship.Ship _ship )
	{
		Debug.Log ("LoadShipParamFromConfig ShipType: "+_ship.Type);

		_ship.setBasicHealthPoint( ConfigShips.Ships[_ship.Type].Health);
		_ship.MaxSpeed = ConfigShips.Ships[_ship.Type].MaxSpeed;
		_ship.BombCount = ConfigShips.Ships[_ship.Type].BombCount;
		_ship.RotationSpeed = ConfigShips.Ships[_ship.Type].RotationSpeed;

		_ship.MaxVelocity = ConfigShips.Ships[_ship.Type].MaxVelocity;

		_ship.MaxSpeed = ConfigShips.Ships[_ship.Type].MaxVelocity;

		_ship.Acceleration = ConfigShips.Ships[_ship.Type].Acceleration;
		_ship.AccelerationDown = ConfigShips.Ships[_ship.Type].AccelerationDown;
	}
	
	#endregion
	

	#region Bonus Resourses
	public BonusBehavior GetBonus()
	{
		BonusBehavior bonusBehaviour = null;

	 	GameObject	bonuses = Resources.Load("Bonus") as GameObject;

		bonusBehaviour = Pool.BonusPop(bonuses.GetComponent<BonusBehavior>())as BonusBehavior;

		return bonusBehaviour;
	}
	
	#endregion


	#region Weapon Resources
	
	private const int ProjectileCount = 5;
	private const int BombCount = 4;
	private const int TorpedoCount = 4;
	
	
	
	private void PushLoadedWeaponsToPool()
	{
		foreach (Object weaponsPrefab in WeaponsPrefabs)
		{
			GameObject weaponsSystem = Instantiate(weaponsPrefab) as GameObject;
			
			if (weaponsSystem == null)
			{
				Debug.LogError("Can't instatiate weapon object");
				continue;
			}
			
			WeaponBehaviour weaponBehaviour = weaponsSystem.GetComponent<WeaponBehaviour>();
			
			if (weaponBehaviour == null)
			{
				Debug.LogError("Can't get component WeaponBehaviour from game object");
				continue;
			}
			Pool.Push(weaponBehaviour);
			int weaponsCount = weaponBehaviour.type == WeaponsType.BasicBomb
				? BombCount
					: weaponBehaviour.type == WeaponsType.BasicProjectile ? ProjectileCount : TorpedoCount;
			Pool.AddMassive(weaponBehaviour, weaponsCount);
		}
	}
	
	private WeaponBehaviour CreateWeaponsBehaviour(string weaponsName)
	{
		
		GameObject bullet = Resources.Load(WeaponBehaviour.PrefabPath + weaponsName) as GameObject;
		GameObject bulletInScene = Instantiate(bullet) as GameObject;
		
		if (bulletInScene != null)
		{
			WeaponBehaviour bulletInSceneObject = bulletInScene.GetComponent<WeaponBehaviour>();
			
			return bulletInSceneObject;
		}
		
		return null;
	}
	
	/// <summary>
	/// Получаем ГО соответсствующего снаряда из пула объектов
	/// </summary>
	/// <param name="weapons">Тип оружия</param>
	/// <param name="player"></param>
	/// <returns></returns>
	public WeaponBehaviour GetWeaponsFromPool(WeaponsType weapons, Player player)
	{
		
		string weaponsName = string.Empty;
		string weaponColor = "_" + (player.Team == TeamColor.BlueTeam ? "Blue" : "Orange");
		
		switch (weapons)
		{
		case WeaponsType.BasicProjectile:
			weaponsName = BasicProjectilleName + weaponColor;
			break;
		case WeaponsType.BasicTorpedo:
			weaponsName = BasicTorpedoName + weaponColor;
			break;
		case WeaponsType.BasicBomb:
			weaponsName = BasicBombName + weaponColor;
			break;
		case WeaponsType.Missile:
			weaponsName = "Rocket" + weaponColor;
			break;
		case WeaponsType.HomingMissile:
			weaponsName = "HommingRocket" + weaponColor;
			break;
		case WeaponsType.OneRicochet:
			weaponsName = "Ricoshet" + weaponColor;
			break;
		case WeaponsType.TwoRicochet:
			weaponsName = "TwoRicoshet" + weaponColor;
			break;
		case WeaponsType.Napalm:
			weaponsName = "Napalm" + weaponColor;
			break;
		case WeaponsType.SuperTorpedo:
			weaponsName = "SuperTorpedo" + weaponColor;
			break;
		case WeaponsType.FrozenProjectile:
			weaponsName = "FrozenProjectile" + weaponColor;
			break;
		case WeaponsType.DeepBomb:
			weaponsName = "BombAdvance" + weaponColor;
			break;
		case WeaponsType.MortalProjectile:
			weaponsName = "Mortal" + weaponColor;
			break;
		}
		Debug.Log (weaponsName);
		Debug.Log("Get weapon: " + weaponsName);
		if (weaponsName == string.Empty)
			return null;
		
		GameObject bullet = null;
		WeaponBehaviour bulletFromPool = null;
		
		bullet = Resources.Load(WeaponBehaviour.PrefabPath + weaponsName) as GameObject;
		
		//	print("ResourceBehaviourController.GetWeaponsFromPool - weapon name " + weaponsName);
		
		if (bullet != null)
		{
			bulletFromPool = Pool.Pop(bullet.GetComponent<WeaponBehaviour>()) as WeaponBehaviour ??
				CreateWeaponsBehaviour(weaponsName).GetComponent<WeaponBehaviour>();
		}

		return bulletFromPool;
	}
	public WeaponBehaviour GetWeaponsFromPool(WeaponsType weapons, TeamColor Team)
	{
		
		string weaponsName = string.Empty;
		string weaponColor = "_" + (Team == TeamColor.BlueTeam ? "Blue" : "Orange");
		
		switch (weapons)
		{
		case WeaponsType.BasicProjectile:
			weaponsName = BasicProjectilleName + weaponColor;
			break;
		case WeaponsType.BasicTorpedo:
			weaponsName = BasicTorpedoName + weaponColor;
			break;
		case WeaponsType.BasicBomb:
			weaponsName = BasicBombName + weaponColor;
			break;
		case WeaponsType.Missile:
			weaponsName = "Rocket" + weaponColor;
			break;
		case WeaponsType.HomingMissile:
			weaponsName = "HommingRocket" + weaponColor;
			break;
		case WeaponsType.OneRicochet:
			weaponsName = "Ricoshet" + weaponColor;
			break;
		case WeaponsType.TwoRicochet:
			weaponsName = "TwoRicoshet" + weaponColor;
			break;
		case WeaponsType.Napalm:
			weaponsName = "Napalm" + weaponColor;
			break;
		case WeaponsType.SuperTorpedo:
			weaponsName = "SuperTorpedo" + weaponColor;
			break;
		case WeaponsType.FrozenProjectile:
			weaponsName = "FrozenProjectile" + weaponColor;
			break;
		case WeaponsType.DeepBomb:
			weaponsName = "BombAdvance" + weaponColor;
			break;
		case WeaponsType.MortalProjectile:
			weaponsName = "Mortal" + weaponColor;
			break;
		}
		
		Debug.Log("Get weapon: " + weaponsName);
		if (weaponsName == string.Empty)
			return null;
		
		GameObject bullet = null;
		WeaponBehaviour bulletFromPool = null;
		
		bullet = Resources.Load(WeaponBehaviour.PrefabPath + weaponsName) as GameObject;
		
		//	print("ResourceBehaviourController.GetWeaponsFromPool - weapon name " + weaponsName);
		
		if (bullet != null)
		{
			bulletFromPool = Pool.Pop(bullet.GetComponent<WeaponBehaviour>()) as WeaponBehaviour ??
				CreateWeaponsBehaviour(weaponsName).GetComponent<WeaponBehaviour>();
		}
		
		return bulletFromPool;
	}
	#endregion


	
	#region Flag Resources
	
	private void PushLoadedFlagsToPool()
	{
		foreach (Object flagsPrefab in FlagsPrefabs)
		{
			
			GameObject flagSystem = Instantiate(flagsPrefab) as GameObject;
			
			if (flagSystem == null)
			{
				Debug.LogError("Can't instatiate flag object");
				continue;
			}
			
			FlagsBehaviour flagsBehaviour = flagSystem.GetComponent<FlagsBehaviour>();
			
			if (flagsBehaviour == null)
			{
				Debug.LogError("Can't get component FlagsBehaviour from game object");
				continue;
			}
			
			Pool.Push(flagsBehaviour);
		}
	}
	
	/// <summary>
	/// Инициализация флагов, используем её из вне, так как необходимо, чтобы нужные данные были проинициализированы
	/// </summary>
	/// <param name="environment"></param>
	/// <param name="color"></param>
	/// <param name="flag"></param>
	public FlagsBehaviour InitFlags(EnvironmentType environment, TeamColor color, FlagParent flag)
	{
		
		//TODO:: change this setting after
		
		string flagColorStr = color == TeamColor.BlueTeam ? "Blue" : "Orange";
		
		string flagPrefabPath = string.Format(FlagsPath + "/Flag_{1}", environment, flagColorStr);
		
		GameObject flags = null;
		FlagsBehaviour colorFlag = null;
		
		flags = Resources.Load(flagPrefabPath) as GameObject;
		
		if (flags == null)
		{
			Debug.LogError("Can't load orange flag prefab");
			return null;
		}
		
		colorFlag = flags.GetComponent<FlagsBehaviour>();
		
		colorFlag = Pool.Pop(colorFlag) as FlagsBehaviour;
		
		if (colorFlag == null)
		{
			Debug.LogError(string.Format("Can't load {0} flag prefab", flagColorStr));
			return null;
		}
		
		colorFlag.Flag = flag;
		
		colorFlag.InitFlags();
		
		return colorFlag;
	}
	
	//TODO:: OnEnd Battle Need Return flag to the pool
	
	#endregion
	
	#region Base Resources
	
	private void PushLoadedFlagSpotsToPool()
	{
		foreach (Object flagSpotsPrefab in FlagSpotPrefabs)
		{
			
			GameObject flagSpotSystem = Instantiate(flagSpotsPrefab) as GameObject;
			
			if (flagSpotSystem == null)
			{
				Debug.LogError("Can't instatiate flagSpot object");
				continue;
			}
			
			FlagSpotBehaviour flagSpotsBehaviour = flagSpotSystem.GetComponent<FlagSpotBehaviour>();
			
			if (flagSpotsBehaviour == null)
			{
				Debug.LogError("Can't get component FlagSpotsBehaviour from game object");
				continue;
			}
			
			Pool.Push(flagSpotsBehaviour);
		}
	}
	
	public FlagSpotBehaviour InitFlagSpot(EnvironmentType environment, TeamColor color, BaseParent baseParent)
	{
		string flagSpotColorStr = color == TeamColor.BlueTeam ? "Blue" : "Orange";
		
		string flagSpotPrefabPath = string.Format(FlagSpotsPath + "/FlagSpot_{1}", environment, flagSpotColorStr);
		
		GameObject flagSpot = Resources.Load(flagSpotPrefabPath) as GameObject;
		
		if (flagSpot == null)
		{
			Debug.LogError(string.Format("Can't load {0} flag prefab", flagSpotColorStr));
			return null;
		}
		
		FlagSpotBehaviour colorFlagSpot = Pool.Pop(flagSpot.GetComponent<FlagSpotBehaviour>()) as FlagSpotBehaviour;
		
		if (colorFlagSpot == null)
		{
			Debug.LogError(string.Format("Can't load {0} flag prefab", flagSpotColorStr));
			return null;
		}
		
		colorFlagSpot.Base = baseParent;
		
		colorFlagSpot.Init();
		
		return colorFlagSpot;
		
	}
	
	#endregion
	
	#region SpawnPoint resources
	
	private void PushLoadedSpawnPointsToPool()
	{
		foreach (Object spawnPointPrefab in SpawnPointPrefabs)
		{
			GameObject spawnPointSystem = Instantiate(spawnPointPrefab) as GameObject;
			
			if (spawnPointSystem == null)
			{
				Debug.LogError("Can't instatiate spawn point object");
				continue;
			}
			
			SpawnPointBehaviour spawnPoint = spawnPointSystem.GetComponent<SpawnPointBehaviour>();
			
			if (spawnPoint == null)
			{
				Debug.LogError("Can't get component SpawnPointBehaviour from the game object");
				continue;
			}
			
			Pool.Push(spawnPoint);
		}
	}
	
	public SpawnPointBehaviour InitSpawnPoints(EnvironmentType environment, TeamColor color)
	{
		string spawnPointColorStr = color == TeamColor.BlueTeam ? "Blue" : "Orange";
		
		string spawnPointPrefabPath = string.Format(SpawnPointPath + "/SpawnPoint_{1}", environment, spawnPointColorStr);
		
		GameObject spawnPointSystem = Resources.Load(spawnPointPrefabPath) as GameObject;
		
		if (spawnPointSystem == null)
		{
			Debug.LogError(string.Format("Can't load {0} spawn point prefab", spawnPointColorStr));
			return null;
		}
		
		SpawnPointBehaviour spawnPoint = Pool.Pop(spawnPointSystem.GetComponent<SpawnPointBehaviour>()) as SpawnPointBehaviour;
		
		if (spawnPoint == null)
		{
			Debug.LogError(string.Format("Can't load {0} spawn point prefab", spawnPointColorStr));
			return null;
		}
		
		return spawnPoint;
	}
	
	#endregion
	
	#region Effects
	
	private const int BasicEffectsCount = 1;
	
	private void PushLoadedEffectsToPool()
	{
		foreach (Object effectPrefab in EffectPrefabs)
		{
			GameObject effecrsSystem = Instantiate(effectPrefab) as GameObject;
			
			if (effecrsSystem == null)
			{
				Debug.LogError("Can't instatiate effects object");
				continue;
			}
			
			EffectsBehaviour effectBehaviour = effecrsSystem.GetComponent<EffectsBehaviour>();
			
			if (effectBehaviour == null)
			{
				Debug.LogError("Can't get component EffectsBehaviour from game object");
				continue;
			}
			
			int weaponsCount = effectBehaviour.Effects == EffectsBehaviour.EffectsType.FlagBlue || effectBehaviour.Effects == EffectsBehaviour.EffectsType.FlagRed
				? BasicEffectsCount
					: 2;
			
			Pool.AddMassive(effectBehaviour, weaponsCount);
		}
	}
	
	private EffectsBehaviour CreateEffectBehaviour(string effectsName)
	{
		GameObject effectFx = Resources.Load(effectsName) as GameObject;
		GameObject effectInScene = Instantiate(effectFx) as GameObject;
		
		if (effectInScene != null)
		{
			EffectsBehaviour explosionBehaviourInSceneObject = effectInScene.GetComponent<EffectsBehaviour>();
			
			return explosionBehaviourInSceneObject;
		}
		
		return null;
	}
	
	
	private bool isTryingGetExplosion = false;
	
	private List<EffectsBehaviour> _activeEffects = new List<EffectsBehaviour>();
	
	public void AddEffect(EffectsBehaviour effects)
	{
		if (_activeEffects == null)
			return;
		
		if (_activeEffects.Exists(effect => effect == effects))
			return;
		
		_activeEffects.Add(effects);
	}
	
	public void RemoveEffect(EffectsBehaviour effects)
	{
		if (_activeEffects == null)
			return;
		
		if (_activeEffects.Exists(effect => effect == effects))
			_activeEffects.Remove(effects);
	}
	
	private void RemoveAllEffect()
	{
		if (_activeEffects.Count == 0)
			return;
		
		while (_activeEffects.Count > 0)
		{
			_activeEffects[0].Remove();
		}
	}
	
	public EffectsBehaviour GetEffectsFromPool(EffectsBehaviour.EffectsType effect)
	{
		string effectName = string.Empty;
		
		switch (effect)
		{
		case EffectsBehaviour.EffectsType.HitExplosion:
			effectName = EffectsBehaviour.HitExplosionPrefab;
			//return null;
			break;
		case EffectsBehaviour.EffectsType.MortarExplosive:
			effectName = EffectsBehaviour.MortalPrefab;
			Debug.Log("MOrtar prefab is "+effectName);
			//return null;
			break;
		case EffectsBehaviour.EffectsType.SparksBlue:
			effectName = EffectsBehaviour.SparksBluePrefab;
			break;
			
		case EffectsBehaviour.EffectsType.SparksRed:
			effectName = EffectsBehaviour.SparksRedPrefab;
			break;
			
		case EffectsBehaviour.EffectsType.SplashForProjectile:
			effectName = EffectsBehaviour.SplashForProjectilePrefab;
			break;
			
		case EffectsBehaviour.EffectsType.SplashForBomb:
			effectName = EffectsBehaviour.SplashForBombPrefab;
			break;
			
		case EffectsBehaviour.EffectsType.GroundExplosion:
			effectName = EffectsBehaviour.GroundExplosionPrefabPath;
			break;
			
		case EffectsBehaviour.EffectsType.Damage:
			effectName = EffectsBehaviour.DamagePrefab;
			break;
			
		case EffectsBehaviour.EffectsType.Smoke:
			effectName = EffectsBehaviour.SmokePrefab;
			break;
			
		case EffectsBehaviour.EffectsType.ShipExplosion:
			effectName = EffectsBehaviour.ShipExplosionPrefab;
			break;
			
		case EffectsBehaviour.EffectsType.BuildingExplosion:
			effectName = EffectsBehaviour.BuildingEplosionPrefab;
			break;
			
		case EffectsBehaviour.EffectsType.FlagRed:
			effectName = EffectsBehaviour.FlagRedPrefab;
			break;
			
		case EffectsBehaviour.EffectsType.FlagBlue:
			effectName = EffectsBehaviour.FlagBluePrefab;
			break;
		}
		
		
		GameObject effectsSmall = null;
		EffectsBehaviour effectBehaviour = null;
		
		effectsSmall = Resources.Load(effectName) as GameObject;
		
		
		
		if (effectsSmall != null)
		{
			effectBehaviour = Pool.Pop(effectsSmall.GetComponent<EffectsBehaviour>()) as EffectsBehaviour ??
				CreateEffectBehaviour(effectName).GetComponent<EffectsBehaviour>();
		}
		
		if (effectBehaviour != null)
			AddEffect(effectBehaviour);
		
		return effectBehaviour;
	}
	
	
	#endregion
	
	#endregion
	
}
