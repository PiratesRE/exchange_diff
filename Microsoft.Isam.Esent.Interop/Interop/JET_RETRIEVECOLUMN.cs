using System;
using System.Globalization;

namespace Microsoft.Isam.Esent.Interop
{
	public class JET_RETRIEVECOLUMN
	{
		public JET_COLUMNID columnid { get; set; }

		public byte[] pvData { get; set; }

		public int ibData { get; set; }

		public int cbData { get; set; }

		public int cbActual { get; private set; }

		public RetrieveColumnGrbit grbit { get; set; }

		public int ibLongValue { get; set; }

		public int itagSequence { get; set; }

		public JET_COLUMNID columnidNextTagged { get; private set; }

		public JET_wrn err { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_RETRIEVECOLUMN(0x{0:x})", new object[]
			{
				this.columnid
			});
		}

		internal void CheckDataSize()
		{
			if (this.cbData < 0)
			{
				throw new ArgumentOutOfRangeException("cbData", this.cbData, "data length cannot be negative");
			}
			if (this.ibData < 0)
			{
				throw new ArgumentOutOfRangeException("ibData", this.cbData, "data offset cannot be negative");
			}
			if (this.ibData != 0 && (this.pvData == null || this.ibData >= this.pvData.Length))
			{
				throw new ArgumentOutOfRangeException("ibData", this.ibData, "cannot be greater than the length of the pvData buffer");
			}
			if ((this.pvData == null && this.cbData != 0) || (this.pvData != null && this.cbData > this.pvData.Length - this.ibData))
			{
				throw new ArgumentOutOfRangeException("cbData", this.cbData, "cannot be greater than the length of the pvData buffer");
			}
		}

		internal void GetNativeRetrievecolumn(ref NATIVE_RETRIEVECOLUMN retrievecolumn)
		{
			retrievecolumn.columnid = this.columnid.Value;
			retrievecolumn.cbData = (uint)this.cbData;
			retrievecolumn.grbit = (uint)this.grbit;
			checked
			{
				retrievecolumn.ibLongValue = (uint)this.ibLongValue;
				retrievecolumn.itagSequence = (uint)this.itagSequence;
			}
		}

		internal void UpdateFromNativeRetrievecolumn(ref NATIVE_RETRIEVECOLUMN native)
		{
			checked
			{
				this.cbActual = (int)native.cbActual;
				this.columnidNextTagged = new JET_COLUMNID
				{
					Value = native.columnidNextTagged
				};
				this.itagSequence = (int)native.itagSequence;
				this.err = (JET_wrn)native.err;
			}
		}
	}
}
