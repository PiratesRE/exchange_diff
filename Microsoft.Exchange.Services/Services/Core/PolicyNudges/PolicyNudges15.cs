using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	[XmlType("PolicyNudges")]
	public sealed class PolicyNudges15 : IVisitee15
	{
		[XmlAttribute("OutlookVersion")]
		public string OutlookVersion { get; set; }

		[XmlAttribute("OutlookLocale")]
		public string OutlookLocale { get; set; }

		[XmlElement("PolicyNudgeRules")]
		public PolicyNudgeRules15 PolicyNudgeRules { get; set; }

		[XmlElement("ClassificationItems")]
		public ClassificationItems15 ClassificationItems { get; set; }

		public void Accept(Visitor15 visitor)
		{
			visitor.Visit(this);
		}
	}
}
