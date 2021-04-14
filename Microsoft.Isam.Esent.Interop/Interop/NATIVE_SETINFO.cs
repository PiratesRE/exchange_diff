using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_SETINFO
	{
		public static readonly int Size = Marshal.SizeOf(typeof(NATIVE_SETINFO));

		public uint cbStruct;

		public uint ibLongValue;

		public uint itagSequence;
	}
}
