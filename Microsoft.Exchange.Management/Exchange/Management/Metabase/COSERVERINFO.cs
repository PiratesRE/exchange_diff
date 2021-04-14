using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Management.Metabase
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	internal struct COSERVERINFO : IDisposable
	{
		public COSERVERINFO(string serverName)
		{
			this.pAuthInfo = IntPtr.Zero;
			this.pwszName = serverName;
			this.dwReserved1 = 0U;
			this.dwReserved2 = 0U;
		}

		public void SetAuthInfo(COAUTHINFO authInfo)
		{
			this.pAuthInfo = Marshal.AllocCoTaskMem(Marshal.SizeOf(authInfo));
			Marshal.StructureToPtr(authInfo, this.pAuthInfo, false);
		}

		public void Dispose()
		{
			if (this.pAuthInfo != IntPtr.Zero)
			{
				Marshal.DestroyStructure(this.pAuthInfo, typeof(COAUTHINFO));
				Marshal.FreeCoTaskMem(this.pAuthInfo);
			}
		}

		private uint dwReserved1;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string pwszName;

		private IntPtr pAuthInfo;

		private uint dwReserved2;
	}
}
