using UnityEngine;

public class UIClickSimulation : MonoBehaviour
{

	void Start()
	{
		Invoke("Click", 0.01f);
	}
	
	void Click()
	{
		gameObject.SendMessage("OnClick");
	}
}
