using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal interface IDataClassificationComplexityValidator
	{
		void Initialize(DataClassificationConfig dcDataClassificationValidationConfig);

		bool IsRuleComplexityLimitExceeded(RuleComplexityData ruleComplexityData);

		LocalizedString CreateExceptionMessage(IList<string> offendingRulesList);
	}
}
