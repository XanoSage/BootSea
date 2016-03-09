using UnityEngine;
using System.Collections;

public class CannonLenearController : CannonBasic {




	// Use this for initialization
	void Start () {
		base.Start ();
	}



 	

	// Update is called once per frame
	public override void Update () {
		if (ShootCurTime > 0) {
			ShootCurTime-=Time.deltaTime;
		}

		//Raycast Checker
		for (int i=0; i<ShootPos.Length; i++) {
			Vector3 forward = ShootPos [i].transform.TransformDirection (Vector3.forward);
			RaycastHit hit;
			Ray ray = new Ray (ShootPos [i].position, forward);
			if (Physics.Raycast (ray, out(hit), ShootDistance)) {
				for (int t=0; i<TargetShips.Count; i++) 
				{
					if(hit.collider.gameObject.layer ==12)
					{
						if (hit.collider.transform.parent.parent.gameObject == TargetShips[t].gameObject)
						{
							Shoot ();
							break;
						}
					}
				}
			}
			Debug.DrawRay (ShootPos [i].position, ShootPos [i].transform.TransformDirection (Vector3.forward) * ShootDistance);
		}

	}
}
