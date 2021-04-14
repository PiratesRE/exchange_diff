using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct EXCEPINFO
	{
		[__DynamicallyInvokable]
		public short wCode;

		[__DynamicallyInvokable]
		public short wReserved;

		[__DynamicallyInvokable]
		[MarshalAs(UnmanagedType.BStr)]
		public string bstrSource;

		[__DynamicallyInvokable]
		[MarshalAs(UnmanagedType.BStr)]
		public string bstrDescription;

		[__DynamicallyInvokable]
		[MarshalAs(UnmanagedType.BStr)]
		public string bstrHelpFile;

		[__DynamicallyInvokable]
		public int dwHelpContext;

		public IntPtr pvReserved;

		public IntPtr pfnDeferredFillIn;

		[__DynamicallyInvokable]
		public int scode;
	}
}
