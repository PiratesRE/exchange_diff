using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_RECPOS
	{
		public static readonly int Size = Marshal.SizeOf(typeof(NATIVE_RECPOS));

		public uint cbStruct;

		public uint centriesLT;

		public uint centriesInRange;

		public uint centriesTotal;
	}
}
