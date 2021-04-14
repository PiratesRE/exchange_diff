using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_OBJECTLIST
	{
		public JET_TABLEID tableid { get; internal set; }

		public int cRecord { get; internal set; }

		public JET_COLUMNID columnidobjectname { get; internal set; }

		public JET_COLUMNID columnidobjtyp { get; internal set; }

		public JET_COLUMNID columnidgrbit { get; internal set; }

		public JET_COLUMNID columnidflags { get; internal set; }

		public JET_COLUMNID columnidcRecord { get; internal set; }

		public JET_COLUMNID columnidcontainername { get; internal set; }

		public JET_COLUMNID columnidcPage { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_OBJECTLIST(0x{0:x},{1} records)", new object[]
			{
				this.tableid,
				this.cRecord
			});
		}

		internal void SetFromNativeObjectlist(NATIVE_OBJECTLIST value)
		{
			this.tableid = new JET_TABLEID
			{
				Value = value.tableid
			};
			this.cRecord = checked((int)value.cRecord);
			this.columnidobjectname = new JET_COLUMNID
			{
				Value = value.columnidobjectname
			};
			this.columnidobjtyp = new JET_COLUMNID
			{
				Value = value.columnidobjtyp
			};
			this.columnidgrbit = new JET_COLUMNID
			{
				Value = value.columnidgrbit
			};
			this.columnidflags = new JET_COLUMNID
			{
				Value = value.columnidflags
			};
			this.columnidcRecord = new JET_COLUMNID
			{
				Value = value.columnidcRecord
			};
			this.columnidcPage = new JET_COLUMNID
			{
				Value = value.columnidcPage
			};
			this.columnidcontainername = new JET_COLUMNID
			{
				Value = value.columnidcontainername
			};
		}
	}
}
