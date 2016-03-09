using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Base;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.Flags {
	public class FlagParent {
		#region Variables

		public TeamColor Color;

		public Vector3 Position;

		public FlagState State { get; set; }

		//public readonly Vector3 BasePosition;

		public BaseParent Base;

		#endregion

		#region Constructor

		public FlagParent (TeamColor color, Vector3 basePosiion) {
			Color = color;

			State = FlagState.OnBase;

			Position = Vector3.zero;

			Position = basePosiion;

			//Debug.Log(string.Format("Create {0} flag, base position:{1}", color, basePosiion));
		}

		public static FlagParent Create (TeamColor color, Vector3 basePosition) {
			return new FlagParent(color, basePosition);
		}

		#endregion

		#region Events

		public void OnFlagTaken () {
			State = FlagState.Taken;
			Base.OnFlagTaken();
			//Debug.Log(string.Format("FlagParent: OnFlagTaken, flag state is {0}", State));
		}

		public void OnFlagDropped () {
			State = FlagState.Dropped;
			//Debug.Log(string.Format("FlagParent: OnFlagDropped, flag state is {0}", State));
		}

		public void OnFlagReturned () {
			State = FlagState.OnBase;
			Position = Base.Position;
			Base.OnFlagReturned();
		}

		public void OnFlagDelivered () {
			State = FlagState.OnBase;
			Position = Base.Position;
			Base.OnFlagDelivered();
		}

		#endregion
	}
}
