using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Common.Email
{
	[XmlType(Namespace = "Email", TypeName = "Flag")]
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class Flag
	{
		[XmlElement(ElementName = "CompleteTime")]
		public string CompleteTime { get; set; }

		[XmlElement(ElementName = "FlagType")]
		public string FlagType { get; set; }

		[XmlElement(ElementName = "Status")]
		public int Status { get; set; }
	}
}
