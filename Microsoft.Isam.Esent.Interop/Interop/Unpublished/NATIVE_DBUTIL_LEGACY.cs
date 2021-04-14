using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop.Unpublished
{
	internal struct NATIVE_DBUTIL_LEGACY
	{
		public uint cbStruct;

		public IntPtr sesid;

		public uint dbid;

		public IntPtr tableid;

		public DBUTIL_OP op;

		public EDBDUMP_OP edbdump;

		public DbutilGrbit grbitOptions;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string szDatabase;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string szSLV;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string szBackup;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string szTable;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string szIndex;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string szIntegPrefix;

		public int pgno;

		public int iline;

		public int lGeneration;

		public int isec;

		public int ib;

		public int cRetry;

		public IntPtr pfnCallback;

		public IntPtr pvCallback;
	}
}
