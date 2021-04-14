using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ForwardSyncTasks
{
	[Cmdlet("Get", "MsoFullSyncOrganization")]
	public sealed class GetMsoFullSyncOrganization : GetSystemConfigurationObjectTask<OrganizationIdParameter, ExchangeConfigurationUnit>
	{
		public GetMsoFullSyncOrganization()
		{
			base.OptionalIdentityData.AdditionalFilter = new OrFilter(new QueryFilter[]
			{
				new ExistsFilter(ExchangeConfigurationUnitSchema.MsoForwardSyncRecipientCookie),
				new ExistsFilter(ExchangeConfigurationUnitSchema.MsoForwardSyncNonRecipientCookie)
			});
		}

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public new AccountPartitionIdParameter AccountPartition
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
				QueryFilter queryFilter = new ExistsFilter(ExchangeConfigurationUnitSchema.MsoForwardSyncNonRecipientCookie);
				QueryFilter queryFilter2 = new ExistsFilter(ExchangeConfigurationUnitSchema.MsoForwardSyncRecipientCookie);
				QueryFilter queryFilter3 = new OrFilter(new QueryFilter[]
				{
					queryFilter,
					queryFilter2
				});
				QueryFilter internalFilter = base.InternalFilter;
				if (internalFilter != null)
				{
					return new AndFilter(new QueryFilter[]
					{
						queryFilter3,
						internalFilter
					});
				}
				return queryFilter3;
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
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, base.SessionSettings, ConfigScopes.TenantSubTree, 80, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\ForwardSync\\GetMsoFullSyncOrganization.cs");
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			ExchangeConfigurationUnit dataObject2 = (ExchangeConfigurationUnit)dataObject;
			TenantOrganizationPresentationObject dataObject3 = new TenantOrganizationPresentationObject(dataObject2);
			base.WriteResult(dataObject3);
		}
	}
}
