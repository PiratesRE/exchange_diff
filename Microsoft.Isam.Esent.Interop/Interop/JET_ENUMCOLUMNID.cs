using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_ENUMCOLUMNID
	{
		public JET_COLUMNID columnid { get; set; }

		public int ctagSequence { get; set; }

		public int[] rgtagSequence { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_ENUMCOLUMNID(0x{0:x})", new object[]
			{
				this.columnid
			});
		}

		internal void CheckDataSize()
		{
			if (this.ctagSequence < 0)
			{
				throw new ArgumentOutOfRangeException("ctagSequence", "ctagSequence cannot be negative");
			}
			if ((this.rgtagSequence == null && this.ctagSequence != 0) || (this.rgtagSequence != null && this.ctagSequence > this.rgtagSequence.Length))
			{
				throw new ArgumentOutOfRangeException("ctagSequence", this.ctagSequence, "cannot be greater than the length of the pvData");
			}
		}

		internal NATIVE_ENUMCOLUMNID GetNativeEnumColumnid()
		{
			this.CheckDataSize();
			return new NATIVE_ENUMCOLUMNID
			{
				columnid = this.columnid.Value,
				ctagSequence = checked((uint)this.ctagSequence)
			};
		}
	}
}
