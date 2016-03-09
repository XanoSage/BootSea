using UnityEngine;
using System.Collections;

public class GoToTheBank : FSMState<Miner>
{

	static readonly GoToTheBank instance = new GoToTheBank();

	public static GoToTheBank Instance
	{
		get { return instance; }
	}

	public override void Enter(Miner entity)
	{
		entity.CurrentTarget = entity.Bank.position;
		Debug.Log("Go to the bank...");
	}

	public override void Execute(Miner entity)
	{
		bool goToTheTarget = entity.GoToTheTarget(entity.CurrentTarget);

		if (goToTheTarget)
		{
			entity.ChangeState(VisitBankAndDepositGold.Instance);
		}
	}

	public override void Exit(Miner entity)
	{
		//throw new System.NotImplementedException();
		if (entity.Location != Locations.Bank)
		{
			Debug.Log("Entering the bank...");
			entity.ChangeLocation(Locations.Bank);
		}
	}
}
