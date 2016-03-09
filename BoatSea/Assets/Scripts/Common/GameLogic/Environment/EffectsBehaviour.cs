using System;
using UnityEngine;
using System.Collections;

public class EffectsBehaviour : PoolItem {

	#region Constants

	public const string GroundExplosionPrefabPath = "Prefabs/Effects/CFXM2_GroundRockHit";

	public const string DamagePrefab = "Prefabs/Effects/Damage";
	public const string HitExplosionPrefab = "Prefabs/Effects/Explosion";
	public const string ShipExplosionPrefab = "Prefabs/Effects/ShipExplosion";
	public const string SparksBluePrefab = "Prefabs/Effects/SparksHit Blue";
	public const string SparksRedPrefab = "Prefabs/Effects/SparksHit Red";
	public const string SplashForBombPrefab = "Prefabs/Effects/Splash for Bomb";
	public const string SplashForProjectilePrefab = "Prefabs/Effects/Splash";
	public const string BuildingEplosionPrefab = "Prefabs/Effects/WWExplosion";
	public const string SmokePrefab = "Prefabs/Effects/Smoke";

	public const string MortalPrefab = "Prefabs/Effects/Mortar_Explosion";

	public const string FlagBluePrefab = "Prefabs/Effects/Flag_Blue";
	public const string FlagRedPrefab = "Prefabs/Effects/Flag_Red";

	#endregion

	private bool isParticlePlayed = false;

	public enum EffectsType
	{
		BasicExplosion,
		ExplosionWithSmoke,
		SparkSplash,
		WaterSplashProjectile,
		WaterSplashBomb,
		GroundExplosion,

		MortarExplosive,

		Damage,
		HitExplosion,
		ShipExplosion,
		FlagBlue,
		FlagRed,
		SparksBlue,
		SparksRed,
		Smoke,
		SplashForBomb,
		SplashForProjectile,
		BuildingExplosion,
	}

	public EffectsType Effects;


	private float particleTime;
	private float particleTimeCount;

	private ParticleSystem _particleSystem;

	public bool IsParticleSystemAlive
	{
		get { return _particleSystem != null && _particleSystem.IsAlive(true); }
	}

	private bool _isEffectFxInit;

	private Transform _parentTransform;

	#region MonoBehaviour functions

	// Use this for initialization
	void Start ()
	{

		_particleSystem = GetComponent<ParticleSystem>();
		if (_particleSystem == null)
			return;

		isParticlePlayed = false;
	}
	
	// Update is called once per frame
	void Update () {

		
	}

	void FixedUpdate()
	{
		if (_particleSystem == null || _particleSystem.transform == null )
			return;

		if (!_isEffectFxInit)
		{
			Pool.Push(this);
			return;
		}

		if (_particleSystem.loop && _particleSystem.isPlaying && _parentTransform != null)
		{
			_particleSystem.transform.position = _parentTransform.position;
			return;
		}

		if (_particleSystem.loop && _particleSystem.isStopped)
		{
			Remove();
		}
		
		if (!_particleSystem.IsAlive(true) || Math.Abs(_particleSystem.time - _particleSystem.duration) < 0.1f)
		{
			Remove();
		}
	}

	void OnDestroy()
	{

	}

	#endregion

	#region EffectsBehaviour functions

	
	/// <summary>
	/// Set basic data for particle that will be played one time
	/// </summary>
	/// <param name="pos">Position of emmitter when particle will be played</param>
	public void SetBasicData(Vector3 pos)
	{
		if (_particleSystem == null)
			_particleSystem = GetComponent<ParticleSystem>();

		if (_particleSystem == null)
		{
			Debug.LogError("particle system not initialized");
			return;
		}
		
		transform.position = pos;

		particleTime = _particleSystem.duration;

		particleTimeCount = 0;

		isParticlePlayed = true;

		_isEffectFxInit = true;

		_particleSystem.time = 0;

		_particleSystem.Play(true);
		
	}

	/// <summary>
	/// Set basic particle data that will be played timeless
	/// </summary>
	/// <param name="parent"></param>
	public void SetBasicData(Transform parent)
	{
		if (_particleSystem == null)
			_particleSystem = GetComponent<ParticleSystem>();

		if (_particleSystem == null)
		{
			Debug.LogError("particle system not initialized");
			return;
		}

		if (_particleSystem)

		_parentTransform = parent;

		_isEffectFxInit = true;

		_particleSystem.Play(true);
	}

	public void Remove()
	{
		_particleSystem.Stop(true);
		_particleSystem.Clear(true);
		_particleSystem.Stop(true);
		_parentTransform = null;
		_isEffectFxInit = false;

		ResourceBehaviourController.Instance.RemoveEffect(this);

		Pool.Push(this);
	}

	#endregion


	#region Ovveride function

	public override bool EqualsTo(PoolItem item)
	{
		if (!(item is EffectsBehaviour))
			return false;

		EffectsBehaviour explosion = item as EffectsBehaviour;

		return explosion.Effects == Effects;
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
