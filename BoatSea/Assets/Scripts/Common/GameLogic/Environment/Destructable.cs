using System.Collections.Generic;
using Pathfinding;
using UnityEngine;
using System.Collections;

public class Destructable : MonoBehaviour {

	#region Variables

	[SerializeField] private GameObject Explosion;
	[SerializeField] private GameObject ModelHolder;
	public int HitPoint;

	[SerializeField] private EffectsBehaviour.EffectsType effects;

	public delegate void OnBuildingDestruction(Vector3 position);

	public static event OnBuildingDestruction OnBuildingDestructionEvent;

	private Vector3 explosionPosition;

	private EffectsBehaviour _hitEffect;

	#endregion


	#region MonoBehaviour events
	// Use this for initialization
	void Start ()
	{
		IsApplicationQuit = false;
		explosionPosition = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {

	}

	private void OnTriggerEnter(Collider other)
	{
	
	}

	private bool IsApplicationQuit;

	void OnDestroy()
	{
		if (IsApplicationQuit)
			return;

	    ShowExplosionEffect();
	}

	void OnApplicationQuit()
	{
		IsApplicationQuit = true;
	}
	#endregion

	#region Common events

	public void Hit(int damage = 1, Vector3 hitPosition = default (Vector3))
	{
		HitPoint -= damage;

		explosionPosition = hitPosition != default (Vector3) ? hitPosition : ModelHolder.transform.position;

		if (HitPoint <= 0)
		{

			if (OnBuildingDestructionEvent != null && ModelHolder != null)
			{
				//TODO: need change map logic in big map
				OnBuildingDestructionEvent(ModelHolder.transform.position);
			}
			UpdateGraphNode();

			ShowExplosionEffect();
			
			ShowHitEffect();
			Destroy(gameObject);
		}
		else
		{
		    ShowExplosionEffect();

		    ShowHitEffect();
		}
	}

    private void ShowExplosionEffect()
    {
        if (ModelHolder != null)
        {
            EffectsBehaviour explosion =
                ResourceBehaviourController.Instance.GetEffectsFromPool(EffectsBehaviour.EffectsType.HitExplosion);

            if (explosion != null)
            {
                explosion.SetBasicData(explosionPosition - new Vector3(0, 0, 3));
                SoundController.PlaySBuildingExplosion(explosion.gameObject);
            }
        }
    }

    private void ShowHitEffect()
	{
		if (_hitEffect == null)
			_hitEffect = ResourceBehaviourController.Instance.GetEffectsFromPool(EffectsBehaviour.EffectsType.Damage);

		if (_hitEffect != null)
			_hitEffect.SetBasicData(transform);
	}

	private void RemoveHitEffect()
	{
		if (_hitEffect == null)
			return;

		_hitEffect.Remove();
		_hitEffect = null;
	}

	private void UpdateGraphNode()
	{
		Collider collider = GetComponentInChildren<Collider>();

		if (null == collider)
		{
			throw new MissingComponentException("Destructable.UpdateGraphNode - cann't find Collider component");
		}

		Bounds b = collider.bounds;

		GraphUpdateObject guo = new GraphUpdateObject(b);

		AstarPath.active.UpdateGraphs(guo, 0.0f);

		if (AstarPath.active.graphUpdateQueue == null) {
			AstarPath.active.graphUpdateQueue = new Queue<GraphUpdateObject> ();
		}

		AstarPath.active.QueueGraphUpdates();
	}

	#endregion

}
