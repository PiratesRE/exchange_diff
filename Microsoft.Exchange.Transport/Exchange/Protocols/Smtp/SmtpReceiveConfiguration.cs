using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpReceiveConfiguration : ISmtpReceiveConfiguration
	{
		public IDiagnosticsConfigProvider DiagnosticsConfiguration { get; private set; }

		public IRoutingConfigProvider RoutingConfiguration { get; private set; }

		public ITransportConfigProvider TransportConfiguration { get; private set; }

		public static SmtpReceiveConfiguration Create(ITransportAppConfig appConfig, ITransportConfiguration transportConfiguration)
		{
			ArgumentValidator.ThrowIfNull("appConfig", appConfig);
			ArgumentValidator.ThrowIfNull("transportConfiguration", transportConfiguration);
			return new SmtpReceiveConfiguration(appConfig, transportConfiguration);
		}

		private SmtpReceiveConfiguration(ITransportAppConfig appConfig, ITransportConfiguration transportConfiguration)
		{
			this.DiagnosticsConfiguration = DiagnosticsConfigAdapter.Create(appConfig);
			this.RoutingConfiguration = RoutingConfigAdapter.Create(appConfig);
			this.TransportConfiguration = TransportConfigAdapter.Create(appConfig, transportConfiguration);
		}
	}
}
