using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _SBinaryArray
	{
		internal static byte[][] Unmarshal(SafeExLinkedMemoryHandle array)
		{
			return _SBinaryArray.Unmarshal(array.DangerousGetHandle());
		}

		internal static byte[][] Unmarshal(IntPtr array)
		{
			int num = Marshal.ReadInt32(array, _SBinaryArray.CountOffset);
			byte[][] array2 = new byte[num][];
			IntPtr intPtr = Marshal.ReadIntPtr(array, _SBinaryArray.DataOffset);
			for (int i = 0; i < num; i++)
			{
				array2[i] = ((intPtr != IntPtr.Zero) ? _SBinary.Unmarshal(intPtr) : null);
				intPtr = (IntPtr)((long)intPtr + (long)_SBinary.SizeOf);
			}
			return array2;
		}

		public static readonly int SizeOf = Marshal.SizeOf(typeof(_SBinaryArray));

		private static readonly int CountOffset = (int)Marshal.OffsetOf(typeof(_SBinaryArray), "cValues");

		private static readonly int DataOffset = (int)Marshal.OffsetOf(typeof(_SBinaryArray), "lpbin");

		internal int cValues;

		internal unsafe _SBinary* lpbin;
	}
}
