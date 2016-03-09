using System;
using Aratog.NavyFight.Models.Games;
using UnityEngine;
using System.Collections;

[Serializable]
public class UIMultiplayerGameInfo : MonoBehaviour
{
	private const string ClassicControlsText = "Rails";
	private const string NewWaveControlsText = "Free";

	private const string CaptureTheFlagIcon = "ctf";

	#region Variables

	public UILabel ControlsLabel;

	public UILabel MapLabel;
	public UILabel ModeLabel;

	public UILabel PlayerCountLabel;

	public UISprite ModeIcon;

	[HideInInspector] public int Id;
	[HideInInspector] public RoomInfo Room;

	private UICheckbox checkbox;

	public bool IsChecked
	{
		get { return checkbox != null && checkbox.isChecked; }
		set { checkbox.isChecked = value; }
	}

	#endregion

	#region MonoBehaviour events

	// Use this for initialization
	private void Start()
	{
		Id = 0;
		checkbox = gameObject.GetComponent<UICheckbox>();
		if (checkbox == null)
		{
			Debug.LogError("Can't find UICheckbox component");
			return;
		}

		checkbox.onStateChange += UIGameList.Instance.OnStateChange;
		checkbox.optionCanBeNone = true;
	}

	// Update is called once per frame
	private void Update()
	{

	}

	#endregion

	#region events

	public void UpdateGameInfo(RoomInfo room, int index)
	{
		Id = index;
		Room = room;

		string[] roomPropertyList = UIController.Instance.RoomPropertyList;

		if (room.customProperties.ContainsKey(roomPropertyList[UIController.MapNameIndex]))
		{
			MapLabel.text = room.customProperties[roomPropertyList[UIController.MapNameIndex]] as string;
		}

		if (room.customProperties.ContainsKey(roomPropertyList[UIController.GameModeIndex]))
		{
			string gameMode = room.customProperties[roomPropertyList[UIController.GameModeIndex]] as string;
			ModeLabel.text = GetGameModeString(gameMode);

			if (ModeLabel.text == "CTF")
			{
				ModeIcon.spriteName = CaptureTheFlagIcon;
			}
		}

		if (room.customProperties.ContainsKey(roomPropertyList[UIController.ControlsIndex]))
		{
			string controlsType = room.customProperties[roomPropertyList[UIController.ControlsIndex]] as string;
			ControlsLabel.text = GetControlsTypeString(controlsType);
		}

		int maxPlayer = room.maxPlayers/2;

		PlayerCountLabel.text = maxPlayer > 0 ? string.Format("{0}x{0}", maxPlayer) : "error";

	}

	private string GetGameModeString(string gameMode)
	{
		string str = "";
		char[] array = gameMode.ToCharArray();

		for (int i = 0; i < array.Length; i++)
		{
			if (char.IsUpper(array[i]))
			{
				str += array[i];
			}
		}

		return str;
	}

	private string GetControlsTypeString(string gameMode)
	{
		string str = "error";
		if (gameMode == MechanicsType.Classic.ToString())
		{
			str = ClassicControlsText;
		}
		else if (gameMode == MechanicsType.NewWave.ToString())
		{
			str = NewWaveControlsText;
		}
		
		return str;
	}

	public void Reset()
	{

		if (checkbox != null)
		{
			//checkbox.startsChecked = false;
			checkbox.isChecked = false;
		}

	}


	#endregion
}
