using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Model.Response.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlType(TypeName = "Action")]
	public class Action
	{
		[XmlElement(ElementName = "Settings")]
		public Settings Settings { get; set; }

		[XmlElement(ElementName = "Error")]
		public ActionError Error { get; set; }

		[XmlElement(ElementName = "Redirect")]
		public string Redirect { get; set; }
	}
}
