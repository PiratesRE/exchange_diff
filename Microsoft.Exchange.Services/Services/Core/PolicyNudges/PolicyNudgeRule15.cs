using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	[XmlType("PolicyNudgeRule")]
	public sealed class PolicyNudgeRule15 : IVisitee15, IVersionedItem, IOtherAttributes
	{
		public PolicyNudgeRule15()
		{
			this.OtherAttributes = new List<OtherAttribute>();
		}

		[XmlAttribute("id")]
		public string ID { get; set; }

		[XmlAttribute("version")]
		public long Version { get; set; }

		[XmlIgnore]
		DateTime IVersionedItem.Version
		{
			get
			{
				return DateTime.FromBinary(this.Version);
			}
		}

		[XmlIgnore]
		public List<OtherAttribute> OtherAttributes { get; private set; }

		public void Accept(Visitor15 visitor)
		{
			visitor.Visit(this);
		}
	}
}
