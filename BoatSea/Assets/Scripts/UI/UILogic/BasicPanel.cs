using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using System.Collections;

public abstract class BasicPanel<T> : MonoBehaviour, IMenuInterface where T : BasicPanel<T> {

	private static T _instance;
	public static T Instance{
		get
        {
            return _instance;
        }
	}

	bool _isActive;
	public bool IsActive
	{
		get
		{
			return _isActive;
		}
		set
		{
			_isActive = value;
		}
	}
	
	UIMenuInterfaceControllsType _menuType;
	public UIMenuInterfaceControllsType MenuType
	{
		get
		{
			return _menuType;
		}
		set
		{
			_menuType = value;
		}
	}
	
	public virtual void SetType()
	{
	}
	
	void Awake () {	
		_instance = (T)this;
		SetType();
		if(GetComponent<UITweener>()){
			var tweener=GetComponent<UITweener>();
			tweener.enabled=true;
			tweener.onFinished+=OnTweenFinished;
			tweener.enabled=false;
		}

	}

	void Start () {
		
		if (_instance == null)
			_instance = (T)this;
	}

	public virtual void OnTweenFinished(UITweener tween)
	{
//		print(tween.gameObject.name);
	}
	public virtual void Show(){
		IsActive=true;
		gameObject.SetActive(true);
		GetComponent<UITweener>().enabled=true;
		GetComponent<UITweener>().Play(true);
        UITopPanel.Instance.CurrentPanels.Add(Instance);

        SoundController.PlayMenuOpen();
	}
	public virtual void Hide(){
		IsActive=false;
		GetComponent<UITweener>().Play(false);
	    //UITopPanel.Instance.CurrentPanels.Remove(Instance);
	}
    
}
