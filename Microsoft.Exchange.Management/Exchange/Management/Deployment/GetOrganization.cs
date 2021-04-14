using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Deployment
{
	[Cmdlet("Get", "Organization", DefaultParameterSetName = "Identity")]
	public sealed class GetOrganization : GetSystemConfigurationObjectTask<OrganizationIdParameter, ExchangeConfigurationUnit>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity", Position = 0, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
		public override OrganizationIdParameter Identity
		{
			get
			{
				return (OrganizationIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public long UsnForReconciliationSearch
		{
			get
			{
				return this.usnForReconciliationSearch;
			}
			set
			{
				this.usnForReconciliationSearch = value;
			}
		}

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false)]
		public string Filter
		{
			get
			{
				return (string)base.Fields["Filter"];
			}
			set
			{
				MonadFilter monadFilter = new MonadFilter(value, this, this.FilterableObjectSchema);
				this.inputFilter = monadFilter.InnerFilter;
				base.Fields["Filter"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Unlimited<uint> ResultSize
		{
			get
			{
				return base.InternalResultSize;
			}
			set
			{
				base.InternalResultSize = value;
			}
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

		internal ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<ExchangeConfigurationUnitSchema>();
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				QueryFilter queryFilter = new ExistsFilter(ExchangeConfigurationUnitSchema.OrganizationalUnitLink);
				QueryFilter queryFilter2;
				if (base.InternalFilter != null)
				{
					queryFilter2 = new AndFilter(new QueryFilter[]
					{
						base.InternalFilter,
						queryFilter
					});
				}
				else
				{
					queryFilter2 = queryFilter;
				}
				if (this.inputFilter != null)
				{
					queryFilter2 = new AndFilter(new QueryFilter[]
					{
						queryFilter2,
						this.inputFilter
					});
				}
				if (this.UsnForReconciliationSearch >= 0L)
				{
					queryFilter2 = new AndFilter(new QueryFilter[]
					{
						queryFilter2,
						new ComparisonFilter(ComparisonOperator.GreaterThan, ExchangeConfigurationUnitSchema.UsnCreated, this.UsnForReconciliationSearch)
					});
				}
				return queryFilter2;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			ADSessionSettings adsessionSettings = (this.accountPartition != null) ? ADSessionSettings.FromAllTenantsPartitionId(this.accountPartition) : ADSessionSettings.RescopeToAllTenants(base.SessionSettings);
			adsessionSettings.TenantConsistencyMode = TenantConsistencyMode.IncludeRetiredTenants;
			return DirectorySessionFactory.Default.CreateTenantConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, null, adsessionSettings, 184, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\GetOrganizationTask.cs");
		}

		protected override void InternalStateReset()
		{
			this.accountPartition = null;
			if (this.AccountPartition != null)
			{
				this.accountPartition = RecipientTaskHelper.ResolvePartitionId(this.AccountPartition, new Task.TaskErrorLoggingDelegate(base.WriteError));
			}
			PartitionId partitionId;
			if (this.RequestForMultipleOrgs() && this.accountPartition == null && ADAccountPartitionLocator.IsSingleForestTopology(out partitionId))
			{
				this.accountPartition = partitionId;
				this.WriteWarning(Strings.ImplicitPartitionIdSupplied(this.accountPartition.ToString()));
			}
			if (this.Identity != null)
			{
				this.Identity.AccountPartition = this.accountPartition;
			}
			base.InternalStateReset();
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			if (this.UsnForReconciliationSearch >= 0L && base.DomainController == null)
			{
				base.ThrowTerminatingError(new ArgumentException(Strings.ErrorDomainControllerNotSpecifiedWithUsnForReconciliationSearch), ErrorCategory.InvalidArgument, null);
			}
			base.InternalBeginProcessing();
			TaskLogger.LogExit();
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			ExchangeConfigurationUnit exchangeConfigurationUnit = (ExchangeConfigurationUnit)dataObject;
			if (exchangeConfigurationUnit.OrganizationStatus != OrganizationStatus.ReadyForRemoval && exchangeConfigurationUnit.OrganizationStatus != OrganizationStatus.SoftDeleted && exchangeConfigurationUnit.OrganizationStatus != OrganizationStatus.Active && exchangeConfigurationUnit.OrganizationStatus != OrganizationStatus.ReadOnly)
			{
				this.WriteWarning(Strings.ErrorNonActiveOrganizationFound(exchangeConfigurationUnit.Identity.ToString()));
			}
			TenantOrganizationPresentationObject tenantOrganizationPresentationObject = new TenantOrganizationPresentationObject(exchangeConfigurationUnit);
			if (exchangeConfigurationUnit.HasSharedConfigurationBL())
			{
				tenantOrganizationPresentationObject.IsSharingConfiguration = true;
				tenantOrganizationPresentationObject.ResetChangeTracking();
			}
			base.WriteResult(tenantOrganizationPresentationObject);
		}

		protected override void InternalValidate()
		{
			if (this.RequestForMultipleOrgs() && this.accountPartition == null)
			{
				base.WriteError(new ArgumentException(Strings.PartitionIdRequiredForMultipleOrgSearch), ErrorCategory.InvalidArgument, null);
			}
			base.InternalValidate();
		}

		private bool RequestForMultipleOrgs()
		{
			return this.Identity == null;
		}

		private QueryFilter inputFilter;

		private long usnForReconciliationSearch = -1L;

		private PartitionId accountPartition;
	}
}
