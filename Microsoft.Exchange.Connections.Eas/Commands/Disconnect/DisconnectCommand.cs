using System;
using Microsoft.Exchange.Connections.Eas.Commands.Autodiscover;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Disconnect
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class DisconnectCommand : EasPseudoCommand<DisconnectRequest, DisconnectResponse>
	{
		protected internal DisconnectCommand(EasConnectionSettings easConnectionSettings) : base(Command.Disconnect, easConnectionSettings)
		{
			base.InitializeExpectedHttpStatusCodes(typeof(HttpStatus));
		}

		internal override DisconnectResponse Execute(DisconnectRequest disconnectRequest)
		{
			AutodiscoverEndpoint mostRecentEndpoint = base.EasConnectionSettings.EasEndpointSettings.MostRecentEndpoint;
			return new DisconnectResponse
			{
				HttpStatus = HttpStatus.OK,
				DisconnectStatus = DisconnectStatus.Success,
				LastResolvedEndpoint = mostRecentEndpoint
			};
		}
	}
}
