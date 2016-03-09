using UnityEngine;
using System.Collections;

public class BonusesRespounManager : MonoBehaviour {

	public Transform [] SpounPoints;
	[SerializeField]
	private float respounTime;
	[SerializeField]
	private float respounTimeCurrent;


	// Use this for initialization
	void Start () {
		respounTime = 60;
	}

	void InitBonus()
	{
		if (ResourceBehaviourController.Instance == null)
			return;

		BonusBehavior bonus = ResourceBehaviourController.Instance.GetBonus ();

		int random = Random.Range (0,SpounPoints.Length-1);

		Vector3 _position = SpounPoints [random].position;


		BonusesType bonusT = new BonusesType();
		int bonusRandom = Random.Range (0,6);
		switch (bonusRandom) {
		case 0:
			bonusT = BonusesType.Health;
			break;
		case 1:
			bonusT = BonusesType.Immortal;
			break;
		case 2:
			bonusT = BonusesType.Invisible;
			break;
		case 3:
			bonusT = BonusesType.Shield;
			break;
		case 4:
			bonusT = BonusesType.TwoShoot;
			break;
		case 5:
			bonusT = BonusesType.TwoSpeed;
			break;
		case 6:
			bonusT = BonusesType.Destroyer;
			break;

		}

		bonus.SetBasicData (bonusT,_position);


	}

	// Update is called once per frame
	void Update () {
	if (respounTimeCurrent > 0) {
			respounTimeCurrent -= Time.deltaTime;
		} else {
			InitBonus();
		respounTimeCurrent = respounTime;
		}
	}
}
