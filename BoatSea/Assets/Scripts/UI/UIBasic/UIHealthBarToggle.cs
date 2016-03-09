using UnityEngine;
using System.Collections;

public class UIHealthBarToggle : MonoBehaviour {
	[SerializeField]
	UILabel label;
	bool _isOn=false;
	public bool IsOn{
		get{return _isOn;}
		set{
			_isOn=value;

			if(_isOn){
				label.text="On";
				label.color=Color.green;
			}
			else{
				label.text="Off";
				label.color=Color.red;
			}
		}
	}
	void Start(){
		IsOn = !PlayerInfo.Instance.HealthBar;
		Debug.Log(IsOn);
	}
	void OnClick(){
		IsOn = !IsOn;
		PlayerInfo.Instance.HealthBar = !IsOn;

	}
}
