using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public class JET_SNOPENCHECKPOINT : JET_RECOVERYCONTROL
	{
		public string wszCheckpoint { get; private set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "JET_SNOPENCHECKPOINT({0})", new object[]
			{
				this.wszCheckpoint
			});
		}

		internal void SetFromNativeSnopencheckpoint(ref NATIVE_SNOPENCHECKPOINT native)
		{
			base.SetFromNativeSnrecoverycontrol(ref native.recoveryControl);
			this.wszCheckpoint = Marshal.PtrToStringUni(native.wszCheckpoint);
		}
	}
}
