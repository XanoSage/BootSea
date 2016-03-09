using System;
using UnityEngine;

public class UITab : MonoBehaviour
{
	public UIUpgradesItem [] Items;

    [SerializeField] private  Color _activeLabel = Color.white;
    [SerializeField] private  string _activeSprite = string.Empty;
    [SerializeField] private  Color _activeSprite2 = Color.white;
    [SerializeField] private  string _inActiveSprite = string.Empty;
    [SerializeField] private  Color _inactiveLabel = Color.black;
    [SerializeField] private  Color _inactiveSprite2 = Color.black;
    [SerializeField] private  UILabel _label = null;
    [SerializeField] private  GameObject _panel = null;
    [SerializeField] private  UISprite _sprite = null;
    [SerializeField] private  UISprite _sprite2 = null;
    public int index { get; set; }
    public event Action<UITab> onClick;

    private void OnClick()
    {
        onClick(this);
    }

    public void SyncOnClick()
    {
        onClick(this);
    }



    public void SetActive(bool active)
    {
        if (_sprite)
            _sprite.spriteName = active ? _activeSprite : _inActiveSprite;
        if (_sprite2)
            _sprite2.color = active ? _activeSprite2 : _inactiveSprite2;
        if (_label)
            _label.color = active ? _activeLabel : _inactiveLabel;
        _panel.SetActive(active);

		UIArmoryPanel.Instance.CheckIcons ();
    }
}