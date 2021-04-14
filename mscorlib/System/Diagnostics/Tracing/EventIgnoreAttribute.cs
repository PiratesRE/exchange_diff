using System;

namespace System.Diagnostics.Tracing
{
	[AttributeUsage(AttributeTargets.Property)]
	[__DynamicallyInvokable]
	public class EventIgnoreAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public EventIgnoreAttribute()
		{
		}
	}
}
