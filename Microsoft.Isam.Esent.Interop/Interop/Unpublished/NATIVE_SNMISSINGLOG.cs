using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal struct NATIVE_SNMISSINGLOG
	{
		public NATIVE_RECOVERY_CONTROL recoveryControl;

		public uint cbStruct;

		public uint lGenMissing;

		public byte fCurrentLog;

		public byte eNextAction;

		public int rgbReserved;

		public IntPtr wszLogFile;

		public uint cdbinfomisc;

		public IntPtr rgdbinfomisc;
	}
}
