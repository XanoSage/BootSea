using UnityEngine;
using System.Collections;

public class UIMoreCoinsPopup : MonoBehaviour {
	[SerializeField]
	UISprite _darker;
	[SerializeField]
	UIEventListener _closeBtn;

	[SerializeField]
	UIEventListener _buy1;
	[SerializeField]
	UIEventListener _buy2;
	[SerializeField]
	UIEventListener _buy3;
	[SerializeField]
	UIEventListener _buy4;

	void Start () {
		_closeBtn.onClick+=Hide;
		_buy1.onClick +=Buy1;
		_buy2.onClick +=Buy2;
		_buy3.onClick +=Buy3;
		_buy4.onClick +=Buy4;
	}

	void Buy1 (GameObject go)
	{
		Debug.Log ("Purchase");
		PlayerInfo.Instance.inventory.MoneyChange (-100);
	}
	void Buy2 (GameObject go)
	{
		Debug.Log ("Purchase");
		PlayerInfo.Instance.inventory.MoneyChange (-200);
	}
	void Buy3 (GameObject go)
	{
		Debug.Log ("Purchase");
		PlayerInfo.Instance.inventory.MoneyChange (-300);
	}
	void Buy4 (GameObject go)
	{
		Debug.Log ("Purchase");
		PlayerInfo.Instance.inventory.MoneyChange (-400);
	}
	public void Show(){
		_darker.alpha=0f;
		gameObject.SetActive(true);
		gameObject.GetComponent<UITweener>().Play(true);

	}
	public void Hide(GameObject sender){
			gameObject.GetComponent<UITweener>().Play(false);

	}
	void BlackOut(){
		if(_darker.alpha<=0)
			StartCoroutine(BlackOutCoroutine(true));
		else
			StartCoroutine(BlackOutCoroutine(false));
	}
	IEnumerator BlackOutCoroutine(bool blackout)
	{
	    if (blackout)
	    {
            _darker.gameObject.SetActive(true);
	        _darker.collider.enabled = true;
	        while (_darker.alpha < 0.4f)
	        {
	            _darker.alpha += 0.02f;
	            yield return new WaitForSeconds(0.01f);
	        }
	    }
	    else
	    {
	        _darker.collider.enabled = false;
	        while (_darker.alpha > 0)
	        {
	            _darker.alpha -= 0.02f;
	            yield return new WaitForSeconds(0.01f);
	        }
            _darker.gameObject.SetActive(false);
	    }
	}
}
