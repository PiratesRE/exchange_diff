using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AssignedCategoriesCondition : StringCondition
	{
		private AssignedCategoriesCondition(Rule rule, string[] text) : base(ConditionType.AssignedCategoriesCondition, rule, text)
		{
		}

		public static AssignedCategoriesCondition Create(Rule rule, string[] text)
		{
			Condition.CheckParams(new object[]
			{
				rule,
				text
			});
			return new AssignedCategoriesCondition(rule, text);
		}

		internal override Restriction BuildRestriction()
		{
			PropTag propertyTag = base.Rule.PropertyDefinitionToPropTagFromCache(Rule.NamedDefinitions[0]);
			return (1 == base.Text.Length) ? Condition.CreatePropertyRestriction<string>(propertyTag, base.Text[0]) : Condition.CreateAndStringPropertyRestriction(propertyTag, base.Text);
		}
	}
}
