using UnityEngine;
using UnityEditor;
using System.Collections;

public class ModelObject : MonoBehaviour 
{
	[MenuItem ("CreateObjects/Create")]
	
	static void CreateGameObject () 
	{
		GameObject SetModelName = new GameObject("SetName");
		GameObject ModelHolder = new GameObject("ModelHolder");
		GameObject Colliders = new GameObject("Colliders");
		GameObject Colloder = new GameObject("Colloder01");
		
		Colloder.transform.parent=Colliders.transform;
		ModelHolder.transform.parent=SetModelName.transform;
		Colliders.transform.parent= SetModelName.transform;
		Colloder.AddComponent("BoxCollider");
		Debug.Log ("Doing Something...");
	}
	
	
}
