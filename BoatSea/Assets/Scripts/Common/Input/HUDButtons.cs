using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;
using Aratog.NavyFight.Models.Unity3D.Weapons;

public class HUDButtons: MonoBehaviour {

	public static HUDButtons Instance { get; private set; }

	[SerializeField]
	private UIEventListener AttackBtn, MineBtn, SpecialBtn, PauseBtn;

	[SerializeField] private UIPauseInGamePanel _pauseInGamePanel;
	[SerializeField] private UILoadingScreen _loadingScreen;

	public UILabel BlueTeamScoreLabel, RedTeamScoreLabel;

	public UILabel BombCountLabel, SpecialCountLabel;



	public UISprite BlueFlagSprite, RedFlagSprite,AdwanceWeapon;

	//variable to flag blinking when it not on flagspot
	private float blueFlagAlpha, redFlagAlpha;
	private float speedOfBlinking = 2;
	private bool isNeedBlueFlagBlinking, isNeedRedFlagBlinking;
	private bool isBlueFlagHiding, isRedFlagHiding;

	private void Awake () {
		Instance = this;
	}

	private void Update () {
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN || UNITY_STANDALONE_LINUX || UNITY_WEBPLAYER

		if (Input.GetKeyDown(KeyCode.Space))
			OnAttack(AttackBtn.gameObject);
		if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.LeftCommand) || Input.GetKeyDown(KeyCode.RightControl)
			|| Input.GetKeyDown(KeyCode.RightCommand))
			OnMine(MineBtn.gameObject);
		if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
			OnSpecial(MineBtn.gameObject);

