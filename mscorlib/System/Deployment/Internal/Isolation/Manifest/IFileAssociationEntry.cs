using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("0C66F299-E08E-48c5-9264-7CCBEB4D5CBB")]
	[ComImport]
	internal interface IFileAssociationEntry
	{
		FileAssociationEntry AllData { [SecurityCritical] get; }

		string Extension { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string Description { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string ProgID { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string DefaultIcon { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string Parameter { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }
	}
}
