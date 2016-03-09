using System.Runtime.InteropServices;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Players;
using Pathfinding;
using UnityEngine;
using System.Collections;

public class AIShipBehaviourHelper : MonoBehaviour {

	#region Variables

	private AIPlayer _aiPlayer;

	private Seeker _seeker;

	#endregion

	#region MonoBehaviour actions


	// Use this for initialization
	void Start ()
	{
		_seeker = GetComponent<Seeker>();

		if (null == _seeker)
		{
			throw new MissingComponentException("Cannot find");
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	#endregion

	#region Actions

	public void InitAiPlayer(AIPlayer aiPlayer)
	{
		if (null == aiPlayer)
			return;

		_aiPlayer = aiPlayer;

		_aiPlayer.FindPathEvent += OnFindPath;

		if (Player.Mechanics == MechanicsType.NewWave)
		{
			Pathfinding.SimpleSmoothModifier modifier = gameObject.AddComponent<Pathfinding.SimpleSmoothModifier>();
			modifier.smoothType = SimpleSmoothModifier.SmoothType.Simple;
		}

		Debug.Log("AIShipBehaviourHelper.InitAiPlayer - OK");
	}

	private void OnFindPath(Vector3 startPos, Vector3 targetPos, AIPlayer.OnPathFindDelegate onPathFindDelegate)
	{

		//Debug.Log(string.Format("AIShipBehaviourHelper.OnFindPath - OK. Player : {0}-{1}, startPos: {2}, targetPos: {3}",
		//						_aiPlayer.Id, _aiPlayer.Team, startPos, targetPos));

		_seeker.StartPath(startPos, targetPos, path =>
			{
				if (!path.error)
				{
					onPathFindDelegate(path.vectorPath);
				}
			});
	}
	
	#endregion
}
