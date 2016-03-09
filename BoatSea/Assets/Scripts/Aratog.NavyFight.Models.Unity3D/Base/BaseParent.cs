using System;
using System.Collections.Generic;
using LinqTools;
using System.Text;
using Aratog.NavyFight.Models.Unity3D.Flags;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Unity3D.Players;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.Base {
	public class BaseParent {
		#region Constants

		public const int GuardRadius = 10;

		#endregion

		#region Vaiables

     


		public TeamColor Team;

		public BaseType Type;

		public Vector3 Position { get; private set; }

		public Point Size;

		public bool IsFlagOnSpot
		{
			get { return Flag != null && Flag.State == FlagState.OnBase; }
		}

		public FlagParent Flag;

		public BaseState State { get; private set; }

		public List<Vector3> AlarmZone;

		public List<Vector3> GuardZone;

		public delegate void OnAlarmOnHandler();

		public delegate void OnAlarmOffHandler();

		private OnAlarmOffHandler onAlarmOffHandler;

		private OnAlarmOnHandler onAlarmOnHandler;

		public event OnAlarmOnHandler OnAlarmOnEvent
		{
			add { onAlarmOnHandler += value; }
			remove { onAlarmOnHandler -= value; }
		}

		public event OnAlarmOffHandler OnAlarmOffEvent
		{
			add { onAlarmOffHandler += value; }
			remove { onAlarmOffHandler -= value; }
		}

		private Vector3 TopLeftAlarmZonePosition
		{
			get { return AlarmZone[0]; }
		}

		private Vector3 TopRightAlarmZonePostion
		{
			get { return AlarmZone[1]; }
		}

		private Vector3 BottomRightAlarmZonePosition
		{
			get { return AlarmZone[2]; }
		}

		private Vector3 BottomLeftAlarmZonePosition
		{
			get { return AlarmZone[3]; }
		}

		private Vector3 TopLeftGuardZonePosition
		{
			get { return GuardZone[0]; }
		}

		private Vector3 BottomRightGuardZonePosition
		{
			get { return GuardZone[2]; }
		}
		#endregion

		#region Condtructors

		public BaseParent (Vector3 position, FlagParent flag, BaseType type = BaseType.FlagKeeper, TeamColor color = TeamColor.BlueTeam) {
			Team = color;
			Type = type;
			Position = position;
			Size = new Point(1, 1);
			//IsFlagOnSpot = true;
			Flag = flag;
			Flag.Base = this;
			State = BaseState.Normal;
			AlarmZone = new List<Vector3>();
			GuardZone = new List<Vector3>();
		}

		public static BaseParent Create (Vector3 position, FlagParent flag, BaseType type = BaseType.FlagKeeper, TeamColor color = TeamColor.BlueTeam) {
			return new BaseParent(position, flag, type, color);
		}

		#endregion

		#region Events

		public void OnFlagTaken () {
			//IsFlagOnSpot = false;
		}

		public void OnFlagReturned () {
			//IsFlagOnSpot = true;
		}

		public void OnFlagDelivered () {
			//IsFlagOnSpot = true;
		}

         

		private void OnAlarmOn()
		{
			if (onAlarmOnHandler != null)
			{
				onAlarmOnHandler();
			}

		}

		private void OnAlarmOff()
		{
			if (onAlarmOffHandler != null)
			{
				onAlarmOffHandler();
			}
		}

		public void AlarmOn()
		{
			State = BaseState.Alarm;
			OnAlarmOn();
		}

		public void AlarmOff()
		{
			State = BaseState.Normal;
			OnAlarmOff();
		}

		public bool IsIntersectAlarmZone(List<IPositionable> positionables)
		{
			if (AlarmZone == null)
				return false;

			if (AlarmZone.Count < 3)
				return false;

			foreach (IPositionable positionable in positionables)
			{
				//Debug.Log(string.Format("Some positionable trying intersect the alarmZone: {0}",positionable.GetType()));

				if (positionable.Position.x >= TopLeftAlarmZonePosition.x && positionable.Position.z >= TopLeftAlarmZonePosition.z
				    && positionable.Position.x <= BottomRightAlarmZonePosition.x &&
				    positionable.Position.z <= BottomRightAlarmZonePosition.z)
				{
					//Debug.Log(string.Format("Some positionable are intersect the alarmZone: {0}",positionable.GetType()));
					if (positionable is Player)
					{
						if ((positionable as Player).Team != Team)
							return true;
					}
				}
			}

			return false;
		}

		public bool IsAiShipPlacedNearBase(IPositionable positionable)
		{
			if (positionable.Position.x >= TopLeftGuardZonePosition.x && positionable.Position.z >= TopLeftGuardZonePosition.z
			    && positionable.Position.x <= BottomRightGuardZonePosition.x &&
			    positionable.Position.z <= BottomRightGuardZonePosition.z)
			{
				//Debug.Log(string.Format("Some positionable are intersect the alarmZone: {0}",positionable.GetType()));
				if (positionable is Player)
				{
					if ((positionable as Player).Team == Team)
					{
						Debug.Log(string.Format("Enemy ship spied, id: {0}", (positionable as Player).Id));
						return true;
					}
				}
			}

			return false;
		}

		#endregion
	}
}
