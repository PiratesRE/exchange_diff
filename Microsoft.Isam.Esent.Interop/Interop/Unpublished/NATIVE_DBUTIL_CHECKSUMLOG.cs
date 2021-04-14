using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal struct NATIVE_DBUTIL_CHECKSUMLOG
	{
		public uint cbStruct;

		public IntPtr sesid;

		public uint dbid;

		public IntPtr tableid;

		public DBUTIL_OP op;

		public EDBDUMP_OP edbdump;

		public DbutilGrbit grbitOptions;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string szLog;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string szBase;

		public IntPtr pvBuffer;

		public int cbBuffer;

		public IntPtr PadPointer1;

		public IntPtr PadPointer2;

		public int PadInt1;

		public int PadInt2;

		public int PadInt3;

		public int PadInt4;

		public int PadInt5;

		public int PadInt6;

		public IntPtr PadPointer3;

		public IntPtr PadPointer4;
	}
}
