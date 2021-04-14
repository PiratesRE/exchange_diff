using System;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal struct NATIVE_SNPATCHREQUEST
	{
		public uint cbStruct;

		public uint pageNumber;

		public IntPtr szLogFile;

		public IntPtr instance;

		public NATIVE_DBINFOMISC7 dbinfomisc;

		public IntPtr pvToken;

		public uint cbToken;

		public IntPtr pvData;

		public uint cbData;
	}
}
