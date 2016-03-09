/* Timer usage example:

	Timer timer1;
	
	timer1 = gameObject.AddComponent<Timer>();
	timer1.OnEnd += Timer1End;
	timer1.Launch(5, true);
	
	void Timer1End()
	{
		Debug.Log("Timer1 end");
		timer1.OnEnd -= Timer1End;
	}

*/

using System;
using UnityEngine;

public class Timer : MonoBehaviour
{
	bool _destroyOnEnd, _isStarted;

	public event Action OnEnd;

	public bool isStarted
	{
		get
		{
			return _isStarted;
		}
		private set
		{
			_isStarted = value;
			CancelInvoke("Tick");
			if (_isStarted)
				InvokeRepeating("Tick", 1, 1);
		}
	}

	public float time { get; private set; }

	public void Launch(float time, bool destroyOnEnd = false)
	{
		this.time = time;
		this._destroyOnEnd = destroyOnEnd;

		isStarted = true;
	}

	public void Pause () {
		isStarted = false;
	}

	void Tick()
	{
		time--;
		if (time <= 0)
		{
			isStarted = false;
			if (OnEnd != null)
				OnEnd();
			if (_destroyOnEnd)
				Destroy(this);
		}
	}
	
	/// <summary>
	/// Получение оставшегося времени.
	/// </summary>
	/// <returns>
	/// Оставшееся время.
	/// </returns>
	/// <param name='min'>
	/// Минуты.
	/// </param>
	/// <param name='sec'>
	/// Секунды.
	/// </param>
	public string GetTimeLeft(string min = " min ", string sec = " sec")
	{
		float total = (float)time / 60;
		int minutes = (int)total;
		int seconds = (int)((total - Mathf.Floor(total)) * 60);
		return String.Format("{0}{1}", minutes, seconds);
	}
}