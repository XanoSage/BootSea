using UnityEngine;
using System.Collections;

public class PlaneHelper : MonoBehaviour {
	#region Variables

	public Collider PlaneCollider { get; private set; }
	
	#endregion


	#region MonoBehaviour Action
	
	// Use this for initialization
	void Start ()
	{

		PlaneCollider = GetComponent<Collider>();

		Init();
	}
	
	// Update is called once per frame
	void Update () {

	}

	private void Init()
	{
		ShipMovement[] shipMovements = FindObjectsOfType<ShipMovement>();

		foreach (ShipMovement shipMovement in shipMovements)
		{
			if (!shipMovement.IsPlaneIgnore)
			{
				shipMovement.IgnorePlaneCollider(this);
			}
		}
	}
	#endregion

}
