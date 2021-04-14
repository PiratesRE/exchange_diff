using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[StructLayout(LayoutKind.Explicit)]
	internal struct SPropValue
	{
		internal static PropValue Unmarshal(IntPtr item)
		{
			return SPropValue.Unmarshal(item, false);
		}

		internal static PropValue Unmarshal(IntPtr item, bool retainAnsiStrings)
		{
			return PropValue.Unmarshal(item, retainAnsiStrings);
		}

		public static readonly int SizeOf = Marshal.SizeOf(typeof(SPropValue));

		[FieldOffset(0)]
		internal int ulPropTag;

		[FieldOffset(8)]
		internal _PV value;
	}
}
