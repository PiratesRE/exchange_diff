using System;

namespace System.Runtime.InteropServices
{
	[Obsolete("Use System.Runtime.InteropServices.ComTypes.STATSTG instead. http://go.microsoft.com/fwlink/?linkid=14202", false)]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct STATSTG
	{
		public string pwcsName;

		public int type;

		public long cbSize;

		public FILETIME mtime;

		public FILETIME ctime;

		public FILETIME atime;

		public int grfMode;

		public int grfLocksSupported;

		public Guid clsid;

		public int grfStateBits;

		public int reserved;
	}
}
