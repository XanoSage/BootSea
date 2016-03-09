using UnityEngine;
using System.Collections;

public class LocalizationText : MonoBehaviour {
	public string key;
	// Use this for initialization
	void Start () {
		if (key == "") {
						key = transform.GetComponent<UILabel> ().text;
				}
	
			transform.GetComponent<UILabel> ().text = LocalizationConfig.getText(key);
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
