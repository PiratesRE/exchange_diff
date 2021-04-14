using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class AssemblyReferenceEntry
	{
		public IReferenceIdentity ReferenceIdentity;

		public uint Flags;

		public AssemblyReferenceDependentAssemblyEntry DependentAssembly;
	}
}
