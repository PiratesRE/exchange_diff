using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.AirSync.SyncStateConverter
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct FolderNodeStruct
	{
		public IntPtr Next;

		public string ServerID;

		public string DisplayName;

		public string ParentID;

		public string ContentClass;

		public string FolderURL;
	}
}
