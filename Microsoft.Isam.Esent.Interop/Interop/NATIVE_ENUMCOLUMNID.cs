using System;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_ENUMCOLUMNID
	{
		public uint columnid;

		public uint ctagSequence;

		public unsafe uint* rgtagSequence;
	}
}
