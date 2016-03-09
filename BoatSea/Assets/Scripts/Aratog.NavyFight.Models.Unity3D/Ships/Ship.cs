using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Weapons;
using UnityEngine;
using Aratog.NavyFight.Models.Unity3D.Ship;
using System.Collections;


namespace Aratog.NavyFight.Models.Unity3D.Ship {
	public abstract class Ship : IInitable {
		#region Variables

		#region Basic Ship Data

		/// <summary>
		/// тип корабля
		/// </summary>
		public ShipType Type;

        /// <summary>
        /// Его скорость
        /// </summary>
        public float Speed;

        /// <summary>
        /// Его максимальная скорость
        /// </summary>
        public float MaxSpeed;

		public const float MinVelocity = 0f;

		public float MaxVelocity;

		public float Acceleration;
		public float AccelerationDown;

		/// <summary>
		/// Коеффициент замедления скорости поворота
		/// </summary>
        public float RotationSpeedCoeff;

		/// <summary>
		/// Скорость поворота
		/// </summary>
        public float RotationSpeed;

		/// <summary>
		/// Количество жизней
		/// </summary>
		public int HealthPoint;

        //armor points
        public int BaseArmor;
        public int Armor;
		/// <summary>
		/// ?
		/// </summary>
		public int Rate = 5;

		/// <summary>
		/// Размер плавсредства
		/// </summary>
		public float Size;

		/// <summary>
		/// Направление движения: Для классического режима
		/// </summary>
		public Direction DirectionClassic;

		/// <summary>
		/// Направление движения: Для режима new wave
		/// </summary>
		public Vector3 DirectionNW;


		/// <summary>
		/// Позиция на карте
		/// </summary>
		public Vector3 Position;

		public bool IsClassicMode { get; protected set; }

		/// <summary>
		/// Владелец: ИИ, человек или человек играющий по сети
		/// </summary>
		public Player Owner;

		/// <summary>
		/// Количество предустановленных мин
		/// </summary>
		public int BombCount;

		/// <summary>
		/// Количество одновременно стреляемых пушек
		/// </summary>
		public int BasicWeaponShellCount;


		/// <summary>
		/// Количество жизней по умолчанию
		/// </summary>
		protected int BasicHealthPoint;
       
		/// <summary>
		/// Количество бомб по умолчанию
		/// </summary>
		protected int BasicBombCount;

	    public float CameraFOVFrom, CameraFOVTo;

		public bool IsNeedMineReset {
			get { return BombCount < BasicBombCount; }
		}

		#endregion

		//TODO: add abstract class weapon that would contain basic data to all weapon in the game (special weapon too)
		public Weapon BasicBomb;
		public Weapon BasicWeapon;
        public Weapon AdvanceBomb;
        public Weapon AdvanceWeapon;

		//public Weapon SpecialWeapon;

		/// <summary>
		/// Список членов твоего флота
		/// </summary>
		public List<Ship> Command;


		/// <summary>
		/// Время респавна
		/// </summary>
		public int TimeRespawn;


		/// <summary>
		/// Время мигания, пока кораблик не могут потопить
		/// </summary>
		public int BlinkingTime;

		public int shellOwnerplayerID { get; private set; }

		/// <summary>
		/// Переменная по которой будем знать, что у корабля есть флаг.
		/// </summary>
		public bool IsFlagTaken;

		public bool IsInvulnerability;

		public bool IsShipSunk;


		public static List<ShipType> PoolOfShipType;

		public static float BeforeRespawnDelay =1;

		public static float InvulnerabilityTime = 3;

		public static float SunkSpeed = 4f;

		public static float SubmarineDiveSpeed = 0.75f;

        public static float SubmarineDiveY = 1.0f;

        public static float KindOfResistance = 0.25f;

		public static float AfterHitBeforeSunkDelay = 2.5f;

        //Bonuses variables
        public bool isImortal = false;
        public float timetoImortal;

        public bool isInvisible = false;


		public enum ShipBehaviourState
		{
			RespawnInvulnerability,
			Normal,
			Destroyed,
			Sunk,
		}

		public ShipBehaviourState StateOfShipBehaviour;

		#endregion

		#region Constructor

		public Ship () {
			Command = new List<Ship>();
			shellOwnerplayerID = -1;
			IsFlagTaken = false;
			IsInvulnerability = false;
			IsShipSunk = false;
			IsClassicMode = true;
			DirectionNW = Vector3.forward;
			BasicBomb = Weapon.CreateWeapon(WeaponsType.BasicBomb);
			BasicWeaponShellCount = 1;
			BasicBombCount = 3;

           
           
			StateOfShipBehaviour = ShipBehaviourState.RespawnInvulnerability;

			Size = 1f;
		}
        public void setBasicHealthPoint(int param)
        { 
        BasicHealthPoint = param;
        }

        public int getBasicHealthPoint()
        {
            return BasicHealthPoint;
        }
	
