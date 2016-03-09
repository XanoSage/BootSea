using UnityEngine;
using System.Collections;

public class TutorialBulletStep : MonoBehaviour {
	[SerializeField]
	private TutorialController _tutorial;
	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider obj)
	{

		if(obj.transform.parent.transform.parent.name == "Base_Bullet_Blue(Clone)(Clone)"||obj.transform.parent.transform.parent.name == "Base_Bullet_Blue(Clone)")
		{

			_tutorial.StepComplet();
			StartCoroutine("Hide");
			Debug.Log("Bullet Check");
		}
		Debug.Log (obj.transform.parent.transform.parent.name);
	}

	
	IEnumerator Hide()
	{
		yield return new WaitForSeconds (0.1f);
		_tutorial.StepComplet();		
	}
}
