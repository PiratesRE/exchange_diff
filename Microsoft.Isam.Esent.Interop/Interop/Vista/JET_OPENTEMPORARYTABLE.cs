using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Vista
{
	public class JET_OPENTEMPORARYTABLE
	{
		public JET_COLUMNDEF[] prgcolumndef { get; set; }

		public int ccolumn { get; set; }

		public JET_UNICODEINDEX pidxunicode { get; set; }

		public TempTableGrbit grbit { get; set; }

		public JET_COLUMNID[] prgcolumnid { get; set; }

		public int cbKeyMost { get; set; }

		public int cbVarSegMac { get; set; }

		public JET_TABLEID tableid { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_OPENTEMPORARYTABLE({0}, {1} columns)", new object[]
			{
				this.grbit,
				this.ccolumn
			});
		}

		internal NATIVE_OPENTEMPORARYTABLE GetNativeOpenTemporaryTable()
		{
			this.CheckDataSize();
			return checked(new NATIVE_OPENTEMPORARYTABLE
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_OPENTEMPORARYTABLE)),
				ccolumn = (uint)this.ccolumn,
				grbit = (uint)this.grbit,
				cbKeyMost = (uint)this.cbKeyMost,
				cbVarSegMac = (uint)this.cbVarSegMac
			});
		}

		private void CheckDataSize()
		{
			if (this.prgcolumndef == null)
			{
				throw new ArgumentNullException("prgcolumndef");
			}
			if (this.prgcolumnid == null)
			{
				throw new ArgumentNullException("prgcolumnid");
			}
			if (this.ccolumn < 0)
			{
				throw new ArgumentOutOfRangeException("ccolumn", this.ccolumn, "cannot be negative");
			}
			if (this.ccolumn > this.prgcolumndef.Length)
			{
				throw new ArgumentOutOfRangeException("ccolumn", this.ccolumn, "cannot be greater than prgcolumndef.Length");
			}
			if (this.ccolumn > this.prgcolumnid.Length)
			{
				throw new ArgumentOutOfRangeException("ccolumn", this.ccolumn, "cannot be greater than prgcolumnid.Length");
			}
		}

		internal NATIVE_OPENTEMPORARYTABLE2 GetNativeOpenTemporaryTable2()
		{
			this.CheckDataSize();
			return checked(new NATIVE_OPENTEMPORARYTABLE2
			{
				cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_OPENTEMPORARYTABLE2)),
				ccolumn = (uint)this.ccolumn,
				grbit = (uint)this.grbit,
				cbKeyMost = (uint)this.cbKeyMost,
				cbVarSegMac = (uint)this.cbVarSegMac
			});
		}
	}
}
