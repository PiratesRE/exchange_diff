using System;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal class RoleEntryValidationRuleTuple
	{
		public RoleEntryValidationRuleTuple(ValidationRuleDefinition ruleDefinition, RoleEntry matchingRoleEntry)
		{
			if (ruleDefinition == null)
			{
				throw new ArgumentNullException("ruleDefinition");
			}
			if (null == matchingRoleEntry)
			{
				throw new ArgumentNullException("matchingRoleEntry");
			}
			this.RuleDefinition = ruleDefinition;
			this.MatchingRoleEntry = matchingRoleEntry;
		}

		public ValidationRuleDefinition RuleDefinition { get; private set; }

		public RoleEntry MatchingRoleEntry { get; private set; }
	}
}
