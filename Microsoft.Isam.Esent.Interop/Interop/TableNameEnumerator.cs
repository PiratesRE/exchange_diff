using System;
using System.Text;

namespace Microsoft.Isam.Esent.Interop
{
	internal sealed class TableNameEnumerator : TableEnumerator<string>
	{
		public TableNameEnumerator(JET_SESID sesid, JET_DBID dbid) : base(sesid)
		{
			this.dbid = dbid;
		}

		protected override void OpenTable()
		{
			Api.JetGetObjectInfo(base.Sesid, this.dbid, out this.objectlist);
			base.TableidToEnumerate = this.objectlist.tableid;
		}

		protected override bool SkipCurrent()
		{
			int value = Api.RetrieveColumnAsInt32(base.Sesid, base.TableidToEnumerate, this.objectlist.columnidflags).Value;
			return int.MinValue == (value & int.MinValue);
		}

		protected override string GetCurrent()
		{
			Encoding encoding = EsentVersion.SupportsVistaFeatures ? Encoding.Unicode : LibraryHelpers.EncodingASCII;
			string s = Api.RetrieveColumnAsString(base.Sesid, base.TableidToEnumerate, this.objectlist.columnidobjectname, encoding, RetrieveColumnGrbit.None);
			return StringCache.TryToIntern(s);
		}

		private readonly JET_DBID dbid;

		private JET_OBJECTLIST objectlist;
	}
}
