using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.AirSync.SyncStateConverter
{
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	internal struct CommonInfo
	{
		public uint Version;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 51)]
		public char[] SyncKey;

		public uint NumSupportedTags;

		public IntPtr Tagnodes;

		public uint NumMapping;

		public IntPtr Mappingnodes;

		public uint NumCommonNodes;

		public IntPtr Nodes;
	}
}
