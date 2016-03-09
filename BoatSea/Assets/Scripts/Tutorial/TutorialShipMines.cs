using UnityEngine;
using System.Collections;

public class TutorialShipMines : MonoBehaviour {
	[SerializeField]
	private TutorialController _tutorial;

	public Transform [] Waipoints;

	private int currWaipoints;
	[SerializeField]
	private GameObject _explosive;

	[SerializeField]
	private float _speed; 

	private bool _isDead = false;
	// Use this for initialization
	void Start () {
	
	}

	void OnTriggerEnter(Collider obj)
	{
		
		if(obj.transform.parent.transform.parent.name == "Base_Mine_Blue(Clone)(Clone)"||obj.transform.parent.transform.parent.name == "Base_Mine_Blue(Clone)")
		{
			
			_tutorial.StepComplet();
/*for(int c =0;c<transform.childCount;c++){
				transform.GetChild(c).gameObject.SetActive(false);
			}*/
			_explosive.SetActive(true);
			_isDead = true;
			StartCoroutine("Hide");
		//	gameObject.SetActive(false);
			Debug.Log("Bullet Check");
		}
		Debug.Log (obj.transform.parent.transform.parent.name);
	}

	IEnumerator Hide()
	{
		yield return new WaitForSeconds (3f);
		//_tutorial.StepComplet();
		gameObject.SetActive (false);
		
	}
	// Update is called once per frame
	void Update () {
	if (!_isDead) {
			transform.position = Vector3.MoveTowards (transform.position, Waipoints [currWaipoints].position, _speed);
			Vector3 angle = Waipoints [currWaipoints].position - transform.position;
			transform.rotation = Quaternion.LookRotation (angle);

			if (Vector3.Distance (transform.position, Waipoints [currWaipoints].position) < 1) {
				if (currWaipoints == 1) {
					currWaipoints = 0;
				} else {
					currWaipoints = 1;
				}
			}

		} else {
			Vector3 pos = transform.position;
			pos.y -= 0.01f;
			transform.position = pos ;
		}
	}
}
