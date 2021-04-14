using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class EntryPointEntry
	{
		[MarshalAs(UnmanagedType.LPWStr)]
		public string Name;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string CommandLine_File;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string CommandLine_Parameters;

		public IReferenceIdentity Identity;

		public uint Flags;
	}
}
