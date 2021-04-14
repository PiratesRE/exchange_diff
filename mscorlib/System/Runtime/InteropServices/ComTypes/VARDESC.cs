using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
	public struct VARDESC
	{
		[__DynamicallyInvokable]
		public int memid;

		[__DynamicallyInvokable]
		public string lpstrSchema;

		[__DynamicallyInvokable]
		public VARDESC.DESCUNION desc;

		[__DynamicallyInvokable]
		public ELEMDESC elemdescVar;

		[__DynamicallyInvokable]
		public short wVarFlags;

		[__DynamicallyInvokable]
		public VARKIND varkind;

		[__DynamicallyInvokable]
		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct DESCUNION
		{
			[__DynamicallyInvokable]
			[FieldOffset(0)]
			public int oInst;

			[FieldOffset(0)]
			public IntPtr lpvarValue;
		}
	}
}
