using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class ResourceTableMappingEntry
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string id;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string FinalStringMapped;
	}
}
