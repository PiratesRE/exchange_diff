using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Security.RightsManagement
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal class DRMClientVersionInfo
	{
		public uint StructVersion = 1U;

		public uint V1;

		public uint V2;

		public uint V3;

		public uint V4;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string Hierarchy;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string ProductId;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		public string ProductDescription;
	}
}
