using UnityEngine;
using System.Collections;

public class BonusBehavior : PoolItem {

	public BonusesType Type;

	private float _timeToLive;

	[SerializeField]
	float rotateSpeed = 1;

	[SerializeField]
	Vector3 rotation = new Vector3();

	[SerializeField]
	private GameObject [] _typeObjects;

	void Start()
	{
		Type =  BonusesType.Immortal;
		_typeObjects [(int)Type].SetActive (true);
	}

	public void SetBasicData(BonusesType _type,Vector3 _position)
	{
		Type = _type;
		_typeObjects [(int)Type].SetActive (true);
		transform.position = _position;
		StartCoroutine ("Hide");
	}




	// Update is called once per frame
	void Update () {
		rotation.x = rotateSpeed;
		transform.Rotate (rotation);
	}


	IEnumerator Hide()
	{
		for(int i =0;i<30;i++)
		{
			if(i==29)
			{
				gameObject.SetActive(false);
				StopCoroutine("Hide");
			}
			yield return new WaitForSeconds(1);;
		}
	}


	// Activating happens on removing object from pool.
	public override void Activate() {
		base.Activate();
		gameObject.SetActive(true);
	}
	
	// Deactivating happens on pushing object to pool.
	public override void Deactivate() {
		base.Deactivate();
		gameObject.SetActive(false);
	}
	
	
	public override bool EqualsTo(PoolItem item)
	{
		return true;
	}
}
