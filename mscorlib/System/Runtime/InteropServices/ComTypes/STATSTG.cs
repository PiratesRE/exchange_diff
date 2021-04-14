using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct STATSTG
	{
		[__DynamicallyInvokable]
		public string pwcsName;

		[__DynamicallyInvokable]
		public int type;

		[__DynamicallyInvokable]
		public long cbSize;

		[__DynamicallyInvokable]
		public FILETIME mtime;

		[__DynamicallyInvokable]
		public FILETIME ctime;

		[__DynamicallyInvokable]
		public FILETIME atime;

		[__DynamicallyInvokable]
		public int grfMode;

		[__DynamicallyInvokable]
		public int grfLocksSupported;

		[__DynamicallyInvokable]
		public Guid clsid;

		[__DynamicallyInvokable]
		public int grfStateBits;

		[__DynamicallyInvokable]
		public int reserved;
	}
}
