using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Classification
{
	internal sealed class ClassificationRulePackage : IVersionedItem
	{
		public string RuleXml { get; set; }

		public string ID { get; set; }

		public DateTime Version { get; set; }

		public HashSet<string> VersionedDataClassificationIds { get; set; }
	}
}
