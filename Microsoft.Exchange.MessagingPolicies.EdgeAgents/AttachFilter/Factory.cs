using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.AttachFilter
{
	public sealed class Factory : SmtpReceiveAgentFactory
	{
		public Factory()
		{
			if (VariantConfiguration.InvariantNoFlightingSnapshot.CompliancePolicy.RuleConfigurationAdChangeNotifications.Enabled)
			{
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(new ADOperation(Configuration.RegisterConfigurationChangeHandlers));
				if (!adoperationResult.Succeeded)
				{
					ExTraceGlobals.AttachmentFilteringTracer.TraceError(0L, "Unable to register for AD Change notification");
					throw new ExchangeConfigurationException(TransportRulesStrings.FailedToRegisterForConfigChangeNotification(Factory.AgentName), adoperationResult.Exception);
				}
			}
			Configuration.Configure(null);
			if (Configuration.Current == null)
			{
				throw new ExchangeConfigurationException(TransportRulesStrings.FailedToLoadAttachmentFilteringConfigOnStartup);
			}
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			return new Agent(server);
		}

		private static readonly string AgentName = "Attachment Filtering";
	}
}
