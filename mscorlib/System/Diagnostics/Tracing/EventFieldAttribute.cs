using System;

namespace System.Diagnostics.Tracing
{
	[AttributeUsage(AttributeTargets.Property)]
	[__DynamicallyInvokable]
	public class EventFieldAttribute : Attribute
	{
		[__DynamicallyInvokable]
		public EventFieldTags Tags { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		internal string Name { get; set; }

		[__DynamicallyInvokable]
		public EventFieldFormat Format { [__DynamicallyInvokable] get; [__DynamicallyInvokable] set; }

		[__DynamicallyInvokable]
		public EventFieldAttribute()
		{
		}
	}
}
