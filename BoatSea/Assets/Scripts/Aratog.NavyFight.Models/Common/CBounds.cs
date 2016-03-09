namespace Aratog.NavyFight.Models.Common
{
	public class CBounds
	{
		public CVector3 Center;
		public CVector3 Extents;

		public CBounds(CVector3 center, CVector3 extents)
		{
			Center = center;
			Extents = extents;
		}
	}
}
