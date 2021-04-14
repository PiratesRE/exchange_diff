using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.PolicyNudges
{
	[XmlType("ClassificationDefinitions")]
	public sealed class ClassificationDefinitions15 : IVisitee15
	{
		[XmlElement("ClassificationDefinition")]
		public List<ClassificationDefinition15> ClassificationDefinitions { get; set; }

		public void Accept(Visitor15 visitor)
		{
			visitor.Visit(this);
		}
	}
}
