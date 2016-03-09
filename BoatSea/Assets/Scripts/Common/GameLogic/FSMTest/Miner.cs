using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections;

public enum Locations
{
	Goldmine,
	Bar,
	Bank,
	Home
};

public class Miner : MonoBehaviour
{

	private FiniteStateMachine<Miner> FSM;

	public Locations Location = Locations.Goldmine;
	public int GoldCarried = 0;
	public int MoneyInBank = 0;
	public int Thirst = 0;
	public int Fatigue = 0;

	//[HideInInspector]
	public float MinerSpeed = 10f;
	[HideInInspector]
	public float RotationSpeed = 10000.0f;

	[HideInInspector]
	public Quaternion Rotation;
	[HideInInspector]
	public Quaternion RotationTo;
	[HideInInspector]
	public Quaternion RotationFrom;
	[HideInInspector]
	public Vector3 Direction;

	public Transform GoldMine;
	public Transform Bank;

	public Vector3 CurrentTarget;

	public void Awake()
	{
		Debug.Log("Miner Awake ...");
		FSM = new FiniteStateMachine<Miner>();
		if (EnterMineAndDigForNugget.Instance == null)
		{
			Debug.Log("EnterMineAndDigForNugget is null");
		}
		FSM.Configure(this, GoToTheMine.Instance);
	}

	public void ChangeState(FSMState<Miner> state)
	{
		FSM.ChangeState(state);
	}

	// Update is called once per frame
	private void Update()
	{
		Thirst++;
		FSM.Update();

	}

	public void ChangeLocation(Locations location)
	{
		Location = location;
	}

	public void AddToGoldCarried(int amount)
	{
		GoldCarried += amount;
	}

	public void AddToMoneyInBank(int amount)
	{
		MoneyInBank += amount;
		GoldCarried = 0;
	}

	public bool ReachEnough()
	{
		return false;
	}

	public bool PocketsFull()
	{
		bool full = GoldCarried == 2;
		return full;
	}

	public bool Thirsty()
	{
		return Thirst == 10;
	}

	public void IncreaseFatigue()
	{
		Fatigue++;
	}

	public bool GoToTheTarget(Vector3 target)
	{
		Direction = (target - transform.position).normalized;

		RotationMiner();

		transform.position += Direction*MinerSpeed*Time.deltaTime;

		if (IsReachingTargetPos(0.2f, target))
			return true;

		return false;
	}

	private bool IsReachingTargetPos(float eps, Vector3 targetPosition)
	{
		//eps = eps;
		//Debug.Log(string.Format("Position {0}, Target:{1}", Position, TargetPosition));
		return transform.position.x <= targetPosition.x + eps && transform.position.x >= targetPosition.x - eps &&
			       transform.position.z <= targetPosition.z + eps && transform.position.z >= targetPosition.z - eps;
	}

	private void RotationMiner()
	{
		RotationTo = Quaternion.LookRotation(Direction);

		Rotation = 
			transform.rotation = 
			Quaternion.Slerp(transform.rotation, RotationTo, RotationSpeed*Time.deltaTime);

	}
}
