using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Model.Request.Autodiscover;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Autodiscover
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[XmlRoot(ElementName = "Autodiscover", Namespace = "http://schemas.microsoft.com/exchange/autodiscover/mobilesync/requestschema/2006", IsNullable = false)]
	public class AutodiscoverRequest
	{
		public AutodiscoverRequest()
		{
			this.Request = new Request();
			this.AutodiscoverOption = AutodiscoverOption.Probes;
		}

		[XmlElement(ElementName = "Request")]
		public Request Request { get; set; }

		[XmlIgnore]
		internal AutodiscoverOption AutodiscoverOption { get; set; }
	}
}
