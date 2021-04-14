using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[StructLayout(LayoutKind.Explicit)]
	internal struct _PV
	{
		[FieldOffset(0)]
		internal short s;

		[FieldOffset(0)]
		internal int i;

		[FieldOffset(0)]
		internal long l;

		[FieldOffset(0)]
		internal float f;

		[FieldOffset(0)]
		internal double d;

		[FieldOffset(0)]
		internal IntPtr intPtr;

		[FieldOffset(0)]
		internal CountAndPtr cp;
	}
}
