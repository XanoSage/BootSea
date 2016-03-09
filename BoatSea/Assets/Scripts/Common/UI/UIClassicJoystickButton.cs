using UnityEngine;
using Aratog.NavyFight.Models.Ships;

public class UIClassicJoystickButton : MonoBehaviour
{
    public Direction Direction;

    [SerializeField]
    Color _pressed = Color.grey;

    [SerializeField]
    float _duration = 0.2f;

    private Color _defaultColor;

    private void Start()
    {
        try
        {
            _defaultColor = GetComponent<UISprite>().color;
        }
        catch
        {
            Debug.LogError("Can't get UISprite component");
        }
    }

    public void OnPress()
	{
        TweenColor.Begin(gameObject, _duration, _pressed);

        // TODO: избавиться от этого жуткого костыля
        CancelInvoke("OnEnd");
        Invoke("OnEnd", 0.15f);
	}

    void OnEnd()
    {
        TweenColor.Begin(gameObject, _duration, _defaultColor);
    }
}
