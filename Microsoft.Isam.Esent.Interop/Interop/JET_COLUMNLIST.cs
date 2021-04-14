using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_COLUMNLIST
	{
		public JET_TABLEID tableid { get; internal set; }

		public int cRecord { get; internal set; }

		public JET_COLUMNID columnidcolumnname { get; internal set; }

		public JET_COLUMNID columnidcolumnid { get; internal set; }

		public JET_COLUMNID columnidcoltyp { get; internal set; }

		public JET_COLUMNID columnidCp { get; internal set; }

		public JET_COLUMNID columnidcbMax { get; internal set; }

		public JET_COLUMNID columnidgrbit { get; internal set; }

		public JET_COLUMNID columnidDefault { get; internal set; }

		public JET_COLUMNID columnidBaseTableName { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_COLUMNLIST(0x{0:x},{1} records)", new object[]
			{
				this.tableid,
				this.cRecord
			});
		}

		internal void SetFromNativeColumnlist(NATIVE_COLUMNLIST value)
		{
			this.tableid = new JET_TABLEID
			{
				Value = value.tableid
			};
			this.cRecord = checked((int)value.cRecord);
			this.columnidcolumnname = new JET_COLUMNID
			{
				Value = value.columnidcolumnname
			};
			this.columnidcolumnid = new JET_COLUMNID
			{
				Value = value.columnidcolumnid
			};
			this.columnidcoltyp = new JET_COLUMNID
			{
				Value = value.columnidcoltyp
			};
			this.columnidCp = new JET_COLUMNID
			{
				Value = value.columnidCp
			};
			this.columnidcbMax = new JET_COLUMNID
			{
				Value = value.columnidcbMax
			};
			this.columnidgrbit = new JET_COLUMNID
			{
				Value = value.columnidgrbit
			};
			this.columnidDefault = new JET_COLUMNID
			{
				Value = value.columnidDefault
			};
			this.columnidBaseTableName = new JET_COLUMNID
			{
				Value = value.columnidBaseTableName
			};
		}
	}
}
