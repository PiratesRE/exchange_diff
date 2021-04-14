using System;

namespace Microsoft.Isam.Esent.Interop.Vista
{
	internal struct NATIVE_OPENTEMPORARYTABLE
	{
		public uint cbStruct;

		public unsafe NATIVE_COLUMNDEF* prgcolumndef;

		public uint ccolumn;

		public unsafe NATIVE_UNICODEINDEX* pidxunicode;

		public uint grbit;

		public unsafe uint* rgcolumnid;

		public uint cbKeyMost;

		public uint cbVarSegMac;

		public IntPtr tableid;
	}
}
