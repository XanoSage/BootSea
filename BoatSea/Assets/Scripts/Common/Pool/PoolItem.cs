using UnityEngine;
using System.Collections;

public abstract class PoolItem : MonoBehaviour {
	
	// True if objects is deactivated and in pool.
	public bool IsInPool { get; private set; }	
	
	// Activating happens on removing object from pool.
	public virtual void Activate() {
		IsInPool = false;
	}
	
	// Deactivating happens on pushing object to pool.
	public virtual void Deactivate() {
		IsInPool = true;
	}

	// Items can have their specific ways to recognize equal ones.
	public abstract bool EqualsTo(PoolItem item);	
}

/*
	#region PoolItem members
	// Activating happens on removing object from pool.
	public override void Activate() {
		base.Activate();
		transform.parent = null;
		gameObject.SetActive(true);
	}
	
	// Deactivating happens on pushing object to pool.
	public override void Deactivate() {
		base.Deactivate();
		transform.parent = Pool.pooledItemsParent;
		gameObject.SetActive(false);
	}
	
	// Items can have their specific ways to recognize equal ones.
	public override bool EqualsTo(PoolItem item) {
		return item is TerrainPart;
	}
	#endregion
 */
