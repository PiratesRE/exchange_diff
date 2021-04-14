using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class ProgIdRedirectionEntry
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string ProgId;

		public Guid RedirectedGuid;
	}
}
