using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal struct NATIVE_SNOPENLOG
	{
		public NATIVE_RECOVERY_CONTROL recoveryControl;

		public uint cbStruct;

		public uint lGenNext;

		public byte fCurrentLog;

		public byte eReason;

		public int rgbReserved;

		public IntPtr wszLogFile;

		public uint cdbinfomisc;

		public IntPtr rgdbinfomisc;
	}
}
