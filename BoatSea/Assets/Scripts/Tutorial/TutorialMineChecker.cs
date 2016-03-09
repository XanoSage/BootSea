using UnityEngine;
using System.Collections;

public class TutorialMineChecker : MonoBehaviour {
	[SerializeField]
	private TutorialController _tutorial;

	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider obj)
	{
		
		if(obj.transform.parent.transform.parent.name == "Base_Mine_Blue(Clone)(Clone)"||obj.transform.parent.transform.parent.name == "Base_Mine_Blue(Clone)")
		{
			StartCoroutine("Hide");
		}
	}

	IEnumerator Hide()
	{
		yield return new WaitForSeconds (0.1f);
		_tutorial.StepComplet();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
