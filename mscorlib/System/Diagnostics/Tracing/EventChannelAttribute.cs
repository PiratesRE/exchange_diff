using System;

namespace System.Diagnostics.Tracing
{
	[AttributeUsage(AttributeTargets.Field)]
	internal class EventChannelAttribute : Attribute
	{
		public bool Enabled { get; set; }

		public EventChannelType EventChannelType { get; set; }
	}
}
