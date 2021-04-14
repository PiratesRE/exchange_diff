using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class TextProcessorIdAndMatchReferencesValidator : IClassificationRuleCollectionValidator
	{
		internal TextProcessorIdAndMatchReferencesValidator(Dictionary<TextProcessorType, TextProcessorGrouping> cannedOobTextProcessorsGroupedByType)
		{
			if (cannedOobTextProcessorsGroupedByType == null)
			{
				throw new ArgumentNullException("cannedOobTextProcessorsGroupedByType");
			}
			this.cannedOobTextProcessorsGroupedByType = cannedOobTextProcessorsGroupedByType;
		}

		private void ValidateProcessorIdAndMatchReferences(XDocument rulePackXDocument)
		{
			ExAssert.RetailAssert(rulePackXDocument != null, "Extra rule package validation must take place after XML schema validation passed!");
			IDictionary<string, ExchangeBuild> textProcessorsFromTextProcessorGrouping = TextProcessorUtils.GetTextProcessorsFromTextProcessorGrouping(this.cannedOobTextProcessorsGroupedByType, null);
			ICollection<string> keys = textProcessorsFromTextProcessorGrouping.Keys;
			Dictionary<TextProcessorType, TextProcessorGrouping> textProcessorsGroupings = TextProcessorUtils.GetRulePackScopedTextProcessorsGroupedByType(rulePackXDocument).ToDictionary((TextProcessorGrouping textProcessorGroup) => textProcessorGroup.Key);
			IDictionary<string, ExchangeBuild> textProcessorsFromTextProcessorGrouping2 = TextProcessorUtils.GetTextProcessorsFromTextProcessorGrouping(textProcessorsGroupings, null);
			ICollection<string> keys2 = textProcessorsFromTextProcessorGrouping2.Keys;
			List<string> list = keys.AsParallel<string>().Union(keys2.AsParallel<string>(), ClassificationDefinitionConstants.TextProcessorIdComparer).ToList<string>();
			if (list.Count != keys.Count + keys2.Count)
			{
				List<string> list2 = keys.AsParallel<string>().Intersect(keys2.AsParallel<string>(), ClassificationDefinitionConstants.TextProcessorIdComparer).ToList<string>();
				LocalizedString message = Strings.ClassificationRuleCollectionReservedProcessorIdViolation(string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, list2));
				throw ClassificationDefinitionUtils.PopulateExceptionSource<ClassificationRuleCollectionIdentifierValidationException, List<string>>(new ClassificationRuleCollectionIdentifierValidationException(message), list2);
			}
			HashSet<string> hashSet = new HashSet<string>(ClassificationDefinitionConstants.TextProcessorIdComparer);
			IEnumerable<KeyValuePair<string, ExchangeBuild>> textProcessorReferences = TextProcessorUtils.GetTextProcessorReferences(rulePackXDocument.Root);
			foreach (KeyValuePair<string, ExchangeBuild> keyValuePair in textProcessorReferences)
			{
				ExchangeBuild objB;
				if ((!textProcessorsFromTextProcessorGrouping2.TryGetValue(keyValuePair.Key, out objB) && !textProcessorsFromTextProcessorGrouping.TryGetValue(keyValuePair.Key, out objB)) || !(keyValuePair.Value >= objB))
				{
					hashSet.Add(keyValuePair.Key);
				}
			}
			IDictionary<string, ExchangeBuild> textProcessorsFromTextProcessorGrouping3 = TextProcessorUtils.GetTextProcessorsFromTextProcessorGrouping(this.cannedOobTextProcessorsGroupedByType, (TextProcessorType textProcessorType) => textProcessorType == TextProcessorType.Fingerprint);
			IDictionary<string, ExchangeBuild> textProcessorsFromTextProcessorGrouping4 = TextProcessorUtils.GetTextProcessorsFromTextProcessorGrouping(textProcessorsGroupings, (TextProcessorType textProcessorType) => textProcessorType == TextProcessorType.Fingerprint);
			IEnumerable<KeyValuePair<string, ExchangeBuild>> textProcessorReferences2 = TextProcessorUtils.GetTextProcessorReferences(rulePackXDocument.Root, ClassificationDefinitionConstants.MceIdMatchElementNames);
			foreach (KeyValuePair<string, ExchangeBuild> keyValuePair2 in textProcessorReferences2)
			{
				if (textProcessorsFromTextProcessorGrouping4.ContainsKey(keyValuePair2.Key) || textProcessorsFromTextProcessorGrouping3.ContainsKey(keyValuePair2.Key))
				{
					hashSet.Add(keyValuePair2.Key);
				}
			}
			if (hashSet.Count > 0)
			{
				LocalizedString message2 = Strings.ClassificationRuleCollectionInvalidProcessorReferenceViolation(string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, hashSet));
				throw ClassificationDefinitionUtils.PopulateExceptionSource<ClassificationRuleCollectionProcessorReferenceValidationException, HashSet<string>>(new ClassificationRuleCollectionProcessorReferenceValidationException(message2), hashSet);
			}
		}

		public void Validate(ValidationContext context, XDocument rulePackXDocument)
		{
			this.ValidateProcessorIdAndMatchReferences(rulePackXDocument);
		}

		private readonly Dictionary<TextProcessorType, TextProcessorGrouping> cannedOobTextProcessorsGroupedByType;
	}
}
