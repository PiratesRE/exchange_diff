using System;

namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false, Inherited = false)]
	[__DynamicallyInvokable]
	public sealed class DisablePrivateReflectionAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public DisablePrivateReflectionAttribute()
		{
		}
	}
}
