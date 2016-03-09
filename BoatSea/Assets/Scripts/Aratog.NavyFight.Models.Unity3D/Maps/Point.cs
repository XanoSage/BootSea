//using System.Collections.Generic;
using Aratog.NavyFight.Models.Games;
using Aratog.NavyFight.Models.Maps;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.Maps
{
	[System.Serializable]
	public class Point
	{
		public int X, Y;

		public Point () {
			X = 0;
			Y = 0;
		}
		
		public Point(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	
		public static bool operator ==(Point a, Point b)
		{
		    // If both are null, or both are same instance, return true.
		    if (System.Object.ReferenceEquals(a, b))
		    {
		        return true;
		    }
		
		    // If one is null, but not both, return false.
		    if (((object)a == null) || ((object)b == null))
		    {
		        return false;
		    }
		
		    // Return true if the fields match:
		    return a.X == b.X && a.Y == b.Y;
		}
	
		public static bool operator !=(Point a, Point b)
		{
		    return !(a == b);
		}

		public override string ToString () {
			return string.Format("X:{0}, Y:{1}", X, Y);
			//return base.ToString();
		}
	}
}
