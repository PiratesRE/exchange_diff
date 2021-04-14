using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessagingPolicies.Rules.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class RegexProcessorsValidator : IClassificationRuleCollectionValidator
	{
		private static void ValidateRegexProcessorsPerformance(XDocument rulePackXDocument, DataClassificationConfig dataClassificationConfig)
		{
			ExAssert.RetailAssert(dataClassificationConfig != null, "Must specify DataClassificationConfig instance when calling ValidateRegexProcessorsPerformance");
			foreach (KeyValuePair<string, string> keyValuePair in XmlProcessingUtils.GetRegexesInRulePackage(rulePackXDocument))
			{
				try
				{
					if (keyValuePair.Value.Length > dataClassificationConfig.RegExLength)
					{
						LocalizedString localizedString = Strings.ClassificationRuleCollectionPatternTooLong(dataClassificationConfig.RegExLength);
						throw new ValidationArgumentException(localizedString, null);
					}
					if (dataClassificationConfig.RegExGrammarLimit)
					{
						Pattern.ValidatePatternDoesNotBeginOrEndWithWildcards(keyValuePair.Value);
						Pattern.ValidatePatternDoesNotContainGroupsOrAssertionsWithWildcards(keyValuePair.Value);
						Pattern.ValidatePatternDoesNotContainMultiMatchOnGroupsOrAssertions(keyValuePair.Value);
						Pattern.ValidatePatternDoesNotHaveSequentialIdenticalMultiMatches(keyValuePair.Value);
						Pattern.ValidatePatternDoesNotContainEmptyAlternations(keyValuePair.Value);
					}
				}
				catch (ValidationArgumentException ex)
				{
					LocalizedString message = Strings.ClassificationRuleCollectionRegexPerformanceValidationFailure(keyValuePair.Key, ex.Message);
					throw new ClassificationRuleCollectionRegexValidationException(message, ex);
				}
			}
		}

		public void Validate(ValidationContext context, XDocument rulePackXDocument)
		{
			if (context.DcValidationConfig == null || context.IsPayloadOobRuleCollection)
			{
				return;
			}
			RegexProcessorsValidator.ValidateRegexProcessorsPerformance(rulePackXDocument, context.DcValidationConfig);
		}
	}
}
