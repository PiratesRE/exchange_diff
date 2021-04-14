using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal sealed class TableidColumnInfoEnumerator : ColumnInfoEnumerator
	{
		public TableidColumnInfoEnumerator(JET_SESID sesid, JET_TABLEID tableid) : base(sesid)
		{
			this.tableid = tableid;
		}

		protected override void OpenTable()
		{
			JET_COLUMNLIST columnlist;
			Api.JetGetTableColumnInfo(base.Sesid, this.tableid, string.Empty, out columnlist);
			base.Columnlist = columnlist;
			base.TableidToEnumerate = base.Columnlist.tableid;
		}

		private readonly JET_TABLEID tableid;
	}
}
