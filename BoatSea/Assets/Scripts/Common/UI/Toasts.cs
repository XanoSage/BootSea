using UnityEngine;


/// <summary>
/// Юзается так:
/// Toasts.Instance.Add("Это тост!");
/// </summary>
public class Toasts : MonoBehaviour
{
	public static Toasts Instance { private set; get; }
	
	// Время показа тостов
	[SerializeField]
    float _timeToShow = 3.0f;
	
	// Отступ от текущего тоста до следующего
	[SerializeField]
	float _offset = 0.15f;
	
	// Тэг, используемый для быстрого нахождения тостов в сцене
	[SerializeField]
	string _messageTag = "UIToast";
	
	// Префаб тоста
	[SerializeField]
	Transform _toastPrefab;
	
	// Место тостов на экране
	internal Vector3 _tracePlace;
	
	void Awake ()
	{
		// Инициализируем синглтон
		Instance = this;
		
		// По умолчанию - положение самого менеджера
		_tracePlace = this.transform.position;

		// Защита от Артёмов
		if (!_toastPrefab) {
			Error("no Toast prefab");
			return;
		}
		
		// Защита от Артёмов, написанная, как видите, Артёмом
		if (string.IsNullOrEmpty(_messageTag))
		{
			if (string.IsNullOrEmpty(_toastPrefab.tag))
			{
				Error("wrong tags");
				return;
			}
			else
			{
				_messageTag = _toastPrefab.tag;
			}
		}
		else if (_messageTag != _toastPrefab.tag)
		{
			if (string.IsNullOrEmpty(_toastPrefab.tag))
			{
				if (string.IsNullOrEmpty(_messageTag))
				{
					Error("wrong tags");
					return;
				}
				else
				{
					_toastPrefab.tag = _messageTag;
				}
			}
			else
			{
				_messageTag = _toastPrefab.tag;
			}
		}
	}

	void Error (string reason)
	{
		Debug.LogError("Toasts system error: " + reason);
	}
	
	internal int toastsLength;
	
	/// <summary>
	/// Создаёт тост.
	/// </summary>
	/// <param name='message'>
	/// Текст тоста.
	/// </param>
	/// <param name='_timeToShow'>
	/// Время показа.
	/// </param>
	public void Add (string message, float timeToShow = -1)
	{
		if (timeToShow < 0)
			timeToShow = _timeToShow;
		
		// Находим самый нижний тост
		float lowestToast = 0;

		GameObject[] toasts = GameObject.FindGameObjectsWithTag(_messageTag);

		foreach(GameObject t in toasts)
			lowestToast = Mathf.Min(lowestToast, t.GetComponent<UIToast>().Y);
	
		// Делаем отступ, если уже есть другие тосты
		if (toasts.Length > 0)
			lowestToast -= _offset;
		
		// Вычисляем положение нового тоста на экране
		Vector3 newToastPosition = _tracePlace + new Vector3(0, lowestToast, 0);

		// Создаём тост
		UIToast toast = null;
		try
		{
			Transform toastObject = (Transform)Instantiate(_toastPrefab, newToastPosition, Quaternion.identity);
//#if UNITY_EDITOR
			toastObject.parent = transform;
//#endif
			toast = toastObject.GetComponent<UIToast>();
		}
		catch
		{
			Error("can't get UIToast component (or invalid cast)");
		}
		finally
		{
			toast.Init(timeToShow, message);
		}
	}

	/// <summary>
	/// Убирает с экрана все тосты.
	/// </summary>
	/// <param name='immediate'>
	/// false - плавное исчезновение, true - мгновенное.
	/// </param>
	public void ClearAll (bool immediate = false)
	{
		foreach(GameObject msg in GameObject.FindGameObjectsWithTag(_messageTag))
		{
			if (immediate)
				Destroy(msg);
			else
				msg.SendMessage("OnClear");
		}
	}
	
#if UNITY_EDITOR
	void OnDrawGizmosSelected ()
	{
		Gizmos.color = Color.yellow;
		Vector3 size = new Vector3(1, 0.055f, 0.1f);
		if (!Application.isPlaying)
			_tracePlace = transform.position;
		Gizmos.DrawCube(_tracePlace - new Vector3(0, size.y * 0.5f, 0), size);
		Gizmos.color = Color.magenta;
		Gizmos.DrawCube(_tracePlace - new Vector3(0, _offset, 0) - new Vector3(0, size.y * 0.5f, 0), size);
	}
#endif
}
