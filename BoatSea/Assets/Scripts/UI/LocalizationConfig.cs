using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class LocalizationConfig  {


	private static CommandData LanguageJson;

	public LocalizationConfig(CommandData json)
	{
		LanguageJson = json;
	}

	public static string getText (string key )
	{
		//return localization 
	//	return LanguageJson.GetString(key);
		//return Only En
		return key;
	}

}
