using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal interface IClassificationRuleCollectionValidator
	{
		void Validate(ValidationContext context, XDocument rulePackXDocument);
	}
}
