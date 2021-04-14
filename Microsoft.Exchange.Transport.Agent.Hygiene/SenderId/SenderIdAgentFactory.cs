using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.SenderId;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.SenderId;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.SenderId
{
	public sealed class SenderIdAgentFactory : SmtpReceiveAgentFactory
	{
		public SenderIdAgentFactory()
		{
			CommonUtils.RegisterConfigurationChangeHandlers("Sender ID", new ADOperation(this.RegisterConfigurationChangeHandlers), ExTraceGlobals.OtherTracer, this);
			this.Configure(true);
		}

		private void RegisterConfigurationChangeHandlers()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 69, "RegisterConfigurationChangeHandlers", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\SenderId\\Agent\\SenderIdAgentFactory.cs");
			ADObjectId orgContainerId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
			ADObjectId childId = orgContainerId.GetChildId("Transport Settings");
			ADObjectId childId2 = childId.GetChildId("Message Hygiene");
			TransportFacades.ConfigChanged += this.ConfigUpdate;
			this.notificationRequestCookie = ADNotificationAdapter.RegisterChangeNotification<SenderIdConfig>(childId2, new ADNotificationCallback(this.Configure));
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
			SenderIdConfig senderIdConfig;
			ADOperationResult adoperationResult;
			if (ADNotificationAdapter.TryReadConfiguration<SenderIdConfig>(() => DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 133, "Configure", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\SenderId\\Agent\\SenderIdAgentFactory.cs").FindSingletonConfigurationObject<SenderIdConfig>(), out senderIdConfig, out adoperationResult))
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (SmtpDomain smtpDomain in senderIdConfig.BypassedSenderDomains)
				{
					hashSet.Add(smtpDomain.Domain);
				}
				this.senderIdConfig = senderIdConfig;
				this.bypassedSenderDomains = hashSet;
				return;
			}
			CommonUtils.FailedToReadConfiguration("Sender ID", onStartup, adoperationResult.Exception, ExTraceGlobals.OtherTracer, SenderIdAgentFactory.eventLogger, this);
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			BypassedRecipients bypassedRecipients = new BypassedRecipients(this.senderIdConfig.BypassedRecipients, (server != null) ? server.AddressBook : null);
			return new SenderIdAgent(this.senderIdConfig, bypassedRecipients, this.bypassedSenderDomains, server);
		}

		public override void Close()
		{
			this.UnregisterConfigurationChangeHandlers();
			Util.PerformanceCounters.RemoveCounters();
		}

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.OtherTracer.Category, "MSExchange Antispam");

		private SenderIdConfig senderIdConfig;

		private HashSet<string> bypassedSenderDomains;

		private ADNotificationRequestCookie notificationRequestCookie;
	}
}
