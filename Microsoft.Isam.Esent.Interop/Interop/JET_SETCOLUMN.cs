using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_SETCOLUMN : IContentEquatable<JET_SETCOLUMN>, IDeepCloneable<JET_SETCOLUMN>
	{
		public JET_COLUMNID columnid { get; set; }

		public byte[] pvData { get; set; }

		public int ibData { get; set; }

		public int cbData { get; set; }

		public SetColumnGrbit grbit { get; set; }

		public int ibLongValue { get; set; }

		public int itagSequence { get; set; }

		public JET_wrn err { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SETCOLUMN(0x{0:x},{1},ibLongValue={2},itagSequence={3})", new object[]
			{
				this.columnid.Value,
				Util.DumpBytes(this.pvData, this.ibData, this.cbData),
				this.ibLongValue,
				this.itagSequence
			});
		}

		public bool ContentEquals(JET_SETCOLUMN other)
		{
			if (other == null)
			{
				return false;
			}
			this.CheckDataSize();
			other.CheckDataSize();
			return this.columnid == other.columnid && this.ibData == other.ibData && this.cbData == other.cbData && this.grbit == other.grbit && this.ibLongValue == other.ibLongValue && this.itagSequence == other.itagSequence && this.err == other.err && Util.ArrayEqual(this.pvData, other.pvData, this.ibData, this.cbData);
		}

		public JET_SETCOLUMN DeepClone()
		{
			JET_SETCOLUMN jet_SETCOLUMN = (JET_SETCOLUMN)base.MemberwiseClone();
			if (this.pvData != null)
			{
				jet_SETCOLUMN.pvData = new byte[this.pvData.Length];
				Array.Copy(this.pvData, jet_SETCOLUMN.pvData, this.cbData);
			}
			return jet_SETCOLUMN;
		}

		internal void CheckDataSize()
		{
			if (this.cbData < 0)
			{
				throw new ArgumentOutOfRangeException("cbData", "data length cannot be negative");
			}
			if (this.ibData < 0)
			{
				throw new ArgumentOutOfRangeException("ibData", "data offset cannot be negative");
			}
			if (this.ibData != 0 && (this.pvData == null || this.ibData >= this.pvData.Length))
			{
				throw new ArgumentOutOfRangeException("ibData", this.ibData, "cannot be greater than the length of the pvData");
			}
			if ((this.pvData == null && this.cbData != 0) || (this.pvData != null && this.cbData > this.pvData.Length - this.ibData))
			{
				throw new ArgumentOutOfRangeException("cbData", this.cbData, "cannot be greater than the length of the pvData");
			}
			if (this.itagSequence < 0)
			{
				throw new ArgumentOutOfRangeException("itagSequence", this.itagSequence, "cannot be negative");
			}
			if (this.ibLongValue < 0)
			{
				throw new ArgumentOutOfRangeException("ibLongValue", this.ibLongValue, "cannot be negative");
			}
		}

		internal NATIVE_SETCOLUMN GetNativeSetcolumn()
		{
			return checked(new NATIVE_SETCOLUMN
			{
				columnid = this.columnid.Value,
				cbData = (uint)this.cbData,
				grbit = (uint)this.grbit,
				ibLongValue = (uint)this.ibLongValue,
				itagSequence = (uint)this.itagSequence
			});
		}
	}
}
