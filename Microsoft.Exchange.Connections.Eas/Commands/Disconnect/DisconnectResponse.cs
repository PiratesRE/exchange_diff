using System;
using Microsoft.Exchange.Connections.Eas.Commands.Autodiscover;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Disconnect
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class DisconnectResponse : IHaveAnHttpStatus
	{
		public HttpStatus HttpStatus { get; set; }

		internal DisconnectStatus DisconnectStatus { get; set; }

		internal AutodiscoverEndpoint LastResolvedEndpoint { get; set; }
	}
}
