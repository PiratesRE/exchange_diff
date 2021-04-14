using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Sync.CookieManager;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Get", "MsoTenantSyncRequest")]
	public sealed class GetMsoTenantSyncRequest : GetSystemConfigurationObjectTask<OrganizationIdParameter, MsoTenantCookieContainer>
	{
		private new AccountPartitionIdParameter AccountPartition
		{
			get
			{
				return base.AccountPartition;
			}
			set
			{
				base.AccountPartition = value;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = QueryFilter.OrTogether(new QueryFilter[]
				{
					new ExistsFilter(ExchangeConfigurationUnitSchema.MsoForwardSyncNonRecipientCookie),
					new ExistsFilter(ExchangeConfigurationUnitSchema.MsoForwardSyncRecipientCookie)
				});
				return QueryFilter.AndTogether(new QueryFilter[]
				{
					base.InternalFilter,
					queryFilter
				});
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, base.SessionSettings, 75, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ForwardSync\\GetMsoTenantSyncRequest.cs");
		}

		protected override IEnumerable<MsoTenantCookieContainer> GetPagedData()
		{
			if (this.Identity == null)
			{
				QueryFilter queryFilter = QueryFilter.OrTogether(new QueryFilter[]
				{
					new ExistsFilter(ExchangeConfigurationUnitSchema.MsoForwardSyncNonRecipientCookie),
					new ExistsFilter(ExchangeConfigurationUnitSchema.MsoForwardSyncRecipientCookie)
				});
				QueryFilter filter = QueryFilter.AndTogether(new QueryFilter[]
				{
					queryFilter,
					new ComparisonFilter(ComparisonOperator.NotEqual, ExchangeConfigurationUnitSchema.OrganizationStatus, OrganizationStatus.ReadyForRemoval),
					new ComparisonFilter(ComparisonOperator.NotEqual, ExchangeConfigurationUnitSchema.OrganizationStatus, OrganizationStatus.SoftDeleted),
					new ComparisonFilter(ComparisonOperator.NotEqual, ExchangeConfigurationUnitSchema.OrganizationStatus, OrganizationStatus.PendingRemoval)
				});
				return PartitionDataAggregator.FindTenantCookieContainers(filter);
			}
			return base.GetPagedData();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			MsoTenantCookieContainer msoTenantCookieContainer = (MsoTenantCookieContainer)dataObject;
			MsoRecipientFullSyncCookieManager msoRecipientFullSyncCookieManager = new MsoRecipientFullSyncCookieManager(Guid.Parse(msoTenantCookieContainer.ExternalDirectoryOrganizationId));
			MsoCompanyFullSyncCookieManager msoCompanyFullSyncCookieManager = new MsoCompanyFullSyncCookieManager(Guid.Parse(msoTenantCookieContainer.ExternalDirectoryOrganizationId));
			msoRecipientFullSyncCookieManager.ReadCookie();
			msoCompanyFullSyncCookieManager.ReadCookie();
			MsoTenantSyncRequest dataObject2 = new MsoTenantSyncRequest(msoTenantCookieContainer, msoRecipientFullSyncCookieManager.LastCookie, msoCompanyFullSyncCookieManager.LastCookie);
			base.WriteResult(dataObject2);
		}
	}
}
