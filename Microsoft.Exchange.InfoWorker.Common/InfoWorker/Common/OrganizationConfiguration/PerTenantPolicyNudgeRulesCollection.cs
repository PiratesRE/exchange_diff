using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.MessagingPolicies.Rules.PolicyNudges;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal sealed class PerTenantPolicyNudgeRulesCollection : PerTenantConfigurationLoader<PerTenantPolicyNudgeRulesCollection.PolicyNudgeRules>
	{
		public PerTenantPolicyNudgeRulesCollection(OrganizationId organizationId) : base(organizationId)
		{
		}

		public PerTenantPolicyNudgeRulesCollection.PolicyNudgeRules Rules
		{
			get
			{
				return this.data;
			}
		}

		public override void Initialize()
		{
			base.Initialize(PerTenantPolicyNudgeRulesCollection.NotificationLock);
		}

		protected override ADNotificationRequestCookie Register(IConfigurationSession session)
		{
			ADNotificationRequestCookie request = ADNotificationAdapter.RegisterChangeNotification<PolicyTipMessageConfig>(PerTenantPolicyNudgeRulesCollection.GetPolicyTipMessageConfigsContainerId(session), new ADNotificationCallback(base.ChangeCallback), session);
			ADNotificationRequestCookie result;
			try
			{
				result = ADNotificationAdapter.RegisterChangeNotification<TransportRule>(PerTenantPolicyNudgeRulesCollection.GetPolicyNudgeRuleContainerId(session), new ADNotificationCallback(base.ChangeCallback), session);
			}
			catch (Exception ex)
			{
				ADNotificationAdapter.UnregisterChangeNotification(request);
				throw ex;
			}
			return result;
		}

		protected override bool RefreshOnChange
		{
			get
			{
				return true;
			}
		}

		protected override PerTenantPolicyNudgeRulesCollection.PolicyNudgeRules Read(IConfigurationSession session)
		{
			ADPagedReader<PolicyTipMessageConfig> adpagedReader = session.FindPaged<PolicyTipMessageConfig>(PerTenantPolicyNudgeRulesCollection.GetPolicyTipMessageConfigsContainerId(session), QueryScope.SubTree, null, null, 0);
			IEnumerable<PolicyTipMessageConfig> tenantPolicyTipMessages = adpagedReader.ReadAllPages();
			ADPagedReader<TransportRule> adpagedReader2 = session.FindPaged<TransportRule>(PerTenantPolicyNudgeRulesCollection.GetPolicyNudgeRuleContainerId(session), QueryScope.SubTree, null, PerTenantPolicyNudgeRulesCollection.PriorityOrder, 0);
			PolicyNudgeRuleParser parser = PolicyNudgeRuleParser.Instance;
			IEnumerable<PolicyNudgeRule> tenantPolicyNudgeRules = (from rule in adpagedReader2.ReadAllPages()
			select parser.GetRule(rule.Xml, rule.Name, rule.WhenChangedUTC ?? (rule.WhenCreatedUTC ?? DateTime.MinValue)) into rule
			where rule != null
			select rule).ToList<PolicyNudgeRule>();
			return new PerTenantPolicyNudgeRulesCollection.PolicyNudgeRules(tenantPolicyNudgeRules, new PerTenantPolicyNudgeRulesCollection.PolicyTipMessages(tenantPolicyTipMessages));
		}

		private static ADObjectId GetPolicyNudgeRuleContainerId(IConfigurationSession session)
		{
			ADObjectId orgContainerId = session.GetOrgContainerId();
			return orgContainerId.GetDescendantId("Transport Settings", "Rules", new string[]
			{
				"TransportVersioned"
			});
		}

		private static ADObjectId GetPolicyTipMessageConfigsContainerId(IConfigurationSession session)
		{
			ADObjectId orgContainerId = session.GetOrgContainerId();
			return orgContainerId.GetDescendantId(PolicyTipMessageConfig.PolicyTipMessageConfigContainer);
		}

		private const string EventSource = "MSExchange OutlookPolicyNudgeRules";

		private static readonly Trace Tracer = ExTraceGlobals.OutlookPolicyNudgeRulesTracer;

		private static readonly ExEventLog Logger = new ExEventLog(ExTraceGlobals.OutlookPolicyNudgeRulesTracer.Category, "MSExchange OutlookPolicyNudgeRules");

		private static readonly object NotificationLock = new object();

		private static readonly SortBy PriorityOrder = new SortBy(TransportRuleSchema.Priority, SortOrder.Ascending);

		internal class PolicyNudgeRules
		{
			internal PolicyNudgeRules(IEnumerable<PolicyNudgeRule> tenantPolicyNudgeRules, PerTenantPolicyNudgeRulesCollection.PolicyTipMessages policyTipMessages)
			{
				this.Rules = tenantPolicyNudgeRules;
				this.Messages = policyTipMessages;
			}

			internal IEnumerable<PolicyNudgeRule> Rules { get; private set; }

			internal PerTenantPolicyNudgeRulesCollection.PolicyTipMessages Messages { get; private set; }

			internal static PerTenantPolicyNudgeRulesCollection.PolicyNudgeRules Empty = new PerTenantPolicyNudgeRulesCollection.PolicyNudgeRules(new PolicyNudgeRule[0], new PerTenantPolicyNudgeRulesCollection.PolicyTipMessages(new PolicyTipMessageConfig[0]));
		}

		internal class PolicyTipMessages
		{
			internal PolicyTipMessages(IEnumerable<PolicyTipMessageConfig> tenantPolicyTipMessages)
			{
				this.tenantPolicyTipMessages = PerTenantPolicyNudgeRulesCollection.PolicyTipMessages.ToDictionary(tenantPolicyTipMessages);
			}

			private static IEnumerable<PolicyTipMessageConfig> GetBuiltIn()
			{
				ADObjectId GlobalScopeContainerId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest().GetDescendantId(PolicyTipMessageConfig.PolicyTipMessageConfigContainer);
				IEnumerable<CultureInfo> supportedCultureInfos = from lcid in LanguagePackInfo.expectedCultureLcids
				select new CultureInfo(lcid);
				PolicyTipMessageConfig policyTipMessageConfig;
				foreach (CultureInfo exchangeCultureInfo in supportedCultureInfos)
				{
					foreach (Tuple<PolicyTipMessageConfigAction, LocalizedString> mapping in PerTenantPolicyNudgeRulesCollection.PolicyTipMessages.builtInActionStringsMapping)
					{
						policyTipMessageConfig = new PolicyTipMessageConfig
						{
							Action = mapping.Item1,
							Locale = exchangeCultureInfo.Name,
							Value = mapping.Item2.ToString(exchangeCultureInfo)
						};
						policyTipMessageConfig.SetId(GlobalScopeContainerId.GetChildId("BuiltIn\\" + exchangeCultureInfo.Name + "\\" + mapping.Item1.ToString()));
						yield return policyTipMessageConfig;
					}
				}
				policyTipMessageConfig = new PolicyTipMessageConfig
				{
					Action = PolicyTipMessageConfigAction.Url,
					Locale = string.Empty,
					Value = string.Empty
				};
				policyTipMessageConfig.SetId(GlobalScopeContainerId.GetChildId("BuiltIn\\" + PolicyTipMessageConfigAction.Url.ToString()));
				yield return policyTipMessageConfig;
				yield break;
			}

			private static IDictionary<Tuple<string, PolicyTipMessageConfigAction>, PolicyTipMessage> ToDictionary(IEnumerable<PolicyTipMessageConfig> policyTipMessageConfigs)
			{
				return policyTipMessageConfigs.ToDictionary((PolicyTipMessageConfig m) => Tuple.Create<string, PolicyTipMessageConfigAction>(m.Locale, m.Action), (PolicyTipMessageConfig m) => new PolicyTipMessage(m.Value, m.Identity.ToString(), m.WhenChangedUTC ?? (m.WhenCreatedUTC ?? DateTime.MinValue)));
			}

			public bool TryGetValue(Tuple<string, PolicyTipMessageConfigAction> key, out PolicyTipMessage value)
			{
				return this.tenantPolicyTipMessages.TryGetValue(key, out value) || PerTenantPolicyNudgeRulesCollection.PolicyTipMessages.builtIn.Value.TryGetValue(key, out value);
			}

			private IDictionary<Tuple<string, PolicyTipMessageConfigAction>, PolicyTipMessage> tenantPolicyTipMessages;

			private static Lazy<IDictionary<Tuple<string, PolicyTipMessageConfigAction>, PolicyTipMessage>> builtIn = new Lazy<IDictionary<Tuple<string, PolicyTipMessageConfigAction>, PolicyTipMessage>>(() => PerTenantPolicyNudgeRulesCollection.PolicyTipMessages.ToDictionary(PerTenantPolicyNudgeRulesCollection.PolicyTipMessages.builtInConfigs.Value), LazyThreadSafetyMode.PublicationOnly);

			internal static Lazy<IEnumerable<PolicyTipMessageConfig>> builtInConfigs = new Lazy<IEnumerable<PolicyTipMessageConfig>>(() => PerTenantPolicyNudgeRulesCollection.PolicyTipMessages.GetBuiltIn().ToList<PolicyTipMessageConfig>(), LazyThreadSafetyMode.PublicationOnly);

			private static Tuple<PolicyTipMessageConfigAction, LocalizedString>[] builtInActionStringsMapping = new Tuple<PolicyTipMessageConfigAction, LocalizedString>[]
			{
				Tuple.Create<PolicyTipMessageConfigAction, LocalizedString>(PolicyTipMessageConfigAction.NotifyOnly, ClientStrings.PolicyTipDefaultMessageNotifyOnly),
				Tuple.Create<PolicyTipMessageConfigAction, LocalizedString>(PolicyTipMessageConfigAction.RejectOverride, ClientStrings.PolicyTipDefaultMessageRejectOverride),
				Tuple.Create<PolicyTipMessageConfigAction, LocalizedString>(PolicyTipMessageConfigAction.Reject, ClientStrings.PolicyTipDefaultMessageReject)
			};
		}
	}
}
