using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi.Unmanaged;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class AdrList
	{
		internal unsafe static AdrEntry[] Unmarshal(_AdrList* padrList)
		{
			AdrEntry[] array = new AdrEntry[padrList->cEntries];
			_AdrEntry* ptr = &padrList->adrEntry1;
			uint num = 0U;
			while ((ulong)num < (ulong)((long)padrList->cEntries))
			{
				array[(int)((UIntPtr)num)] = new AdrEntry(ptr + (ulong)num * (ulong)((long)sizeof(_AdrEntry)) / (ulong)sizeof(_AdrEntry));
				num += 1U;
			}
			return array;
		}

		internal static int GetBytesToMarshal(params AdrEntry[] adrEntries)
		{
			int num = _AdrList.SizeOf + 7 & -8;
			for (int i = 0; i < adrEntries.Length; i++)
			{
				num += adrEntries[i].GetBytesToMarshal();
			}
			return num;
		}

		internal unsafe static void MarshalToNative(byte* pb, params AdrEntry[] adrEntries)
		{
			((_AdrList*)pb)->cEntries = adrEntries.Length;
			_AdrEntry* ptr = &((_AdrList*)pb)->adrEntry1;
			SPropValue* ptr2 = (SPropValue*)(pb + (_AdrList.SizeOf + 7 & -8) + (IntPtr)(_AdrEntry.SizeOf + 7 & -8) * (IntPtr)(adrEntries.Length - 1));
			int num = 0;
			for (int i = 0; i < adrEntries.Length; i++)
			{
				num += adrEntries[i].propValues.Length;
			}
			byte* ptr3 = (byte*)(ptr2 + (IntPtr)(SPropValue.SizeOf + 7 & -8) * (IntPtr)num / (IntPtr)sizeof(SPropValue));
			for (int j = 0; j < adrEntries.Length; j++)
			{
				adrEntries[j].MarshalToNative(ptr, ref ptr2, ref ptr3);
				ptr++;
			}
		}
	}
}
