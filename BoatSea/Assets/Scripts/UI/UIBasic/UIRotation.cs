using UnityEngine;
using System.Collections;

public class UIRotation : MonoBehaviour {

	public Vector3 rotationAmount=new Vector3(0,0,1f);
	//public Quaternion rotAmount;
	Transform cachedTransform;
	void Start(){
		StartCoroutine(PermanentRotation());
		//cachedTransform=transform;
	}
	IEnumerator PermanentRotation()
	{
		while(true){
			transform.Rotate(rotationAmount*Time.deltaTime);
			//cachedTransform.localRotation+=rotAmount;//*Time.deltaTime;
			yield return null;
		}
	}
}
