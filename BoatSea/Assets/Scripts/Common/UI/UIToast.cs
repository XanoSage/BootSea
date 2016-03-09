using UnityEngine;

public class UIToast : MonoBehaviour
{
	internal float Y;
	
	// Скорость ухода в прозрачность
	[SerializeField]
	float _transparencySpeed = 1;
	
	// Текущая прозрачность тоста
	float _transparency;

	// Ссылка на NGUI'вский Label
	UILabel _label;

	// Состояние анимации (true - уменьшается прозрачность, false - увеличивается)
	bool _hiding;
	
	// Таймер
	Timer _timer;

	// Инициализация, вызываемая в Toast.cs при создании копии надписи
	public void Init (float timeToShow, string message)
	{
		this.Y = transform.localPosition.y;

		_label = GetComponent<UILabel>();
		_label.text = message;

		if (!_label)
		{
			Debug.LogError("UIToast: can't find UILabel component");
			return;
		}

		_transparency = 0.01f;
		_label.color = new Color(1, 1, 1, _transparency);

		Invoke("OnTimerEnd", timeToShow);
	}

	void Update ()
	{
		// Ограничиваем прозрачность, которую мы увеличиваем или уменьшаем в зависимости от состояния анимации
		if (_transparency <= 1)
			_transparency += Time.deltaTime * (_hiding ? -_transparencySpeed : _transparencySpeed);
		else
			_transparency = 1;

		// Автоуничтожение тоста в случае, если он ушёл в полную прозрачность
		if (_hiding && _transparency <= 0)
		{
			Destroy(gameObject);
		}
		else
		{
			_label.color = new Color(1, 1, 1, _transparency);
		}
	}
	
	// Вызывается по истечению времени показа тоста
	void OnTimerEnd ()
	{
		_hiding = true;
		_transparencySpeed *= 10f;
	}
	
	// Вызывается при очистке консоли, вызываемой из Toasts.cs
	void OnClear ()
	{
		_hiding = true;
		_transparencySpeed *= 5f;
	}
}