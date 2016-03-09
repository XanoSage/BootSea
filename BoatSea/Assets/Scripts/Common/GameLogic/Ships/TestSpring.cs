using System.Collections.Generic;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Extensions;
using Aratog.NavyFight.Models.Unity3D.Players;
using Aratog.NavyFight.Models.Unity3D.Ship;
using UnityEngine;
using System.Collections;

public class TestSpring : MonoBehaviour {

	#region Constants

	private const float DemperCoeff = 1f;

	#endregion

	#region Variables

	private Vector3 Position;

	private ShipBehaviour _shipBehaviour;

	private Rigidbody _rigidbody;

	[SerializeField] private Transform _compass;

	#endregion


	#region MonoBehaviour action

	// Use this for initialization
	private void Start()
	{
		//Position = transform.position;

		_shipBehaviour = GetComponent<ShipBehaviour>();

		_rigidbody = GetComponent<Rigidbody>();

		InitPoints();
		InitMovement();

		//if (_shipBehaviour == null)
		//{
		//	throw new MissingComponentException("TestSpring.Start - cann't find ShipBehaviour component");
		//}
		//Destroy(this);
	}

	// Update is called once per frame
	private void Update()
	{
		
	}

	private void FixedUpdate()
	{
		UpdateMotion();
		UpdateMovement();
		UpdateAcceleration();
		UpdateCompassRotation();
		UpdateShipRotation();

		UpdateClassicMovement();

		FreeMovementTestUpdate();

		UpdateRigidBodyMovement();

		MoveToNearestPoint();
		
	}

	private void OnCollisionEnter(Collision collision)
	{

		Vector3 springVector = Vector3.zero;

		foreach (ContactPoint contact in collision.contacts)
		{
			Debug.DrawRay(contact.point, contact.normal, Color.white);
			//Debug.Log("TestSpring.OnCollisionEnter : points " + contact.point);

			//Vector3 pos = Position - contact.point;
			Vector3 pos = contact.point - Position;

			springVector += pos;
		}

		springVector = new Vector3(springVector.x, 0.0f, springVector.z);

		if (collision.relativeVelocity.magnitude > 2)
		{
			Debug.Log("TestSpring.OnCollisionEnter - collision relativeVelocity more than 2");
		}

		//Spring(springVector);
	}

	private void OnCollisionStay(Collision collisionInfo)
	{
		Vector3 springVector = Vector3.zero;

		//List<Vector3> contactPoints = new List<Vector3>();


		foreach (ContactPoint contact in collisionInfo.contacts)
		{
			Debug.DrawRay(contact.point, contact.normal, Color.white);

			//need add

			Vector3 pos = contact.point - Position;
			//Vector3 pos = Position + contact.point;

			springVector += pos;
			
			//Debug.Log("TestSpring.OnCollisionEnter : points " + contact.point + " " + springVector);

		}

		if (_shipBehaviour != null && _shipBehaviour.Player.MyShip.StateOfShipBehaviour == Ship.ShipBehaviourState.RespawnInvulnerability &&
		    (collisionInfo.collider.CompareTag("Mine") || collisionInfo.collider.CompareTag("Shell") ||
		     collisionInfo.collider.CompareTag("Untouchable")))
			return;

		if (collisionInfo.collider.CompareTag("UiRadar"))
		{
			return;
		}

		//if (collisionInfo.collider.name == "Collider01")
		//	return;

		//Debug.Log("ShipBehaviour.OnCollisionStay - collision name:" + other.transform.tag);

		if (collisionInfo.transform.GetComponentInChildren<FlagsBehaviour>() != null ||
		    collisionInfo.transform.GetComponentInChildren<WeaponBehaviour>() != null)
		{
			return;
		}

		springVector = new Vector3(springVector.x, 0.0f, springVector.z);

		//Spring(springVector);

		//Debug.Log("TestSpring.OnCollisionStay :" + collisionInfo.relativeVelocity+ " " + collisionInfo.relativeVelocity.magnitude);
	}

	private void OnCollisionExit(Collision collision)
	{
		Vector3 springVector = Vector3.zero;

		foreach (ContactPoint contact in collision.contacts)
		{
			Debug.DrawRay(contact.point, contact.normal, Color.white);
		//	Debug.Log("TestSpring.OnCollisionExit : points " + contact.point);

			Vector3 pos = contact.point - Position;
			//Vector3 pos = Position - contact.point;

			springVector += pos;
		}

		springVector = new Vector3(springVector.x, 0.0f, springVector.z);
		//Spring(springVector);
	}

	#endregion

	#region Actions

	private float _timeCoeff = 10f;

