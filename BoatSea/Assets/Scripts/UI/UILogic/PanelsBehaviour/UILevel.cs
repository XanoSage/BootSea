using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class UILevel : MonoBehaviour {
	private UISprite _spriteComplite;
	private UISprite _sprite;
	private UILabel _label;
	void Start()
	{
		_spriteComplite = transform.FindChild ("Sprite (shesternya)").GetComponent<UISprite>();
		_sprite = transform.FindChild ("Sprite (open_level)").GetComponent<UISprite>();
		_label = transform.FindChild ("Label").GetComponent<UILabel>();
	}


	public void Show()
	{
		_sprite.color = new Color(1,1,1,1);
		_spriteComplite.color = new Color(1,1,1,1);
		_label.alpha = 1f;
	}

	public void Hide()
	{
		_sprite.color = new Color(1,1,1,0.4f);
		_spriteComplite.color = new Color(1,1,1,0.4f);
		_label.alpha = 0f;
	}
}
