using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Routing;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.TransportRuleAgent;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Filtering;

namespace Microsoft.Exchange.MessagingPolicies.TransportRuleAgent
{
	public class TransportRuleAgentFactory : RoutingAgentFactory
	{
		public TransportRuleAgentFactory()
		{
			Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.TransportRuleAgent.ExTraceGlobals.FaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(TransportRuleAgentFactory.FaultInjectionCallback));
			if (VariantConfiguration.InvariantNoFlightingSnapshot.CompliancePolicy.RuleConfigurationAdChangeNotifications.Enabled)
			{
				ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(new ADOperation(this.RegisterConfigurationChangeHandlers));
				if (!adoperationResult.Succeeded)
				{
					Microsoft.Exchange.Diagnostics.Components.MessagingPolicies.ExTraceGlobals.TransportRulesEngineTracer.TraceError(0L, "Unable to register for AD Change notification");
					throw new ExchangeConfigurationException(TransportRulesStrings.FailedToRegisterForConfigChangeNotification("Transport Rule"), adoperationResult.Exception);
				}
			}
			this.transportRulesCache = new TenantConfigurationCache<TransportRulesPerTenantSettings>((long)Components.TransportAppConfig.PerTenantCache.TransportRulesCacheMaxSize.ToBytes(), Components.TransportAppConfig.PerTenantCache.TransportRulesCacheExpiryInterval, Components.TransportAppConfig.PerTenantCache.TransportRulesCacheCleanupInterval, new PerTenantCacheTracer(Microsoft.Exchange.Diagnostics.Components.Transport.ExTraceGlobals.TransportRulesCacheTracer, "TransportRules"), new PerTenantCachePerformanceCounters(Components.Configuration.ProcessTransportRole, "TransportRules"));
			TransportRuleAgentFactory.Configure(null);
			if (TransportRuleAgentFactory.shouldDefer)
			{
				throw new ExchangeConfigurationException(TransportRulesStrings.FailedToLoadRuleCollection("Transport Rule"));
			}
			if (this.totalCounter == null)
			{
				this.totalCounter = RulesCounters.GetInstance("_Total");
			}
			Components.PerfCountersLoaderComponent.AddCounterToGetExchangeDiagnostics(typeof(RulesCounters), "TransportRuleAgentCounters");
		}

		public override RoutingAgent CreateAgent(SmtpServer server)
		{
			return new TransportRuleAgent(server, TransportRuleAgentFactory.rules, TransportRuleAgentFactory.shouldDefer, this.totalCounter, this.transportRulesCache);
		}

