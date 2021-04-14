using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_RSTINFO
	{
		public static readonly int SizeOfRstinfo = Marshal.SizeOf(typeof(NATIVE_RSTINFO));

		public uint cbStruct;

		public unsafe NATIVE_RSTMAP* rgrstmap;

		public uint crstmap;

		public JET_LGPOS lgposStop;

		public JET_LOGTIME logtimeStop;

		public NATIVE_PFNSTATUS pfnStatus;
	}
}
