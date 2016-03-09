using UnityEngine;
using System.Collections;

public class UIMessagePanel : BasicPanel<UIMessagePanel>  {
	private string message;

	[SerializeField]
	private UILabel _label;

	// Use this for initialization
	void Start () {
	
	}

	public void SetMessage(string text )
	{
		message = text;
	}
	
	public override void Show()
	{
		base.Show ();
		StartCoroutine ("Timer");
		_label.text = message;
	}
	IEnumerator Timer()
	{
		yield return new WaitForSeconds(2.0f);
		Hide ();
	}

	public virtual void Hide(){
		base.Hide ();
	}
	// Update is called once per frame
	void Update () {
	
	}
}
