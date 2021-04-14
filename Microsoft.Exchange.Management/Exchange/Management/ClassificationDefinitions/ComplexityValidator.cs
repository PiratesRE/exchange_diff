using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class ComplexityValidator : IClassificationRuleCollectionValidator
	{
		internal ComplexityValidator(Dictionary<TextProcessorType, TextProcessorGrouping> cannedOobTextProcessorIdsGroupedByType, IList<IDataClassificationComplexityValidator> dataClassificationComplexityValidators)
		{
			if (cannedOobTextProcessorIdsGroupedByType == null)
			{
				throw new ArgumentNullException("cannedOobTextProcessorIdsGroupedByType");
			}
			if (dataClassificationComplexityValidators == null)
			{
				throw new ArgumentNullException("dataClassificationComplexityValidators");
			}
			if (dataClassificationComplexityValidators.Any((IDataClassificationComplexityValidator complexityValidator) => null == complexityValidator))
			{
				throw new ArgumentException(new ArgumentException().Message, "dataClassificationComplexityValidators");
			}
			this.cannedOobTextProcessorIdsGroupedByType = cannedOobTextProcessorIdsGroupedByType;
			this.dataClassificationComplexityValidators = dataClassificationComplexityValidators;
		}

		private static Dictionary<string, int> GetKeywordProcessorTermsCount(XDocument rulePackXDocument)
		{
			return (from keywordElement in rulePackXDocument.Descendants(XmlProcessingUtils.GetMceNsQualifiedNodeName("Keyword"))
			select keywordElement).ToDictionary((XElement keywordElement) => keywordElement.Attribute("id").Value, (XElement keywordElement) => keywordElement.Descendants(XmlProcessingUtils.GetMceNsQualifiedNodeName("Term")).AsParallel<XElement>().Count<XElement>());
		}

		private IEnumerable<KeyValuePair<string, RuleComplexityData>> GetRulePackComplexityData(XDocument rulePackXDocument)
		{
			ExAssert.RetailAssert(rulePackXDocument != null, "The rule pack document instance passed to GetRulePackComplexityData cannot be null!");
			Dictionary<TextProcessorType, TextProcessorGrouping> customTextProcessorGroups = TextProcessorUtils.GetRulePackScopedTextProcessorsGroupedByType(rulePackXDocument).ToDictionary((TextProcessorGrouping textProcessorGroup) => textProcessorGroup.Key);
			Dictionary<string, int> customKeywordProcessorsTermsCount = ComplexityValidator.GetKeywordProcessorTermsCount(rulePackXDocument);
			IEnumerable<XElement> source = from rulePackElement in rulePackXDocument.Descendants()
			where ClassificationDefinitionConstants.MceRuleElementNames.Contains(rulePackElement.Name.LocalName)
			select rulePackElement;
			return from ruleElement in source.AsParallel<XElement>().AsOrdered<XElement>()
			select new KeyValuePair<string, RuleComplexityData>(ruleElement.Attribute("id").Value, RuleComplexityData.Create(ruleElement, customTextProcessorGroups, customKeywordProcessorsTermsCount, this.cannedOobTextProcessorIdsGroupedByType));
		}

		private static void ValidateRuleComplexity(IEnumerable<KeyValuePair<string, RuleComplexityData>> rulePackComplexityData, IDataClassificationComplexityValidator complexityValidator)
		{
			List<string> list = (from ruleComplexityData in rulePackComplexityData
			where complexityValidator.IsRuleComplexityLimitExceeded(ruleComplexityData.Value)
			select ruleComplexityData.Key).ToList<string>();
			if (list.Count > 0)
			{
				LocalizedString message = complexityValidator.CreateExceptionMessage(list);
				throw ClassificationDefinitionUtils.PopulateExceptionSource<ClassificationRuleCollectionComplexityValidationException, List<string>>(new ClassificationRuleCollectionComplexityValidationException(message), list);
			}
		}

		public void Validate(ValidationContext context, XDocument rulePackXDocument)
		{
			if (context.DcValidationConfig == null || context.IsPayloadOobRuleCollection)
			{
				return;
			}
			DataClassificationConfig dcValidationConfig = context.DcValidationConfig;
			List<KeyValuePair<string, RuleComplexityData>> rulePackComplexityData = this.GetRulePackComplexityData(rulePackXDocument).ToList<KeyValuePair<string, RuleComplexityData>>();
			foreach (IDataClassificationComplexityValidator dataClassificationComplexityValidator in this.dataClassificationComplexityValidators)
			{
				dataClassificationComplexityValidator.Initialize(dcValidationConfig);
				ComplexityValidator.ValidateRuleComplexity(rulePackComplexityData, dataClassificationComplexityValidator);
			}
		}

		private readonly Dictionary<TextProcessorType, TextProcessorGrouping> cannedOobTextProcessorIdsGroupedByType;

		private readonly IList<IDataClassificationComplexityValidator> dataClassificationComplexityValidators;
	}
}
