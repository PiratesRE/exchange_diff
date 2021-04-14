using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.InfoWorker.EventLog;
using Microsoft.Exchange.MessagingPolicies.Rules;
using Microsoft.Exchange.MessagingPolicies.Rules.Classification;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal sealed class PerTenantClassificationDefinitionCollection : PerTenantConfigurationLoader<IEnumerable<ClassificationRulePackage>>
	{
		public PerTenantClassificationDefinitionCollection(OrganizationId organizationId) : base(organizationId)
		{
		}

		public IEnumerable<ClassificationRulePackage> ClassificationDefinitions
		{
			get
			{
				if (!this.organizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					return this.data.Concat(PerTenantClassificationDefinitionCollection.oobClassificationDefinitions.Value.ClassificationDefinitions);
				}
				return this.data;
			}
		}

		public override void Initialize()
		{
			base.Initialize(PerTenantClassificationDefinitionCollection.NotificationLock);
		}

		protected override ADNotificationRequestCookie Register(IConfigurationSession session)
		{
			return ADNotificationAdapter.RegisterChangeNotification<TransportRule>(PerTenantClassificationDefinitionCollection.GetClassificationDefinitionsContainerId(session), new ADNotificationCallback(base.ChangeCallback), session);
		}

		protected override bool RefreshOnChange
		{
			get
			{
				return true;
			}
		}

		protected override IEnumerable<ClassificationRulePackage> Read(IConfigurationSession session)
		{
			ADPagedReader<TransportRule> adpagedReader = session.FindPaged<TransportRule>(PerTenantClassificationDefinitionCollection.GetClassificationDefinitionsContainerId(session), QueryScope.SubTree, null, PerTenantClassificationDefinitionCollection.PriorityOrder, 0);
			TransportRule[] source = adpagedReader.ReadAllPages();
			ClassificationParser parser = ClassificationParser.Instance;
			return (from rule in source.Select(delegate(TransportRule adRule)
			{
				try
				{
					ClassificationRulePackage rulePackage = parser.GetRulePackage(adRule.ReplicationSignature);
					rulePackage.Version = (adRule.WhenChangedUTC ?? (adRule.WhenCreatedUTC ?? DateTime.MinValue));
					rulePackage.ID = adRule.Name;
					return rulePackage;
				}
				catch (ParserException ex)
				{
					PerTenantClassificationDefinitionCollection.Tracer.TraceError<ADObjectId, Exception>((long)this.GetHashCode(), "Rule with identity {0} is corrupted and will not be returned to clients.  Details: {1}", adRule.Id, ex);
					CachedOrganizationConfiguration.Logger.LogEvent(InfoWorkerEventLogConstants.Tuple_CorruptClassificationDefinition, adRule.Id.ToString(), new object[]
					{
						adRule.Id,
						ex
					});
				}
				return null;
			})
			where rule != null
			select rule).ToList<ClassificationRulePackage>();
		}

		private static ADObjectId GetClassificationDefinitionsContainerId(IConfigurationSession session)
		{
			ADObjectId orgContainerId = session.GetOrgContainerId();
			return orgContainerId.GetDescendantId("Transport Settings", "Rules", new string[]
			{
				"ClassificationDefinitions"
			});
		}

		private const string EventSource = "MSExchange ClassificationDefinitions";

		private static readonly Trace Tracer = ExTraceGlobals.ClassificationDefinitionsTracer;

		private static readonly ExEventLog Logger = new ExEventLog(ExTraceGlobals.ClassificationDefinitionsTracer.Category, "MSExchange ClassificationDefinitions");

		private static readonly object NotificationLock = new object();

		private static readonly SortBy PriorityOrder = new SortBy(TransportRuleSchema.Priority, SortOrder.Ascending);

		private static Lazy<PerTenantClassificationDefinitionCollection> oobClassificationDefinitions = new Lazy<PerTenantClassificationDefinitionCollection>(delegate()
		{
			PerTenantClassificationDefinitionCollection perTenantClassificationDefinitionCollection = new PerTenantClassificationDefinitionCollection(OrganizationId.ForestWideOrgId);
			perTenantClassificationDefinitionCollection.Initialize();
			return perTenantClassificationDefinitionCollection;
		}, LazyThreadSafetyMode.ExecutionAndPublication);
	}
}
