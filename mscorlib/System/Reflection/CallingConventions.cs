using System;
using System.Runtime.InteropServices;

namespace System.Reflection
{
	[Flags]
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum CallingConventions
	{
		[__DynamicallyInvokable]
		Standard = 1,
		[__DynamicallyInvokable]
		VarArgs = 2,
		[__DynamicallyInvokable]
		Any = 3,
		[__DynamicallyInvokable]
		HasThis = 32,
		[__DynamicallyInvokable]
		ExplicitThis = 64
	}
}
