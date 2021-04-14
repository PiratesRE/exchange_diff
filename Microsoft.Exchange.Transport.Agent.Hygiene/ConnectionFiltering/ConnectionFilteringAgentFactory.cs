using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics.Components.ConnectionFiltering;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.ConnectionFiltering
{
	public sealed class ConnectionFilteringAgentFactory : SmtpReceiveAgentFactory
	{
		public ConnectionFilteringAgentFactory()
		{
			CommonUtils.RegisterConfigurationChangeHandlers("Connection Filtering", new ADOperation(this.RegisterConfigurationChangeHandlers), ExTraceGlobals.FactoryTracer, this);
			this.Configure(true);
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			BypassedRecipients blockListProviderBypassedRecipients = new BypassedRecipients(this.config.BlockListProviderConfig.BypassedRecipients, (server != null) ? server.AddressBook : null);
			return new ConnectionFilteringAgent(this.config, server, blockListProviderBypassedRecipients);
		}

		public override void Close()
		{
			this.UnregisterConfigurationChangeHandlers();
			ConnectionFilteringAgent.PerformanceCounters.RemoveCounters();
		}

		private void RegisterConfigurationChangeHandlers()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 150, "RegisterConfigurationChangeHandlers", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\ConnectionFiltering\\Agent\\ConnectionFilteringAgent.cs");
			ADObjectId orgContainerId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
			ADObjectId childId = orgContainerId.GetChildId("Transport Settings");
			ADObjectId childId2 = childId.GetChildId("Message Hygiene");
			ADObjectId childId3 = childId2.GetChildId("IPAllowListProviderConfig");
			ADObjectId childId4 = childId2.GetChildId("IPBlockListProviderConfig");
			TransportFacades.ConfigChanged += this.ConfigUpdate;
			this.ipAllowListProviderRequestCookie = ADNotificationAdapter.RegisterChangeNotification<IPAllowListProvider>(childId3, new ADNotificationCallback(this.Configure));
			this.ipBlockListProviderRequestCookie = ADNotificationAdapter.RegisterChangeNotification<IPBlockListProvider>(childId4, new ADNotificationCallback(this.Configure));
			this.ipAllowListConfigRequestCookie = ADNotificationAdapter.RegisterChangeNotification<IPAllowListConfig>(childId2, new ADNotificationCallback(this.Configure));
			this.ipBlockListConfigRequestCookie = ADNotificationAdapter.RegisterChangeNotification<IPBlockListConfig>(childId2, new ADNotificationCallback(this.Configure));
			this.ipAllowListProviderConfigRequestCookie = ADNotificationAdapter.RegisterChangeNotification<IPAllowListProviderConfig>(childId2, new ADNotificationCallback(this.Configure));
			this.ipBlockListProviderConfigRequestCookie = ADNotificationAdapter.RegisterChangeNotification<IPBlockListProviderConfig>(childId2, new ADNotificationCallback(this.Configure));
		}

		private void UnregisterConfigurationChangeHandlers()
		{
			TransportFacades.ConfigChanged -= this.ConfigUpdate;
			if (this.ipAllowListProviderRequestCookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(this.ipAllowListProviderRequestCookie);
			}
			if (this.ipBlockListProviderRequestCookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(this.ipBlockListProviderRequestCookie);
			}
			if (this.ipAllowListConfigRequestCookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(this.ipAllowListConfigRequestCookie);
			}
			if (this.ipBlockListConfigRequestCookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(this.ipBlockListConfigRequestCookie);
			}
			if (this.ipAllowListProviderConfigRequestCookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(this.ipAllowListProviderConfigRequestCookie);
			}
			if (this.ipBlockListProviderConfigRequestCookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(this.ipBlockListProviderConfigRequestCookie);
			}
		}

		private void ConfigUpdate(object source, EventArgs args)
		{
			this.Configure(false);
		}

		private void Configure(ADNotificationEventArgs args)
		{
			this.Configure(false);
		}

		private void Configure(bool onStartup)
		{
			ConnectionFilterConfig connectionFilterConfig;
			ADOperationResult adoperationResult;
			if (ADNotificationAdapter.TryReadConfiguration<ConnectionFilterConfig>(() => new ConnectionFilterConfig(DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 266, "Configure", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\ConnectionFiltering\\Agent\\ConnectionFilteringAgent.cs")), out connectionFilterConfig, out adoperationResult))
			{
				this.config = connectionFilterConfig;
				return;
			}
			CommonUtils.FailedToReadConfiguration("Connection Filtering", onStartup, adoperationResult.Exception, ExTraceGlobals.FactoryTracer, ConnectionFilteringAgent.EventLogger, this);
		}

		private ConnectionFilterConfig config;

		private ADNotificationRequestCookie ipAllowListProviderRequestCookie;

		private ADNotificationRequestCookie ipBlockListProviderRequestCookie;

		private ADNotificationRequestCookie ipAllowListConfigRequestCookie;

		private ADNotificationRequestCookie ipBlockListConfigRequestCookie;

		private ADNotificationRequestCookie ipAllowListProviderConfigRequestCookie;

		private ADNotificationRequestCookie ipBlockListProviderConfigRequestCookie;
	}
}
