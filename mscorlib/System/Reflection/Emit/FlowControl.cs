using System;
using System.Runtime.InteropServices;

namespace System.Reflection.Emit
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum FlowControl
	{
		[__DynamicallyInvokable]
		Branch,
		[__DynamicallyInvokable]
		Break,
		[__DynamicallyInvokable]
		Call,
		[__DynamicallyInvokable]
		Cond_Branch,
		[__DynamicallyInvokable]
		Meta,
		[__DynamicallyInvokable]
		Next,
		[Obsolete("This API has been deprecated. http://go.microsoft.com/fwlink/?linkid=14202")]
		Phi,
		[__DynamicallyInvokable]
		Return,
		[__DynamicallyInvokable]
		Throw
	}
}
