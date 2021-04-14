using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct IDLDESC
	{
		public IntPtr dwReserved;

		[__DynamicallyInvokable]
		public IDLFLAG wIDLFlags;
	}
}
