using System;
using System.Text;

namespace Microsoft.Isam.Esent.Interop
{
	internal abstract class ColumnInfoEnumerator : TableEnumerator<ColumnInfo>
	{
		protected ColumnInfoEnumerator(JET_SESID sesid) : base(sesid)
		{
		}

		protected JET_COLUMNLIST Columnlist { get; set; }

		protected override ColumnInfo GetCurrent()
		{
			return ColumnInfoEnumerator.GetColumnInfoFromColumnlist(base.Sesid, this.Columnlist);
		}

		private static ColumnInfo GetColumnInfoFromColumnlist(JET_SESID sesid, JET_COLUMNLIST columnlist)
		{
			Encoding encoding = EsentVersion.SupportsVistaFeatures ? Encoding.Unicode : LibraryHelpers.EncodingASCII;
			string text = Api.RetrieveColumnAsString(sesid, columnlist.tableid, columnlist.columnidcolumnname, encoding, RetrieveColumnGrbit.None);
			text = StringCache.TryToIntern(text);
			uint value = Api.RetrieveColumnAsUInt32(sesid, columnlist.tableid, columnlist.columnidcolumnid).Value;
			uint value2 = Api.RetrieveColumnAsUInt32(sesid, columnlist.tableid, columnlist.columnidcoltyp).Value;
			uint value3 = (uint)Api.RetrieveColumnAsUInt16(sesid, columnlist.tableid, columnlist.columnidCp).Value;
			uint value4 = Api.RetrieveColumnAsUInt32(sesid, columnlist.tableid, columnlist.columnidcbMax).Value;
			byte[] defaultValue = Api.RetrieveColumn(sesid, columnlist.tableid, columnlist.columnidDefault);
			uint value5 = Api.RetrieveColumnAsUInt32(sesid, columnlist.tableid, columnlist.columnidgrbit).Value;
			return new ColumnInfo(text, new JET_COLUMNID
			{
				Value = value
			}, (JET_coltyp)value2, (JET_CP)value3, (int)value4, defaultValue, (ColumndefGrbit)value5);
		}
	}
}
