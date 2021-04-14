using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "AccountPartition", DefaultParameterSetName = "Identity")]
	public sealed class GetAccountPartition : GetSystemConfigurationObjectTask<AccountPartitionIdParameter, AccountPartition>
	{
		[Parameter]
		public SwitchParameter IncludeSecondaryPartitions { private get; set; }

		protected override ObjectId RootId
		{
			get
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				return configurationSession.GetOrgContainerId().GetChildId(Microsoft.Exchange.Data.Directory.SystemConfiguration.AccountPartition.AccountForestContainerName);
			}
		}

		protected override QueryFilter InternalFilter
		{
			get
			{
				if (this.IncludeSecondaryPartitions)
				{
					return base.InternalFilter;
				}
				QueryFilter queryFilter = new ComparisonFilter(ComparisonOperator.NotEqual, AccountPartitionSchema.IsSecondary, true);
				if (base.InternalFilter != null)
				{
					return new AndFilter(new QueryFilter[]
					{
						base.InternalFilter,
						queryFilter
					});
				}
				return queryFilter;
			}
		}
	}
}
