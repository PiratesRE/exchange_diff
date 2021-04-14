using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	internal struct NATIVE_CHECKPOINTINFO
	{
		public static readonly int Size = Marshal.SizeOf(typeof(NATIVE_CHECKPOINTINFO));

		public uint genMin;

		public uint genMax;

		public JET_LOGTIME logtimeGenMaxCreate;
	}
}
