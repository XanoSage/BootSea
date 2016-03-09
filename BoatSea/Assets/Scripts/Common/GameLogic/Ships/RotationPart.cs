using UnityEngine;

public class RotationPart : MonoBehaviour
{
	[SerializeField]
	Vector3 _axis = new Vector3(1, 0, 0);
	
	[SerializeField]
	float _speed = 1;
	
	Transform _tm;
	
	void Awake()
	{
		_tm = transform;
	}
	
    public void Rotate(float boatSpeed)
	{
		if (_axis.x > 0) {
			_axis.x += Time.deltaTime * boatSpeed * this._speed;
		}
		else if (_axis.y > 0) {
			_axis.y += Time.deltaTime * boatSpeed * this._speed;
		}
		else if (_axis.z > 0) {
			_axis.z += Time.deltaTime * boatSpeed * this._speed;
		}
		_tm.Rotate(_axis);
    }
}
