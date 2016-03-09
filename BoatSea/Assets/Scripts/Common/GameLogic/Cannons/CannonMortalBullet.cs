using UnityEngine;
using System.Collections;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Ship;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Players;

public class CannonMortalBullet : MonoBehaviour {
	public GameObject explosive;
	
	public float speed;
	
	public int damage;
	
	public float lifeTime;
	private float currLifeTime;

	public Vector3 enemieTarget;

	public float gravity;

	public float firingAngle;

	// Use this for initialization
	void Start () {
		lifeTime = 3;
		firingAngle = 45;
		gravity = 9.8f;
		currLifeTime = lifeTime;
		speed = 5;
		damage = 1;
	}
	public void SetTarget(Vector3 pos)
	{
		enemieTarget = pos;
		StartCoroutine(SimulateProjectile());
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

	void Explosive()
	{
		EffectsBehaviour.EffectsType effectsType = EffectsBehaviour.EffectsType.HitExplosion;
			
		
		EffectsBehaviour fireFx =
			ResourceBehaviourController.Instance.GetEffectsFromPool(effectsType);
		
		if (fireFx != null)
		{
			fireFx.transform.position = transform.position;
			//	fireFx.transform.parent = transform;
			//	fireFx.transform.localPosition = Vector3.zero;
			//	fireFx.transform.localPosition = pos;
			
			fireFx.SetBasicData(fireFx.transform.position);
		//	SoundController.PlaySalvoShoot(fireFx.gameObject, ShipType.Boat, Player is HumanPlayer);
		}

		gameObject.SetActive(false);
		currLifeTime = lifeTime;
	}



	IEnumerator SimulateProjectile()
	{
		// Short delay added before Projectile is thrown
		yield return new WaitForSeconds(.1f);
		
	
		
		// Calculate distance to target
		float target_Distance = Vector3.Distance(transform.position, enemieTarget);
		
		// Calculate the velocity needed to throw the object to the target at specified angle.
		float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);
		
		// Extract the X  Y componenent of the velocity
		float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
		float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);
		
		// Calculate flight time.
		float flightDuration = target_Distance / Vx;
		
		// Rotate projectile to face the target.
		transform.rotation = Quaternion.LookRotation(enemieTarget - transform.position);
		
		float elapse_time = 0;
	
		while (elapse_time < flightDuration)
		{
			transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
		
			if(Vector3.Distance(transform.position,enemieTarget)<=4)
			{
			
				Explosive();
			}
			elapse_time += Time.deltaTime;

			yield return null;
		}
	}  

	// Update is called once per frame
	void Update () {
	



	


		Vector3 forward = transform.TransformDirection (Vector3.forward);
		RaycastHit hit;
		Ray ray = new Ray (transform.position, forward);
		if (Physics.Raycast (ray, out(hit), 0.5f)) {
			Debug.Log(hit.collider.name);
			
		}

		
		currLifeTime -= Time.deltaTime;
		if (currLifeTime <= 0) {
			Deactivate();
		}
	}
}
