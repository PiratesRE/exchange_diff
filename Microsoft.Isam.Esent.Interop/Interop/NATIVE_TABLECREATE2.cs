using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_TABLECREATE2
	{
		public uint cbStruct;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string szTableName;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string szTemplateTableName;

		public uint ulPages;

		public uint ulDensity;

		public unsafe NATIVE_COLUMNCREATE* rgcolumncreate;

		public uint cColumns;

		public IntPtr rgindexcreate;

		public uint cIndexes;

		[MarshalAs(UnmanagedType.LPTStr)]
		public string szCallback;

		public JET_cbtyp cbtyp;

		public uint grbit;

		public IntPtr tableid;

		public uint cCreated;
	}
}
