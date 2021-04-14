using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[Serializable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct TYPELIBATTR
	{
		[__DynamicallyInvokable]
		public Guid guid;

		[__DynamicallyInvokable]
		public int lcid;

		[__DynamicallyInvokable]
		public SYSKIND syskind;

		[__DynamicallyInvokable]
		public short wMajorVerNum;

		[__DynamicallyInvokable]
		public short wMinorVerNum;

		[__DynamicallyInvokable]
		public LIBFLAGS wLibFlags;
	}
}
