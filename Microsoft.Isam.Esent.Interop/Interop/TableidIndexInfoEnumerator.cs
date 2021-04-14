using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal sealed class TableidIndexInfoEnumerator : IndexInfoEnumerator
	{
		public TableidIndexInfoEnumerator(JET_SESID sesid, JET_TABLEID tableid) : base(sesid)
		{
			this.tableid = tableid;
		}

		protected override void OpenTable()
		{
			JET_INDEXLIST indexlist;
			Api.JetGetTableIndexInfo(base.Sesid, this.tableid, string.Empty, out indexlist, JET_IdxInfo.List);
			base.Indexlist = indexlist;
			base.TableidToEnumerate = base.Indexlist.tableid;
		}

		protected override void GetIndexInfo(JET_SESID sesid, string indexname, out string result, JET_IdxInfo infoLevel)
		{
			Api.JetGetTableIndexInfo(sesid, this.tableid, indexname, out result, infoLevel);
		}

		private readonly JET_TABLEID tableid;
	}
}
