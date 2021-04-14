using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "AcceptedDomain", DefaultParameterSetName = "Identity")]
	public sealed class GetAcceptedDomain : GetMultitenancySystemConfigurationObjectTask<AcceptedDomainIdParameter, AcceptedDomain>
	{
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

		[Parameter(Mandatory = false)]
		[ValidateNotNullOrEmpty]
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
				base.OptionalIdentityData.AdditionalFilter = monadFilter.InnerFilter;
				base.Fields["Filter"] = value;
			}
		}

		internal ObjectSchema FilterableObjectSchema
		{
			get
			{
				return ObjectSchema.GetInstance<AcceptedDomainSchema>();
			}
		}

		[Parameter(Mandatory = false)]
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
				QueryFilter queryFilter = base.InternalFilter;
				if (this.inputFilter != null)
				{
					if (queryFilter != null)
					{
						queryFilter = new AndFilter(new QueryFilter[]
						{
							queryFilter,
							this.inputFilter
						});
					}
					else
					{
						queryFilter = this.inputFilter;
					}
				}
				if (this.UsnForReconciliationSearch >= 0L)
				{
					ComparisonFilter comparisonFilter = new ComparisonFilter(ComparisonOperator.GreaterThan, AcceptedDomainSchema.UsnCreated, this.UsnForReconciliationSearch);
					if (queryFilter != null)
					{
						queryFilter = new AndFilter(new QueryFilter[]
						{
							queryFilter,
							comparisonFilter
						});
					}
					else
					{
						queryFilter = comparisonFilter;
					}
				}
				return queryFilter;
			}
		}

		protected override void InternalBeginProcessing()
		{
			TaskLogger.LogEnter();
			base.InternalBeginProcessing();
			if (this.UsnForReconciliationSearch >= 0L)
			{
				if (base.DomainController == null)
				{
					base.ThrowTerminatingError(new ArgumentException(Strings.ErrorDomainControllerNotSpecifiedWithUsnForReconciliationSearch), ErrorCategory.InvalidArgument, null);
				}
				base.InternalResultSize = Unlimited<uint>.UnlimitedValue;
				base.OptionalIdentityData.AdditionalFilter = new ComparisonFilter(ComparisonOperator.GreaterThan, AcceptedDomainSchema.UsnCreated, this.UsnForReconciliationSearch);
			}
			TaskLogger.LogExit();
		}

		protected override bool DeepSearch
		{
			get
			{
				return true;
			}
		}

		private QueryFilter inputFilter;

		private long usnForReconciliationSearch = -1L;
	}
}
