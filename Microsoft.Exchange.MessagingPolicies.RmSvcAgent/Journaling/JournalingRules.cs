using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.Transport;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Diagnostics.FaultInjection;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal sealed class JournalingRules
	{
		public TransportRuleCollection Rules
		{
			get
			{
				return this.journalingRules;
			}
		}

		static JournalingRules()
		{
			ExTraceGlobals.FaultInjectionTracer.RegisterExceptionInjectionCallback(new ExceptionInjectionCallback(JournalingRules.FaultInjectionCallback));
		}

		private JournalingRules(TransportRuleCollection rules)
		{
			this.journalingRules = rules;
		}

		public static List<GccRuleEntry> GetGccConfig()
		{
			if (JournalingRules.currentGcc == null)
			{
				lock (JournalingRules.staticLock)
				{
					if (JournalingRules.currentGcc == null)
					{
						JournalingRules.currentGcc = JournalingRules.LoadGccRules();
						if (!JournalingRules.registeredForDynamicNotificationForGcc)
						{
							IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId.ForestWideOrgId), 120, "GetGccConfig", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\Journaling\\Agent\\JournalingRules.cs");
							ADJournalRuleStorageManager adjournalRuleStorageManager = new ADJournalRuleStorageManager("JournalingVersioned", tenantOrTopologyConfigurationSession);
							TransportADNotificationAdapter.Instance.RegisterForJournalRuleNotifications(adjournalRuleStorageManager.RuleCollectionId, new ADNotificationCallback(JournalingRules.ConfigureGcc));
							JournalingRules.registeredForDynamicNotificationForGcc = true;
						}
					}
				}
			}
			return JournalingRules.currentGcc;
		}

		public static JournalingRules GetConfig(OrganizationId organizationId)
		{
			if (organizationId != OrganizationId.ForestWideOrgId)
			{
				return JournalingRules.Load(organizationId);
			}
			if (JournalingRules.current == null)
			{
				lock (JournalingRules.staticLock)
				{
					if (JournalingRules.current == null)
					{
						JournalingRules journalingRules = JournalingRules.Load(organizationId);
						if (journalingRules == null)
						{
							return null;
						}
						JournalingRules.current = journalingRules;
						if (!JournalingRules.registeredForDynamicNotification)
						{
							IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 176, "GetConfig", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\Journaling\\Agent\\JournalingRules.cs");
							ADJournalRuleStorageManager adjournalRuleStorageManager = new ADJournalRuleStorageManager("JournalingVersioned", tenantOrTopologyConfigurationSession);
							TransportADNotificationAdapter.Instance.RegisterForJournalRuleNotifications(adjournalRuleStorageManager.RuleCollectionId, new ADNotificationCallback(JournalingRules.Configure));
							JournalingRules.registeredForDynamicNotification = true;
						}
					}
				}
			}
			return JournalingRules.current;
		}

		public bool IsConfiguredJournalTargetAddress(string smtpAddress)
		{
			foreach (Rule rule in this.Rules)
			{
				JournalingRule journalingRule = (JournalingRule)rule;
				if (journalingRule.Enabled == RuleState.Enabled)
				{
					foreach (Microsoft.Exchange.MessagingPolicies.Rules.Action action in journalingRule.Actions)
					{
						Journal journal = (Journal)action;
						string targetAddress = journal.GetTargetAddress();
						if (string.Compare(targetAddress, smtpAddress, StringComparison.OrdinalIgnoreCase) == 0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private JournalingRules()
		{
		}

		private static JournalingRules Load(OrganizationId organizationId)
		{
			JournalingRules journalingRules = null;
			Exception ex = null;
			try
			{
				ADJournalRuleStorageManager adjournalRuleStorageManager;
				if (organizationId == OrganizationId.ForestWideOrgId)
				{
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 252, "Load", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\Journaling\\Agent\\JournalingRules.cs");
					adjournalRuleStorageManager = new ADJournalRuleStorageManager("JournalingVersioned", tenantOrTopologyConfigurationSession);
					adjournalRuleStorageManager.LoadRuleCollection();
				}
				else
				{
					JournalingRulesPerTenantSettings journalingRulesPerTenantSettings;
					if (!Components.Configuration.TryGetJournalingRules(organizationId, out journalingRulesPerTenantSettings))
					{
						ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "Failed to load journal rules for tenant {0}", organizationId);
						return null;
					}
					adjournalRuleStorageManager = new ADJournalRuleStorageManager("JournalingVersioned", journalingRulesPerTenantSettings.JournalRuleDataList);
					ExTraceGlobals.FaultInjectionTracer.TraceTest<string>(3609603389U, organizationId.OrganizationalUnit.Name);
					adjournalRuleStorageManager.ParseRuleCollection();
				}
				TransportRuleCollection ruleCollection = adjournalRuleStorageManager.GetRuleCollection();
				if (ruleCollection == null)
				{
					ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "Failed to get journal rules for tenant {0}", organizationId);
					return null;
				}
				journalingRules = new JournalingRules(ruleCollection);
			}
			catch (TransientException ex2)
			{
				ex = ex2;
			}
			catch (DataValidationException ex3)
			{
				ex = ex3;
			}
			catch (ParserException ex4)
			{
				ex = ex4;
			}
			catch (ExchangeConfigurationException ex5)
			{
				ex = ex5;
			}
			finally
			{
				if (organizationId == OrganizationId.ForestWideOrgId)
				{
					if (journalingRules != null)
					{
						RuleAuditProvider.LogSuccess("JournalingVersioned");
					}
					else
					{
						RuleAuditProvider.LogFailure("JournalingVersioned");
					}
				}
			}
			if (ex != null)
			{
				ExTraceGlobals.JournalingTracer.TraceError<Exception>(0L, "Failed to load rules, Exception: {0}", ex);
			}
			return journalingRules;
		}

		private static List<GccRuleEntry> LoadGccRules()
		{
			List<GccRuleEntry> list = null;
			Exception ex = null;
			try
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId.ForestWideOrgId), 356, "LoadGccRules", "f:\\15.00.1497\\sources\\dev\\MessagingPolicies\\src\\Journaling\\Agent\\JournalingRules.cs");
				ADJournalRuleStorageManager adjournalRuleStorageManager = new ADJournalRuleStorageManager("JournalingVersioned", tenantOrTopologyConfigurationSession);
				adjournalRuleStorageManager.LoadRuleCollection();
				TransportRuleCollection ruleCollection = adjournalRuleStorageManager.GetRuleCollection();
				if (ruleCollection == null)
				{
					ExTraceGlobals.JournalingTracer.TraceError(0L, "Failed to get GCC journal rules");
					return null;
				}
				list = new List<GccRuleEntry>(ruleCollection.Count);
				foreach (Rule rule in ruleCollection)
				{
					JournalingRule journalingRule = (JournalingRule)rule;
					if (!journalingRule.IsTooAdvancedToParse && journalingRule.GccRuleType != GccType.None && journalingRule.Enabled == RuleState.Enabled && (journalingRule.ExpiryDate == null || !(DateTime.UtcNow.Date > journalingRule.ExpiryDate.Value.Date)))
					{
						GccRuleEntry item = new GccRuleEntry(journalingRule.ImmutableId, journalingRule.Name, JournalingRules.ParseGccRecipient(journalingRule), journalingRule.GccRuleType == GccType.Full, journalingRule.ExpiryDate, JournalingRules.ParseGccJournalEmailAddress(journalingRule));
						list.Add(item);
					}
				}
			}
			catch (TransientException ex2)
			{
				ex = ex2;
			}
			catch (DataValidationException ex3)
			{
				ex = ex3;
			}
			catch (ParserException ex4)
			{
				ex = ex4;
			}
			catch (ExchangeConfigurationException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				ExTraceGlobals.JournalingTracer.TraceError<Exception>(0L, "Exception hit while loading GCC rules, Details: {0}", ex);
				list = null;
			}
			return list;
		}

		private static SmtpAddress ParseGccRecipient(JournalingRule rule)
		{
			if (rule == null)
			{
				return SmtpAddress.Empty;
			}
			OrCondition orCondition = rule.Condition as OrCondition;
			if (orCondition == null)
			{
				return SmtpAddress.Empty;
			}
			List<Condition> subConditions = orCondition.SubConditions;
			if (subConditions == null || subConditions.Count == 0)
			{
				return SmtpAddress.Empty;
			}
			OrCondition orCondition2 = subConditions[0] as OrCondition;
			if (orCondition2 == null || orCondition2.SubConditions == null || orCondition2.SubConditions.Count == 0)
			{
				return SmtpAddress.Empty;
			}
			PredicateCondition predicateCondition = orCondition2.SubConditions[0] as PredicateCondition;
			if (predicateCondition == null || predicateCondition.Value == null)
			{
				return SmtpAddress.Empty;
			}
			return new SmtpAddress(predicateCondition.Value.RawValues[0]);
		}

		private static SmtpAddress ParseGccJournalEmailAddress(JournalingRule rule)
		{
			if (rule == null || rule.Actions == null || rule.Actions.Count == 0 || rule.Actions[0] == null || rule.Actions[0].Arguments.Count == 0)
			{
				return SmtpAddress.Empty;
			}
			Value value = (Value)rule.Actions[0].Arguments[0];
			if (value == null || !SmtpAddress.IsValidSmtpAddress(value.ParsedValue.ToString()))
			{
				return SmtpAddress.Empty;
			}
			return SmtpAddress.Parse(value.ParsedValue.ToString());
		}

		private static void Configure(ADNotificationEventArgs args)
		{
			lock (JournalingRules.staticLock)
			{
				JournalingRules.current = JournalingRules.Load(OrganizationId.ForestWideOrgId);
			}
		}

		private static void ConfigureGcc(ADNotificationEventArgs args)
		{
			lock (JournalingRules.staticLock)
			{
				JournalingRules.currentGcc = JournalingRules.LoadGccRules();
			}
		}

		private static Exception FaultInjectionCallback(string exceptionType)
		{
			LocalizedString localizedString = new LocalizedString("Fault injection.");
			if (exceptionType.Equals("Microsoft.Exchange.MessagingPolicies.Rules.ParserException", StringComparison.OrdinalIgnoreCase))
			{
				return new ParserException(localizedString);
			}
			return new Exception(localizedString);
		}

		private static object staticLock = new object();

		private static JournalingRules current = null;

		private static List<GccRuleEntry> currentGcc = null;

		private static bool registeredForDynamicNotification = false;

		private static bool registeredForDynamicNotificationForGcc = false;

		private TransportRuleCollection journalingRules;
	}
}
