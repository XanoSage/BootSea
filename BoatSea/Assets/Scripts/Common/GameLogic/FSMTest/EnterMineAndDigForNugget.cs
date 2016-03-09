using UnityEngine;
using System.Collections;

public class EnterMineAndDigForNugget : FSMState<Miner>
{

	static readonly EnterMineAndDigForNugget instance = new EnterMineAndDigForNugget();

	public static EnterMineAndDigForNugget Instance
	{
		get { return instance; }
	}

	static EnterMineAndDigForNugget()
	{
		
	}

	private EnterMineAndDigForNugget()
	{
		
	}

	public override void Enter(Miner entity)
	{
		//if (entity.Location != Locations.Goldmine)
		//{
			Debug.Log("Start dig gold...");
		//	entity.ChangeLocation(Locations.Goldmine);
		//}
	}

	public override void Execute(Miner entity)
	{
		entity.AddToGoldCarried(1);
		Debug.Log("Picking ap nugget and that's " + entity.GoldCarried);

		entity.IncreaseFatigue();

		if (entity.PocketsFull())
		{
			entity.ChangeState(GoToTheBank.Instance);
		}
	}

	public override void Exit(Miner entity)
	{
		Debug.Log("Leaving the mine with my pockets full...");
	}
}
