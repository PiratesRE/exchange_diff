using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Office.CompliancePolicy.Classification
{
	internal static class ClassificationUtils
	{
		public static RuleDefinitionDetails ValidateRuleId(string ruleId, IDictionary<Guid, RuleDefinitionDetails> ruleDefinitionTable)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("ruleId", ruleId);
			ArgumentValidator.ThrowIfCollectionNullOrEmpty<KeyValuePair<Guid, RuleDefinitionDetails>>("ruleDefinitionTable", ruleDefinitionTable);
			RuleDefinitionDetails ruleDefinitionDetails = null;
			Guid key;
			if (Guid.TryParse(ruleId, out key))
			{
				if (ruleDefinitionTable.ContainsKey(key))
				{
					ruleDefinitionDetails = ruleDefinitionTable[key];
				}
			}
			else
			{
				ruleDefinitionDetails = ruleDefinitionTable.Values.FirstOrDefault((RuleDefinitionDetails p) => p.LocalizableDetails.Values.Any((CLASSIFICATION_DEFINITION_DETAILS v) => v.DefinitionName.Equals(ruleId, StringComparison.InvariantCultureIgnoreCase)));
			}
			if (ruleDefinitionDetails == null)
			{
				return null;
			}
			return ruleDefinitionDetails.Clone();
		}
	}
}
