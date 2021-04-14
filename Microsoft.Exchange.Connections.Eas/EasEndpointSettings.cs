using System;
using System.Net;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Connections.Eas.Commands.Autodiscover;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Eas
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class EasEndpointSettings
	{
		public EasEndpointSettings(EasAuthenticationParameters authenticationParameters)
		{
			this.UserSmtpAddress = authenticationParameters.UserSmtpAddress;
			this.NetworkCredential = authenticationParameters.NetworkCredential;
			this.MostRecentEndpoint = new AutodiscoverEndpoint();
			if (!string.IsNullOrEmpty(authenticationParameters.EndpointOverride))
			{
				this.MostRecentEndpoint.DiscoveryDateTime = new DateTime?(DateTime.UtcNow);
				this.MostRecentEndpoint.Url = authenticationParameters.EndpointOverride;
				this.MostRecentDomain = authenticationParameters.EndpointOverride;
			}
		}

		internal UserSmtpAddress UserSmtpAddress { get; set; }

		internal NetworkCredential NetworkCredential { get; set; }

		internal AutodiscoverEndpoint MostRecentEndpoint { get; set; }

		internal string MostRecentDomain { get; set; }

		internal string UserSmtpAddressString
		{
			get
			{
				return (string)this.UserSmtpAddress;
			}
		}

		internal string Local
		{
			get
			{
				return this.UserSmtpAddress.Local;
			}
		}

		internal string Domain
		{
			get
			{
				return this.MostRecentDomain ?? this.UserSmtpAddress.Domain;
			}
		}
	}
}
