using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.AirSync.SyncStateConverter
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct MappingNodeStruct
	{
		public IntPtr Next;

		public string ShortId;

		public string LongId;
	}
}
