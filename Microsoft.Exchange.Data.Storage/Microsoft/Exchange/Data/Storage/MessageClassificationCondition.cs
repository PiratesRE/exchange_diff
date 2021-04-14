using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MessageClassificationCondition : StringCondition
	{
		private MessageClassificationCondition(Rule rule, string[] text) : base(ConditionType.MessageClassificationCondition, rule, text)
		{
		}

		public static MessageClassificationCondition Create(Rule rule, string[] text)
		{
			Condition.CheckParams(new object[]
			{
				rule
			});
			return new MessageClassificationCondition(rule, text);
		}

		public override Rule.ProviderIdEnum ProviderId
		{
			get
			{
				return Rule.ProviderIdEnum.Exchange14;
			}
		}

		internal override Restriction BuildRestriction()
		{
			PropTag propertyTag = base.Rule.PropertyDefinitionToPropTagFromCache(Rule.NamedDefinitions[1]);
			if (base.Text == null || base.Text.Length == 0)
			{
				return Condition.CreatePropertyRestriction<bool>(propertyTag, true);
			}
			PropTag propertyTag2 = base.Rule.PropertyDefinitionToPropTagFromCache(Rule.NamedDefinitions[2]);
			Restriction restriction = (1 == base.Text.Length) ? Condition.CreatePropertyRestriction<string>(propertyTag2, base.Text[0]) : Condition.CreateAndStringPropertyRestriction(propertyTag2, base.Text);
			return Condition.CreateAndRestriction(new Restriction[]
			{
				Condition.CreatePropertyRestriction<bool>(propertyTag, true),
				restriction
			});
		}
	}
}
