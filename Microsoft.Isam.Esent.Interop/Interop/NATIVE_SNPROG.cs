using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_SNPROG
	{
		public static readonly int Size = Marshal.SizeOf(typeof(NATIVE_SNPROG));

		public uint cbStruct;

		public uint cunitDone;

		public uint cunitTotal;
	}
}
