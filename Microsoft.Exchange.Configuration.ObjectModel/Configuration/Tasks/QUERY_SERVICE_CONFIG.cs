using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[StructLayout(LayoutKind.Sequential)]
	internal class QUERY_SERVICE_CONFIG
	{
		[MarshalAs(UnmanagedType.U4)]
		public uint dwServiceType;

		[MarshalAs(UnmanagedType.U4)]
		public uint dwStartType;

		[MarshalAs(UnmanagedType.U4)]
		public uint dwErrorControl;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string binaryPathName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string loadOrderGroup;

		[MarshalAs(UnmanagedType.U4)]
		public uint dwTagId;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string dependencies;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string serviceStartName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string displayName;
	}
}
