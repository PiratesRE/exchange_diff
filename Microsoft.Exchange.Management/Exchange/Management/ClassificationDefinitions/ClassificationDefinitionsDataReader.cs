using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.ClassificationDefinitions
{
	internal class ClassificationDefinitionsDataReader : IClassificationDefinitionsDataReader
	{
		internal ClassificationDefinitionsDataReader(bool isAggregateReader = true)
		{
			this.isAggregateReader = isAggregateReader;
		}

		internal static IClassificationDefinitionsDataReader DefaultInstance
		{
			get
			{
				return ClassificationDefinitionsDataReader.singletonDefaultInstance;
			}
		}

		public IEnumerable<TransportRule> GetAllClassificationRuleCollection(OrganizationId organizationId, IConfigurationSession currentDataSession, QueryFilter additionalFilter)
		{
			if (object.ReferenceEquals(null, organizationId))
			{
				throw new ArgumentNullException("organizationId");
			}
			if (currentDataSession != null && !organizationId.Equals(currentDataSession.SessionSettings.CurrentOrganizationId))
			{
				throw new ArgumentException(new ArgumentException().Message, "currentDataSession");
			}
			HashSet<TransportRule> hashSet = new HashSet<TransportRule>(ClassificationDefinitionsDataReader.transportRuleComparer);
			bool flag = OrganizationId.ForestWideOrgId.Equals(organizationId);
			IConfigurationSession configurationSession = null;
			if (flag && currentDataSession != null)
			{
				configurationSession = currentDataSession;
			}
			else if (flag || this.isAggregateReader)
			{
				configurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 1092, "GetAllClassificationRuleCollection", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\ClassificationDefinitions\\ClassificationDefinitionUtils.cs");
			}
			if (configurationSession != null)
			{
				hashSet.UnionWith(configurationSession.FindPaged<TransportRule>(additionalFilter, configurationSession.GetOrgContainerId().GetDescendantId(ClassificationDefinitionConstants.ClassificationDefinitionsRdn), false, null, 0));
			}
			if (!flag)
			{
				IConfigurationSession configurationSession2 = currentDataSession ?? DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), organizationId, null, false), 1114, "GetAllClassificationRuleCollection", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\ClassificationDefinitions\\ClassificationDefinitionUtils.cs");
				hashSet.UnionWith(configurationSession2.FindPaged<TransportRule>(additionalFilter, configurationSession2.GetOrgContainerId().GetDescendantId(ClassificationDefinitionConstants.ClassificationDefinitionsRdn), false, null, 0));
			}
			return hashSet;
		}

		public DataClassificationConfig GetDataClassificationConfig(OrganizationId organizationId, IConfigurationSession currentDataSession)
		{
			if (object.ReferenceEquals(null, organizationId))
			{
				throw new ArgumentNullException("organizationId");
			}
			if (currentDataSession == null)
			{
				throw new ArgumentNullException("currentDataSession");
			}
			if (!VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled || OrganizationId.ForestWideOrgId.Equals(organizationId))
			{
				return null;
			}
			if (!organizationId.Equals(currentDataSession.SessionSettings.CurrentOrganizationId))
			{
				throw new ArgumentException(new ArgumentException().Message, "currentDataSession");
			}
			SharedConfiguration sharedConfiguration = SharedConfiguration.GetSharedConfiguration(organizationId);
			IConfigurationSession configurationSession;
			if (sharedConfiguration != null)
			{
				configurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.PartiallyConsistent, sharedConfiguration.GetSharedConfigurationSessionSettings(), 1186, "GetDataClassificationConfig", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\ClassificationDefinitions\\ClassificationDefinitionUtils.cs");
			}
			else
			{
				configurationSession = currentDataSession;
			}
			DataClassificationConfig[] array = configurationSession.Find<DataClassificationConfig>(null, QueryScope.SubTree, null, null, 1);
			ExAssert.RetailAssert(array != null && 1 == array.Length, "There should be one and only one DataClassificationConfig applicable to a particular tenant.");
			return array[0];
		}

		private static readonly IEqualityComparer<TransportRule> transportRuleComparer = new EqualityComparer<TransportRule>((TransportRule lhsTransportRule, TransportRule rhsTransportRule) => StringComparer.OrdinalIgnoreCase.Equals(lhsTransportRule.DistinguishedName, rhsTransportRule.DistinguishedName), (TransportRule currentTransportRule) => StringComparer.OrdinalIgnoreCase.GetHashCode(currentTransportRule.DistinguishedName));

		private static readonly ClassificationDefinitionsDataReader singletonDefaultInstance = new ClassificationDefinitionsDataReader(true);

		private readonly bool isAggregateReader;
	}
}
