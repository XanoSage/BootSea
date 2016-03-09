using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Extensions;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Ship;
using UnityEngine;
using System.Collections;

public class ShipMovement : MonoBehaviour
{

	#region Constants

	private const float EpsMagnitude = 0.06125f;

	#endregion

	#region Variables

	private ShipBehaviour _shipBehaviour;

	private Rigidbody _rigidbody;

	[SerializeField] private Transform _compass;

	private Quaternion _previousRotation;

	private const float Coeff = 60f;

	// for movement
	private const float MinVelocity = 0f;
	private const float MaxVelocity = 10.0f*Coeff;

	private const float Acceleration = 10.5f*Coeff*0.75f;
	private const float AccelerationDown = -8.5f*Coeff*0.75f;

	private float _currentVelocity = 0f;

	private Vector3 _vectorVelocity = Vector3.zero;

	private float _acceleration = 0f;

	private Vector3 _direction = Vector3.zero;

	private bool _isPowerOn = false;
	//--------------

	// for rotation
	private const float ShipRotationSpeed = 6f;
	private const float CompassRotationSpeed = 40f;
	private bool _isRotated = false;
	private const float EpsAngle = 0.2f;
	//-------------

	// for test
	private float _angleCount = 0;
	private float _angleMax = 360;

	private float _movementRadius = 0.5f;

	private float _angleDelta = 0.01f;

	private bool _isMoveOnFree = false;
	//-------------

	//for classic handling mode

	private const int FieldWidth = 24;

	private const int FieldHeight = 24;

	private int _pointsCount = FieldHeight*FieldWidth;

	private const int CellSize = 2;

	private Vector3[] _points;

	private bool _isPointsInit = false;

	private Vector2 _input;

	private Vector3 _startPos, _endPos;

	private Vector3 _shipPosition;

	private Vector3 Position
	{
		//get { return _shipBehaviour.Player.Position; }
		//set { _shipBehaviour.Player.Position = value; }
		get { return _shipPosition; }
		set { _shipPosition = value; }
	}

	public bool IsInMovement
	{
		get { return _isInMovement; }
	}

	private float _nextPointMagnitude;
	private bool _isMoving = false;

	private Vector3 _previousDirection = Vector3.zero;

	private bool _isNeedCorrectingPosBeforeTurn = false;

	private Vector3 _currentNearestPoint;

	//-------------


	// for inner variables

	private float _shipAcceleration;
	private float _shipAccelerationDown;

	private float _shipRotationSpeed;
	private float _shipMaxVelocity;

	// for player joystick handler
	private bool _isPlayerJoystickHandled = false;

	#endregion

	#region MonoBehaviour actions

	// Use this for initialization
	private void Start()
	{
		_shipBehaviour = GetComponent<ShipBehaviour>();

		_rigidbody = GetComponent<Rigidbody>();

		InitPoints();
		//InitMovement();
		InitInnerVariables();

		IgnorePlaneCollider();

		SubscribeEvents();
	}

	// Update is called once per frame
	private void Update()
	{

	}

	private void FixedUpdate()
	{
		//if (_shipBehaviour != null && _shipBehaviour.Player.MyShip.StateOfShipBehaviour != Ship.ShipBehaviourState.Normal)
		//	return;

		UpdateDirection();
		UpdateMovement();
		UpdateAcceleration();
		UpdateCompassRotation();
		UpdateShipRotation();

		UpdateClassicMovement();

		UpdateRigidbodyMovement();

		MoveToNearestPoint();

	}

