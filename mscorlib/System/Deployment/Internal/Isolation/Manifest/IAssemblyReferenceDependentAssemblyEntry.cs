using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("C31FF59E-CD25-47b8-9EF3-CF4433EB97CC")]
	[ComImport]
	internal interface IAssemblyReferenceDependentAssemblyEntry
	{
		AssemblyReferenceDependentAssemblyEntry AllData { [SecurityCritical] get; }

		string Group { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string Codebase { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		ulong Size { [SecurityCritical] get; }

		object HashValue { [SecurityCritical] [return: MarshalAs(UnmanagedType.Interface)] get; }

		uint HashAlgorithm { [SecurityCritical] get; }

		uint Flags { [SecurityCritical] get; }

		string ResourceFallbackCulture { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string Description { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		string SupportUrl { [SecurityCritical] [return: MarshalAs(UnmanagedType.LPWStr)] get; }

		ISection HashElements { [SecurityCritical] get; }
	}
}
