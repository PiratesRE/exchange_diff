using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal sealed class TableColumnInfoEnumerator : ColumnInfoEnumerator
	{
		public TableColumnInfoEnumerator(JET_SESID sesid, JET_DBID dbid, string tablename) : base(sesid)
		{
			this.dbid = dbid;
			this.tablename = tablename;
		}

		protected override void OpenTable()
		{
			JET_COLUMNLIST columnlist;
			Api.JetGetColumnInfo(base.Sesid, this.dbid, this.tablename, string.Empty, out columnlist);
			base.Columnlist = columnlist;
			base.TableidToEnumerate = base.Columnlist.tableid;
		}

		private readonly JET_DBID dbid;

		private readonly string tablename;
	}
}
