using System.Collections.Generic;
using Assets.Scripts.Common.Useful;
using UnityEngine;
using System.Collections;

public class UILoadingScreen : MonoBehaviour, IShowable
{

	private const string LoadingText = "Loading";
	
	#region Variables
	public bool Visible { get; private set; }

	[SerializeField] private UILabel _loadingLabel;
	[SerializeField] private Transform _elementContainer;

	private List<string> _loadingLabelText;

	private float _changeLoadingTime = 0.15f;
	private float _changeLoadingTimer = 0f;

	private bool _isNeedShowLoading = true;

	private int _changeCount = 5;
	private int _changeCounter = 0;


	#endregion


	#region Monobehavoiur actions

	// Use this for initialization
	void Start ()
	{
		_loadingLabel.text = LoadingText;
		_loadingLabelText = new List<string>();
		_loadingLabelText.Add(LoadingText);
		_loadingLabelText.Add(LoadingText + ".");
		_loadingLabelText.Add(LoadingText + "..");
		_loadingLabelText.Add(LoadingText + "...");
		_loadingLabelText.Add(LoadingText + "....");
		_loadingLabelText.Add(LoadingText + ".....");
		Hide();
	}
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetKeyDown(KeyCode.L))
		{
			if (!_isNeedShowLoading)
				StartLoadingAnimate();
			else
			{
				Hide();
			}
		}
	}

	#endregion

	#region Actions

	private void LoadingAnimate(int currentTextId)
	{
		_loadingLabel.text = _loadingLabelText[currentTextId];
	}

	private IEnumerator UpdateAnimate()
	{
		while (_isNeedShowLoading)
		{
			if (_changeLoadingTimer < _changeLoadingTime)
			{
				_changeLoadingTimer += Time.deltaTime;
			}

			if (_changeLoadingTimer >= _changeLoadingTime)
			{
				_changeLoadingTimer = 0;
				LoadingAnimate(_changeCounter);

				_changeCounter++;

				if (_changeCounter > _changeCount)
				{
					_changeCounter = 0;
				}
			}

			yield return null;
		}
	}

	public void StartLoadingAnimate()
	{
		_isNeedShowLoading = true;
		StartCoroutine(UpdateAnimate());
	}

	public void StopLoadingAnimate()
	{
		_isNeedShowLoading = false;
		_changeCounter = 0;
		LoadingAnimate(_changeCounter);
	}

	#endregion

	#region IShowable implementation

	public void Show()
	{
		Visible = true;
		_elementContainer.gameObject.SetActive(true);

		StartLoadingAnimate();
	}

	public void Hide()
	{
		Visible = false;

		_elementContainer.gameObject.SetActive(false);
		_isNeedShowLoading = false;

		StopLoadingAnimate();
	}

	
	#endregion
}
