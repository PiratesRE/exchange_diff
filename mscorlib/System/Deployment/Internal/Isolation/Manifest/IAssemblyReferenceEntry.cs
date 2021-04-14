using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("FD47B733-AFBC-45e4-B7C2-BBEB1D9F766C")]
	[ComImport]
	internal interface IAssemblyReferenceEntry
	{
		AssemblyReferenceEntry AllData { [SecurityCritical] get; }

		IReferenceIdentity ReferenceIdentity { [SecurityCritical] get; }

		uint Flags { [SecurityCritical] get; }

		IAssemblyReferenceDependentAssemblyEntry DependentAssembly { [SecurityCritical] get; }
	}
}
