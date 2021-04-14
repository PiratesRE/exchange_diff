using System;

namespace System.Runtime.InteropServices.ComTypes
{
	[__DynamicallyInvokable]
	public struct FUNCDESC
	{
		[__DynamicallyInvokable]
		public int memid;

		public IntPtr lprgscode;

		public IntPtr lprgelemdescParam;

		[__DynamicallyInvokable]
		public FUNCKIND funckind;

		[__DynamicallyInvokable]
		public INVOKEKIND invkind;

		[__DynamicallyInvokable]
		public CALLCONV callconv;

		[__DynamicallyInvokable]
		public short cParams;

		[__DynamicallyInvokable]
		public short cParamsOpt;

		[__DynamicallyInvokable]
		public short oVft;

		[__DynamicallyInvokable]
		public short cScodes;

		[__DynamicallyInvokable]
		public ELEMDESC elemdescFunc;

		[__DynamicallyInvokable]
		public short wFuncFlags;
	}
}
