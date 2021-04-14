using System;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageEventRuleSchema
	{
		internal static readonly HygienePropertyDefinition EventRuleIdProperty = CommonMessageTraceSchema.EventRuleIdProperty;

		internal static readonly HygienePropertyDefinition RuleIdProperty = new HygienePropertyDefinition("RuleId", typeof(Guid));

		internal static readonly HygienePropertyDefinition EventIdProperty = CommonMessageTraceSchema.EventIdProperty;

		internal static readonly HygienePropertyDefinition RuleTypeProperty = new HygienePropertyDefinition("RuleType", typeof(string));
	}
}