		/// <summary>
		/// Используем шаблон фабричный метод для создания корабликов
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Ship CreateShip (ShipType type) {
			Ship ship = null;
			switch (type) {
				case ShipType.Boat :
					ship = new BoatShip();
					break;
				case ShipType.Submarine :
					ship = new Submarine();
					break;
				case ShipType.BigShip :
                    //TODO:: for Playable Demo Only
					ship = new BigShip();	
					break;
                case ShipType.ComandBase:
                    ship = new ComandBase();
                    break;
                case ShipType.SmallMetal:
                    ship = new BoatShip();
                    ship.Type = ShipType.SmallMetal;
                    break;
                case ShipType.MiddleMetal:
                    ship = new Submarine();
                    ship.Type = ShipType.MiddleMetal;
                    break;
                case ShipType.BigMetal:
                    ship = new BigShip();	
                    ship.Type = ShipType.BigMetal;
                    break;
                case ShipType.SmallAtlant:
                    ship = new BoatShip();
                    ship.Type = ShipType.SmallAtlant;
                    break;
                case ShipType.MiddleAtlant:
                    ship = new Submarine();
                    ship.Type = ShipType.MiddleAtlant;
                    break;
                case ShipType.BigAtlant:
                    ship = new BigShip();	
                    ship.Type = ShipType.BigAtlant;
                    break;
                case ShipType.SmallDark:
                    ship = new BoatShip();
                    ship.Type = ShipType.SmallDark;
                    break;
                case ShipType.MiddleDark:
                    ship = new Submarine();
                    ship.Type = ShipType.MiddleDark;
                    break;
                case ShipType.BigDark:
                    ship = new BigShip();	
                    ship.Type = ShipType.BigDark;
                    break;




                default:
                    ship = new BoatShip();
                    break;
			}
			return ship;
		}

		public static List<Ship> BasicShipPool () {
			List<Ship> ships = new List<Ship>();

			foreach (ShipType shipType in PoolOfShipType) {
				ships.Add(CreateShip(shipType));
			}

			return ships;
		}

		#endregion

		#region Events

		public delegate void OnFireBasicHandler (bool fromServer);

        public delegate void OnFireAdwanceWeaponHandler(bool fromServer);

		public delegate void OnRespawHandler (bool isInit);

		public delegate void OnShipSunkHandler ();

		public delegate void OnShipDestroyedHandler ();

		public delegate void OnFlagTakenHandler (TeamColor color);

		public delegate void OnFlagDroppedHandler (TeamColor color);

		public delegate void OnFlagDeliveredHandler (TeamColor color);

		public delegate void OnFlagReturnedHandler (TeamColor color);

		public delegate void OnFireBombHandler (bool fromServer);

		public delegate void OnMineResetHandler ();

		public delegate void OnDamageTakenHandler ();

		public delegate void OnCriticalDamageTakenHandler ();

		private OnDamageTakenHandler onDamageTakenHandler;

		public event OnDamageTakenHandler OnDamageTakenEvent
		{
			add { onDamageTakenHandler += value; }
			remove { onDamageTakenHandler += value; }
		}

		private OnCriticalDamageTakenHandler onCriticalDamageTakenHandler;

		public event OnCriticalDamageTakenHandler OnCriticalDamageTakenEvent
		{
			add { onCriticalDamageTakenHandler += value; }
			remove { onCriticalDamageTakenHandler -= value; }
		}

		public event OnMineResetHandler OnMineResetEvent;

		private OnFireBombHandler onFireBombHandler;

		public event OnFireBombHandler OnFireBombEvent
		{
			add { onFireBombHandler += value; }
			remove { onFireBombHandler -= value; }
		}


      

		private OnFlagTakenHandler onFlagTakenHandler;

		public event OnFlagTakenHandler OnFlagTakenEvent
		{
			add { onFlagTakenHandler += value; }
			remove { onFlagTakenHandler -= value; }
		}

		private OnFlagDeliveredHandler onFlagDeliveredHandler;

		public event OnFlagDeliveredHandler OnFlagDeliveredEvent
		{
			add { onFlagDeliveredHandler += value; }
			remove { onFlagDeliveredHandler -= value; }
		}

		private OnFlagDroppedHandler onFlagDroppedHandler;

		public event OnFlagDroppedHandler OnFlagDroppedEvent
		{
			add { onFlagDroppedHandler += value; }
			remove { onFlagDroppedHandler -= value; }
		}

		private OnFlagReturnedHandler onFlagReturnedHandler;

		public event OnFlagReturnedHandler OnFlagReturnedEvent
		{
			add { onFlagReturnedHandler += value; }
			remove { onFlagReturnedHandler -= value; }
		}

		private OnFireBasicHandler onFireBasicHandler;

		public event OnFireBasicHandler OnFireBasicEvent
		{
			add {
              
                onFireBasicHandler += value; }
			remove { onFireBasicHandler -= value; }
		}

        private OnFireAdwanceWeaponHandler onFireAdvanceHandler;

        public event OnFireAdwanceWeaponHandler OnFireAdvanceEvent
        {
            add
            {

                onFireAdvanceHandler += value;
            }
            remove { onFireAdvanceHandler -= value; }
        }

		private OnRespawHandler onRespawHandler;

		public event OnRespawHandler OnRespawnEvent
		{
			add { onRespawHandler += value; }
			remove { onRespawHandler -= value; }
		}

