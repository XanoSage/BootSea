using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UITacticSelection : MonoBehaviour {
    public event Action<string> OnStateChange;
	[SerializeField]
	List<UILabel> labels;
	[SerializeField]
	List<UISprite> sprites;
	[SerializeField]
	List<UIEventListener> tacticBtns=new List<UIEventListener>();
	[SerializeField]
	string defaultSelection="";
	[SerializeField]
	List<string> options;
	string _currentSelection;
	public string CurrentSelection{
		get{
			return _currentSelection;
		}
		set{
			_currentSelection=value;
		}
	}
	void Awake () {
		_currentSelection=options[0];
		tacticBtns.ForEach(btn=>btn.onClick+=ChangeSelection);
		SetListeners();
	}

	void ChangeSelection(GameObject go)
	{
		_currentSelection=options[tacticBtns.IndexOf(go.GetComponent<UIEventListener>())];
		SetListeners();
		OnStateChange(_currentSelection);
	}
	public void ChangeScelection(string selection){
		_currentSelection=selection;
		SetListeners();
		OnStateChange(_currentSelection);
	}
	void SetListeners()
	{
	 labels.ForEach(l=>l.text=_currentSelection);
	 sprites.ForEach(s=>s.spriteName=_currentSelection+"_ico");
	}
}
