using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class WindowClassEntry
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string ClassName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string HostDll;

		public bool fVersioned;
	}
}
