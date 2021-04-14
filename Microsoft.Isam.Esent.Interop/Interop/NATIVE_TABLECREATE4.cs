using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_TABLECREATE4
	{
		public uint cbStruct;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string szTableName;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string szTemplateTableName;

		public uint ulPages;

		public uint ulDensity;

		public unsafe NATIVE_COLUMNCREATE* rgcolumncreate;

		public uint cColumns;

		public IntPtr rgindexcreate;

		public uint cIndexes;

		[MarshalAs(UnmanagedType.LPWStr)]
		public string szCallback;

		public JET_cbtyp cbtyp;

		public uint grbit;

		public unsafe NATIVE_SPACEHINTS* pSeqSpacehints;

		public unsafe NATIVE_SPACEHINTS* pLVSpacehints;

		public uint cbSeparateLV;

		public IntPtr tableid;

		public uint cCreated;
	}
}
