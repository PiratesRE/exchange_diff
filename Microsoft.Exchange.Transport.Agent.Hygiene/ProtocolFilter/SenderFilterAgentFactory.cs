using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProtocolFilter;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.ProtocolFilter
{
	public sealed class SenderFilterAgentFactory : SmtpReceiveAgentFactory
	{
		public SenderFilterAgentFactory()
		{
			CommonUtils.RegisterConfigurationChangeHandlers("Sender Filtering", new ADOperation(this.RegisterConfigurationChangeHandlers), ExTraceGlobals.SenderFilterAgentTracer, this);
			this.Configure(true);
		}

		private void RegisterConfigurationChangeHandlers()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 72, "RegisterConfigurationChangeHandlers", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\ProtocolFilter\\Agent\\SenderFilterAgent.cs");
			ADObjectId orgContainerId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
			ADObjectId childId = orgContainerId.GetChildId("Transport Settings");
			ADObjectId childId2 = childId.GetChildId("Message Hygiene");
			TransportFacades.ConfigChanged += this.ConfigUpdate;
			this.notificationRequestCookie = ADNotificationAdapter.RegisterChangeNotification<SenderFilterConfig>(childId2, new ADNotificationCallback(this.Configure));
		}

		private void UnregisterConfigurationChangeHandlers()
		{
			TransportFacades.ConfigChanged -= this.ConfigUpdate;
			if (this.notificationRequestCookie != null)
			{
				ADNotificationAdapter.UnregisterChangeNotification(this.notificationRequestCookie);
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
			SenderFilterConfig senderFilterConfig;
			ADOperationResult adoperationResult;
			if (ADNotificationAdapter.TryReadConfiguration<SenderFilterConfig>(() => DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 136, "Configure", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\ProtocolFilter\\Agent\\SenderFilterAgent.cs").FindSingletonConfigurationObject<SenderFilterConfig>(), out senderFilterConfig, out adoperationResult))
			{
				this.senderFilterConfig = senderFilterConfig;
				this.blockedSenders = new BlockedSenders(senderFilterConfig.BlockedSenders, senderFilterConfig.BlockedDomains, senderFilterConfig.BlockedDomainsAndSubdomains);
				return;
			}
			CommonUtils.FailedToReadConfiguration("Sender Filtering", onStartup, adoperationResult.Exception, ExTraceGlobals.SenderFilterAgentTracer, SenderFilterAgentFactory.eventLogger, this);
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			return new SenderFilterAgent(this.senderFilterConfig, this.blockedSenders, server.AddressBook);
		}

		public override void Close()
		{
			this.UnregisterConfigurationChangeHandlers();
			Util.PerformanceCounters.SenderFilter.RemoveCounters();
		}

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.SenderFilterAgentTracer.Category, "MSExchange Antispam");

		private SenderFilterConfig senderFilterConfig;

		private BlockedSenders blockedSenders;

		private ADNotificationRequestCookie notificationRequestCookie;
	}
}
