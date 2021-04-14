using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class CustomKeywordProcessorReferencesValidator : IDataClassificationComplexityValidator
	{
		public void Initialize(DataClassificationConfig dcDataClassificationValidationConfig)
		{
			if (dcDataClassificationValidationConfig == null)
			{
				throw new ArgumentNullException("dcDataClassificationValidationConfig");
			}
			this.distinctTermsFromCustomKeywordProcessorRefsCountLimit = dcDataClassificationValidationConfig.NumberOfKeywords;
		}

		public bool IsRuleComplexityLimitExceeded(RuleComplexityData ruleComplexityData)
		{
			if (ruleComplexityData == null)
			{
				throw new ArgumentNullException("ruleComplexityData");
			}
			return ruleComplexityData.TermsFromCustomKeywordProcessorReferencesCount > this.distinctTermsFromCustomKeywordProcessorRefsCountLimit;
		}

		public LocalizedString CreateExceptionMessage(IList<string> offendingRulesList)
		{
			return Strings.ClassificationRuleCollectionCustomTermsCountExceedLimit(this.distinctTermsFromCustomKeywordProcessorRefsCountLimit, string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, offendingRulesList));
		}

		private int distinctTermsFromCustomKeywordProcessorRefsCountLimit;
	}
}
