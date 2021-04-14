using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_SNDBATTACHED : JET_RECOVERYCONTROL
	{
		public string wszDbPath { get; internal set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SNDBATTACHED({0})", new object[]
			{
				this.wszDbPath
			});
		}

		internal void SetFromNativeSndbattached(ref NATIVE_SNDBATTACHED native)
		{
			base.SetFromNativeSnrecoverycontrol(ref native.recoveryControl);
			this.wszDbPath = Marshal.PtrToStringUni(native.wszDbPath);
		}
	}
}
