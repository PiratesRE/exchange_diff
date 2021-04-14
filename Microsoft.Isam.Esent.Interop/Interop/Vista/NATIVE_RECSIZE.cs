using System;

namespace Microsoft.Isam.Esent.Interop.Vista
{
	internal struct NATIVE_RECSIZE
	{
		public ulong cbData;

		public ulong cbLongValueData;

		public ulong cbOverhead;

		public ulong cbLongValueOverhead;

		public ulong cNonTaggedColumns;

		public ulong cTaggedColumns;

		public ulong cLongValues;

		public ulong cMultiValues;
	}
}
