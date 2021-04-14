using System;

namespace System.Security
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Interface | AttributeTargets.Delegate, AllowMultiple = false, Inherited = false)]
	[__DynamicallyInvokable]
	public sealed class SecuritySafeCriticalAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public SecuritySafeCriticalAttribute()
		{
		}
	}
}
