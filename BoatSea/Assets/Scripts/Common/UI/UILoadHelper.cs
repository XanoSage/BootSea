using UnityEngine;
using System.Collections;

public class UILoadHelper : MonoBehaviour
{
	private UILoadingScreen _loadingScreen;
	// Use this for initialization
	void Start ()
	{
		_loadingScreen = FindObjectOfType<UILoadingScreen>();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (_loadingScreen != null && _loadingScreen.Visible)
		{
			_loadingScreen.Hide();
		}

	}
}
