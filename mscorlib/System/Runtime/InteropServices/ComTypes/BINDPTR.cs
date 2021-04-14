using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
	public struct BINDPTR
	{
		[FieldOffset(0)]
		public IntPtr lpfuncdesc;

		[FieldOffset(0)]
		public IntPtr lpvardesc;

		[FieldOffset(0)]
		public IntPtr lptcomp;
	}
}
