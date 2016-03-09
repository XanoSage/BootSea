using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections;

public class ShipTest : MonoBehaviour {

	private Vector3 direction = Vector3.zero;

	// Use this for initialization
	void Start () {
	
	}

	// Update is called once per frame
	private void Update () {
	
		if (direction != Vector3.zero)
			transform.position += direction * Time.deltaTime * 6.25f;

	}

	private void FixedUpdate () {
	
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
			direction = Vector3.down;
		}
		else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
			direction = Vector3.up;
		}
		else if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
			direction = Vector3.left;
		}
		else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
			direction = Vector3.right;
		}
		else if (Input.GetKey(KeyCode.Q)) {
			direction = Vector3.forward;
		}
		else if (Input.GetKey(KeyCode.E)) {
			direction = Vector3.back;
		}

		else {
			direction = Vector3.zero;
		}

		if (Input.GetKey(KeyCode.X)) {
			gameObject.rigidbody.AddForce(Vector3.zero);
		}

	}
}
