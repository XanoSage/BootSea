using UnityEngine;
using System.Collections;

public class FiniteStateMachine <T>
{
	private T Owner;
	private FSMState<T> CurrentState;
	private FSMState<T> PreviousState;
	private FSMState<T> GlobalState;
	private FSMState<T> MainState;

	public FSMState<T>  Current
	{
		get { return CurrentState; }
	}

	public void Awake()
	{
		CurrentState = null;
		PreviousState = null;
		GlobalState = null;
        
	}

	public void Configure(T owner, FSMState<T> InitialState)
	{
		Owner = owner;
		Configure(InitialState);
	}

	public void Configure(FSMState<T> InitialState)
	{
		MainState = InitialState;
		ChangeState(InitialState);
	}

	public void Update()
	{
		if (GlobalState != null)
			GlobalState.Execute(Owner);

		if (CurrentState != null)
			CurrentState.Execute(Owner);
	}

	public void ChangeState(FSMState<T> NewState)
	{
		PreviousState = CurrentState;
		if (CurrentState != null)
			CurrentState.Exit(Owner);

		CurrentState = NewState;

		if (CurrentState != null)
			CurrentState.Enter(Owner);
	}

	public void RevertToPreviousState()
	{
		if (PreviousState != null)
			ChangeState(PreviousState);
	}

	public void RevertToMainState()
	{
		if (CurrentState != null)
		{
			ChangeState(MainState);
		}
	}
}