	private void OnDrawGizmos()
	{
		if (_shipBehaviour != null)
		{
			return;
		}

		//if (!_isPointsInit)
		//	return;

		//Gizmos.color = Color.white;

		//foreach (Vector3 t in _points)
		//{
		//	Gizmos.DrawWireSphere(t, 0.5f);
		//}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.collider.CompareTag("Mine"))
		{
			return;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.collider.CompareTag("Mine"))
		{
			return;
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		if (collision.collider.CompareTag("Mine"))
		{
			return;
		}
	}

	private void OnDestroy()
	{
		UnsubscribeEvents();
	}

	#endregion

	#region Common Movement Action

	private bool _isInMovement = false;

	private bool IsShipNormal()
	{
		return _shipBehaviour != null &&
		       (_shipBehaviour.Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.RespawnInvulnerability ||
		        _shipBehaviour.Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.Normal);
	}

	private void UpdateDirection()
	{
		if (!IsShipNormal())
		{
			_currentVelocity = 0f;
			_acceleration = 0;
			return;
		}

		Vector3 direction = Vector3.zero;
		_isPlayerJoystickHandled = false;

		if (_shipBehaviour != null)
		{
			//_shipBehaviour.Player.IsInMovement = false;
			_isInMovement = false;
			if (_shipBehaviour.Player == GameSetObserver.Instance.Human && HUDJoystick.Instance.IsPressed)
			{
				if (Player.Mechanics == MechanicsType.NewWave)
				{
					 direction = HUDJoystick.Instance.DirectionVector.ToXZ();
					_isPlayerJoystickHandled = true;
					
				}
			}
			else
			{
				if (_shipBehaviour.Player == GameSetObserver.Instance.Human)
				{
					_isMoving = false;
					_isPowerOn = false;
				}
				return;
			}
		}

		

		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)|| HUDJoystick.Instance.IsLeftPressed)
		{
			MoveToLeft(ref direction);
		}
		else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || HUDJoystick.Instance.IsRightPressed)
		{
			MoveToRight(ref direction);
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || HUDJoystick.Instance.IsUpPressed)
		{
			MoveToForward(ref direction);
		}
		else if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || HUDJoystick.Instance.IsDownPressed))
		{
			MoveToBackward(ref direction);
		}

		//Debug.Log("direction = " + direction);

		_isPowerOn = false;
		//_isPowerOn = true;

		if (Input.GetKeyUp(KeyCode.X) || HUDJoystick.Instance.IsStopPressed)
		{
			_isMoving = false;
			_isPowerOn = false;
		}

		if (Input.GetKey(KeyCode.G) )
		{
			_isPowerOn = true;
		}


		if (direction != Vector3.zero)
		{
			_isPowerOn = true;

			if (!_isMoving)
				_direction = direction;

			if (_shipBehaviour != null)
			{
				//_shipBehaviour.Player.IsInMovement = true;
				_isInMovement = true;
			}


		}

		if (_shipBehaviour != null)
		{
			//_isInMovement = _shipBehaviour.Player.IsInMovement;
		}

		//Debug.Log(string.Format("ShipMovement.UpdateDirection - OK, IsInMovement: {0} ", _isInMovement));
	}

	private void UpdateMovement()
	{
		if (!IsShipNormal())
			return;

		_vectorVelocity = _rigidbody.transform.forward * /*_direction**/
									 (_currentVelocity*Time.deltaTime + _acceleration*0.5f*Time.deltaTime*Time.deltaTime);

		if (!_isPlayerJoystickHandled)
		{
			_currentVelocity += _acceleration*Time.deltaTime;
		}
		else
		{
			_currentVelocity = HUDJoystick.Instance.Speed * _shipMaxVelocity;
		}

		_currentVelocity = Mathf.Clamp(_currentVelocity, Ship.MinVelocity, _shipMaxVelocity);

		Position = _vectorVelocity;
	}

	private const float BoardSize = 7f;

	private void UpdateRigidbodyMovement()
	{
		_shipBehaviour.Player.Position = _rigidbody.position;

		if (!IsShipNormal())
			return;

		if (_isMoveOnFree)
			_isRotated = false;

		if (_isNeedCorrectingPosBeforeTurn)
			return;

		if (_isRotated)
			return;

		//_rigidbody.MovePosition(Position);
		_rigidbody.velocity = Position;
	}

	private void UpdateAcceleration()
	{
		if (!IsShipNormal())
			return;

		if (!_isPowerOn)
		{
			_acceleration = _currentVelocity > MinVelocity ? _shipAccelerationDown : 0;
		}

		else if (_isPowerOn)
		{
			_acceleration = _currentVelocity >= MaxVelocity ? 0f : _shipAcceleration;
		}
	}

	#endregion

	#region Rotation Action

	private void UpdateCompassRotation()
	{
		if (!IsShipNormal())
		{
			//Debug.Log(string.Format("ShipMovement.UpdateCompassRotation - OK, current rotation: {0} (before return)", _compass.rotation.eulerAngles));
			return;
		}

		float mag = _direction.magnitude;

		float eulerCompas = _compass.rotation.eulerAngles.y;
		float eulerPrev = _previousRotation.eulerAngles.y;

		if (mag > 0.001f)
		{
			Quaternion lookRot = Quaternion.LookRotation(_direction);

			if (Mathf.Abs(lookRot.eulerAngles.y - eulerPrev) > EpsAngle)
				_compass.rotation = Quaternion.Slerp(_compass.rotation, lookRot,
				                                     Mathf.Clamp01(CompassRotationSpeed /**Time.deltaTime*/));

			//Debug.Log(string.Format("ShipMovement.UpdateCompassRotation - OK, current rotation: {0} ", _compass.rotation.eulerAngles));
		}
		_previousRotation = _compass.rotation;
	}

	private void UpdateShipRotation()
	{
		if (!IsShipNormal())
			return;

		if (!_rigidbody.rotation.AlmostEquals(_compass.rotation, EpsAngle))
		//if (Mathf.Abs(_rigidbody.rotation.eulerAngles.y - _compass.rotation.eulerAngles.y) > EpsAngle)
		{
			_isRotated = true;

			_rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, _compass.rotation,
			                                       Mathf.Clamp01(_shipRotationSpeed*Time.deltaTime));

			//Debug.Log(string.Format( "ShipMovement.UpdateShipRotation - OK (rotateted true), ship rotation {0}, compass rotation: {1}", _rigidbody.rotation.eulerAngles, _compass.rotation.eulerAngles));
		}
		else
		{
			_isRotated = false;
			//_rigidbody.rotation = _compass.rotation;
			//Debug.Log(string.Format( "ShipMovement.UpdateShipRotation - OK (rotateted false), ship rotation {0}, compass rotation: {1}", _rigidbody.rotation.eulerAngles, _compass.rotation.eulerAngles));
		}
	}

	#endregion

	#region Rails (classic) movement actions

	private void InitPoints()
	{
		if (_shipBehaviour != null)
		{

			Map currentMap = BattleController.Instance.ActiveBattle.Map;

			if (_shipBehaviour.Player.MyShip.Type == ShipType.BigShip)
			{
				_points = currentMap.AdvancedWayPoints;
			}
			else
			{
				_points = currentMap.BasicWayPoints;
			}
		}
		else
		{
			int counter = 0;

			_points = new Vector3[FieldWidth*FieldHeight];

			for (int i = - FieldWidth/2; i < FieldWidth/2; i++)
			{
				for (int j = -FieldHeight/2; j < FieldHeight/2; j++)
				{
					Vector3 point = new Vector3(i*CellSize, 0, j*CellSize);

					_points[counter] = point;

					counter++;
				}

			}
		}

		_previousRotation = _compass.rotation;
		_isPointsInit = true;
	}

	private void InitMovement()
	{
		_rigidbody.MovePosition(GetNearestPoint(_rigidbody.position));

		_startPos = _rigidbody.position;
	}

	private void StartMovement(Vector3 endPos = default(Vector3))
	{
		_startPos = GetNearestPoint(_rigidbody.position);//transform.position;

		if (endPos == default(Vector3))
		{
			_endPos = new Vector3(_startPos.x + System.Math.Sign(_input.x)*CellSize,
			                      _startPos.y, _startPos.z + System.Math.Sign(_input.y)*CellSize);
			_endPos = GetNearestPoint(_endPos, _startPos);
		}
		else
		{
			_endPos = endPos;
		}

		if (_endPos == default(Vector3))
		{
			return;
		}

		_nextPointMagnitude = (_endPos.To2D() - _startPos.To2D()).magnitude;

		_isMoving = true;
	}

	private void UpdateClassicMovement()
	{
		//if (_isMoveOnFree)
		//	return;

		if (!_isMoving)
			return;
		Vector2 v1 = new Vector2(_endPos.x, _endPos.z);
		Vector2 v2 = new Vector2(_rigidbody.position.x, _rigidbody.position.z);

		Vector3 dir = (_endPos - _startPos).normalized;
		_direction = dir;

		if (!IsVector3Equal(_previousDirection, _direction) && !_isNeedCorrectingPosBeforeTurn)
		{
			_isNeedCorrectingPosBeforeTurn = true;

			_currentNearestPoint = GetNearestPoint(_rigidbody.position);
		}

		_previousDirection = _direction;

		if ((v2 - v1).sqrMagnitude < EpsMagnitude)
		{
			_isMoving = false;
		}
	}

	private bool IsVector3Equal(Vector3 a, Vector3 b)
	{
		return (Mathf.Abs(a.x - b.x) <= EpsMagnitude && Mathf.Abs(a.z - b.z) <= EpsMagnitude);
	}

	private Vector3 GetNearestPoint(Vector3 forPoint, Vector3 currentPos = default (Vector3))
	{
		float minDistance = float.MaxValue;
		int indexOfTheNearestPoint = -1;
		for (int i = 0; i < _points.Length; i++)
		{
			float currDistance = (_points[i] - forPoint).sqrMagnitude;
			if (currDistance < minDistance)
			{
				minDistance = currDistance;
				indexOfTheNearestPoint = i;
			}
		}

		if (currentPos != default(Vector3) && (currentPos - _points[indexOfTheNearestPoint]).sqrMagnitude <= EpsMagnitude)
			return currentPos;
		return _points[indexOfTheNearestPoint];
	}

	private void MoveToNearestPoint()
	{
		if (!_isNeedCorrectingPosBeforeTurn)
			return;

		Vector3 pos = _rigidbody.position;

		Vector3 newPos = Vector3.Slerp(pos, _currentNearestPoint,
		                               (_currentVelocity*Time.deltaTime + _acceleration*0.5f*Time.deltaTime*Time.deltaTime));
		//Position = newPos;

		_rigidbody.MovePosition(newPos);

		Vector3 newVector = (_currentNearestPoint - _rigidbody.position);

		Debug.Log(string.Format("ShipMovement.MoveToNearestPoint - NewVector {0}, sqrMagnitude: {1}", newVector, newVector.sqrMagnitude));

		if (newVector.sqrMagnitude < EpsMagnitude*0.001075f)
		{
			_isNeedCorrectingPosBeforeTurn = false;
		}
	}


	private void MoveToLeft(ref Vector3 direction)
	{
		direction += Vector3.left;
		_input = new Vector2(-1, 0);
		StartMovement();
	}

	private void MoveToRight(ref Vector3 direction)
	{
		direction += Vector3.right;
		_input = new Vector2(1, 0);
		StartMovement();
	}

	private void MoveToForward(ref Vector3 direction)
	{
		direction += Vector3.forward;
		_input = new Vector2(0, 1);
		StartMovement();
	}

	private void MoveToBackward(ref Vector3 direction)
	{
		direction += Vector3.back;
		_input = new Vector2(0, -1);
		StartMovement();
	}

	#endregion

	#region Inner Variables Action

	private void InitInnerVariables()
	{
		if (_shipBehaviour != null)
		{
			_shipRotationSpeed = _shipBehaviour.Player.MyShip.RotationSpeed;
			_shipMaxVelocity = _shipBehaviour.Player.MyShip.MaxVelocity*Coeff;

			_shipAcceleration = _shipBehaviour.Player.MyShip.Acceleration *Coeff*0.75f;
			_shipAccelerationDown = _shipBehaviour.Player.MyShip.AccelerationDown *Coeff*0.75f;

			Debug.Log("ShipMovement.InitInnerVaribles - use config variables");
		}
		else
		{
			_shipRotationSpeed = ShipRotationSpeed;
			_shipMaxVelocity = MaxVelocity;

			_shipAcceleration = Acceleration;
			_shipAccelerationDown = AccelerationDown;

			Debug.Log("ShipMovement.InitInnerVaribles - use const variables");
		}

		
	}

	private bool _isPlaneIgnore = false;

	public bool IsPlaneIgnore {get { return _isPlaneIgnore; }}

	public void IgnorePlaneCollider(PlaneHelper planeHelper = null)
	{
		PlaneHelper plane =  planeHelper ?? FindObjectOfType<PlaneHelper>();

		if (plane != null)
		{
			Collider[] colliders = GetComponentsInChildren<Collider>();

			foreach (var collider1 in colliders)
			{
				Physics.IgnoreCollision(plane.PlaneCollider, collider1);

				Debug.Log(string.Format("ShipMovement.IgnorePlaneCollider - ignore collider {0} from collider {1}", plane.PlaneCollider.name, collider1.name));
			}

			_isPlaneIgnore = true;
		}
	}

	private void SubscribeEvents()
	{
		if (Player.Mechanics == MechanicsType.NewWave)
		{
			_isMoveOnFree = true;
		}

		if (_shipBehaviour != null && _shipBehaviour.Player is AIPlayer)
		{
			AIPlayer aiPlayer = _shipBehaviour.Player as AIPlayer;

			aiPlayer.MoveShipEvent += UpdateAiMovement;

			Debug.Log(string.Format("ShipMovement.SubscribeEvents - player Id: {0} subscribe",aiPlayer.Id));

			_isThisAiPlayer = true;
		}
	}

	private void UnsubscribeEvents()
	{
		if (_shipBehaviour != null )
		{
			if ( _shipBehaviour.Player is AIPlayer)
			{
				AIPlayer aiPlayer = _shipBehaviour.Player as AIPlayer;

				aiPlayer.MoveShipEvent -= UpdateAiMovement;

				Debug.Log(string.Format("ShipMovement.SubscribeEvents - player Id: {0} unsubscribe", aiPlayer.Id));
			}
		}
	}

	#endregion

	#region AI Ship movement Update

	private bool _isThisAiPlayer = false; 

	private void UpdateAiMovement(Vector3 direction)
	{
		if (!_isThisAiPlayer)
			return;

		_direction = direction;

		_isPowerOn = false;

		if (_direction != Vector3.zero)
		{
			_isPowerOn = true;

			_isInMovement = true;
		}
	}

	#endregion
}
