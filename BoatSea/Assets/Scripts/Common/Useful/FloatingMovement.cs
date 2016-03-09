using UnityEngine;
using Aratog.NavyFight.Models.Unity3D.Extensions;

public class FloatingMovement : MonoBehaviour
{
    enum Axis { x, y, z };

    [SerializeField]
    Axis MovingAxis;

    [SerializeField]
    float WaveHeight = 0.0f;
    [SerializeField]
    float WaveLength = 1.0f;

    [SerializeField]
    float HeightAmplitude = 1.0f;		// this one is a wave amplitude
    [SerializeField]
    float PhaseDelay = 0.0f;			// for using on multiple objects and de-sync

    [SerializeField]
    Axis RotationAxis;

    [SerializeField]
    float RotationAmplitude = 30.0f;	// that one is the same, but for rotation
    [SerializeField]
    float RotationPhaseDelay = 0.0f;	// and here we control rotation delay

    float startHeight = 0.0f;

    Transform cachedTransform;

    [SerializeField]
    bool RandomRotation;

    void Start()
    {
        if (RandomRotation)
        {
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + Random.Range(-90, 90), transform.eulerAngles.z);
        }
        cachedTransform = transform;
        startHeight = ((MovingAxis == Axis.x) ? cachedTransform.position.x : ((MovingAxis == Axis.y) ? cachedTransform.position.y : cachedTransform.position.z));

		Destroy(this);
    }

    /*void Set(Vector3 v, float? x = null, float? y = null, float? z = null)
    {
        v.position = new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
    }*/

    void Update()
    {
		return;
        WaveLength = Mathf.Max(WaveLength, 0.1f);
        float _base_wave_phase = Time.time * Mathf.PI * 2 / WaveLength;
        float _movingT = startHeight + WaveHeight + HeightAmplitude * Mathf.Sin(_base_wave_phase + PhaseDelay * Mathf.Deg2Rad);
        switch (MovingAxis)
        {
            case Axis.x:
                cachedTransform.SetPos(x: _movingT);
                break;
            case Axis.y:
                cachedTransform.SetPos(y: _movingT);
                break;
            case Axis.z:
                cachedTransform.SetPos(z: _movingT);
                break;
        }
        float _rotationT = RotationAmplitude * Mathf.Cos(_base_wave_phase + RotationPhaseDelay * Mathf.Deg2Rad);
        switch (MovingAxis)
        {
            case Axis.x:
                cachedTransform.eulerAngles = cachedTransform.eulerAngles.Set(x: _rotationT);
                break;
            case Axis.y:
                cachedTransform.eulerAngles = cachedTransform.eulerAngles.Set(y: _rotationT);
                break;
            case Axis.z:
                cachedTransform.eulerAngles = cachedTransform.eulerAngles.Set(z: _rotationT);
                break;
        }

        //transform.position = new Vector3(transform.position.x, transform.position.y, startHeight + WaveHeight + HeightAmplitude *Mathf.Sin(_base_wave_phase + PhaseDelay *Mathf.Deg2Rad));
        //cachedTransform.eulerAngles = new Vector3(0, 0, RotationAmplitude *Mathf.Cos(_base_wave_phase + RotationPhaseDelay *Mathf.Deg2Rad));
    }


}