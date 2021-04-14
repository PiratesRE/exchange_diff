using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	[Serializable]
	internal sealed class ClassificationRuleCollectionNameMatcher : ClassificationIdentityMatcher<Tuple<TransportRule, XDocument>>
	{
		private ClassificationRuleCollectionNameMatcher(string searchName, string rawSearchName, MatchOptions matchOptions) : base(searchName, rawSearchName, matchOptions)
		{
		}

		internal static ClassificationRuleCollectionNameMatcher CreateFrom(TextFilter queryFilter, string rawSearchName)
		{
			ClassificationRuleCollectionNameMatcher result = null;
			if (queryFilter != null)
			{
				result = new ClassificationRuleCollectionNameMatcher(queryFilter.Text, rawSearchName, queryFilter.MatchOptions);
			}
			return result;
		}

		protected override bool EvaluateMatch(Tuple<TransportRule, XDocument> rulePackDataObject, Func<string, CultureInfo, CompareOptions, bool> objectPropertyMatchOperator)
		{
			ExAssert.RetailAssert(rulePackDataObject != null && rulePackDataObject.Item1 != null && rulePackDataObject.Item2 != null, "The rule pack data object passed to the name matcher must not be null");
			bool flag = false;
			DlpUtils.DataClassificationQueryContext dataClassificationQueryContext = new DlpUtils.DataClassificationQueryContext(rulePackDataObject.Item1.OrganizationId, ClassificationDefinitionsDiagnosticsReporter.Instance)
			{
				CurrentRuleCollectionTransportRuleObject = rulePackDataObject.Item1,
				CurrentRuleCollectionXDoc = rulePackDataObject.Item2
			};
			XElement operationArgument;
			if (!DlpUtils.TryExecuteOperation<XmlException, XDocument, XElement>(dataClassificationQueryContext, new Func<XDocument, XElement>(XmlProcessingUtils.GetRulePackageMetadataElement), dataClassificationQueryContext.CurrentRuleCollectionXDoc, out operationArgument))
			{
				return flag;
			}
			ClassificationRuleCollectionLocalizableDetails classificationRuleCollectionLocalizableDetails;
			if (!DlpUtils.TryExecuteOperation<XmlException, XElement, ClassificationRuleCollectionLocalizableDetails>(dataClassificationQueryContext, new Func<XElement, ClassificationRuleCollectionLocalizableDetails>(XmlProcessingUtils.ReadDefaultRulePackageMetadata), operationArgument, out classificationRuleCollectionLocalizableDetails))
			{
				return flag;
			}
			flag = objectPropertyMatchOperator(classificationRuleCollectionLocalizableDetails.Name, classificationRuleCollectionLocalizableDetails.Culture, CompareOptions.IgnoreCase);
			if (!flag)
			{
				Dictionary<CultureInfo, ClassificationRuleCollectionLocalizableDetails> source;
				if (!DlpUtils.TryExecuteOperation<XmlException, XElement, Dictionary<CultureInfo, ClassificationRuleCollectionLocalizableDetails>>(dataClassificationQueryContext, new Func<XElement, Dictionary<CultureInfo, ClassificationRuleCollectionLocalizableDetails>>(XmlProcessingUtils.ReadAllRulePackageMetadata), operationArgument, out source))
				{
					return flag;
				}
				flag = source.Any((KeyValuePair<CultureInfo, ClassificationRuleCollectionLocalizableDetails> localizedDetail) => objectPropertyMatchOperator(localizedDetail.Value.Name, localizedDetail.Key, CompareOptions.IgnoreCase));
			}
			return flag;
		}
	}
}
