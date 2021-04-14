using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal sealed class NameUniquenessValidator : IClassificationRuleCollectionValidator
	{
		public void Validate(ValidationContext context, XDocument rulePackXDocument)
		{
			ArgumentValidator.ThrowIfNull("context", context);
			ArgumentValidator.ThrowIfNull("rulePackXDocument", rulePackXDocument);
			if (context.IsPayloadFingerprintsRuleCollection)
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (XElement xelement in rulePackXDocument.Descendants(XmlProcessingUtils.GetMceNsQualifiedNodeName("Resource")))
				{
					HashSet<string> hashSet2 = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
					foreach (XElement xelement2 in xelement.Descendants(XmlProcessingUtils.GetMceNsQualifiedNodeName("Name")))
					{
						hashSet2.Add(xelement2.Value);
					}
					List<string> list = hashSet.Intersect(hashSet2).ToList<string>();
					if (list.Count > 0)
					{
						throw new DataClassificationNonUniqueNameViolationException(string.Join(Strings.ClassificationRuleCollectionOffendingListSeparator, list));
					}
					hashSet.UnionWith(hashSet2);
				}
			}
		}
	}
}
