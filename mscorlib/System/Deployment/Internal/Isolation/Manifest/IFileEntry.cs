using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("A2A55FAD-349B-469b-BF12-ADC33D14A937")]
	[ComImport]
	internal interface IFileEntry
	{
		FileEntry AllData { [SecurityCritical] get; }

		string Name { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		uint HashAlgorithm { [SecurityCritical] get; }

		string LoadFrom { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string SourcePath { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string ImportPath { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string SourceName { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string Location { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		object HashValue { [SecurityCritical] [return: MarshalAs(UnmanagedType.Interface)] get; }

		ulong Size { [SecurityCritical] get; }

		string Group { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		uint Flags { [SecurityCritical] get; }

		IMuiResourceMapEntry MuiMapping { [SecurityCritical] get; }

		uint WritableType { [SecurityCritical] get; }

		ISection HashElements { [SecurityCritical] get; }
	}
}
