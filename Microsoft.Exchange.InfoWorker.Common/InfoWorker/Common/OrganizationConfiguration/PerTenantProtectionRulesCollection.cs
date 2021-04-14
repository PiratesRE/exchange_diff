using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.OutlookProtection;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal sealed class PerTenantProtectionRulesCollection : PerTenantConfigurationLoader<IEnumerable<OutlookProtectionRule>>
	{
		public PerTenantProtectionRulesCollection(OrganizationId organizationId) : base(organizationId)
		{
		}

		public IEnumerable<OutlookProtectionRule> ProtectionRules
		{
			get
			{
				return this.data;
			}
		}

		public override void Initialize()
		{
			base.Initialize(PerTenantProtectionRulesCollection.NotificationLock);
		}

		protected override ADNotificationRequestCookie Register(IConfigurationSession session)
		{
			return ADNotificationAdapter.RegisterChangeNotification<TransportRule>(PerTenantProtectionRulesCollection.GetProtectionRuleContainerId(session), new ADNotificationCallback(base.ChangeCallback), session);
		}

		protected override IEnumerable<OutlookProtectionRule> Read(IConfigurationSession session)
		{
			ADPagedReader<TransportRule> adpagedReader = session.FindPaged<TransportRule>(PerTenantProtectionRulesCollection.GetProtectionRuleContainerId(session), QueryScope.SubTree, null, PerTenantProtectionRulesCollection.PriorityOrder, 0);
			TransportRule[] array = adpagedReader.ReadAllPages();
			List<OutlookProtectionRule> list = new List<OutlookProtectionRule>(array.Length);
			OutlookProtectionRuleParser instance = OutlookProtectionRuleParser.Instance;
			foreach (TransportRule transportRule in array)
			{
				try
				{
					list.Add((OutlookProtectionRule)instance.GetRule(transportRule.Xml));
				}
				catch (ParserException ex)
				{
					PerTenantProtectionRulesCollection.Tracer.TraceError<ADObjectId, Exception>((long)this.GetHashCode(), "Rule with identity {0} is corrupted and will not be returned to clients.  Details: {1}", transportRule.Id, ex);
					CachedOrganizationConfiguration.Logger.LogEvent(InfoWorkerEventLogConstants.Tuple_CorruptOutlookProtectionRule, transportRule.Id.ToString(), new object[]
					{
						transportRule.Id,
						ex
					});
				}
			}
			return list;
		}

		private static ADObjectId GetProtectionRuleContainerId(IConfigurationSession session)
		{
			ADObjectId orgContainerId = session.GetOrgContainerId();
			return orgContainerId.GetDescendantId("Transport Settings", "Rules", new string[]
			{
				"OutlookProtectionRules"
			});
		}

		private const string EventSource = "MSExchange OutlookProtectionRules";

		private static readonly Trace Tracer = ExTraceGlobals.OutlookProtectionRulesTracer;

		private static readonly ExEventLog Logger = new ExEventLog(ExTraceGlobals.OutlookProtectionRulesTracer.Category, "MSExchange OutlookProtectionRules");

		private static readonly object NotificationLock = new object();

		private static readonly SortBy PriorityOrder = new SortBy(TransportRuleSchema.Priority, SortOrder.Ascending);
	}
}
