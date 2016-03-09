#define AUTOMOVEMENT_ENABLED

using System.Runtime.InteropServices;
using UnityEngine;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Extensions;
using Aratog.NavyFight.Models.Unity3D.Players;

public class HUDJoystick : MonoBehaviour
{
	public static HUDJoystick Instance { get; private set; }

	// Для более простой проверки текущей механики
	internal bool IsNewWave {
		get {
			return GameSetObserver.Instance.Mechanics == Aratog.NavyFight.Models.Games.MechanicsType.NewWave;
		}
	}

	// Нажата ли кнопка джойстика
	public bool IsPressed = false;

#region NewWave

	// Вектор, образованный точками положения позиции джойстика и пальца
	public Vector3 Vector;

	// Скорость, образованная длиной вектора Vector
	public float Speed;

	private Vector2 dir;

	// Длина вектора, образованного точками позиции середины джойстика и пальца
	float _vectorMagnitude = -1;
	
	[SerializeField]
	Rect _joystickZone = new Rect(-0.15f, -0.15f, 0.72f, 0.72f);
	
	[SerializeField]
	float _thumbZoneRadius = 0.39f;
	
	[SerializeField]
	float _thumbRadiusLimit = 0.25f;

	[SerializeField]
	GameObject NewWave;
	
	[SerializeField]
	Transform Thumb;
	
	[SerializeField]
	UISprite UIThumb, UIBack;
	
	[SerializeField]
	Color _UIInactive = new Color(1, 1, 1, 0.45f), _UIActive = new Color(1, 1, 1, 1);
	
	Vector3 _defaultJoystickPos, _defaultThumbPos;
#endregion
	
#region Classic

    private float _directionDelay;

    public bool IsLeftPressed, IsRightPressed, IsDownPressed, IsUpPressed, IsStopPressed;

	public System.Action OnStop;

	[SerializeField]
    UIClassicJoystickButton RightButton, DownButton, UpButton, LeftButton, StopButton;

	[SerializeField]
	GameObject Classic;

    int _fingerID = -1;

    // Направление движения и механика автодвижения
    Direction _dir = Direction.UP, _lastDir;

	public delegate void OnDirectionChangeHandler();

	public delegate void On90DegreeRotationHandler();

	public event On90DegreeRotationHandler On90DegreeRotationEvent;
	public event OnDirectionChangeHandler OnDirectionChangeEvent;

    public System.Action OnDirectionChange, On90DegreeRotation;

    bool _cancelAutomovement;

#if AUTOMOVEMENT_ENABLED
    Direction _automovement = Direction.NONE;
#endif

	public bool IsAutomovementActive
	{
#if AUTOMOVEMENT_ENABLED
		get { return _automovement != Direction.NONE && _automovement != Direction.DNIWE; }
#endif
//		get { return false; }
//#endif

	}

	public void CancelAutoMovement()
	{
		Direction = Direction.STOP;
		_automovement = Direction.NONE;
	}

    public Direction Direction
    {
        get { return _dir; }
        set
        {
#if AUTOMOVEMENT_ENABLED
            // Не пытайся это понять - просто имей ввиду, что сие условие срабатывает при изменении направления (вне зависимости от того, активен режим автодвижения или нет)
            if ((_automovement != Direction.NONE && value != Direction.NONE && value != _dir) || (value != Direction.NONE && value != Direction.STOP && value != _dir && value != _lastDir))
            {
                if (OnDirectionChangeEvent != null) {
                    OnDirectionChangeEvent();
                }
                if (!IsOpposite(value, _dir))
                {
                    if (On90DegreeRotationEvent != null) {
                        On90DegreeRotationEvent();
                    }
                }
            }

            if (value == Direction.NONE)
            {
                if (_automovement != Direction.NONE)
                    return;
                _cancelAutomovement = false;
                _lastDir = _dir;
            }
            else if (value == Direction.STOP)
            {
                _cancelAutomovement = false;
                _automovement = Direction.NONE;
                _dir = Direction.NONE;
                return;
            }
            else if (_automovement == Direction.NONE)
            {
                if (value != _dir && value != _lastDir)
                {
                    _cancelAutomovement = true;
                    _automovement = Direction.NONE;
                }
                else if (value == _dir && !_cancelAutomovement)
                {
                    _automovement = _dir;
                }
            }
#else
            if (value == Direction.STOP)
                return;

            if (value != _dir)
            {
                if (OnDirectionChange != null)
                {
                    OnDirectionChange();
                }
                if (!IsOpposite(value, _dir))
                {
                    if (On90DegreeRotation != null)
                    {
                        On90DegreeRotation();
                    }
                }
                _lastDir = _dir;
            }
#endif

            _dir = value;
        }
    }
#endregion

