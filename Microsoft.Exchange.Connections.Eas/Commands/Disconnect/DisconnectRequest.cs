using System;
using System.Xml.Serialization;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Disconnect
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class DisconnectRequest
	{
		[XmlIgnore]
		internal static DisconnectRequest Default { get; set; } = new DisconnectRequest();
	}
}
