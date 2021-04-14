using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	internal class OrganizationValidationRuleDefinition : ValidationRuleDefinition
	{
		public OrganizationValidationRuleDefinition(string name, string feature, ValidationRuleSkus applicableSku, List<RoleEntry> applicableRoleEntries, ValidationErrorStringProvider errorStringProvider, List<ValidationRuleExpression> restrictionExpressions, List<ValidationRuleExpression> overridingAllowExpressions) : base(name, feature, applicableSku, applicableRoleEntries, new List<Capability>(), new List<Capability>(), errorStringProvider)
		{
			this.RestrictionExpressions = restrictionExpressions;
			this.OverridingAllowExpressions = overridingAllowExpressions;
		}

		public List<ValidationRuleExpression> RestrictionExpressions { get; private set; }

		public List<ValidationRuleExpression> OverridingAllowExpressions { get; private set; }
	}
}
