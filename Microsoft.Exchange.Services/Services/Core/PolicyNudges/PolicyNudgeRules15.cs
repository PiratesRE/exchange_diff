using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	[XmlType("PolicyNudgeRules")]
	public sealed class PolicyNudgeRules15 : IVisitee15
	{
		[XmlElement("PolicyNudgeRule")]
		public List<PolicyNudgeRule15> Rules { get; set; }

		public void Accept(Visitor15 visitor)
		{
			visitor.Visit(this);
		}
	}
}
