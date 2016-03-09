using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TutorialController : MonoBehaviour {

	/*
	 * 
	 * 0-3  движение 
	 * 
	 * 4 - стрельба
	 * 5 - мина
	 * 6- подрыв на мине
	 * 7 - бонус
	 * */
	public bool [] Steps;
	public GameObject [] TutorialSteps;
	private UiTutorialController UiController;

	// Use this for initialization
	void Start () {
		UiController = GameObject.Find("HUD").GetComponent<UiTutorialController>();
		UiController.UiSteps [0].SetActive (true);
		UIAdmiralMessageTutorial.Instance.SetMessage("Move to point ");
		UIAdmiralMessageTutorial.Instance.Show();
	}

	public void StepComplet()
	{
		for(int i =0;i<Steps.Length;i++)
		{
			// смотрим какой шаг мы выполнили и активируем следующий 
			if(Steps[i]==true)
			{
				ActivateNewSteps(i+1);

				UIAdmiralMessageTutorial.Instance.SetMessage("Step is "+i);
				UIAdmiralMessageTutorial.Instance.Show();

				Steps[i+1] = true;
				Steps[i] = false;
				//деактивируем всех детей (нужно для фикса бага нгуи)
				for(int c =0;c<TutorialSteps[i].transform.childCount;c++){
					TutorialSteps[i].transform.GetChild(c).gameObject.SetActive(false);
				}

				if(i!=4)
				{
				TutorialSteps[i].SetActive(false);
				}
				//активируем UI палец


				if(i==0)
				{
					UiController.UiSteps[0].SetActive(false);
				}

				else if(i==3)
				{
					UiController.UiSteps[2].SetActive(true);
				}
				else if(i==4)
				{
					UiController.UiSteps[2].SetActive(false);
				}
				else if(i==5)
				{
					Toasts.Instance.Add("Wait...");
				}

			break;
			}
		}
	}

	void ActivateNewSteps (int step)
	{
		if (TutorialSteps [step] is GameObject) {
			TutorialSteps [step].SetActive (true);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}
}
