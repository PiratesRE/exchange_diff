using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas.Commands.Options
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public class OptionsResponse : IHaveAnHttpStatus
	{
		public OptionsResponse()
		{
			this.EasServerCapabilities = new EasServerCapabilities();
			this.EasExtensionCapabilities = new EasExtensionCapabilities();
		}

		public HttpStatus HttpStatus { get; set; }

		internal OptionsStatus OptionsStatus { get; set; }

		internal EasServerCapabilities EasServerCapabilities { get; set; }

		internal EasExtensionCapabilities EasExtensionCapabilities { get; set; }
	}
}
