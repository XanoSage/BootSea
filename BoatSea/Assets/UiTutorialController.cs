using UnityEngine;
using System.Collections;

public class UiTutorialController : MonoBehaviour {

	public GameObject [] UiSteps;

	// Use this for initialization
	void Start () {
		for(int i = 0;i<UiSteps.Length;i++)
		{
			UiSteps[i].SetActive(false);
		}
	}



	// Update is called once per frame
	void Update () {
	
	}
}
