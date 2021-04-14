using System;

namespace System.Diagnostics.Tracing
{
	[AttributeUsage(AttributeTargets.Method)]
	[__DynamicallyInvokable]
	public sealed class NonEventAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public NonEventAttribute()
		{
		}
	}
}
