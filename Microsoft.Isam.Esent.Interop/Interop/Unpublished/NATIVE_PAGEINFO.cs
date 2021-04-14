using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	public struct NATIVE_PAGEINFO
	{
		public uint pgno;

		public uint bitField;

		public ulong checksumActual;

		public ulong checksumExpected;

		public ulong dbtime;

		public ulong structureChecksum;

		public ulong flags;
	}
}
