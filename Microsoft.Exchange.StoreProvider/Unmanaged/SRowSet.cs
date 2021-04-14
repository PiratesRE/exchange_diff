using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi.Unmanaged
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal struct SRowSet
	{
		internal static PropValue[][] Unmarshal(SafeHandle array)
		{
			return SRowSet.Unmarshal(array, false);
		}

		internal static PropValue[][] Unmarshal(SafeHandle array, bool retainAnsiStrings)
		{
			if (array.IsInvalid)
			{
				return Array<PropValue[]>.Empty;
			}
			int num = Marshal.ReadInt32(array.DangerousGetHandle(), SRowSet.CountOffset);
			if (num == 0)
			{
				return Array<PropValue[]>.Empty;
			}
			PropValue[][] array2 = new PropValue[num][];
			IntPtr intPtr = (IntPtr)((long)array.DangerousGetHandle() + (long)SRowSet.DataOffset);
			for (int i = 0; i < num; i++)
			{
				array2[i] = SRow.Unmarshal(intPtr, retainAnsiStrings);
				intPtr = (IntPtr)((long)intPtr + (long)SRow.SizeOf);
			}
			return array2;
		}

		[Conditional("ValidateMarshalled")]
		private unsafe static void Validate(IntPtr pointer, PropValue[][] expected)
		{
			SRowSet* ptr = (SRowSet*)pointer.ToPointer();
			if (ptr == null)
			{
				throw new InvalidOperationException("Should not be null");
			}
			PropValue[][] array = new PropValue[ptr->cRows][];
			SRow* ptr2 = &ptr->aRow;
			int i = 0;
			while (i < ptr->cRows)
			{
				array[i] = new PropValue[ptr2->cValues];
				SPropValue* ptr3 = (SPropValue*)ptr2->lpProps.ToPointer();
				for (int j = 0; j < ptr2->cValues; j++)
				{
					array[i][j] = new PropValue(ptr3 + j);
				}
				i++;
				ptr2++;
			}
			if (expected.Length != array.Length)
			{
				throw new InvalidOperationException("Lengthes don't match!");
			}
			for (int k = 0; k < expected.Length; k++)
			{
				if (expected[k].Length != array[k].Length)
				{
					throw new InvalidOperationException("Lengthes don't match!");
				}
				for (int l = 0; l < expected[k].Length; l++)
				{
					if (!expected[k][l].IsEqualTo(array[k][l]))
					{
						throw new InvalidOperationException("Property is not the same!");
					}
				}
			}
		}

		public static readonly int SizeOf = Marshal.SizeOf(typeof(SRowSet));

		private static readonly int CountOffset = (int)Marshal.OffsetOf(typeof(SRowSet), "cRows");

		internal static readonly int DataOffset = (int)Marshal.OffsetOf(typeof(SRowSet), "aRow");

		internal int cRows;

		internal SRow aRow;
	}
}
