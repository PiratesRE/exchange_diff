using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("CB73147E-5FC2-4c31-B4E6-58D13DBE1A08")]
	[ComImport]
	internal interface IDescriptionMetadataEntry
	{
		DescriptionMetadataEntry AllData { [SecurityCritical] get; }

		string Publisher { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string Product { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string SupportUrl { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string IconFile { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string ErrorReportUrl { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string SuiteName { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }
	}
}
