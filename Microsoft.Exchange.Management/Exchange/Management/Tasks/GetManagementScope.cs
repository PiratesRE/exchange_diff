using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("Get", "ManagementScope", DefaultParameterSetName = "Identity")]
	public sealed class GetManagementScope : GetMultitenancySystemConfigurationObjectTask<ManagementScopeIdParameter, ManagementScope>
	{
		protected override SharedTenantConfigurationMode SharedTenantConfigurationMode
		{
			get
			{
				return SharedTenantConfigurationMode.Static;
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
				return base.OptionalIdentityData.AdditionalFilter;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter Orphan
		{
			get
			{
				return (SwitchParameter)(base.Fields["Orphan"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Orphan"] = value;
				if (value.ToBool())
				{
					base.OptionalIdentityData.AdditionalFilter = QueryFilter.AndTogether(new QueryFilter[]
					{
						base.OptionalIdentityData.AdditionalFilter,
						GetManagementScope.orphanedFilter
					});
				}
			}
		}

		[Parameter(Mandatory = false)]
		public bool Exclusive
		{
			get
			{
				return (bool)base.Fields["Exclusive"];
			}
			set
			{
				base.Fields["Exclusive"] = value;
				base.OptionalIdentityData.AdditionalFilter = QueryFilter.AndTogether(new QueryFilter[]
				{
					base.OptionalIdentityData.AdditionalFilter,
					new ComparisonFilter(ComparisonOperator.Equal, ManagementScopeSchema.Exclusive, value)
				});
			}
		}

		public static void StampQueryFilterOnManagementScope(ManagementScope managementScope)
		{
			ExchangeRunspaceConfiguration.TryStampQueryFilterOnManagementScope(managementScope);
		}

		private static QueryFilter orphanedFilter = new AndFilter(new QueryFilter[]
		{
			new NotFilter(new ExistsFilter(ManagementScopeSchema.RecipientWriteScopeBL)),
			new NotFilter(new ExistsFilter(ManagementScopeSchema.ConfigWriteScopeBL))
		});
	}
}
