using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "TenantContainer")]
	public sealed class InstallTenantContainerTask : InstallContainerTaskBase<Container>
	{
		[Parameter(Mandatory = true)]
		public Guid AccountPartition
		{
			get
			{
				return (Guid)(base.Fields["Partition"] ?? Guid.Empty);
			}
			set
			{
				base.Fields["Partition"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			base.CreateSession();
			return DirectorySessionFactory.Default.CreateTenantConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromAllTenantsPartitionId(new PartitionId(this.AccountPartition)), 41, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\InstallTenantContainerTask.cs");
		}

		protected override ADObjectId GetBaseContainer()
		{
			return ADSession.GetConfigurationUnitsRoot(new PartitionId(this.AccountPartition).ForestFQDN);
		}
	}
}
