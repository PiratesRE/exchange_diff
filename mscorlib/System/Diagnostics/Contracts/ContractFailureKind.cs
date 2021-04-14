using System;

namespace System.Diagnostics.Contracts
{
	[__DynamicallyInvokable]
	public enum ContractFailureKind
	{
		[__DynamicallyInvokable]
		Precondition,
		[__DynamicallyInvokable]
		Postcondition,
		[__DynamicallyInvokable]
		PostconditionOnException,
		[__DynamicallyInvokable]
		Invariant,
		[__DynamicallyInvokable]
		Assert,
		[__DynamicallyInvokable]
		Assume
	}
}
