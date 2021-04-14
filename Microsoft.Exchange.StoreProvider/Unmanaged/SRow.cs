using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[StructLayout(LayoutKind.Explicit)]
	internal struct SRow
	{
		internal static PropValue[] Unmarshal(IntPtr array)
		{
			return SRow.Unmarshal(array, false);
		}

		internal static PropValue[] Unmarshal(IntPtr array, bool retainAnsiStrings)
		{
			int num = Marshal.ReadInt32(array, SRow.CountOffset);
			PropValue[] array2 = new PropValue[num];
			IntPtr intPtr = Marshal.ReadIntPtr(array, SRow.DataOffset);
			for (int i = 0; i < num; i++)
			{
				array2[i] = SPropValue.Unmarshal(intPtr, retainAnsiStrings);
				intPtr = (IntPtr)((long)intPtr + (long)SPropValue.SizeOf);
			}
			return array2;
		}

		public static readonly int SizeOf = Marshal.SizeOf(typeof(SRow));

		private static readonly int CountOffset = (int)Marshal.OffsetOf(typeof(SRow), "cValues");

		internal static readonly int DataOffset = (int)Marshal.OffsetOf(typeof(SRow), "lpProps");

		[FieldOffset(4)]
		internal int cValues;

		[FieldOffset(8)]
		internal IntPtr lpProps;
	}
}
