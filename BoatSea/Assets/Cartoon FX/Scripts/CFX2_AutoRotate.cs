using UnityEngine;
using System.Collections;

public class CFX2_AutoRotate : MonoBehaviour
{
	public Vector3 speed = new Vector3(0,40f,0);
	
	// Use this for initialization
	void Start ()
	{
	}
	
	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(speed * Time.deltaTime);
	}
}
