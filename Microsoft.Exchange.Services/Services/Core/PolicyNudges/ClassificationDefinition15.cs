using System;
using System.Xml.Serialization;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	[XmlType("ClassificationDefinition")]
	public sealed class ClassificationDefinition15 : IVisitee15, IVersionedItem
	{
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

		public void Accept(Visitor15 visitor)
		{
			visitor.Visit(this);
		}
	}
}
