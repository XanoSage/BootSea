using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UICounter : MonoBehaviour {
 event Action<string> OnStateChange;
	[SerializeField]
	List<UILabel> labels=new List<UILabel>();
	[SerializeField]
	int defaultValue=1;
	[SerializeField]
	int min;
	[SerializeField]
	int max;
	[SerializeField]
	List<string> options;
	[SerializeField]
	UIEventListener plusButton;
	[SerializeField]
	UIEventListener minusButton;
	int _currentValue;
	public int CurrentValue{
		get{
			return _currentValue;
		}
	}
	void Start () {
		_currentValue=defaultValue;
		SetListeners();
		plusButton.onClick+=ChangeSelection;
		minusButton.onClick+=ChangeSelection;
	}
	
	public void SetLimitations(int min, int max)
	{
		this.min = min;
		this.max = max;
	}
	void ChangeSelection(GameObject go)
	{
		if(go==plusButton.gameObject){
		_currentValue=(_currentValue+1)>max?min:_currentValue+1;
		}	
		if(go==minusButton.gameObject){
			_currentValue=(_currentValue-1)<min?max:_currentValue-1;
		}	
		SetListeners();
	}
	void SetListeners(){
		labels.ForEach(x=>x.text=_currentValue.ToString());
		
	}
}