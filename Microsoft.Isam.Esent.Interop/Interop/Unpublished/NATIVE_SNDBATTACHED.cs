using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal struct NATIVE_SNDBATTACHED
	{
		public NATIVE_RECOVERY_CONTROL recoveryControl;

		public uint cbStruct;

		public IntPtr wszDbPath;
	}
}
