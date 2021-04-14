using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal sealed class IntersectIndexesEnumerator : TableEnumerator<byte[]>
	{
		public IntersectIndexesEnumerator(JET_SESID sesid, JET_INDEXRANGE[] ranges) : base(sesid)
		{
			this.ranges = ranges;
		}

		protected override void OpenTable()
		{
			Api.JetIntersectIndexes(base.Sesid, this.ranges, this.ranges.Length, out this.recordlist, IntersectIndexesGrbit.None);
			base.TableidToEnumerate = this.recordlist.tableid;
		}

		protected override byte[] GetCurrent()
		{
			return Api.RetrieveColumn(base.Sesid, base.TableidToEnumerate, this.recordlist.columnidBookmark);
		}

		private readonly JET_INDEXRANGE[] ranges;

		private JET_RECORDLIST recordlist;
	}
}
