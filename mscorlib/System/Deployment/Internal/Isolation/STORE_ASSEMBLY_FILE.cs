using System;
using System.Runtime.InteropServices;

namespace System.Deployment.Internal.Isolation
{
	internal struct STORE_ASSEMBLY_FILE
	{
		public uint Size;

		public uint Flags;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string FileName;

		public uint FileStatusFlags;
	}
}