	private void Spring(/*Collision collision*/ Vector3 springVector)
	{
		//if (collision.relativeVelocity.magnitude > _velocityValue)
		{

			Vector3 pos = _shipBehaviour == null ? Position : _shipBehaviour.Player.Position;
			Vector3 newPos = pos - springVector*DemperCoeff;
			//Vector3 newPos = pos - collision.relativeVelocity*DemperCoeff;

			Position = Vector3.Lerp(pos, newPos, Time.deltaTime*_timeCoeff);

			if (_shipBehaviour != null)
			{
				_shipBehaviour.Player.Position = Position;
			}
		}
	}

	private void UpdateMotion()
	{
		Vector3 direction =  Vector3.zero;

		if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
		{
			direction += Vector3.left;
			_input = new Vector2(-1, 0);
			StartMoving();
		}
		else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
		{
			direction += Vector3.right;
			_input = new Vector2(1, 0);
			StartMoving();
		}
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
		{
			direction += Vector3.forward;
			_input = new Vector2(0, 1);
			StartMoving();
		}
		else if ((Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)))
		{
			direction += Vector3.back;
			_input = new Vector2(0, -1);
			StartMoving();
		}

		
		_isPowerOn = false;
		//_isPowerOn = true;

		if (Input.GetKeyUp(KeyCode.X))
		{
			_isMoveOnFree = !_isMoveOnFree;
		}

		if (Input.GetKey(KeyCode.G))
		{
			_isPowerOn = true;
		}