    internal Vector2 DirectionVector
	{
		get
		{
			//Debug.Log("Direction is " + Direction);

			if (IsNewWave)
				return Vector.To2D();

			return Direction.ToVector2();
		}
	}

    GameObject _active;

    public void SetDefaultDirection(Direction defaultDirection)
    {
        _dir = defaultDirection;

        // Отключаем автодвижение
        Direction = Direction.STOP;
    }

	void Awake()
	{
		Instance = this;

#if !AUTOMOVEMENT_ENABLED
	    StopButton.gameObject.SetActive(false);
#endif
	}

	void Start()
	{
		Classic.SetActive(!IsNewWave);
		NewWave.SetActive(IsNewWave);

		_active = (Classic.activeSelf) ? Classic : NewWave;

		_defaultJoystickPos = Thumb.position;
		_defaultThumbPos = _defaultJoystickPos;
		if (_active == NewWave)
			UIThumb.color = UIBack.color = _UIInactive;

	    Player.VisibleMagnitude = ((_defaultThumbPos.To2D() + Vector2.up * _thumbRadiusLimit) - _defaultThumbPos.To2D()).magnitude;

		Debug.Log("Player.VisibleMagniude: " + Player.VisibleMagnitude);
	}
	
	[SerializeField]
	LayerMask _layerMask;
	
