using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct _SBinary
	{
		internal static byte[] Unmarshal(IntPtr array)
		{
			int num = Marshal.ReadInt32(array, _SBinary.CountOffset);
			byte[] array2 = new byte[num];
			IntPtr source = Marshal.ReadIntPtr(array, _SBinary.DataOffset);
			Marshal.Copy(source, array2, 0, num);
			return array2;
		}

		public static readonly int SizeOf = Marshal.SizeOf(typeof(_SBinary));

		private static readonly int CountOffset = (int)Marshal.OffsetOf(typeof(_SBinary), "cb");

		private static readonly int DataOffset = (int)Marshal.OffsetOf(typeof(_SBinary), "lpb");

		internal int cb;

		internal unsafe byte* lpb;
	}
}
