using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal sealed class TableIndexInfoEnumerator : IndexInfoEnumerator
	{
		public TableIndexInfoEnumerator(JET_SESID sesid, JET_DBID dbid, string tablename) : base(sesid)
		{
			this.dbid = dbid;
			this.tablename = tablename;
		}

		protected override void OpenTable()
		{
			JET_INDEXLIST indexlist;
			Api.JetGetIndexInfo(base.Sesid, this.dbid, this.tablename, string.Empty, out indexlist, JET_IdxInfo.List);
			base.Indexlist = indexlist;
			base.TableidToEnumerate = base.Indexlist.tableid;
		}

		protected override void GetIndexInfo(JET_SESID sesid, string indexname, out string result, JET_IdxInfo infoLevel)
		{
			Api.JetGetIndexInfo(sesid, this.dbid, this.tablename, indexname, out result, infoLevel);
		}

		private readonly JET_DBID dbid;

		private readonly string tablename;
	}
}
