using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct MapiLtidNative
	{
		internal unsafe MapiLtidNative(NativeLtid* pLtid)
		{
			this.padding = new byte[2];
			this.globCount = new byte[6];
			Marshal.Copy(new IntPtr((void*)(pLtid + sizeof(Guid) / sizeof(NativeLtid))), this.globCount, 0, 6);
			this.replGuid = pLtid->replGuid;
		}

		internal Guid replGuid;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
		internal byte[] globCount;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
		internal byte[] padding;

		internal static readonly int Size = 24;
	}
}
