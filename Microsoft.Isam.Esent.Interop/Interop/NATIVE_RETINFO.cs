using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_RETINFO
	{
		public static readonly int Size = Marshal.SizeOf(typeof(NATIVE_RETINFO));

		public uint cbStruct;

		public uint ibLongValue;

		public uint itagSequence;

		public uint columnidNextTagged;
	}
}
