using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class UIChooser : MonoBehaviour {
	event Action<string> OnStateChange;
	[SerializeField]
	List<UILabel> labels=new List<UILabel>();
	[SerializeField]
	List<UISprite> sprites=new List<UISprite>();
	[SerializeField]
	string defaultSelection="";
	public List<string> options;
	[SerializeField]
	public UIEventListener nextButton;
	[SerializeField]
	public UIEventListener prevButton;

	string _currentSelection;
	public string CurrentSelection{
		get{
			return _currentSelection;
		}
	}

	public int IndexOfCurrentSelection
	{
		get { return options.IndexOf(_currentSelection); }
	}

	void Start () {
	if (defaultSelection != "") {
		
						_currentSelection = defaultSelection;
				} else {
		
						_currentSelection = options [0];
				}

		SetListeners();
		prevButton.onClick+=ChangeSelection;
		nextButton.onClick+=ChangeSelection;
	}

	void ChangeSelection(GameObject go)
	{
		if(go==nextButton.gameObject){

			_currentSelection=options[((options.IndexOf(_currentSelection)+1)==options.Count)?0:options.IndexOf(_currentSelection)+1];
		}
		if(go==prevButton.gameObject){
			_currentSelection=options[((options.IndexOf(_currentSelection)-1)==-1)?options.Count-1:options.IndexOf(_currentSelection)-1];
		}
		SetListeners();
	}

	void SetListeners(){
		labels.ForEach(x=>x.text=_currentSelection);
		sprites.ForEach(x=>x.spriteName=_currentSelection);

	}

	public List<string> GetOptions () {
		return options ?? null;
	}

	public void SetOptions (List<string> newoptions) {
		options = newoptions;
		ChangeSelection(nextButton.gameObject);
		//ChangeSelection(prevButton.gameObject);
	}
	public void SetOption (string option) {
	//	List<string> newoptions = new List<string>();
	//	newoptions.Add(option);
	//	options.Add (option);  //newoptions;
	//	ChangeSelection(nextButton.gameObject);
	}
}
