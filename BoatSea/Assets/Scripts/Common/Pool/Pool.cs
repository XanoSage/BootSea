using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Pool : MonoBehaviour {
		
	public static Pool instance;
	private static List<PoolItem> items;	
	private static List<PoolItem>_shipItems;
	private static List<PoolItem>_bonusItems;
	public static Transform pooledItemsParent;
	public static bool IsMultiplayer;
	
	#region Initialization
	[System.Serializable]
	public class ItemCountPair {
	    public PoolItem itemPrefab;
	    public int count;
	}
	public List<ItemCountPair> startItemsDescription;
	
	void Awake () {
		instance = this;
		InitFirst();
	}

	public void InitFirst() {
		pooledItemsParent = transform;

		items = new List<PoolItem>();

		_shipItems = new List<PoolItem> ();
		_bonusItems = new List<PoolItem> ();

		foreach (ItemCountPair pair in startItemsDescription) {
			for (int i=0;i<pair.count;i++)
				InstantiateItem(pair.itemPrefab);
		}
	}
	

	public static void AddMassive(PoolItem item, int count)
	{
		for (int i = 0; i < count; i++)
		{
			InstantiateItem(item);
		}
	}
	#endregion


	private static int GetShipObjectIndex(PoolItem itemPrefab) {

		//TODO: Need repair sherch logic!!!!!!
		for (int i = 0; i < _shipItems.Count; i++) {
			
			if (_shipItems[i].EqualsTo(itemPrefab)) {
				Debug.Log ("return from Pool "+_shipItems[i].name);
				return i;
			}	
		}
		InstantiateShipItem(itemPrefab);
		//		Debug.LogWarning(string.Format("Not enough {0} in pool, instantiate used", itemPrefab.ToString()));
		return _shipItems.Count-1;
	}

	private static int GetBonusObjectIndex(PoolItem itemPrefab) {
			for (int i = 0; i < _bonusItems.Count; i++) {
			
			if (_bonusItems[i].EqualsTo(itemPrefab)) {
				Debug.Log ("return from Pool "+_bonusItems[i].name);
				return i;
			}	
		}
		PoolItem newItem = null;	
		newItem = Instantiate(itemPrefab) as PoolItem;
		if (newItem != null)
			Pool.PushBonus(newItem);			

		return _bonusItems.Count-1;
	}


	private static int GetObjectIndex(PoolItem itemPrefab) {


		for (int i = 0; i < items.Count; i++) {

			if (items[i].EqualsTo(itemPrefab)) {
			
				return i;
			}

		}
				
		InstantiateItem(itemPrefab);
//		Debug.LogWarning(string.Format("Not enough {0} in pool, instantiate used", itemPrefab.ToString()));
		return items.Count-1;
	}



	private static void InstantiateShipItem(PoolItem itemPrefab) {
		PoolItem newItem = null;
		
		//if (IsMultiplayer) {
		//	//Object someObject = PhotonNetwork.Instantiate(itemPrefab.name, Vector3.zero, Quaternion.identity, 0) as Object;
		//	//newItem = someObject as PoolItem;
		//}
		//else
		newItem = Instantiate(itemPrefab) as PoolItem;
		
		if (newItem != null)
			Pool.PushShip(newItem);
	}


	private static void InstantiateItem(PoolItem itemPrefab) {
		PoolItem newItem = null;

		//if (IsMultiplayer) {
		//	//Object someObject = PhotonNetwork.Instantiate(itemPrefab.name, Vector3.zero, Quaternion.identity, 0) as Object;
		//	//newItem = someObject as PoolItem;
		//}
		//else
			newItem = Instantiate(itemPrefab) as PoolItem;

		if (newItem != null)
		Pool.Push(newItem);
	}

	public static PoolItem ShipPop(PoolItem itemPrefab) {
		

		int index = GetShipObjectIndex(itemPrefab);
		
		if (index == -1) {
			Debug.LogError(string.Format("POP. No such object in pool: {0}", itemPrefab));
			return null;
		}
		
		PoolItem item = _shipItems[index];
		_shipItems.RemoveAt(index);
		item.Activate();	

		return item;
	}

	public static PoolItem BonusPop(PoolItem itemPrefab) {
		
		
		int index = GetBonusObjectIndex(itemPrefab);
		
		if (index == -1) {
			Debug.LogError(string.Format("POP. No such object in pool: {0}", itemPrefab));
			return null;
		}
		
		PoolItem item = _bonusItems[index];
		_bonusItems.RemoveAt(index);
		item.Activate();	
		
		return item;
	}


	public static PoolItem Pop(PoolItem itemPrefab) {


		int index = GetObjectIndex(itemPrefab);
		
		if (index == -1) {
			Debug.LogError(string.Format("POP. No such object in pool: {0}", itemPrefab));
			return null;
		}
		
		PoolItem item = items[index];
		items.RemoveAt(index);
		item.Activate();	

		return item;
	}
	public static void PushBonus(PoolItem item) {
		item.Deactivate();
		_bonusItems.Add(item);
	}

	public static void PushShip(PoolItem item) {
		item.Deactivate();
		_shipItems.Add(item);
	}

	public static void Push(PoolItem item) {
		item.Deactivate();
		items.Add(item);
	}

	public static void UnloadItem () {
		for (int i = 0; i != items.Count; i++) {
			Destroy(items[i].gameObject);
			items.RemoveAt(i);
			i--;
		}
	}
}
