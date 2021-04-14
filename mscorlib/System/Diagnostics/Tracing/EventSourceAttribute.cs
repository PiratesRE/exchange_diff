using System;

namespace System.Diagnostics.Tracing
{
	[AttributeUsage(AttributeTargets.Class)]
	[__DynamicallyInvokable]
	public sealed class EventSourceAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public string Name { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		[__DynamicallyInvokable]
		public string Guid { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		[__DynamicallyInvokable]
		public string LocalizationResources { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		[__DynamicallyInvokable]
		public EventSourceAttribute()
		{
		}
	}
}
