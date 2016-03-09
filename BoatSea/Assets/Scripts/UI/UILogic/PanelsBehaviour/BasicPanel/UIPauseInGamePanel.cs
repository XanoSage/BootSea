using System.Collections.Generic;
using Aratog.NavyFight.Models.Unity3D.Battles;
using Aratog.NavyFight.Models.Unity3D.Players;
using Assets.Scripts.Common.Useful;
using UnityEngine;
using System.Collections;

public class UIPauseInGamePanel : MonoBehaviour, IShowable  {

	#region Variables

	[SerializeField] private UIPanel _shipTacticPanel;
	[SerializeField] private UIPanel _scoreAndButtonPanel;
	[SerializeField] private UIPanel _topPausePanel;

	[SerializeField] private UIEventListener _pauseBackButon;

	[SerializeField] private UIEventListener _resumeButton;
	[SerializeField] private UIEventListener _optionsButton;
	[SerializeField] private UIEventListener _exitButton;
	[SerializeField] private UIEventListener _resumeOptionsButton;




	[SerializeField] private UIBattleInfo _battleInfo;


	[SerializeField] private List<UIShipItemSimple> _uiShips;

	[SerializeField] private UISprite _backgroundFade;

	public bool Visible { get; private set; }

	[SerializeField] private UILoadingScreen _loadingScreen;

	private bool _isTacticPanelInited;

	#endregion

	#region MonoBehaviour actions

	// Use this for initialization
	void Start ()
	{
		_isTacticPanelInited = false;

		_pauseBackButon.onClick = OnPauseBackButton;
		_resumeButton.onClick = OnResumeButton;
		_optionsButton.onClick = OnOptionsButton;
		_exitButton.onClick = OnExitButton;
		_resumeOptionsButton.onClick = OnOtionsResumeButton;

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	#endregion

	#region Interface implementation

	public void Show()
	{
		Visible = true;

		ShowShipTacticPanel();
		ShowTopPausePanel();
		ShowScoreAndButtonPanel();
		ShowBackgroundFade();

		if (_isTacticPanelInited)
			return;

		Init();
	}

	public void Hide()
	{
		Visible = false;

		_isTacticPanelInited = false;

		HideShipTacticPanel();
		HideTopPausePanel();
		HideScoreAndButtonPanel();
		HideBackgroundFade();
	}

	#endregion

	#region actions

	public void Init()
	{
		InitTacticPanel();
		ScoreInit(GameSetObserver.Instance.CurrentBattle);
	}

	private void InitTacticPanel()
	{
		int counter = 0;
		List<Player> playersFleet = GameSetObserver.Instance.Human.PlayersFleet;

		foreach (Player player in playersFleet)
		{
			UIShipItemSimple shipItem = _uiShips[counter];
			shipItem.Init(player);
			shipItem.Show();
			counter++;
		}

		for (int i = counter ; i < _uiShips.Count; i ++)
		{
			_uiShips[i].Hide();
		}

		_isTacticPanelInited = true;
	}

	#region ship tactic panel
	private void ShipTacticPanelDisplaying(bool show)
	{
		UITweener tweener = _shipTacticPanel.GetComponent<UITweener>();
	
		if (tweener != null)
			tweener.Play(show);
	}

	private void ShowShipTacticPanel()
	{
		ShipTacticPanelDisplaying(true);
	}

	private void HideShipTacticPanel()
	{
		ShipTacticPanelDisplaying(false);
	}
	#endregion


	#region TopPausePanel
	private void ShowTopPausePanel()
	{
		TopPanelDisplaying(true);
	}

	private void HideTopPausePanel()
	{
		TopPanelDisplaying(false);
	}

	private void TopPanelDisplaying(bool show)
	{
		UITweener tweener = _topPausePanel.GetComponent<UITweener>();

		if (tweener != null)
			tweener.Play(show);
	}
	private void OnOtionsResumeButton(GameObject sender)
	{
		UIOptionsBatllePanel.Instance.Hide ();
	}
	private void OnPauseBackButton(GameObject sender)
	{
		Debug.Log("UIPauseInGamePanel.OnPauseBack - OK");
		UIOptionsBatllePanel.Instance.Hide ();
		Hide();
		GameController.Instance.PauseGame();
	}
	#endregion

	#region Score and buttons panel

	private void ScoreAndButtonsPanelDisplaying(bool show)
	{
		UITweener tweener = _scoreAndButtonPanel.GetComponent<UITweener>();

		if (tweener != null)
			tweener.Play(show);
	}

	private void ShowScoreAndButtonPanel()
	{
		ScoreAndButtonsPanelDisplaying(true);
	}

	private void HideScoreAndButtonPanel()
	{
		ScoreAndButtonsPanelDisplaying(false);
	}

	private void ScoreInit(Battle currentBattle)
	{
		_battleInfo.Init(currentBattle);
	}
	#endregion

	#region Background Fade

	private void BackgroundFadeDisplaying(bool show)
	{
		UITweener tweener = _backgroundFade.GetComponent<UITweener>();

		if (tweener != null)
		{
			tweener.Play(show);
		}
	}

	private void ShowBackgroundFade()
	{
		BackgroundFadeDisplaying(true);
	}

	private void HideBackgroundFade()
	{
		BackgroundFadeDisplaying(false);
	}
	#endregion

	#region pause button actions
	private void OnResumeButton(GameObject sender)
	{
		Debug.Log("UIPauseInGamePanel.OnResumeButton - OK");
		UIOptionsBatllePanel.Instance.Hide ();
		OnPauseBackButton(_resumeButton.gameObject);

	}

	private void OnOptionsButton(GameObject sender)
	{
		Debug.Log("UIPauseInGamePanel.OnOptionsButton - OK");

		UIOptionsBatllePanel.Instance.Show();
	}

	private void OnExitButton(GameObject sender)
	{
		Debug.Log("UIPauseInGamePanel.OnExitButton - OK");
		if (GameController.Instance != null)
			GameController.Instance.BattleEnd();

	}
	#endregion

	#endregion
}
