using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections;

public class PathfindHelper : MonoBehaviour {

	#region Variables

	public MechanicsType Mechanics;

	private string _pathfindFremovePath = "/Resources/MachineCityFreemove.zip";
	private string _pathfindClassicPath = "/Resources/MachineCityClassic.zip";

	#endregion

	// Use this for initialization
	void Start ()
	{

		Debug.Log(Application.dataPath);


		AstarPath astarPath = FindObjectOfType<AstarPath>();

		if (null == astarPath)
		{
			throw new MissingComponentException("PathfindHelper.Start - cann't find AstarPath game object");
		}

		//AstarPath.active;

		LoadGraphFromFile(Player.Mechanics);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void LoadGraphFromFile(MechanicsType mechanics)
	{
		byte[] bytes;

		string path = Application.dataPath +
		              (mechanics == MechanicsType.Classic ? _pathfindClassicPath : _pathfindFremovePath);

		try
		{
			bytes = Pathfinding.Serialization.AstarSerializer.LoadFromFile(path);
		}
		catch (System.Exception e)
		{
			Debug.LogError("Could not load from file at '" + path + "'\n" + e);
			bytes = null;
		}

		if (bytes != null) AstarPath.active.astarData.DeserializeGraphs(bytes);
	}
}
