using System;
using System.Collections.Generic;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Eas.Commands.Autodiscover;
using Microsoft.Exchange.Connections.Eas.Commands.Options;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Connect
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConnectCommand : EasPseudoCommand<ConnectRequest, ConnectResponse>
	{
		protected internal ConnectCommand(EasConnectionSettings easConnectionSettings) : base(Command.Connect, easConnectionSettings)
		{
			base.InitializeExpectedHttpStatusCodes(typeof(HttpStatus));
		}

		internal override ConnectResponse Execute(ConnectRequest connectRequest)
		{
			AutodiscoverRequest autodiscoverRequest = new AutodiscoverRequest();
			autodiscoverRequest.AutodiscoverOption = connectRequest.AutodiscoverOption;
			autodiscoverRequest.Request.EMailAddress = base.EasConnectionSettings.EasEndpointSettings.UserSmtpAddressString;
			AutodiscoverCommand autodiscoverCommand = new AutodiscoverCommand(base.EasConnectionSettings);
			AutodiscoverResponse autodiscoverResponse = autodiscoverCommand.Execute(autodiscoverRequest);
			return new ConnectResponse(autodiscoverResponse, connectRequest.AutodiscoverOption);
		}

		internal ConnectResponse Execute(ConnectRequest connectRequest, IServerCapabilities requiredCapabilities)
		{
			ConnectResponse connectResponse = this.Execute(connectRequest);
			connectResponse.UserSmtpAddressString = base.EasConnectionSettings.EasEndpointSettings.UserSmtpAddressString;
			if (connectResponse.ConnectStatus != ConnectStatus.Success || requiredCapabilities == null)
			{
				return connectResponse;
			}
			OptionsCommand optionsCommand = new OptionsCommand(base.EasConnectionSettings);
			OptionsResponse optionsResponse = optionsCommand.Execute(new OptionsRequest());
			connectResponse.OptionsResponse = optionsResponse;
			EasServerCapabilities easServerCapabilities = optionsResponse.EasServerCapabilities;
			if (easServerCapabilities.Supports(requiredCapabilities))
			{
				base.EasConnectionSettings.ExtensionCapabilities = optionsResponse.EasExtensionCapabilities;
				return connectResponse;
			}
			IEnumerable<string> values = requiredCapabilities.NotIn(easServerCapabilities);
			string text = string.Join(", ", values);
			base.EasConnectionSettings.Log.Debug("ConnectCommand, missing capabilities: '{0}'.", new object[]
			{
				text
			});
			throw new MissingCapabilitiesException(text);
		}
	}
}
