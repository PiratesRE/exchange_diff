using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class FunctionProcessorReferencesValidator : IDataClassificationComplexityValidator
	{
		public void Initialize(DataClassificationConfig dcDataClassificationValidationConfig)
		{
			if (dcDataClassificationValidationConfig == null)
			{
				throw new ArgumentNullException("dcDataClassificationValidationConfig");
			}
			this.distinctFunctionProcessorReferencesCountLimit = dcDataClassificationValidationConfig.DistinctFunctions;
		}

		public bool IsRuleComplexityLimitExceeded(RuleComplexityData ruleComplexityData)
		{
			if (ruleComplexityData == null)
			{
				throw new ArgumentNullException("ruleComplexityData");
			}
			return ruleComplexityData.DistinctFunctionProcessorReferencesCount > this.distinctFunctionProcessorReferencesCountLimit;
		}

		public LocalizedString CreateExceptionMessage(IList<string> offendingRulesList)
		{
			return Strings.ClassificationRuleCollectionDistinctFunctionsExceedLimit(this.distinctFunctionProcessorReferencesCountLimit, string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, offendingRulesList));
		}

		private int distinctFunctionProcessorReferencesCountLimit;
	}
}
