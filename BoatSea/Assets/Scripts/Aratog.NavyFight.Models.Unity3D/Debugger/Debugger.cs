using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.Debugger {
	public static class Debugger
	{
		public static bool IsNeedShowDebugLog = true;

		public static void Log(object msg)
		{
			if (!IsNeedShowDebugLog)
				return;

			Debug.Log(msg);
		}

		public static void LogError(object msg)
		{
			if (!IsNeedShowDebugLog)
				return;

			Debug.LogError(msg);
		}
	}
}
