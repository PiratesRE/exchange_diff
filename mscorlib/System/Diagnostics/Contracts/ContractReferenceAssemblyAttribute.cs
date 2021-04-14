using System;

namespace System.Diagnostics.Contracts
{
	[AttributeUsage(AttributeTargets.Assembly)]
	[__DynamicallyInvokable]
	public sealed class ContractReferenceAssemblyAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public ContractReferenceAssemblyAttribute()
		{
		}
	}
}
