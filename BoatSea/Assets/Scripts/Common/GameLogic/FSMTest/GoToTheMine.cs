using UnityEngine;
using System.Collections;

public class GoToTheMine : FSMState<Miner>
{

	static readonly GoToTheMine instance = new GoToTheMine();

	public static GoToTheMine Instance
	{
		get { return instance; }
	}

	static GoToTheMine()
	{
		
	}

	private GoToTheMine()
	{
		
	}

	public override void Enter(Miner entity)
	{
		entity.CurrentTarget = entity.GoldMine.position;
		Debug.Log("Go to the gold mine...");
	}

	public override void Execute(Miner entity)
	{
		bool goToTheTarget = entity.GoToTheTarget(entity.CurrentTarget);

		if (goToTheTarget)
		{
			entity.ChangeState(EnterMineAndDigForNugget.Instance);
		}
	}

	public override void Exit(Miner entity)
	{
		//throw new System.NotImplementedException();

		if (entity.Location != Locations.Goldmine)
		{
			Debug.Log("Entering the mine...");
			entity.ChangeLocation(Locations.Goldmine);
		}
	}
}
