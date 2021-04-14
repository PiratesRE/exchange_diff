using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_SNCORRUPTEDPAGE
	{
		public string wszDatabase { get; private set; }

		public JET_DBID dbid { get; private set; }

		public JET_DBINFOMISC dbinfomisc { get; private set; }

		public int pageNumber { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SNCORRUPTEDPAGE({0})", new object[]
			{
				this.pageNumber
			});
		}

		internal void SetFromNativeSncorruptedpage(ref NATIVE_SNCORRUPTEDPAGE native)
		{
			this.wszDatabase = Marshal.PtrToStringUni(native.wszDatabase);
			this.dbid = new JET_DBID
			{
				Value = native.dbid
			};
			this.dbinfomisc = new JET_DBINFOMISC();
			this.dbinfomisc.SetFromNativeDbinfoMisc(ref native.dbinfomisc);
			this.pageNumber = checked((int)native.pageNumber);
		}
	}
}