	void Update ()
	{
		if (!GameSetObserver.Instance.IsBattleStarted || GameSetObserver.Instance.IsPause)
			return;

	    if (IsNewWave)
	    {
		    Vector = Vector3.zero;
	        if (AInput.Pressed() > 0)
	        {
	            Ray ray = default(Ray);
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER
	            ray = UIManager.Camera.ScreenPointToRay(AInput.ScreenPos());
	            Vector = (ray.origin.To2D() - _defaultThumbPos.To2D());
				Vector = new Vector3(Mathf.Clamp(Vector.x, -1f, 1f), Mathf.Clamp(Vector.y, -1f, 1f), Mathf.Clamp(Vector.z, -1f, 1f));
	            _vectorMagnitude = Vector2.Distance(ray.origin, _defaultThumbPos);
	            if (Vector2.Distance(ray.origin, _defaultThumbPos) < _thumbZoneRadius ||
                    _joystickZone.Contains(ray.origin - _defaultJoystickPos))
	            {
					if (!IsPressed && _joystickZone.Contains(ray.origin - _defaultJoystickPos))
					{
						_active.transform.position = new Vector3(ray.origin.x, ray.origin.y, _active.transform.position.z);
						_defaultThumbPos = Thumb.position;
						UIThumb.color = UIBack.color = _UIActive;
					}
	                IsPressed = true;
	            }
#elif UNITY_IPHONE || UNITY_ANDROID
				for(int i=0; i<Input.touches.Length; i++)
				{
                    if (_fingerID != -1 && i != _fingerID)
				        break;
                    ray = UIManager.Camera.ScreenPointToRay(new Vector3(Input.touches[i].position.x, Input.touches[i].position.y, 0));
					Vector = (ray.origin.To2D() - _defaultThumbPos.To2D());
					_vectorMagnitude = Vector2.Distance(ray.origin, _defaultThumbPos);
					if (Vector2.Distance(ray.origin, _defaultThumbPos) < _thumbZoneRadius || _joystickZone.Contains(ray.origin - _defaultJoystickPos)) {
						if (!IsPressed && _joystickZone.Contains(ray.origin - _defaultJoystickPos)) {
						    _fingerID = i;
							_active.transform.position = new Vector3(ray.origin.x, ray.origin.y, _active.transform.position.z);
							_defaultThumbPos = Thumb.position;
							UIThumb.color = UIBack.color = _UIActive;
						}
						IsPressed = true;
					}
				}
#endif
	            if (IsPressed)
	            {
	                if (_vectorMagnitude < _thumbRadiusLimit)
	                    Thumb.position = new Vector3(ray.origin.x, ray.origin.y, _defaultThumbPos.z);
	                else
	                    Thumb.position = _defaultThumbPos + Vector.normalized * _thumbRadiusLimit;

		            Speed = Mathf.Clamp(Mathf.Lerp(0, 1, Mathf.InverseLerp(0, _thumbZoneRadius, _vectorMagnitude)), 0, 1f);
	            }
	        }
	        else if (IsPressed)
	        {
	            _fingerID = -1;
	            Speed = 0;
	            Thumb.position = _defaultThumbPos;
	            UIThumb.color = UIBack.color = _UIInactive;
	            IsPressed = false;
	            _active.transform.position = _defaultJoystickPos;
	            _defaultThumbPos = Thumb.position;
	        }
	        else
	        {
		        UpdateKeyPressed();
				CheckDirection(true);
		        if (IsPressed)
		        {
			        Speed = 1f;
		        }
	        }
	    }
	    else
	    {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER
		    UpdateKeyPressed();
#endif

            if (AInput.Pressed() > 0)
            {
                Ray ray = default(Ray);
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER
                ray = UIManager.Camera.ScreenPointToRay(AInput.ScreenPos());
                RaycastHit hitInfo = default(RaycastHit);
                if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, _layerMask) && hitInfo.collider != null)
                {
                    UIClassicJoystickButton button = hitInfo.collider.GetComponent<UIClassicJoystickButton>();
                    if (button)
                    {
                        if (button.Direction == Direction.LEFT)
                        {
                            IsLeftPressed = true;
                        }
                        else if (button.Direction == Direction.RIGHT)
                        {
                            IsRightPressed = true;
                        }
                        else if (button.Direction == Direction.DOWN)
                        {
                            IsDownPressed = true;
                        }
                        else if (button.Direction == Direction.UP)
                        {
                            IsUpPressed = true;
                        }
                        else if (button.Direction == Direction.STOP)
                        {
                            IsStopPressed = true;
                            if (OnStop != null)
                                OnStop();
                        }
                        button.OnPress();
                    }
					
                }
#elif UNITY_IPHONE || UNITY_ANDROID
				//for(int i=0; i<Input.touches.Length; i++)
				{
                    /*if (_fingerID != -1 && i != _fingerID)
				        break;*/
                    ray = UIManager.Camera.ScreenPointToRay(AInput.ScreenPos());
					RaycastHit hitInfo =  UICamera.lastHit; //default(RaycastHit);
                    if (/*Physics.Raycast(ray, out hitInfo) && */hitInfo.collider != null && UICamera.hoveredObject != null)
                    {

						UIClassicJoystickButton button = UICamera.hoveredObject.GetComponent<UIClassicJoystickButton>(); //hitInfo.collider.GetComponent<UIClassicJoystickButton>();
                        if (button != null)
                        {
                            //_fingerID = i;
                            if (button.Direction == Direction.LEFT)
                            {
                                IsLeftPressed = true;
                            }
                            else if (button.Direction == Direction.RIGHT)
                            {
                                IsRightPressed = true;
                            }
                            else if (button.Direction == Direction.DOWN)
                            {
                                IsDownPressed = true;
                            }
                            else if (button.Direction == Direction.UP)
                            {
                                IsUpPressed = true;
                            }
                            else if (button.Direction == Direction.STOP)
                            {
                                IsStopPressed = true;
                                if (OnStop != null)
                                    OnStop();
                            }
                            else
                            {
                                IsLeftPressed = IsRightPressed = IsDownPressed = IsUpPressed = IsStopPressed = false;
                            }
                            button.OnPress();
                        }
                    }
                    else
                    {
                        IsLeftPressed = IsRightPressed = IsDownPressed = IsUpPressed = IsStopPressed = false;
                    }
				}
#endif
            }
            else
            {
                _fingerID = -1;
#if (!(UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER) && (UNITY_IPHONE || UNITY_ANDROID))
                IsLeftPressed = IsRightPressed = IsDownPressed = IsUpPressed = IsStopPressed = false;
#endif
            }

