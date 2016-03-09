// Универсальный (платформонезависимый) инпут

#pragma warning disable

using UnityEngine;

public static class AInput
{
	public static int Pressed()
	{
		#if (UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_FLASH)
			if (Input.GetMouseButton(0))
				return 1;
			else if (Input.GetMouseButton(1))
				return 2;
			else
				return 0;
		#endif
		#if (UNITY_IPHONE || UNITY_ANDROID)
			return Input.touchCount;
		#endif
	}
	
	public static int Down()
	{
		#if (UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_FLASH)
			if (Input.GetMouseButtonDown(0))
				return 1;
			else if (Input.GetMouseButtonDown(1))
				return 2;
			else
				return 0;
		#endif
		#if (UNITY_IPHONE || UNITY_ANDROID)
			if (Input.touchCount > 0 && Input.GetTouch(Input.touchCount - 1).phase == TouchPhase.Began)
				return Input.touchCount;
			return 0;
		#endif
	}
	
	public static int Up()
	{
		#if (UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_FLASH)
			if (Input.GetMouseButtonUp(0))
				return 1;
			else if (Input.GetMouseButtonUp(1))
				return 2;
			else
				return 0;
		#endif
		#if (UNITY_IPHONE || UNITY_ANDROID)
			if (Input.touchCount > 0 && Input.GetTouch(Input.touchCount - 1).phase == TouchPhase.Ended || Input.GetTouch(Input.touchCount - 1).phase == TouchPhase.Canceled)
				return Input.touchCount;
			return 0;
		#endif
	}
	
#region Coordinates
	
#if (UNITY_IPHONE || UNITY_ANDROID)
	static UnityEngine.Vector3 _lastScreenCoord;
#endif

	public static Vector3 WorldPos(Camera camera = default(Camera))
	{
		if (camera == default(Camera))
			camera = Camera.main;
		#if (UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_FLASH)
			return camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
		#endif
		#if (UNITY_IPHONE || UNITY_ANDROID)
			return camera.ScreenToWorldPoint(new UnityEngine.Vector3(UnityEngine.Input.GetTouch(UnityEngine.Input.touchCount - 1).position.x, UnityEngine.Input.GetTouch(UnityEngine.Input.touchCount - 1).position.y, 0));
		#endif
	}

	public static Vector3 ScreenPos()
	{
		#if (UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_WEBPLAYER || UNITY_FLASH)
			return new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
		#endif
		#if (UNITY_IPHONE || UNITY_ANDROID)
			if (Input.touchCount > 0)
				_lastScreenCoord = new UnityEngine.Vector3(Input.GetTouch(Input.touchCount - 1).position.x, Input.GetTouch(Input.touchCount - 1).position.y, 0);
			return _lastScreenCoord;
		#endif
	}
#endregion

}