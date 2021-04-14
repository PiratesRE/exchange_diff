using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_ENUMCOLUMNVALUE
	{
		public int itagSequence { get; internal set; }

		public JET_wrn err { get; internal set; }

		public int cbData { get; internal set; }

		public IntPtr pvData { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_ENUMCOLUMNVALUE(itagSequence = {0}, cbData = {1})", new object[]
			{
				this.itagSequence,
				this.cbData
			});
		}

		internal void SetFromNativeEnumColumnValue(NATIVE_ENUMCOLUMNVALUE value)
		{
			checked
			{
				this.itagSequence = (int)value.itagSequence;
				this.err = (JET_wrn)value.err;
				this.cbData = (int)value.cbData;
				this.pvData = value.pvData;
			}
		}
	}
}
