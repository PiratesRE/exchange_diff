using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class CLRSurrogateEntry
	{
		public Guid Clsid;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string RuntimeVersion;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string ClassName;
	}
}
