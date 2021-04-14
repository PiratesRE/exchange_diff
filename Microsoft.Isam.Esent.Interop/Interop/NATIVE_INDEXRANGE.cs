using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_INDEXRANGE
	{
		public static NATIVE_INDEXRANGE MakeIndexRangeFromTableid(JET_TABLEID tableid)
		{
			NATIVE_INDEXRANGE result = new NATIVE_INDEXRANGE
			{
				tableid = tableid.Value,
				grbit = 1U
			};
			result.cbStruct = (uint)Marshal.SizeOf(typeof(NATIVE_INDEXRANGE));
			return result;
		}

		public uint cbStruct;

		public IntPtr tableid;

		public uint grbit;
	}
}
