using System;

namespace System.StubHelpers
{
	internal struct NativeVariant
	{
		private ushort vt;

		private ushort wReserved1;

		private ushort wReserved2;

		private ushort wReserved3;

		private IntPtr data1;

		private IntPtr data2;
	}
}
