using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class SBinaryArray
	{
		internal static int GetBytesToMarshal(SBinary[] sbins)
		{
			int num = _SBinaryArray.SizeOf + 7 & -8;
			for (int i = 0; i < sbins.Length; i++)
			{
				num += sbins[i].GetBytesToMarshal();
			}
			return num;
		}

		internal unsafe static void MarshalToNative(byte* pb, SBinary[] sbins)
		{
			((_SBinaryArray*)pb)->cValues = sbins.Length;
			_SBinary* ptr = (_SBinary*)(pb + (_SBinaryArray.SizeOf + 7 & -8));
			((_SBinaryArray*)pb)->lpbin = ptr;
			byte* ptr2 = pb + (_SBinaryArray.SizeOf + 7 & -8) + (IntPtr)sbins.Length * (IntPtr)(_SBinary.SizeOf + 7 & -8);
			for (int i = 0; i < sbins.Length; i++)
			{
				sbins[i].MarshalToNative(ptr, ref ptr2);
				ptr++;
			}
		}

		public static byte[][] UnmarshalFromNative(IntPtr pEntryIds)
		{
			return _SBinaryArray.Unmarshal(pEntryIds);
		}
	}
}
