using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_ENUMCOLUMN
	{
		public JET_COLUMNID columnid { get; internal set; }

		public JET_wrn err { get; internal set; }

		public int cEnumColumnValue { get; internal set; }

		public JET_ENUMCOLUMNVALUE[] rgEnumColumnValue { get; internal set; }

		public int cbData { get; internal set; }

		public IntPtr pvData { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_ENUMCOLUMN(0x{0:x})", new object[]
			{
				this.columnid
			});
		}

		internal void SetFromNativeEnumColumn(NATIVE_ENUMCOLUMN value)
		{
			this.columnid = new JET_COLUMNID
			{
				Value = value.columnid
			};
			this.err = (JET_wrn)value.err;
			checked
			{
				if (JET_wrn.ColumnSingleValue == this.err)
				{
					this.cbData = (int)value.cbData;
					this.pvData = value.pvData;
					return;
				}
				this.cEnumColumnValue = (int)value.cEnumColumnValue;
				this.rgEnumColumnValue = null;
			}
		}
	}
}
