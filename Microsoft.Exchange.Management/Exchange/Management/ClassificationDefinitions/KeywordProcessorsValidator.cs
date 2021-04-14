using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class KeywordProcessorsValidator : IClassificationRuleCollectionValidator
	{
		private static IEnumerable<KeyValuePair<string, List<string>>> GetKeywordProcessorsTerms(XDocument rulePackXDocument)
		{
			return from keywordElement in rulePackXDocument.Descendants(XmlProcessingUtils.GetMceNsQualifiedNodeName("Keyword"))
			let keywordProcessorId = keywordElement.Attribute("id").Value
			let keywordTermsList = (from termElement in keywordElement.Descendants(XmlProcessingUtils.GetMceNsQualifiedNodeName("Term")).AsParallel<XElement>()
			select termElement.Value).ToList<string>()
			select new KeyValuePair<string, List<string>>(keywordProcessorId, keywordTermsList);
		}

		private static void ValidateKeywordProcessorsPerformance(XDocument rulePackXDocument, int keywordLengthLimit)
		{
			List<string> list = (from keywordProcessorDefinition in KeywordProcessorsValidator.GetKeywordProcessorsTerms(rulePackXDocument).AsParallel<KeyValuePair<string, List<string>>>()
			where keywordProcessorDefinition.Value.Any((string term) => term.Length > keywordLengthLimit)
			select keywordProcessorDefinition.Key).ToList<string>();
			if (list.Count > 0)
			{
				LocalizedString message = Strings.ClassificationRuleCollectionKeywordTooLong(keywordLengthLimit, string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, list));
				throw ClassificationDefinitionUtils.PopulateExceptionSource<ClassificationRuleCollectionKeywordValidationException, List<string>>(new ClassificationRuleCollectionKeywordValidationException(message), list);
			}
		}

		public void Validate(ValidationContext context, XDocument rulePackXDocument)
		{
			if (context.DcValidationConfig == null || context.IsPayloadOobRuleCollection)
			{
				return;
			}
			KeywordProcessorsValidator.ValidateKeywordProcessorsPerformance(rulePackXDocument, context.DcValidationConfig.KeywordLength);
		}
	}
}
