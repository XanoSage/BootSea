using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using System.Collections;

public class SpawnPointBehaviour: PoolItem {

	#region Variables

	public TeamColor Color;

	#endregion

	#region MonoBehaviour events

	// Use this for initialization
	private void Start () {


	}

	// Update is called once per frame
	private void Update () {

	}

	private void OnTriggerEnter (Collider other) {
		if (!GameSetObserver.Instance.IsBattleStarted || GameSetObserver.Instance.IsPause)
			return;

		ShipBehaviour ship = null;
		if (other.transform.parent != null) 
			if (other.transform.parent.parent != null)
				ship = other.transform.parent.parent.GetComponent<ShipBehaviour>();

		if (ship != null) {
			if (ship.Player.Team == Color && ship.Player.MyShip.IsNeedMineReset) {
				ship.Player.MyShip.OnMineReset();
				BattleController.Instance.UpdateBombCount(ship.Player);

				Debug.Log(string.Format("Reset bomb counter, color {0}", Color));
			}
		}
	}

	#endregion


	#region Ovverride methods

	public override bool EqualsTo (PoolItem item) {
		if (!(item is SpawnPointBehaviour))
			return false;

		SpawnPointBehaviour spawnPoint = item as SpawnPointBehaviour;

		if (spawnPoint.Color != Color)
			return false;

		return true;
	}

	public override void Activate () {
		base.Activate();

		gameObject.SetActive(true);


	}

	public override void Deactivate () {
		base.Deactivate();

		gameObject.SetActive(false);
	}

	#endregion
}
