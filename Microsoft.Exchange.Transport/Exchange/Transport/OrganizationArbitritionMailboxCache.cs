using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Transport;

namespace Microsoft.Exchange.Transport
{
	internal sealed class OrganizationArbitritionMailboxCache : LazyLookupTimeoutCache<OrganizationId, List<ADObjectId>>
	{
		public OrganizationArbitritionMailboxCache() : base(2, 100, false, TimeSpan.FromMinutes(10.0), TimeSpan.FromHours(1.0))
		{
		}

		protected override List<ADObjectId> CreateOnCacheMiss(OrganizationId key, ref bool shouldAdd)
		{
			ExTraceGlobals.ResolverTracer.TraceError((long)this.GetHashCode(), "No arbitration mailbox address.");
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(key), 61, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\transport\\OrganizationArbitritionMailboxCache.cs");
			ADObjectId descendantId = tenantOrTopologyConfigurationSession.GetOrgContainerId().GetDescendantId(ApprovalApplication.ParentPathInternal);
			ADObjectId childId = descendantId.GetChildId("ModeratedRecipients");
			QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientTypeDetailsValue, RecipientTypeDetails.ArbitrationMailbox);
			QueryFilter queryFilter2 = new ComparisonFilter(ComparisonOperator.Equal, ADUserSchema.ApprovalApplications, childId);
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				queryFilter,
				queryFilter2
			});
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(key), ConfigScopes.TenantSubTree, 81, "CreateOnCacheMiss", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\transport\\OrganizationArbitritionMailboxCache.cs");
			ADRawEntry[] array = tenantOrRootOrgRecipientSession.Find(null, QueryScope.SubTree, filter, null, 350, OrganizationArbitritionMailboxCache.idProperties);
			List<ADObjectId> list = new List<ADObjectId>();
			for (int i = 0; i < array.Length; i++)
			{
				ADObjectId adobjectId = (ADObjectId)array[i][ADObjectSchema.Id];
				if (adobjectId != null && !adobjectId.IsDeleted)
				{
					shouldAdd = true;
					list.Add(adobjectId);
				}
			}
			if (list.Count == 0)
			{
				shouldAdd = false;
			}
			return list;
		}

		private static readonly PropertyDefinition[] idProperties = new PropertyDefinition[]
		{
			ADObjectSchema.Id
		};
	}
}
