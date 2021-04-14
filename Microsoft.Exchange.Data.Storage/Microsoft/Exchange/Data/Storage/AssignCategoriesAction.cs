using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AssignCategoriesAction : ActionBase
	{
		private AssignCategoriesAction(ActionType actionType, string[] categories, Rule rule) : base(actionType, rule)
		{
			this.categories = categories;
		}

		public static AssignCategoriesAction Create(string[] categories, Rule rule)
		{
			ActionBase.CheckParams(new object[]
			{
				rule
			});
			return new AssignCategoriesAction(ActionType.AssignCategoriesAction, categories, rule);
		}

		public string[] Categories
		{
			get
			{
				return this.categories;
			}
		}

		public override Rule.ProviderIdEnum ProviderId
		{
			get
			{
				return Rule.ProviderIdEnum.Exchange14;
			}
		}

		internal override RuleAction BuildRuleAction()
		{
			PropTag propTag = base.Rule.PropertyDefinitionToPropTagFromCache(Rule.NamedDefinitions[0]);
			return new RuleAction.Tag(new PropValue(propTag, this.categories));
		}

		private string[] categories;
	}
}
