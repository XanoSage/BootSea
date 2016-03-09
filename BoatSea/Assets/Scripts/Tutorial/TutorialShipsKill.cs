using UnityEngine;
using System.Collections;

public class TutorialShipsKill : MonoBehaviour {
	private int currWaipoints;
	[SerializeField]
	private GameObject _explosive;

	bool isDead = false;

	// Use this for initialization
	void Start () {
	
	}
	void OnTriggerEnter(Collider obj)
	{
		
		if(obj.transform.parent.transform.parent.name == "Base_Bullet_Blue(Clone)(Clone)"||obj.transform.parent.transform.parent.name == "Base_Bullet_Blue(Clone)")
		{
			isDead = true;
		//	_tutorial.StepComplet();
			for(int c =0;c<transform.childCount;c++){
				transform.GetChild(c).gameObject.SetActive(false);
			}
			_explosive.SetActive(true);
			StartCoroutine("Hide");
			Debug.Log("Bullet Check");
		}
	}

	IEnumerator Hide()
	{
		yield return new WaitForSeconds (3f);
		//_tutorial.StepComplet();
		gameObject.SetActive (false);
		
	}
	// Update is called once per frame
	void Update () {
		if (isDead) {
			Vector3 pos = transform.position;
			pos.y -= 0.01f;
			transform.position = pos;
		}
		}
}
