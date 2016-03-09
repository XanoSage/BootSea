using Pathfinding;
using UnityEngine;
using System.Collections;

public class AIPathTest : MonoBehaviour
{

	#region Variables

	public Transform Targret;

	private Seeker _seeker;
	private Path _path;

	private int _currentWaypoint;

	private float _moveSpeed = 7.5f;
	private float _turnSpeed = 5f;


	private float _maxWaypointDistance = 0.3f;

	[SerializeField] private Transform _compass;
	[SerializeField] private Transform _body;

	#endregion

	#region MonoBehaviour Actions

	// Use this for initialization
	private void Start()
	{

		_seeker = GetComponent<Seeker>();

		GetNewPath();

		//_characterController = GetComponent<CharacterController>();

		_currentWaypoint = 0;

	}

	private void OnPathComplete(Path p)
	{
		if (!p.error)
		{
			_path = p;
			_currentWaypoint = 0;
		}
		else
		{
			Debug.Log(p.error);
		}

	}

	// Update is called once per frame
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			GetNewPath();
		}

		UpdatePosition();
	}

	private void FixedUpdate()
	{

	}

	#endregion

	#region Actions

	private void UpdatePosition()
	{
		if (_path == null)
		{
			return;
		}

		if (_currentWaypoint >= _path.vectorPath.Count)
		{
			_path.Cleanup();
			GetNewPath();
			return;
		}

		Vector3 vectorFrom = new Vector3(_path.vectorPath[_currentWaypoint].x, 0, _path.vectorPath[_currentWaypoint].z);
		Vector3 vectorTo = new Vector3(transform.position.x, 0, transform.position.z);

		Vector3 pos = vectorTo;

		Vector3 dir = (vectorFrom - vectorTo).normalized;

		Vector3 newPosition = pos + dir*_moveSpeed*Time.deltaTime;

		//transform.position = Vector3.Lerp(pos, newPosition, Time.deltaTime);
		transform.position = newPosition;

		pos = transform.position;

		_compass.LookAt(_path.vectorPath[_currentWaypoint]);
		_body.rotation = Quaternion.Lerp(_body.rotation, _compass.rotation, Time.deltaTime*_turnSpeed);

		float distance = Vector3.Distance(pos, vectorFrom);

		if (distance < _maxWaypointDistance)
			_currentWaypoint ++;
	}

	private void GetNewPath()
	{
		if (null != _path)
		{
			_path.Cleanup();
		}

		RandomTargetPosition();
		_seeker.StartPath(transform.position, Targret.position, OnPathComplete);
	}

	private void RandomTargetPosition()
	{
		float posX = Random.Range(-30f, 30f);
		float posZ = Random.Range(-30f, 30f);

		Targret.position = new Vector3(posX, 0, posZ);
	}

	#endregion
}
