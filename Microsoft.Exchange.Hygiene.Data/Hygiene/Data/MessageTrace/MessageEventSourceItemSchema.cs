using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal sealed class MessageEventSourceItemSchema
	{
		internal static readonly HygienePropertyDefinition SourceItemIdProperty = CommonMessageTraceSchema.SourceItemIdProperty;

		internal static readonly HygienePropertyDefinition EventIdProperty = CommonMessageTraceSchema.EventIdProperty;

		internal static readonly HygienePropertyDefinition NameProperty = new HygienePropertyDefinition("Name", typeof(string));
	}
}
