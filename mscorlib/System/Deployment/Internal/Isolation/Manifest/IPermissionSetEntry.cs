using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("EBE5A1ED-FEBC-42c4-A9E1-E087C6E36635")]
	[ComImport]
	internal interface IPermissionSetEntry
	{
		PermissionSetEntry AllData { [SecurityCritical] get; }

		string Id { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string XmlSegment { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }
	}
}
