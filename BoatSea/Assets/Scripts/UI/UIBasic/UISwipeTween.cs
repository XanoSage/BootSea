using UnityEngine;
using System.Collections.Generic;
//public enum 
public class UISwipeTween : MonoBehaviour {
	[SerializeField]
	float minSwipeLenght=20;
	[SerializeField]
	List<UITweener> tweeners;
	bool forward=false;
	void OnDrag(Vector2 delta)
	{
		print (delta.x);
		if(delta.x>0)forward=false;
		if(delta.x<0)forward=true;
		print (forward);
		if(Mathf.Abs(delta.y)>minSwipeLenght)tweeners.ForEach(t=>t.Play(forward));
	}
}
