using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageClassificationSchema
	{
		internal static readonly HygienePropertyDefinition ClassificationIdProperty = CommonMessageTraceSchema.ClassificationIdProperty;

		internal static readonly HygienePropertyDefinition ExMessageIdProperty = CommonMessageTraceSchema.ExMessageIdProperty;

		internal static readonly HygienePropertyDefinition DataClassificationIdProperty = CommonMessageTraceSchema.DataClassificationIdProperty;
	}
}
