using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.ValidationRules
{
	[Serializable]
	internal class RestrictedValidationRule : ValidationRule
	{
		public RestrictedValidationRule(ValidationRuleDefinition ruleDefinition, IList<CapabilityIdentifierEvaluator> restrictedCapabilityEvaluators, IList<CapabilityIdentifierEvaluator> overridingAllowCapabilityEvaluators, RoleEntry roleEntry) : base(ruleDefinition, restrictedCapabilityEvaluators, overridingAllowCapabilityEvaluators, roleEntry)
		{
		}

		protected override bool InternalTryValidate(ADRawEntry adObject, out RuleValidationException validationException)
		{
			validationException = null;
			if (!base.IsOverridingAllowCapabilityFound(adObject))
			{
				foreach (CapabilityIdentifierEvaluator capabilityIdentifierEvaluator in base.RestrictedCapabilityEvaluators)
				{
					switch (capabilityIdentifierEvaluator.Evaluate(adObject))
					{
					case CapabilityEvaluationResult.Yes:
						validationException = new RuleValidationException(base.GetValidationRuleErrorMessage(adObject, capabilityIdentifierEvaluator.Capability));
						return false;
					}
				}
				return true;
			}
			return true;
		}
	}
}
