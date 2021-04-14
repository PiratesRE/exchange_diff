using System;
using System.Runtime.InteropServices;

namespace Microsoft.Isam.Esent.Interop
{
	[Serializable]
	internal struct NATIVE_SIGNATURE
	{
		public const int ComputerNameSize = 16;

		public static readonly int Size = Marshal.SizeOf(typeof(NATIVE_SIGNATURE));

		public uint ulRandom;

		public JET_LOGTIME logtimeCreate;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
		public string szComputerName;
	}
}
