using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
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
	public sealed class RecipientFilterAgentFactory : SmtpReceiveAgentFactory
	{
		public RecipientFilterAgentFactory()
		{
			CommonUtils.RegisterConfigurationChangeHandlers("Recipient Filtering", new ADOperation(this.RegisterConfigurationChangeHandlers), ExTraceGlobals.RecipientFilterAgentTracer, this);
			this.Configure(true);
		}

		public override SmtpReceiveAgent CreateAgent(SmtpServer server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			AddressBook addressBook = this.recipientFilterConfig.RecipientValidationEnabled ? server.AddressBook : null;
			return new RecipientFilterAgent(this.recipientFilterConfig, this.blockedRecipients, addressBook, server.AcceptedDomains);
		}

		public override void Close()
		{
			this.UnregisterConfigurationChangeHandlers();
			Util.PerformanceCounters.RecipientFilter.RemoveCounters();
		}

		private void RegisterConfigurationChangeHandlers()
		{
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 117, "RegisterConfigurationChangeHandlers", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\ProtocolFilter\\Agent\\RecipientFilterAgent.cs");
			ADObjectId orgContainerId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
			ADObjectId childId = orgContainerId.GetChildId("Transport Settings");
			ADObjectId childId2 = childId.GetChildId("Message Hygiene");
			TransportFacades.ConfigChanged += this.ConfigUpdate;
			this.notificationRequestCookie = ADNotificationAdapter.RegisterChangeNotification<RecipientFilterConfig>(childId2, new ADNotificationCallback(this.Configure));
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
			RecipientFilterConfig recipientFilterConfig;
			ADOperationResult adoperationResult;
			if (ADNotificationAdapter.TryReadConfiguration<RecipientFilterConfig>(() => DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 181, "Configure", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\ProtocolFilter\\Agent\\RecipientFilterAgent.cs").FindSingletonConfigurationObject<RecipientFilterConfig>(), out recipientFilterConfig, out adoperationResult))
			{
				Dictionary<RoutingAddress, bool> dictionary = new Dictionary<RoutingAddress, bool>();
				foreach (SmtpAddress smtpAddress in recipientFilterConfig.BlockedRecipients)
				{
					dictionary.Add(new RoutingAddress(smtpAddress.ToString()), true);
				}
				this.recipientFilterConfig = recipientFilterConfig;
				this.blockedRecipients = dictionary;
				return;
			}
			CommonUtils.FailedToReadConfiguration("Recipient Filtering", onStartup, adoperationResult.Exception, ExTraceGlobals.RecipientFilterAgentTracer, RecipientFilterAgentFactory.eventLogger, this);
		}

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.RecipientFilterAgentTracer.Category, "MSExchange Antispam");

		private RecipientFilterConfig recipientFilterConfig;

		private Dictionary<RoutingAddress, bool> blockedRecipients;

		private ADNotificationRequestCookie notificationRequestCookie;
	}
}
