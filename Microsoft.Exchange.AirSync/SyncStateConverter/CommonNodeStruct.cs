using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.AirSync.SyncStateConverter
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct CommonNodeStruct
	{
		public string ServerId;

		public string VersionId;

		public byte SentToClient;

		public byte IsEmail;

		public byte Read;

		public byte IsCalendar;

		public IntPtr Next;

		public long EndTime;
	}
}
