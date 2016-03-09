using UnityEngine;

public class TubeSmoke : MonoBehaviour
{
	[SerializeField] Transform tubeSmoke;

	void Start ()
	{
		((Transform)Instantiate(tubeSmoke, transform.position + new Vector3(0, 1.300141f, 0), Quaternion.identity)).parent = transform;
	}
}
