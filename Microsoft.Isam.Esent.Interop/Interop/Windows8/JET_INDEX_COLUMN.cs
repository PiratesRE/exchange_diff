using System;
using System.Globalization;
using Microsoft.Isam.Esent.Interop.Implementation;

namespace Microsoft.Isam.Esent.Interop.Windows8
{
	public class JET_INDEX_COLUMN
	{
		public JET_COLUMNID columnid { get; set; }

		public JetRelop relop { get; set; }

		public byte[] pvData { get; set; }

		public JetIndexColumnGrbit grbit { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_INDEX_COLUMN(0x{0:x})", new object[]
			{
				this.columnid
			});
		}

		internal NATIVE_INDEX_COLUMN GetNativeIndexColumn(ref GCHandleCollection handles)
		{
			NATIVE_INDEX_COLUMN result = default(NATIVE_INDEX_COLUMN);
			result.columnid = this.columnid.Value;
			result.relop = (uint)this.relop;
			result.grbit = (uint)this.grbit;
			if (this.pvData != null)
			{
				result.pvData = handles.Add(this.pvData);
				result.cbData = (uint)this.pvData.Length;
			}
			return result;
		}
	}
}
