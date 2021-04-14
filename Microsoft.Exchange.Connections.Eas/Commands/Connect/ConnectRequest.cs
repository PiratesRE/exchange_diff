using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Connections.Eas.Commands.Autodiscover;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Connect
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class ConnectRequest
	{
		internal ConnectRequest()
		{
			this.AutodiscoverOption = AutodiscoverOption.ExistingEndpointAndProbes;
		}

		[XmlIgnore]
		internal static ConnectRequest Default { get; set; } = new ConnectRequest();

		[XmlIgnore]
		internal AutodiscoverOption AutodiscoverOption { get; set; }
	}
}
