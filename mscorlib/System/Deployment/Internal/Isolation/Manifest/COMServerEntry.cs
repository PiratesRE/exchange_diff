using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation.Manifest
{
	[StructLayout(LayoutKind.Sequential)]
	internal class COMServerEntry
	{
		public Guid Clsid;

		public uint Flags;

		public Guid ConfiguredGuid;

		public Guid ImplementedClsid;

		public Guid TypeLibrary;

		public uint ThreadingModel;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string RuntimeVersion;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string HostFile;
	}
}
