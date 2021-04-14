using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("DA0C3B27-6B6B-4b80-A8F8-6CE14F4BC0A4")]
	[ComImport]
	internal interface ICategoryMembershipDataEntry
	{
		CategoryMembershipDataEntry AllData { [SecurityCritical] get; }

		uint index { [SecurityCritical] get; }

		string Xml { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string Description { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }
	}
}
