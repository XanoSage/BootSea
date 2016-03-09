using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class camerafacing : MonoBehaviour
{
	public Transform camera;

	void Update () {
		transform.LookAt(new Vector3(camera.transform.position.x,transform.position.y,camera.transform.position.z));
	}
}
