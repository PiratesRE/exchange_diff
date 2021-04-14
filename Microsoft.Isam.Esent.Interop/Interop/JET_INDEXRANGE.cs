using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_INDEXRANGE : IContentEquatable<JET_INDEXRANGE>, IDeepCloneable<JET_INDEXRANGE>
	{
		public JET_INDEXRANGE()
		{
			this.grbit = IndexRangeGrbit.RecordInIndex;
		}

		public JET_TABLEID tableid { get; set; }

		public IndexRangeGrbit grbit { get; set; }

		public JET_INDEXRANGE DeepClone()
		{
			return (JET_INDEXRANGE)base.MemberwiseClone();
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_INDEXRANGE(0x{0:x},{1})", new object[]
			{
				this.tableid.Value,
				this.grbit
			});
		}

		public bool ContentEquals(JET_INDEXRANGE other)
		{
			return other != null && this.tableid == other.tableid && this.grbit == other.grbit;
		}

		internal NATIVE_INDEXRANGE GetNativeIndexRange()
		{
			return new NATIVE_INDEXRANGE
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_INDEXRANGE)),
				tableid = this.tableid.Value,
				grbit = (uint)this.grbit
			};
		}
	}
}
