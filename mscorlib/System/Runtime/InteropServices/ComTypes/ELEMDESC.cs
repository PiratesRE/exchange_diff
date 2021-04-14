using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct ELEMDESC
	{
		[__DynamicallyInvokable]
		public TYPEDESC tdesc;

		[__DynamicallyInvokable]
		public ELEMDESC.DESCUNION desc;

		[__DynamicallyInvokable]
		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct DESCUNION
		{
			[__DynamicallyInvokable]
			[FieldOffset(0)]
			public IDLDESC idldesc;

			[__DynamicallyInvokable]
			[FieldOffset(0)]
			public PARAMDESC paramdesc;
		}
	}
}
