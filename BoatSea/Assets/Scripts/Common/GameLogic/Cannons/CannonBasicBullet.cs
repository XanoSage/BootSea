using UnityEngine;
using System.Collections;

public class CannonBasicBullet : MonoBehaviour {

	public float speed;

	public int damage;

	public float lifeTime;
	private float currLifeTime;

	// Use this for initialization
	void Start () {
		currLifeTime = lifeTime;
		speed = 5;
		damage = 1;
	}

	private void OnTriggerEnter (Collider other) {

		if (other.gameObject.layer == 12) {
			ShipBehaviour _ship = other.transform.parent.parent.gameObject.GetComponent<ShipBehaviour> ();

			_ship.GunAddDamage(damage);
			Deactivate();
		} 
		else if (other.gameObject.CompareTag("Destructable"))
		{
			Destructable destructable = null;
			destructable = other.transform.parent.parent.GetComponent<Destructable>();
			
			if (destructable != null)
			{
				Vector3 correctPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 3);
			//	destructable.Hit(weaponShell.Damage, correctPos);
			//	BlowUp(false);
				Debug.Log("Destructable");
			}
			return;
		}
	}


	void OnTriggerStay(Collider c)
	{
		OnTriggerEnter(c);
	}



	void Deactivate()
	{
		gameObject.SetActive(false);
		currLifeTime = lifeTime;
	}

	// Update is called once per frame
	void Update () {
	//	transform.position = transform.position +Vector3.forward*speed;
		transform.Translate (Vector3.forward * Time.deltaTime * speed);

		Vector3 forward = transform.TransformDirection (Vector3.forward);
		RaycastHit hit;
		Ray ray = new Ray (transform.position, forward);
		if (Physics.Raycast (ray, out(hit), 0.5f)) {
			if (hit.collider.gameObject.layer == 12) {

			}

		}


		currLifeTime -= Time.deltaTime;
		if (currLifeTime <= 0) {
			Deactivate();
		}
	}
}
