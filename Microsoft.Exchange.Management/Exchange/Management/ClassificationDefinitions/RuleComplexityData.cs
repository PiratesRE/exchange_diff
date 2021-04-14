using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Serializable]
	internal class RuleComplexityData
	{
		internal int DistinctRegexProcessorReferencesCount { get; private set; }

		internal int TermsFromCustomKeywordProcessorReferencesCount { get; private set; }

		internal int DistinctFunctionProcessorReferencesCount { get; private set; }

		internal int AnyBlocksCount { get; private set; }

		internal int MaxAnyBlocksDepth { get; private set; }

		private void ProcessAnyBlocksComplexityData(XElement ruleElement)
		{
			List<XElement> list = ruleElement.Descendants(RuleComplexityData.AnyElementQualifiedName).AsParallel<XElement>().ToList<XElement>();
			List<XElement> list2 = (from anyElement in list
			where !anyElement.Elements(RuleComplexityData.AnyElementQualifiedName).Any<XElement>()
			select anyElement).ToList<XElement>();
			int maxAnyBlocksDepth = (list2.Count > 0) ? list2.Max(new Func<XElement, int>(RuleComplexityData.CalculateAnyNestingDepth)) : 0;
			this.AnyBlocksCount = list.Count;
			this.MaxAnyBlocksDepth = maxAnyBlocksDepth;
		}

		private static int CalculateAnyNestingDepth(XElement anyLeafElement)
		{
			int num = 0;
			XElement xelement = anyLeafElement;
			while (xelement.Parent != null && xelement.Parent.Name.Equals(RuleComplexityData.AnyElementQualifiedName))
			{
				num++;
				xelement = xelement.Parent;
			}
			return num;
		}

		private static bool IsRegexProcessorReference(string textProcessorRef, Dictionary<TextProcessorType, TextProcessorGrouping> oobTextProcessorGroups, Dictionary<TextProcessorType, TextProcessorGrouping> customTextProcessorGroups)
		{
			TextProcessorGrouping textProcessorGrouping;
			return (customTextProcessorGroups.TryGetValue(TextProcessorType.Regex, out textProcessorGrouping) && textProcessorGrouping.Contains(textProcessorRef)) || (oobTextProcessorGroups.TryGetValue(TextProcessorType.Regex, out textProcessorGrouping) && textProcessorGrouping.Contains(textProcessorRef));
		}

		private static bool IsFunctionProcessorReference(string textProcessorRef, Dictionary<TextProcessorType, TextProcessorGrouping> oobTextProcessorGroups)
		{
			TextProcessorGrouping textProcessorGrouping;
			return oobTextProcessorGroups.TryGetValue(TextProcessorType.Function, out textProcessorGrouping) && textProcessorGrouping.Contains(textProcessorRef);
		}

		private static int GetTermsFromCustomKeywordProcessorCount(string textProcessorRef, Dictionary<TextProcessorType, TextProcessorGrouping> customTextProcessorGroups, Dictionary<string, int> customKeywordProcessorsTermsCount)
		{
			int result = 0;
			TextProcessorGrouping textProcessorGrouping;
			if (customTextProcessorGroups.TryGetValue(TextProcessorType.Keyword, out textProcessorGrouping) && textProcessorGrouping.Contains(textProcessorRef))
			{
				bool condition = customKeywordProcessorsTermsCount.TryGetValue(textProcessorRef, out result);
				ExAssert.RetailAssert(condition, "The terms count lookup for custom keyword processor has unexpectedly failed!");
			}
			return result;
		}

		internal static RuleComplexityData Create(XElement ruleElement, Dictionary<TextProcessorType, TextProcessorGrouping> customTextProcessorGroups, Dictionary<string, int> customKeywordProcessorsTermsCount, Dictionary<TextProcessorType, TextProcessorGrouping> oobTextProcessorGroups = null)
		{
			if (ruleElement == null)
			{
				throw new ArgumentNullException("ruleElement");
			}
			if (customTextProcessorGroups == null)
			{
				throw new ArgumentNullException("customTextProcessorGroups");
			}
			if (customKeywordProcessorsTermsCount == null)
			{
				throw new ArgumentNullException("customKeywordProcessorsTermsCount");
			}
			oobTextProcessorGroups = (oobTextProcessorGroups ?? TextProcessorUtils.OobProcessorsGroupedByType);
			List<string> list = (from versionedTextProcessorReference in TextProcessorUtils.GetTextProcessorReferences(ruleElement)
			select versionedTextProcessorReference.Key).Distinct(ClassificationDefinitionConstants.TextProcessorIdComparer).ToList<string>();
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			foreach (string textProcessorRef in list)
			{
				if (RuleComplexityData.IsRegexProcessorReference(textProcessorRef, oobTextProcessorGroups, customTextProcessorGroups))
				{
					num++;
				}
				else if (RuleComplexityData.IsFunctionProcessorReference(textProcessorRef, oobTextProcessorGroups))
				{
					num3++;
				}
				else
				{
					num2 += RuleComplexityData.GetTermsFromCustomKeywordProcessorCount(textProcessorRef, customTextProcessorGroups, customKeywordProcessorsTermsCount);
				}
			}
			RuleComplexityData ruleComplexityData = new RuleComplexityData
			{
				DistinctRegexProcessorReferencesCount = num,
				TermsFromCustomKeywordProcessorReferencesCount = num2,
				DistinctFunctionProcessorReferencesCount = num3
			};
			ruleComplexityData.ProcessAnyBlocksComplexityData(ruleElement);
			return ruleComplexityData;
		}

		private static readonly XName AnyElementQualifiedName = XmlProcessingUtils.GetMceNsQualifiedNodeName("Any");
	}
}