		if (direction != Vector3.zero)
		{
			_isPowerOn = true;

			if (!_isMoving)
				_direction = direction;
		}

	}

	#region Movement action

	private const float MinVelocity = 0f;
	private const float MaxVelocity = 10.0f;

	private const float Acceleration = 10.5f;
	private const float AccelerationDown = -6.5f;

	private float _currentVelocity = 0f;

	private float _acceleration = 0f;

	private Vector3 _direction = Vector3.zero;

	private bool _isPowerOn = false;

	private Vector3 _forceValue = Vector3.zero;

	private void UpdateMovement()
	{

		Vector3 currentPos = _rigidbody.position ;

		Vector3 newPos = currentPos + _direction*(_currentVelocity*Time.deltaTime + _acceleration*0.5f*Time.deltaTime*Time.deltaTime);
		_currentVelocity += _acceleration*Time.deltaTime;

		_currentVelocity = Mathf.Clamp(_currentVelocity, MinVelocity, MaxVelocity);

		Position = newPos; 
	}

	private const float BoardSize = 7f;

	private void UpdateRigidBodyMovement()
	{
		if (_isMoveOnFree)
			_isRotated = false;

		if (_isNeedCorrectingPosBeforeTurn)
			return;

		if (_isRotated)
			return;

		_rigidbody.MovePosition(Position);
		
		if (_rigidbody.position.x > BoardSize)
		{
			_rigidbody.position = new Vector3(-BoardSize, 0, _rigidbody.position.z);
		}
		if (_rigidbody.position.x < -BoardSize)
		{
			_rigidbody.position = new Vector3(BoardSize, 0, _rigidbody.position.z);
		}

		if (_rigidbody.position.z > BoardSize)
		{
			_rigidbody.position = new Vector3(_rigidbody.position.x, 0, -BoardSize);
		}
		if (_rigidbody.position.z < -BoardSize)
		{
			_rigidbody.position = new Vector3(_rigidbody.position.x, 0, BoardSize);
		}
	}

	private void UpdateAcceleration()
	{
		if (!_isPowerOn)
		{
			_acceleration = _currentVelocity > MinVelocity ? AccelerationDown : 0;
		}

		else if (_isPowerOn)
		{
			_acceleration = _currentVelocity >= MaxVelocity ? 0f : Acceleration;
		}
	}

	private const float ShipRotationSpeed = 8f;
	private const float CompassRotationSpeed = 24f;

	private void UpdateCompassRotation()
	{
		float mag = _direction.magnitude;

		if (mag > 0.001f)
		{
			Quaternion lookRot = Quaternion.LookRotation(_direction);
			_compass.rotation = Quaternion.Slerp(_compass.rotation, lookRot, Mathf.Clamp01(CompassRotationSpeed*Time.deltaTime));
		}
	}

	private bool _isRotated = false;
	private const float EpsAngle = 0.25f;

	private void UpdateShipRotation()
	{
		if (!_rigidbody.rotation.AlmostEquals(_compass.rotation, EpsAngle))
		{
			_isRotated = true;

			_rigidbody.rotation = Quaternion.Slerp(_rigidbody.rotation, _compass.rotation,
			                                       Mathf.Clamp01(ShipRotationSpeed*Time.deltaTime));
		}
		else
		{
			_isRotated = false;
		}
	}

	private float _angleCount = 0;
	private float _angleMax = 360;

	private float _movementRadius = 0.5f;

	private float _angleDelta = 0.01f;

	private bool _isMoveOnFree = false;

	private void FreeMovementTestUpdate()
	{
		if (!_isMoveOnFree)
			return;

		_angleCount += _angleDelta;

		if (_angleCount >= _angleMax)
		{
			_angleCount -= _angleMax;
		}

		float x = _movementRadius*Mathf.Cos(_angleCount*Mathf.PI);
		float z = _movementRadius*Mathf.Sin(_angleCount*Mathf.PI);

		_direction = new Vector3(x, 0, z).normalized;
		//Debug.Log(string.Format("TestSpring.FreeMovementTestUpdate: _direction {0}", _direction));
	}

	#endregion

	#region Classic movement using points

	private const int FieldWidth = 8;

	private const int FieldHeight = 8;

	private int _pointsCount = FieldHeight * FieldWidth;

	private const int CellSize = 2;

	private Vector3[] _points;

	private bool _isPointsInit = false;

	private Vector2 _input;

	private Vector3 _startPos, _endPos;

	private float _nextPointMagnitude;
	private bool _isMoving = false;

	private Vector3 _previousDirection = Vector3.zero;

	private void InitPoints()
	{
		_points = new Vector3[_pointsCount];

		int counter = 0;

		for (int i = - FieldWidth/2; i < FieldWidth/2; i++)
		{
			for (int j = -FieldHeight/2; j < FieldHeight/2; j++)
			{
				Vector3 point = new Vector3(i*CellSize, 0, j*CellSize);

				_points[counter] = point;

				counter++;
			}

		}

		_isPointsInit = true;
	}

	private void InitMovement()
	{
		_rigidbody.MovePosition(GetNearestPoint(_rigidbody.position));

		_startPos = _rigidbody.position;
	}

	private void StartMoving(Vector3 endPos = default(Vector3))
	{
		_startPos = transform.position;

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

		//Debug.Log(string.Format("TestSpring.StartMoving - OK"));
	}

	private void UpdateClassicMovement()
	{
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

			Debug.Log(string.Format("TestSpring.UpdateClassicMovement - _previousDirection: {0}, direction: {1}",
			                        _previousDirection, _direction));
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

	private void OnDrawGizmos()
	{
		if (!_isPointsInit)
			return;

		Gizmos.color = Color.white;

		foreach (Vector3 t in _points)
		{
			Gizmos.DrawWireSphere(t, 0.5f);
		}
	}

	private const float EpsMagnitude = 0.06125f;

	private Vector3 GetNearestPoint(Vector3 forPoint, Vector3 currentPos = default(Vector3))
	{
		float minDistance = float.MaxValue;
		int indexOfTheNearestPoint = -1;
		for(int i=0; i<_points.Length; i++)
		{
			float currDistance = (_points[i] - forPoint).sqrMagnitude;
			if (currDistance < minDistance) {
				minDistance = currDistance;
				indexOfTheNearestPoint = i;
			}
		}

		if (currentPos != default(Vector3) && (currentPos - _points[indexOfTheNearestPoint]).sqrMagnitude <= EpsMagnitude)
			return currentPos;
		return _points[indexOfTheNearestPoint];
		
	}

	private bool _isNeedCorrectingPosBeforeTurn = false;

	private Vector3 _currentNearestPoint;

	private void MoveToNearestPoint()
	{
		if (!_isNeedCorrectingPosBeforeTurn)
			return;

		Vector3 pos = _rigidbody.position;

		Vector3 newPos = Vector3.Slerp(pos, _currentNearestPoint,
		                               (_currentVelocity*Time.deltaTime + _acceleration*0.5f*Time.deltaTime*Time.deltaTime));
			
		//	pos + _direction*(_currentVelocity*Time.deltaTime + _acceleration*0.5f*Time.deltaTime*Time.deltaTime);
		//_currentVelocity += _acceleration*Time.deltaTime;

		Position = newPos; 

		_rigidbody.MovePosition(Position);

		if ((_currentNearestPoint - _rigidbody.position).sqrMagnitude < EpsMagnitude*0.001f)
		{
			_isNeedCorrectingPosBeforeTurn = false;

			//Debug.Log(string.Format("TestSpring.MoveToNearestPoint - nearestPoint: {0}, _rigidbody: {1}, magnitude {2}",
			//						_currentNearestPoint, _rigidbody.position,
			//						(_currentNearestPoint - _rigidbody.position).sqrMagnitude));
		}
		
	}

	#endregion

	#endregion
}
