using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum OperandType
	{
		[__DynamicallyInvokable]
		InlineBrTarget,
		[__DynamicallyInvokable]
		InlineField,
		[__DynamicallyInvokable]
		InlineI,
		[__DynamicallyInvokable]
		InlineI8,
		[__DynamicallyInvokable]
		InlineMethod,
		[__DynamicallyInvokable]
		InlineNone,
		[Obsolete("This API has been deprecated. http://go.microsoft.com/fwlink/?linkid=14202")]
		InlinePhi,
		[__DynamicallyInvokable]
		InlineR,
		[__DynamicallyInvokable]
		InlineSig = 9,
		[__DynamicallyInvokable]
		InlineString,
		[__DynamicallyInvokable]
		InlineSwitch,
		[__DynamicallyInvokable]
		InlineTok,
		[__DynamicallyInvokable]
		InlineType,
		[__DynamicallyInvokable]
		InlineVar,
		[__DynamicallyInvokable]
		ShortInlineBrTarget,
		[__DynamicallyInvokable]
		ShortInlineI,
		[__DynamicallyInvokable]
		ShortInlineR,
		[__DynamicallyInvokable]
		ShortInlineVar
	}
}