		private OnShipSunkHandler onShipSunkHandler;

		public event OnShipSunkHandler OnShipSunkEvent
		{
			add { onShipSunkHandler += value; }
			remove { onShipSunkHandler -= value; }
		}

		private OnShipDestroyedHandler onShipDestroyedHandler;

		public event OnShipDestroyedHandler OnShipDestroyedEvent
		{
			add { onShipDestroyedHandler += value; }
			remove { onShipDestroyedHandler -= value; }
		}




		/// <summary>
		/// Описывает поведение управления для каждого типа кораблей а также собитий возникающих при движении
		/// </summary>
		public virtual void OnMove () {

		}

		/// <summary>
		/// Описывает поведение мины базового оружия, а также событий возникающих при выстреле
		/// </summary>
		public virtual void OnFireBomb (bool fromServer = false) {
			if (!BasicBomb.IsCanShoot)
				return;

			if (BombCount < 1)
				return;

			BombCount--;

			if (onFireBombHandler != null) {

				onFireBombHandler(fromServer);
				BasicBomb.OnFire();
			}
		}

		/// <summary>
		/// Описывает поведение снарядом базового оружия, а также событий возникающих при выстреле
		/// </summary>
		public virtual void OnFireBasic (bool fromServer = false) {
			if (onFireBasicHandler != null) {
				onFireBasicHandler(fromServer);
			}
		}

        /// <summary>
        /// Описывает поведение снарядом Улучшеного оружия, а также событий возникающих при выстреле
        /// </summary>
        public virtual void OnFireAdwance(bool fromServer = false)
        {
            if (onFireAdvanceHandler != null)
            {
                onFireAdvanceHandler(fromServer);
            }
        }

		public void SetShellOwnerId (int playerId) {
			shellOwnerplayerID = playerId;
		}

		/// <summary>
		/// Загрузка мин при приближении к точке респавна (к базе), также выполнение событий происходящий при этом
		/// </summary>
		public virtual void OnMineReset () {

			MineReset();

			if (OnMineResetEvent != null)
				OnMineResetEvent();
		}

		public virtual void MineReset () {
			if (BombCount >= BasicBombCount)
				return;

			BombCount = BasicBombCount;
		}

		/// <summary>
		/// Cобытия и действия происходящие в случае, если корабль затонул (нас подбили)
		/// </summary>
		public virtual void OnShipSunk () {

			//if (IsFlagTakken && Owner != null)
			//	OnFlagDropped(Owner.Team);

			MineReset();

			if (onShipSunkHandler != null)
				onShipSunkHandler();


			OnRespawn(true);
		}

		/// <summary>
		/// События и действия происходящие в случае, еслы мы подбили корабль
		/// </summary>
		public virtual void OnShipDestroyed () {
			Debug.Log("OnShipDestroyed() - parent");
			if (onShipDestroyedHandler != null)
				onShipDestroyedHandler();
		}


		/// <summary>
		/// Cобытия и действия происходящие в случае, если в корабль попали
		/// </summary>
		public virtual bool TakeDamage (int damage) {

            if (isImortal == false)
            {

                if (Armor > 0)
                {
                    Armor -= damage;
                    if (Armor < 0)
                    {
                        HealthPoint -= Armor * (-1);
                    }
                }
                else
                {
                    HealthPoint -= damage;
                }
            }

                //  Debug.Log("Ship was Damagedd health: "+HealthPoint+"  Damage: "+damage);
                if (onDamageTakenHandler != null)
                    onDamageTakenHandler();


                if (HealthPoint <= 0)
                {

                    if (onCriticalDamageTakenHandler != null)
                        onCriticalDamageTakenHandler();

                    return true;
                }
                return false;
           

			
		}

		/// <summary>
		/// События и действия происходящие в момент респавна
		/// </summary>
		public virtual void OnRespawn (bool invulnerability = false) {
			// Инициализируем камеру
			IsInvulnerability = invulnerability;
			if (onRespawHandler != null)
				onRespawHandler(invulnerability);

		}

		/// <summary>
		/// События происходящие во время подбора флага
		/// </summary>
		public virtual void OnFlagTaken (TeamColor color) {
			IsFlagTaken = true;

			if (onFlagTakenHandler != null)
				onFlagTakenHandler(color);
		}

		public virtual void OnFlagDelivered (TeamColor color) {
			IsFlagTaken = false;

			if (onFlagDeliveredHandler != null)
				onFlagDeliveredHandler(color);
		}

		public virtual void OnFlagDropped (TeamColor color) {
			IsFlagTaken = false;

			if (onFlagDroppedHandler != null)
				onFlagDroppedHandler(color);
		}

		public virtual void OnFlagReturned (TeamColor color) {
			if (onFlagReturnedHandler != null)
				onFlagReturnedHandler(color);
		}

		#endregion


		public void Init () {
			throw new NotImplementedException();
            
		}

       


		public static void InitPoolOfShipType () {
           
			PoolOfShipType = new List<ShipType>
				{
					ShipType.Boat,
					ShipType.Submarine,
					ShipType.BigShip
					//ShipType.Submarine,
				};
		}

	}
}
