using System;

namespace Microsoft.Isam.Esent.Interop.Win32
{
	[Flags]
	internal enum AllocationType : uint
	{
		MEM_COMMIT = 4096U,
		MEM_RESERVE = 8192U
	}
}
