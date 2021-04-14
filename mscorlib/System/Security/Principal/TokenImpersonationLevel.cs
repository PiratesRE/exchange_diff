using System;
using System.Runtime.InteropServices;

namespace System.Security.Principal
{
	[ComVisible(true)]
	[__DynamicallyInvokable]
	[Serializable]
	public enum TokenImpersonationLevel
	{
		[__DynamicallyInvokable]
		None,
		[__DynamicallyInvokable]
		Anonymous,
		[__DynamicallyInvokable]
		Identification,
		[__DynamicallyInvokable]
		Impersonation,
		[__DynamicallyInvokable]
		Delegation
	}
}