#endif
		if (isNeedBlueFlagBlinking) {
			blueFlagAlpha += Time.deltaTime * (isBlueFlagHiding ? -speedOfBlinking : speedOfBlinking);

			if (blueFlagAlpha >= 1f && !isBlueFlagHiding) {
				blueFlagAlpha = 1f;
				isBlueFlagHiding = true;
			}

			if (blueFlagAlpha <= 0f && isBlueFlagHiding) {
				blueFlagAlpha = 0f;
				isBlueFlagHiding = false;
			}

			BlueFlagSprite.alpha = blueFlagAlpha;
		}

		if (isNeedRedFlagBlinking) {
			redFlagAlpha += Time.deltaTime * (isRedFlagHiding ? -speedOfBlinking : speedOfBlinking);

			if (redFlagAlpha >= 1f && !isRedFlagHiding) {
				redFlagAlpha = 1f;
				isRedFlagHiding = true;
			}

			if (redFlagAlpha <= 0f && isRedFlagHiding) {
				redFlagAlpha = 0f;
				isRedFlagHiding = false;
			}

			RedFlagSprite.alpha = redFlagAlpha;
		}
	}

	private void Start () {
		AttackBtn.onClick += OnAttack;
		MineBtn.onClick += OnMine;
		SpecialBtn.onClick += OnSpecial;
		PauseBtn.onClick += OnPause;

		_pauseInGamePanel = FindObjectOfType<UIPauseInGamePanel>();

	

	}

	private void OnAttack (GameObject go) {
		Debug.Log("atack click");

		if (GameSetObserver.Instance.Human != null && GameSetObserver.Instance.Human.MyShip != null)
			GameSetObserver.Instance.Human.MyShip.OnFireBasic();
	}

	private void OnMine (GameObject go) {
		if (GameSetObserver.Instance.Human != null && GameSetObserver.Instance.Human.MyShip != null)
			GameSetObserver.Instance.Human.MyShip.OnFireBomb();
	}

	private void OnSpecial (GameObject go) {
		if (GameSetObserver.Instance.Human != null && GameSetObserver.Instance.Human.MyShip != null)
			GameSetObserver.Instance.Human.MyShip.OnFireAdwance();
	}

	private void OnPause (GameObject go) {

		if (!GameSetObserver.Instance.IsBattleStarted || GameSetObserver.Instance.IsPause)
			return;

		GameController.Instance.PauseGame();
		_pauseInGamePanel.Show();
	}


	#region Displaying HUD informtion



	public void HudIconsActivation()
	{
		if (GameSetObserver.Instance.CurrentBattle.Mode == Aratog.NavyFight.Models.Games.GameMode.Deathmatch) {
			BlueFlagSprite.spriteName = "defalt";
			RedFlagSprite.spriteName ="defalt"; 
			Debug.Log("ICON STATE: "+GameSetObserver.Instance.CurrentBattle.Mode);
		}
		else if (GameSetObserver.Instance.CurrentBattle.Mode == Aratog.NavyFight.Models.Games.GameMode.BaseDefense) {
			BlueFlagSprite.spriteName = "star";
			RedFlagSprite.spriteName ="star"; 
			Debug.Log("ICON STATE: "+GameSetObserver.Instance.CurrentBattle.Mode);
		}
		else if (GameSetObserver.Instance.CurrentBattle.Mode == Aratog.NavyFight.Models.Games.GameMode.Survival) {
			BlueFlagSprite.gameObject.SetActive(false);
			RedFlagSprite.spriteName ="star"; 
			Debug.Log("ICON STATE: "+GameSetObserver.Instance.CurrentBattle.Mode);
		}
		else {
			BlueFlagSprite.spriteName = "flag_gmae_ico";
			RedFlagSprite.spriteName ="flag_gmae_ico_OR"; 
			Debug.Log("ICON STATE: "+GameSetObserver.Instance.CurrentBattle.Mode);
		}

		BlueFlagSprite.transform.localScale = new Vector3 (40,40,1);
		RedFlagSprite.transform.localScale = new Vector3 (40,40,1);
	}



	public void SetTeamScoreLabel (int score, TeamColor team) {
		if (team == TeamColor.BlueTeam) {
		
			BlueTeamScoreLabel.text = score.ToString();
			BlueTeamScoreLabel.color = Color.blue;
		}
		else {
			RedTeamScoreLabel.text = score.ToString();
			RedTeamScoreLabel.color = Color.red;
		}
	}

	public void HideRedScore()
	{
		RedTeamScoreLabel.gameObject.SetActive (false);
	}
	public void SetTeamScoreLabel (float score, TeamColor team)
	{
		BlueTeamScoreLabel.text = score.ToString("N2");
	}
	public void SetBombCountLabel (int bombCount) {
		BombCountLabel.text = bombCount.ToString();
	}


	public void SetAdvanceWeaponIcon(WeaponsType type)
	{
	
		switch(type){
		case WeaponsType.Missile:
			AdwanceWeapon.spriteName = "wp_rocket1";
			break;
		case WeaponsType.HomingMissile:
			AdwanceWeapon.spriteName = "wp_rocket";
			break;
		case WeaponsType.OneRicochet:
			AdwanceWeapon.spriteName = "wp_ricoshet1";
			break;
		case WeaponsType.TwoRicochet:
			AdwanceWeapon.spriteName = "wp_ricoshet";
			break;
		case WeaponsType.Napalm:
			AdwanceWeapon.spriteName = "wp_napalm";
			break;
		case WeaponsType.SuperTorpedo:
			AdwanceWeapon.spriteName = "wp_torpedo";
			break;
		case WeaponsType.FrozenProjectile:
			AdwanceWeapon.spriteName = "wp_icecube";
			break;
		case WeaponsType.DeepBomb:
			AdwanceWeapon.spriteName = "wp_mine";
			break;
		case WeaponsType.Construct:
			AdwanceWeapon.spriteName = "wp_construct";
			break;
		case WeaponsType.AdvanceConstruct:
			AdwanceWeapon.spriteName = "wp_construct1";
			break;
		case WeaponsType.Energy:
			AdwanceWeapon.spriteName = "wp_energy1";
			break;
		case WeaponsType.AdvanceEnergy:
			AdwanceWeapon.spriteName = "wp_energy";
			break;
		}
	}


	public void SetSpecialCountLabel (int specialCount, bool isUsing = false) {

		SpecialCountLabel.text = specialCount.ToString();
	}

	#endregion


	#region Flag events

	public void StartFlagBlinking (TeamColor color) {

		if (color == TeamColor.BlueTeam) {
			isNeedBlueFlagBlinking = true;
			blueFlagAlpha = 1f;
			isBlueFlagHiding = true;
		}
		else {
			isNeedRedFlagBlinking = true;
			redFlagAlpha = 1f;
			isRedFlagHiding = true;
		}
	}

	public void StopFlagBlinking (TeamColor color) {

		if (color == TeamColor.BlueTeam) {
			isNeedBlueFlagBlinking = false;
			blueFlagAlpha = 1f;
			isBlueFlagHiding = true;

			BlueFlagSprite.alpha = blueFlagAlpha;
		}
		else {
			isNeedRedFlagBlinking = false;
			redFlagAlpha = 1f;
			isRedFlagHiding = true;

			RedFlagSprite.alpha = redFlagAlpha;
		}
	}

	#endregion

}
