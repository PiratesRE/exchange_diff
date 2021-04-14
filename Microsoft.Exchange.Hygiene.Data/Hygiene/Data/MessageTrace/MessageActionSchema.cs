using System;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Hygiene.Data.MessageTrace
{
	internal class MessageActionSchema
	{
		internal static readonly HygienePropertyDefinition RuleActionIdProperty = new HygienePropertyDefinition("RuleActionId", typeof(Guid));

		internal static readonly HygienePropertyDefinition EventRuleIdProperty = CommonMessageTraceSchema.EventRuleIdProperty;

		internal static readonly HygienePropertyDefinition NameProperty = new HygienePropertyDefinition("Name", typeof(string), string.Empty, ADPropertyDefinitionFlags.PersistDefaultValue);
	}
}
