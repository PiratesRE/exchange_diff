using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.AirSync.SyncStateConverter
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct TagNodeStruct
	{
		public IntPtr Next;

		public ushort NameSpace;

		public ushort Tag;
	}
}
