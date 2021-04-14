using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_SNPATCHREQUEST
	{
		public int pageNumber { get; internal set; }

		public string szLogFile { get; internal set; }

		public JET_INSTANCE instance { get; private set; }

		public JET_DBINFOMISC dbinfomisc { get; internal set; }

		public byte[] pvToken { get; internal set; }

		public int cbToken { get; internal set; }

		public byte[] pvData { get; internal set; }

		public int cbData { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SNPATCHREQUEST({0})", new object[]
			{
				this.pageNumber
			});
		}

		internal void SetFromNativeSnpatchrequest(ref NATIVE_SNPATCHREQUEST native)
		{
			checked
			{
				this.pageNumber = (int)native.pageNumber;
				this.szLogFile = Marshal.PtrToStringUni(native.szLogFile);
				this.instance = new JET_INSTANCE
				{
					Value = native.instance
				};
				this.dbinfomisc = new JET_DBINFOMISC();
				this.dbinfomisc.SetFromNativeDbinfoMisc(ref native.dbinfomisc);
				this.cbToken = (int)native.cbToken;
				this.pvToken = new byte[this.cbToken];
				if (this.cbToken > 0)
				{
					Marshal.Copy(native.pvToken, this.pvToken, 0, this.cbToken);
				}
				this.cbData = (int)native.cbData;
				this.pvData = new byte[this.cbData];
				if (this.cbData > 0)
				{
					Marshal.Copy(native.pvData, this.pvData, 0, this.cbData);
				}
			}
		}
	}
}
