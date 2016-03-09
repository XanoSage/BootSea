using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections;

public class VisitBankAndDepositGold : FSMState<Miner>
{

	static readonly VisitBankAndDepositGold instance = new VisitBankAndDepositGold();

	public static VisitBankAndDepositGold Instance
	{
		get { return instance; }
	}

	static VisitBankAndDepositGold()
	{
		
	}

	private VisitBankAndDepositGold()
	{
		
	}

	public override void Enter(Miner entity)
	{
		//if (entity.Location != Locations.Bank)
		//{
			Debug.Log("Deposit the gold...");
		//	entity.ChangeLocation(Locations.Bank);
		//}
	}

	public override void Execute(Miner entity)
	{
		Debug.Log("Feeding The System with My Gold... " + entity.MoneyInBank);
		entity.AddToMoneyInBank(entity.GoldCarried);
		entity.ChangeState(GoToTheMine.Instance);
	}

	public override void Exit(Miner entity)
	{
		Debug.Log("Leaving the bank...");
	}
}
