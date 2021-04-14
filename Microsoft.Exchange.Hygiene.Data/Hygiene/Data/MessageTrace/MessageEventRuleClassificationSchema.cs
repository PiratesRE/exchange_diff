using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageEventRuleClassificationSchema
	{
		internal static readonly HygienePropertyDefinition EventRuleClassificationIdProperty = CommonMessageTraceSchema.EventRuleClassificationIdProperty;

		internal static readonly HygienePropertyDefinition EventRuleIdProperty = CommonMessageTraceSchema.EventRuleIdProperty;

		internal static readonly HygienePropertyDefinition DataClassificationIdProperty = CommonMessageTraceSchema.DataClassificationIdProperty;

		internal static readonly HygienePropertyDefinition ExMessageIdProperty = CommonMessageTraceSchema.ExMessageIdProperty;
	}
}
