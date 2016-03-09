using UnityEngine;
using System.Collections;

public class CannonRotationController : CannonBasic {




	// Use this for initialization
	void Start () {

		base.Start ();
	}

	// Update is called once per frame
	void Update () {
		//поворачиваем пушку
		if (CurrTarget) {
			Quaternion rotation = Quaternion.LookRotation(CurrTarget.transform.position-transform.position,Vector3.back);
			rotation.x = 0;
			rotation.z = 0;
			transform.rotation = Quaternion.Slerp(transform.rotation,rotation,Time.deltaTime*RotationSpeed);
		}


		base.Update ();
	}
}
