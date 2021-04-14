using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class DescriptionMetadataEntry
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Publisher;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string Product;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string SupportUrl;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string IconFile;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string ErrorReportUrl;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string SuiteName;
	}
}
