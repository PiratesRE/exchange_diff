using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public sealed class JET_INDEXLIST
	{
		public JET_TABLEID tableid { get; internal set; }

		public int cRecord { get; internal set; }

		public JET_COLUMNID columnidindexname { get; internal set; }

		public JET_COLUMNID columnidgrbitIndex { get; internal set; }

		public JET_COLUMNID columnidcKey { get; internal set; }

		public JET_COLUMNID columnidcEntry { get; internal set; }

		public JET_COLUMNID columnidcPage { get; internal set; }

		public JET_COLUMNID columnidcColumn { get; internal set; }

		public JET_COLUMNID columnidiColumn { get; internal set; }

		public JET_COLUMNID columnidcolumnid { get; internal set; }

		public JET_COLUMNID columnidcoltyp { get; internal set; }

		public JET_COLUMNID columnidLangid { get; internal set; }

		public JET_COLUMNID columnidCp { get; internal set; }

		public JET_COLUMNID columnidgrbitColumn { get; internal set; }

		public JET_COLUMNID columnidcolumnname { get; internal set; }

		public JET_COLUMNID columnidLCMapFlags { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_INDEXLIST(0x{0:x},{1} records)", new object[]
			{
				this.tableid,
				this.cRecord
			});
		}

		internal void SetFromNativeIndexlist(NATIVE_INDEXLIST value)
		{
			this.tableid = new JET_TABLEID
			{
				Value = value.tableid
			};
			this.cRecord = checked((int)value.cRecord);
			this.columnidindexname = new JET_COLUMNID
			{
				Value = value.columnidindexname
			};
			this.columnidgrbitIndex = new JET_COLUMNID
			{
				Value = value.columnidgrbitIndex
			};
			this.columnidcKey = new JET_COLUMNID
			{
				Value = value.columnidcKey
			};
			this.columnidcEntry = new JET_COLUMNID
			{
				Value = value.columnidcEntry
			};
			this.columnidcPage = new JET_COLUMNID
			{
				Value = value.columnidcPage
			};
			this.columnidcColumn = new JET_COLUMNID
			{
				Value = value.columnidcColumn
			};
			this.columnidiColumn = new JET_COLUMNID
			{
				Value = value.columnidiColumn
			};
			this.columnidcolumnid = new JET_COLUMNID
			{
				Value = value.columnidcolumnid
			};
			this.columnidcoltyp = new JET_COLUMNID
			{
				Value = value.columnidcoltyp
			};
			this.columnidLangid = new JET_COLUMNID
			{
				Value = value.columnidLangid
			};
			this.columnidCp = new JET_COLUMNID
			{
				Value = value.columnidCp
			};
			this.columnidgrbitColumn = new JET_COLUMNID
			{
				Value = value.columnidgrbitColumn
			};
			this.columnidcolumnname = new JET_COLUMNID
			{
				Value = value.columnidcolumnname
			};
			this.columnidLCMapFlags = new JET_COLUMNID
			{
				Value = value.columnidLCMapFlags
			};
		}
	}
}
