using Aratog.NavyFight.Models.Common;
using UnityEngine;

namespace Aratog.NavyFight.Models.Unity3D.Converters
{
	public static class ConverterHelpers
	{
		public static Vector3 ToVector3(this CVector3 source)
		{
			return new Vector3(source.X, source.Y, source.Z);
		}

		public static CVector3 ToCVector3(this Vector3 source)
		{
			return new CVector3(source.x, source.y, source.z);
		}

		public static Bounds ToBounds(this CBounds source)
		{
			return new Bounds(source.Center.ToVector3(), source.Extents.ToVector3());
		}

		public static CBounds ToCBounds(this Bounds source)
		{
			return new CBounds(source.center.ToCVector3(), source.extents.ToCVector3());
		}

		public static Rect ToRect(this CRect source)
		{
			return new Rect(source.X, source.Y, source.Width, source.Height);
		}

		public static CRect ToCRect(this Rect source)
		{
			return new CRect(source.x, source.y, source.width, source.height);
		}
	}
}