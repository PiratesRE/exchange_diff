using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_RECORDLIST
	{
		public JET_TABLEID tableid { get; internal set; }

		public int cRecords { get; internal set; }

		public JET_COLUMNID columnidBookmark { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_RECORDLIST(0x{0:x},{1} records)", new object[]
			{
				this.tableid,
				this.cRecords
			});
		}

		internal void SetFromNativeRecordlist(NATIVE_RECORDLIST value)
		{
			this.tableid = new JET_TABLEID
			{
				Value = value.tableid
			};
			this.cRecords = checked((int)value.cRecords);
			this.columnidBookmark = new JET_COLUMNID
			{
				Value = value.columnidBookmark
			};
		}
	}
}
