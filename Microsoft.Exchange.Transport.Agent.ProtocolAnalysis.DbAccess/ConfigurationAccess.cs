using System;
using Microsoft.Exchange.Configuration.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ProtocolAnalysis;
using Microsoft.Exchange.Transport.Agent.AntiSpam.Common;

namespace Microsoft.Exchange.Transport.Agent.ProtocolAnalysis.DbAccess
{
	internal class ConfigurationAccess
	{
		public static event HandleConfigChange HandleConfigChangeEvent
		{
			add
			{
				ConfigurationAccess.OnHandleConfigChangeEvent += value;
				CommonUtils.RegisterConfigurationChangeHandlers("Sender Reputation", new ADOperation(ConfigurationAccess.RegisterConfigurationChangeHandlers), ExTraceGlobals.FactoryTracer, null);
			}
			remove
			{
				ConfigurationAccess.OnHandleConfigChangeEvent -= value;
				ConfigurationAccess.UnregisterConfigurationChangeHandlers();
			}
		}

		public static event HandleConfigChange OnHandleConfigChangeEvent;

		public static SenderReputationConfig ConfigSettings
		{
			get
			{
				if (ConfigurationAccess.settings == null)
				{
					ConfigurationAccess.LoadConfiguration(true);
				}
				return ConfigurationAccess.settings;
			}
		}

		private static void LoadConfiguration(bool onStartup)
		{
			SenderReputationConfig senderReputationConfig;
			ADOperationResult adoperationResult;
			if (ADNotificationAdapter.TryReadConfiguration<SenderReputationConfig>(() => DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 172, "LoadConfiguration", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\Sts\\DbAccess\\DataRowAccess.cs").FindSingletonConfigurationObject<SenderReputationConfig>(), out senderReputationConfig, out adoperationResult))
			{
				ConfigurationAccess.settings = senderReputationConfig;
				return;
			}
			CommonUtils.FailedToReadConfiguration("Sender Reputation", onStartup, adoperationResult.Exception, ExTraceGlobals.FactoryTracer, ConfigurationAccess.eventLogger, null);
		}

		private static void RegisterConfigurationChangeHandlers()
		{
			if (ConfigurationAccess.notificationRequestCookie != null)
			{
				return;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 202, "RegisterConfigurationChangeHandlers", "f:\\15.00.1497\\sources\\dev\\mexagents\\src\\Sts\\DbAccess\\DataRowAccess.cs");
			ADObjectId orgContainerId = tenantOrTopologyConfigurationSession.GetOrgContainerId();
			ADObjectId childId = orgContainerId.GetChildId("Transport Settings");
			ADObjectId childId2 = childId.GetChildId("Message Hygiene");
			ConfigurationAccess.notificationRequestCookie = ADNotificationAdapter.RegisterChangeNotification<SenderReputationConfig>(childId2, new ADNotificationCallback(ConfigurationAccess.Configure));
		}

		private static void UnregisterConfigurationChangeHandlers()
		{
			if (ConfigurationAccess.notificationRequestCookie == null)
			{
				return;
			}
			ADNotificationAdapter.UnregisterChangeNotification(ConfigurationAccess.notificationRequestCookie);
			ConfigurationAccess.notificationRequestCookie = null;
		}

		public static void Unsubscribe()
		{
			ConfigurationAccess.UnregisterConfigurationChangeHandlers();
		}

		private static void Configure(ADNotificationEventArgs args)
		{
			ConfigurationAccess.LoadConfiguration(false);
			if (ConfigurationAccess.OnHandleConfigChangeEvent != null)
			{
				ConfigurationAccess.OnHandleConfigChangeEvent(null, null);
			}
		}

		public static void NotifySrlConfigChanged(PropertyBag newFields)
		{
			ConfigChangedEventArgs e = new ConfigChangedEventArgs(newFields);
			if (ConfigurationAccess.OnHandleConfigChangeEvent != null)
			{
				ConfigurationAccess.OnHandleConfigChangeEvent(null, e);
			}
		}

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.FactoryTracer.Category, "MSExchange Antispam");

		private static SenderReputationConfig settings = null;

		private static ADNotificationRequestCookie notificationRequestCookie = null;
	}
}
