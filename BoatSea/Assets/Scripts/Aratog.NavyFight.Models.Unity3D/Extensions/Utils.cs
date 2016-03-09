using UnityEngine;
using System.Collections.Generic;
using Aratog.NavyFight.Models.Unity3D.Maps;
using Aratog.NavyFight.Models.Maps;
using Aratog.NavyFight.Models.Ships;
using Aratog.NavyFight.Models.Unity3D.Players;

namespace Aratog.NavyFight.Models.Unity3D.Extensions
{
	public static class Utils
	{
		public static void Swap<T>(ref T left, ref T right)
		{
			T temp;
			temp = left;
			left = right;
			right = temp;
		}

		public static List<T> Unique<T>(this IEnumerable<T> list)
		{
			List<T> result = new List<T>();

			foreach (T item in list)
			{
				if (result.Contains(item))
					continue;

				result.Add(item);
			}

			return result;
		}

		#region Converters

		public static Vector2 To2D(this Vector3 vector, bool xz = false)
		{
			return (xz ? new Vector2(vector.x, vector.z) : new Vector2(vector.x, vector.y));
		}

		public static Vector3 To3D(this Vector2 vector, bool xz = false)
		{
			return xz ? new Vector3(vector.x, 0, vector.y) : new Vector3(vector.x, vector.y, 0);
		}

		public static Point ToPoint(this Vector3 vector, bool xz = false)
		{
			return xz ? new Point((int) vector.x, (int) vector.z) : new Point((int) vector.x, (int) vector.y);
		}

		public static Vector2 ToVector(this Point point)
		{
			return new Vector2(point.X, point.Y);
		}

		#region Helpers

		public static Vector3 ToXZ(this Vector3 vector)
		{
			return new Vector3(vector.x, 0, vector.y);
		}

		public static Vector3 ToXZ(this Vector2 vector)
		{
			return new Vector3(vector.x, 0, vector.y);
		}

		public static Vector2 ToVector2(this Direction direction)
		{
			Vector2 vector = Vector2.zero;
			switch (direction)
			{
				case Direction.LEFT:
					vector = new Vector2(-1, 0);
					break;
				case Direction.UP:
					vector = new Vector2(0, 1);
					break;
				case Direction.RIGHT:
					vector = new Vector2(1, 0);
					break;
				case Direction.DOWN:
					vector = new Vector2(0, -1);
					break;
			}
			return vector;
		}

		public static Vector3 ToVector3(this Direction direction)
		{
			Vector3 vector = Vector3.zero;
			switch (direction)
			{
				case Direction.LEFT:
					vector = new Vector3(-1, 0, 0);
					break;
				case Direction.UP:
					vector = new Vector3(0, 0, 1);
					break;
				case Direction.RIGHT:
					vector = new Vector3(1, 0, 0);
					break;
				case Direction.DOWN:
					vector = new Vector3(0, 0, -1);
					break;
			}
			return vector;
		}

		public static Vector3 normilizedWithoutY(this Vector3 vector)
		{
			return (new Vector3(vector.x, 0, vector.y)).normalized;
		}

		public static void SetPos(this Transform tm, float? x = null, float? y = null, float? z = null)
		{
			tm.position = new Vector3(x ?? tm.position.x, y ?? tm.position.y, z ?? tm.position.z);
		}

		public static Vector3 Set(this Vector3 v, float? x = null, float? y = null, float? z = null)
		{
			return new Vector3(x ?? v.x, y ?? v.y, z ?? v.z);
		}

		#endregion

		#endregion

		public static Quaternion EaseInQuad(Quaternion from, Quaternion to, float value)
		{
			float x = easeInQuad(from.x, to.x, value);
			float y = easeInQuad(from.y, to.y, value);
			float z = easeInQuad(from.z, to.z, value);
			float w = easeInQuad(from.w, to.w, value);
			return new Quaternion(x, y, z, w);
		}

		private static float easeInQuad(float start, float end, float value)
		{
			end -= start;
			return end*value*value + start;
		}

		#region Vector3 rotation

		public static Vector3 RotateX(this Vector3 v, float angle)

		{
			Vector3 vR = new Vector3(v.x, v.y, v.z);

			float sin = Mathf.Sin(angle);

			float cos = Mathf.Cos(angle);



			float ty = v.y;

			float tz = v.z;

			vR.y = (cos*ty) - (sin*tz);

			vR.z = (cos*tz) + (sin*ty);

			return vR;
		}



		public static Vector3 RotateY(this Vector3 v, float angle)

		{
			Vector3 vR = new Vector3(v.x, v.y, v.z);

			float sin = Mathf.Sin(angle);

			float cos = Mathf.Cos(angle);

			
			float tx = v.x;

			float tz = v.z;

			vR.x = (cos*tx) + (sin*tz);

			vR.z = (cos*tz) - (sin*tx);

			return vR;
		}



		public static Vector3 RotateZ(this Vector3 v, float angle)

		{
			Vector3 vR = new Vector3(v.x, v.y, v.z);

			float sin = Mathf.Sin(angle);

			float cos = Mathf.Cos(angle);



			float tx = v.x;

			float ty = v.y;

			vR.x = (cos*tx) - (sin*ty);

			vR.y = (cos*ty) + (sin*tx);

			return vR;
		}

		#endregion
	}
}