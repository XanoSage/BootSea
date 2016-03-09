using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraFollowsShip : MonoBehaviour
{
	public static CameraFollowsShip Instance { private set; get; }

	//public Transform Target;
	[HideInInspector]
	public ShipBehaviour Target;

	private Rigidbody _rigidbody;

	public Vector3 _positionOffset = new Vector3(0, 0, 8);

    internal float
        FovFrom = 54 *1.29f,
			FovTo = 75 * 1.29f;

    const float
        ZoomInSpeed = 0.75f,
        ZoomOutSpeed = 0.5f,
        DelayAfterZoom = 0.2f;

    float CurrentZoomSpeed
    {
        get
        {
            return IsMoving ? ZoomInSpeed : ZoomOutSpeed;
        }
    }

	float _fovStartTime;

	Camera _camera;

	Transform _cameraTransform;

	Vector3 _cameraPosition;

	bool _lastIsMoving;
	bool IsMoving
	{
	    get
	    {
		    ShipMovement shipMovement = Target.GetComponent<ShipMovement>();
			
			// Определяем, двигается/поворачивается ли корабль
			bool result = shipMovement != null ? shipMovement.IsInMovement : Target.Player.IsInMovement;

	        // Определяем, изменилось ли состояние с предыдущего вызова; если да - стартуем анимацию
	        if (result != _lastIsMoving)
	        {
	            if (result == false && !IsInvoking("AfterZoomDelay"))
	            {
	                Invoke("AfterZoomDelay", DelayAfterZoom);
	            }
                //CancelInvoke("AfterZoomDelay");
                _fovStartTime = Time.time;

				
	        }

			//Debug.Log(string.Format("CameraFollowsShip.IsMoving - OK, result: {0} ", result));

	        // Сохраняем последнее состояние
			_lastIsMoving = result;
			return result;
		}
	}

	void Awake()
	{
		Instance = this;

		_camera = this.camera;
		_cameraTransform = this.transform;

       Rect rect = new Rect();
        
	}

    private const float SmoothTime = 0.5F;
    Vector3 _velocity = Vector3.zero;

	private float _timeCoeff = 10f;

    void LateUpdate()
    {
		if (Target == null)
		{
			return;
		}

		if (_rigidbody == null)
		{
			_rigidbody = Target.GetComponent<Rigidbody>();
			return;
		}

        AudioListenerUpdate();

        if (!IsInvoking("AfterZoomDelay"))
            _camera.fieldOfView = Mathf.Lerp(_camera.fieldOfView, IsMoving ? FovFrom : FovTo, (Time.time - _fovStartTime) * CurrentZoomSpeed);

		_cameraPosition = _rigidbody.position - _positionOffset;
		Vector3 nextCameraPosition = new Vector3(_cameraPosition.x, _cameraTransform.position.y, _cameraPosition.z);

        _cameraPosition = Vector3.SmoothDamp(_cameraTransform.position, nextCameraPosition, ref _velocity, SmoothTime);

	    _cameraTransform.position = _cameraPosition;
    }

    #region Audio

    private bool _audioWasSetup;

    private void AudioListenerUpdate()
    {
        if (_audioWasSetup)
        {
            return;
        }
        
        AudioListener audioListenerCamera = _camera.GetComponent<AudioListener>();

        if (audioListenerCamera != null)
        {
            Destroy(_camera.GetComponent<AudioListener>());
        }

        AudioListener audioListenerTarget = Target.GetComponent<AudioListener>();

        if (audioListenerTarget == null)
        {
            Target.gameObject.AddComponent<AudioListener>();
        }

        _audioWasSetup = true;
        
    }

    #endregion

    #region Zoom

    private void AfterZoomDelay()
    {
        _fovStartTime = Time.time;
    }

    #endregion

}