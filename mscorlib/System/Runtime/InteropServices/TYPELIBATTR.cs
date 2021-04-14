using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.TYPELIBATTR instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[Serializable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct TYPELIBATTR
	{
		public Guid guid;

		public int lcid;

		public SYSKIND syskind;

		public short wMajorVerNum;

		public short wMinorVerNum;

		public LIBFLAGS wLibFlags;
	}
}
