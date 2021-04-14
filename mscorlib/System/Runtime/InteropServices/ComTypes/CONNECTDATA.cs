using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct CONNECTDATA
	{
		[__DynamicallyInvokable]
		[MarshalAs(UnmanagedType.Interface)]
		public object pUnk;

		[__DynamicallyInvokable]
		public int dwCookie;
	}
}
