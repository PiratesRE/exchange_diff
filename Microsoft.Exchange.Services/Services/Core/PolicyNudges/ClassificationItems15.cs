using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	[XmlType("ClassificationItems")]
	public sealed class ClassificationItems15 : IVisitee15
	{
		[XmlAttribute("EngineVersion")]
		public string EngineVersion { get; set; }

		[XmlElement("ClassificationDefinitions")]
		public ClassificationDefinitions15 ClassificationDefinitions { get; set; }

		public void Accept(Visitor15 visitor)
		{
			visitor.Visit(this);
		}
	}
}