		private static void Configure(ADNotificationEventArgs args)
		{
			bool flag = false;
			try
			{
				ADNotificationAdapter.RunADOperation(delegate()
				{
					TransportRuleAgentFactory.LoadRules();
				}, 5);
				flag = !TransportRuleAgentFactory.shouldDefer;
			}
			catch (TransientException)
			{
				TransportAction.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RuleCollectionLoadingTransientError, null, new object[]
				{
					"TransportVersioned"
				});
			}
			finally
			{
				if (!flag)
				{
					RuleAuditProvider.LogFailure("TransportVersioned");
				}
			}
		}

		private static void LoadRules()
		{
			ADRuleStorageManager adruleStorageManager = null;
			IConfigurationSession session = TransportUtils.CreateSession(OrganizationId.ForestWideOrgId);
			Exception ex = null;
			lock (TransportRuleAgentFactory.lockVar)
			{
				TransportUtils.LoadRuleCollectionFromAd("TransportVersioned", session, out adruleStorageManager, out ex);
				if (ex != null)
				{
					TransportRuleAgentFactory.rules = null;
					TransportRuleAgentFactory.shouldDefer = true;
					TransportAction.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RuleCollectionLoadingError, null, new object[]
					{
						"TransportVersioned",
						ex.ToString()
					});
				}
				else
				{
					bool flag2 = false;
					string hashOfRuleCollection = adruleStorageManager.GetHashOfRuleCollection();
					if (hashOfRuleCollection != TransportRuleAgentFactory.cachedRuleCollectionHash)
					{
						flag2 = true;
						TransportRuleAgentFactory.cachedRuleCollectionHash = hashOfRuleCollection;
					}
					if (flag2)
					{
						TransportRuleCollection transportRuleCollection = adruleStorageManager.GetRuleCollection();
						if (transportRuleCollection.CountAllNotDisabled > 0)
						{
							transportRuleCollection.SupportsBifurcation = true;
							transportRuleCollection.CreatePerformanceCounters();
						}
						else
						{
							transportRuleCollection = null;
						}
						TransportRuleAgentFactory.rules = transportRuleCollection;
						TransportRuleAgentFactory.shouldDefer = false;
						TransportAction.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_RuleCollectionLoaded, null, new object[]
						{
							"TransportVersioned"
						});
						RuleAuditProvider.LogSuccess("TransportVersioned");
					}
				}
			}
		}

		private static Exception FaultInjectionCallback(string exceptionType)
		{
			LocalizedString localizedString = new LocalizedString("Fault injection");
			if (exceptionType.Equals("Microsoft.Exchange.Data.Directory.ADTransientException", StringComparison.OrdinalIgnoreCase))
			{
				return new ADTransientException(localizedString);
			}
			if (exceptionType.Equals("Microsoft.Exchange.MessagingPolicies.Rules.FilteringServiceFailureException", StringComparison.OrdinalIgnoreCase))
			{
				return new FilteringServiceFailureException(localizedString);
			}
			if (exceptionType.Equals("Microsoft.Exchange.MessagingPolicies.Rules.FilteringServiceTimeoutException", StringComparison.OrdinalIgnoreCase))
			{
				return new FilteringServiceTimeoutException(localizedString);
			}
			if (exceptionType.Equals("Microsoft.Exchange.MessagingPolicies.Rules.RuleInvalidOperationException", StringComparison.OrdinalIgnoreCase))
			{
				return new RuleInvalidOperationException(localizedString);
			}
			if (exceptionType.Equals("Microsoft.Filtering.ScannerCrashException", StringComparison.OrdinalIgnoreCase))
			{
				return new ScannerCrashException(localizedString, null);
			}
			if (exceptionType.Equals("Microsoft.Filtering.ScanQueueTimeoutException", StringComparison.OrdinalIgnoreCase))
			{
				return new ScanQueueTimeoutException(localizedString, null);
			}
			if (exceptionType.Equals("Microsoft.Exchange.MessagingPolicies.Rules.TransportRulePermanentException", StringComparison.OrdinalIgnoreCase))
			{
				return new TransportRulePermanentException(localizedString);
			}
			if (exceptionType.Equals("Microsoft.Exchange.MessagingPolicies.Rules.TransportRuleTimeoutException", StringComparison.OrdinalIgnoreCase))
			{
				return new TransportRuleTimeoutException(localizedString);
			}
			if (exceptionType.Equals("Microsoft.Exchange.MessagingPolicies.Rules.TransportRuleTransientException", StringComparison.OrdinalIgnoreCase))
			{
				return new TransportRuleTransientException(localizedString);
			}
			return new Exception(localizedString);
		}

		private void RegisterConfigurationChangeHandlers()
		{
			IConfigurationSession session = TransportUtils.CreateSession(OrganizationId.ForestWideOrgId);
			ADRuleStorageManager adruleStorageManager = new ADRuleStorageManager("TransportVersioned", session);
			ADObjectId ruleCollectionId = adruleStorageManager.RuleCollectionId;
			ADNotificationAdapter.RegisterChangeNotification<TransportRule>(ruleCollectionId, new ADNotificationCallback(TransportRuleAgentFactory.Configure));
		}

		private const string TotalCounterInstanceName = "_Total";

		private static object lockVar = new object();

		private static TransportRuleCollection rules = null;

		private static string cachedRuleCollectionHash = string.Empty;

		private static bool shouldDefer = true;

		private RulesCountersInstance totalCounter;

		private TenantConfigurationCache<TransportRulesPerTenantSettings> transportRulesCache;
	}
}
