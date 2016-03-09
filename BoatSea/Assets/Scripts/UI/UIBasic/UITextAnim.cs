using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System;
using LinqTools;

public class TextAnim : MonoBehaviour {

	string s=null;
	List<int> availableUTF;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnEnable()
	{	
		//Temporary available UTF32 code initialization
		availableUTF=new List<int>();
		for(int i=Char.ConvertToUtf32("A",0);i<=Char.ConvertToUtf32("z",0);i++)availableUTF.Add (i);
		availableUTF.ForEach(delegate(int i){print (i);print (Char.ConvertFromUtf32(i));});
		for(int i=Char.ConvertToUtf32(" ",0);i<=Char.ConvertToUtf32("@",0);i++)availableUTF.Add (i);
		availableUTF.ForEach(delegate(int i){print (i);print (Char.ConvertFromUtf32(i));});	
		s =GetComponent<UILabel>().text; 
		for(int i=1040;i<=1103;i++)availableUTF.Add (i);
		availableUTF.ForEach(delegate(int i){print (i);print (Char.ConvertFromUtf32(i));});	
		s =GetComponent<UILabel>().text; 
		
		
		/*for(int i=0;i<s.Length;i++){
		print (Char.ConvertToUtf32(s,i));
		}*/
		//s=new StringBuilder(GetComponent<UILabel>().text);
		/*int j=0;
		while(j<2000)
		{
			print (j.ToString()+"   "+Char.ConvertFromUtf32(j));
			j++;
		}*/
	StartCoroutine(RunLetters2());
		
		
	}
	IEnumerator RunLetters()
	{
		print ("yarrrr");
		for(int i=0;i<s.Length;i++)
		{
			int utf32=Char.ConvertToUtf32(s,i);
			for(int j=1040;j<=utf32;j++)
			{
				GetComponent<UILabel>().text=s.Substring(0,i)+Char.ConvertFromUtf32(j);
				yield return null;//new WaitForSeconds(0.001f);
			}
		}
	}
		IEnumerator RunLetters2()
	{
		StringBuilder sb=new StringBuilder(s);
		List<int> utfArr=new List<int>();
		List<int> currLettersPositions=new List<int>();
		for(int i=0;i<s.Length;i++)
		{
			utfArr.Add(Char.ConvertToUtf32(s,i));
		}	
		//utfArr.CopyTo(utfArrCurr);
		/*for(int j=1040;j<=1120;j++)
		{*/
		int j;	
		int count=0;
		//System.Random random= new System.Random();
			//sb=new StringBuilder();
		while(currLettersPositions.Count<sb.Length){
			int letterIndex=0;
			
			foreach(int u in utfArr)
			{ 
				j=UnityEngine.Random.Range(0,availableUTF.Count);
				//j=random.Next(64)+1040;
				count++;
				if(currLettersPositions.Contains(letterIndex)){letterIndex++;continue;}	
				if(availableUTF[j]!=u)sb[letterIndex]=(Char.ConvertFromUtf32(availableUTF[j]))[0];
				else {
					sb[letterIndex]=(Char.ConvertFromUtf32(u))[0];
					//print ("equal");
					currLettersPositions.Add(letterIndex);
					//currLettersPositions.ForEach(delegate(int y){print (y);});
					//print(sb.ToString());
					//print ("-------------------------------------------------------------------------------------");
				}
				letterIndex++;
			}
			GetComponent<UILabel>().text=sb.ToString();
			yield return new WaitForSeconds(0.01f);
		}
		print (count);
	}
}
