using System;
using System.Xml.Linq;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal class QueryMatchResult
	{
		internal string QueryString { get; set; }

		internal string MatchingRuleId { get; set; }

		internal XElement MatchingRuleXElement { get; set; }

		internal XElement MatchingResourceXElement { get; set; }
	}
}
