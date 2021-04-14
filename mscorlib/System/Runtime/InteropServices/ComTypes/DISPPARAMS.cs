using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct DISPPARAMS
	{
		[__DynamicallyInvokable]
		public IntPtr rgvarg;

		[__DynamicallyInvokable]
		public IntPtr rgdispidNamedArgs;

		[__DynamicallyInvokable]
		public int cArgs;

		[__DynamicallyInvokable]
		public int cNamedArgs;
	}
}
