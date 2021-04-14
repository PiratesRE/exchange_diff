using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal struct NATIVE_SNBEGINUNDO
	{
		public NATIVE_RECOVERY_CONTROL recoveryControl;

		public uint cbStruct;

		public uint cdbinfomisc;

		public IntPtr rgdbinfomisc;
	}
}
