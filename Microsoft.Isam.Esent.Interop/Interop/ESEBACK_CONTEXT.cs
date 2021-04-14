using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	public class ESEBACK_CONTEXT
	{
		public string wszServerName { get; set; }

		public IntPtr pvApplicationData { get; set; }

		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "ESEBACK_CONTEXT({0})", new object[]
			{
				this.wszServerName
			});
		}

		internal NATIVE_ESEBACK_CONTEXT GetNativeEsebackContext()
		{
			return new NATIVE_ESEBACK_CONTEXT
			{
				cbSize = (uint)Marshal.SizeOf(typeof(NATIVE_ESEBACK_CONTEXT)),
				wszServerName = Marshal.StringToHGlobalUni(this.wszServerName),
				pvApplicationData = this.pvApplicationData
			};
		}

		internal void SetFromNativeEsebackContext(IntPtr nativeBackupContextPtr)
		{
			if (nativeBackupContextPtr != IntPtr.Zero)
			{
				NATIVE_ESEBACK_CONTEXT native_ESEBACK_CONTEXT = (NATIVE_ESEBACK_CONTEXT)Marshal.PtrToStructure(nativeBackupContextPtr, typeof(NATIVE_ESEBACK_CONTEXT));
				this.wszServerName = Marshal.PtrToStringUni(native_ESEBACK_CONTEXT.wszServerName);
				this.pvApplicationData = native_ESEBACK_CONTEXT.pvApplicationData;
				return;
			}
			this.wszServerName = string.Empty;
			this.pvApplicationData = IntPtr.Zero;
		}
	}
}
