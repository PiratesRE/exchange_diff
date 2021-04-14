using System;
using System.Data.SqlTypes;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageEventSchema
	{
		internal static readonly HygienePropertyDefinition ExMessageIdProperty = CommonMessageTraceSchema.ExMessageIdProperty;

		internal static readonly HygienePropertyDefinition EventIdProperty = CommonMessageTraceSchema.EventIdProperty;

		internal static readonly HygienePropertyDefinition EventTypeProperty = new HygienePropertyDefinition("EventType", typeof(MessageTrackingEvent));

		internal static readonly HygienePropertyDefinition EventSourceProperty = new HygienePropertyDefinition("EventSource", typeof(MessageTrackingSource));

		internal static readonly HygienePropertyDefinition TimeStampProperty = new HygienePropertyDefinition("TimeStamp", typeof(DateTime), SqlDateTime.MinValue.Value, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
