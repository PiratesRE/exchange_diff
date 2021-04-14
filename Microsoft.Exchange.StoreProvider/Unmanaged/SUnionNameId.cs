using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[StructLayout(LayoutKind.Explicit)]
	internal struct SUnionNameId
	{
		[FieldOffset(0)]
		internal IntPtr lpStr;

		[FieldOffset(0)]
		internal int id;
	}
}