			CheckDirection();

#if AUTOMOVEMENT_ENABLED
	        if (_automovement != Direction.NONE)
	        {
	            IsPressed = true;
	        }
#endif
			//Debug.Log("Direction is " + Direction);
	    }
	}

	private void CheckDirection(bool isNewWave = false)
	{
		IsPressed = (IsLeftPressed || IsRightPressed || IsUpPressed || IsDownPressed || IsStopPressed);

		if (isNewWave)
		{
			if (IsLeftPressed)
			{
				Vector += Vector3.left;
			}
			else if (IsRightPressed)
			{
				Vector += Vector3.right;
			}

			if (IsUpPressed)
			{
				Vector += Vector3.up;
			}
			else if (IsDownPressed)
			{
				Vector += Vector3.down;
			}

			if (IsStopPressed)
			{
				Vector += Vector3.zero ;
			}
			else if (Direction != Direction.NONE)
			{
				Direction = Direction.NONE;
			}
		}
		else
		{
			if (IsLeftPressed)
			{
				Direction = Direction.LEFT;
			}
			else if (IsRightPressed)
			{
				Direction = Direction.RIGHT;
			}
			else if (IsUpPressed)
			{
				Direction = Direction.UP;
			}
			else if (IsDownPressed)
			{
				Direction = Direction.DOWN;
			}
			else if (IsStopPressed)
			{
				Direction = Direction.STOP;
			}
			else if (Direction != Direction.NONE)
			{
				Direction = Direction.NONE;
			}
		}


	}

	bool IsOpposite(Direction currentDir, Direction newDir)
    {
        bool isOpposite = false;
        switch (currentDir)
        {
            case Direction.UP:
                if (newDir == Direction.DOWN)
                    isOpposite = true;
                break;
            case Direction.DOWN:
                if (newDir == Direction.UP)
                    isOpposite = true;
                break;
            case Direction.LEFT:
                if (newDir == Direction.RIGHT)
                    isOpposite = true;
                break;
            case Direction.RIGHT:
                if (newDir == Direction.LEFT)
                    isOpposite = true;
                break;
        }
        return isOpposite;
    }

	private void UpdateKeyPressed()
	{
		IsLeftPressed = (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A));
		IsRightPressed = (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D));
		IsUpPressed = (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W));
		IsDownPressed = (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S));
		IsStopPressed = Input.GetKey(KeyCode.X);
		if (IsStopPressed) {
			if (OnStop != null)
				OnStop();
		}
	}

	public static Direction GetDirection(Vector3 direction)
	{
		Direction dir = Direction.NONE;

		Debug.Log(string.Format("HUDJoystick.GetDirection - direction is : {0}", direction));

		Vector3 currentDir = new Vector3(Mathf.Floor(direction.x), 0, Mathf.Floor(direction.z));
		 
		return dir;
	}

    void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(Thumb.position, _thumbZoneRadius);

		Gizmos.color = Color.yellow;
		Vector3 size = new Vector3(_joystickZone.width, _joystickZone.height, 0);
		if (!Application.isPlaying)
			_defaultJoystickPos = NewWave.transform.position;
		Gizmos.DrawWireCube(_defaultJoystickPos + new Vector3(_joystickZone.x + size.x * 0.5f, _joystickZone.y + size.y * 0.5f, 0), size);
		Gizmos.color = Color.red;
		Gizmos.DrawWireCube(new Vector3(_joystickZone.x + size.x * 0.5f, _joystickZone.y + size.y * 0.5f, 0), size);
	}
}