using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class PermissionSetEntry
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Id;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string XmlSegment;
	}
}
