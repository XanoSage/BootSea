using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Unity3D.Battles;
using UnityEngine;
using System.Collections;

public class UIBattleInfo : MonoBehaviour {

	#region Constants

	private const string CaptureTheFlagTypeIcon = "ctf";
	private const string CaptureTheFlagTypeText = "Capture The Flag";

	#endregion

	#region Variables

	[SerializeField] private UISprite _battleTypeIcon;
	[SerializeField] private UILabel _battleTypeLabel;

	[SerializeField] private UILabel _blueTeamScoreLabel;
	[SerializeField] private UILabel _redTeamScoreLabel;
	[SerializeField] private UILabel _battleTimeLabel;

	private Battle _currentBattle;

	#endregion

	#region MonoBeehaviour

	// Use this for initialization
	void Start ()
	{
		_currentBattle = null;
	}
	
	// Update is called once per frame
	void Update () {

	}
	#endregion

	#region actions

	public void Init(Battle currentBattle)
	{
		_currentBattle = currentBattle;
		UpdateBattleData(_currentBattle);
	}

	public void UpdateBattleData(Battle currentBattle)
	{
		_currentBattle = currentBattle;

		if (_currentBattle == null)
			return;
		
		Debug.Log("UIBattleInfo.UpdateBattleData - OK");

		switch (_currentBattle.Mode)
		{
			case GameMode.CaptureTheFlag:
				_battleTypeIcon.spriteName = CaptureTheFlagTypeIcon;
				_battleTypeLabel.text = CaptureTheFlagTypeText;

				_blueTeamScoreLabel.text = _currentBattle.BlueFlagCounter.ToString();
				_redTeamScoreLabel.text = _currentBattle.OrangeFlagCounter.ToString();

				int minutes = (int) _currentBattle.TimeSpent/60;
				int seconds = (int) _currentBattle.TimeSpent%60;

				_battleTimeLabel.text =  string.Format("{0:00}:{1:00}", minutes, seconds);
				
				break;
		}
	}
	#endregion
}
