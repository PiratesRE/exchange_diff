using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.AirSync.SyncStateConverter
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct FolderInfo
	{
		public uint Version;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
		public char[] SyncKey;

		public uint NumItems;

		public IntPtr Foldernodes;
	}
}
