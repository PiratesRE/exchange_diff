using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct PARAMDESC
	{
		public IntPtr lpVarValue;

		[__DynamicallyInvokable]
		public PARAMFLAG wParamFlags;
	}
}
