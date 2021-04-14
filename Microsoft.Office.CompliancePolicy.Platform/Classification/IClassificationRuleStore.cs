using System;
using System.Collections.Generic;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	public interface IClassificationRuleStore : IRulePackageLoader
	{
		RULE_PACKAGE_DETAILS[] GetRulePackageDetails(IClassificationItem classificationItem);

		RuleDefinitionDetails GetRuleDetails(string ruleId, string localeName = null);

		IEnumerable<RuleDefinitionDetails> GetAllRuleDetails(bool loadLocalizableData = false);
	}
}
