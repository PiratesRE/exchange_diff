using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "AccountPartition", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveAccountPartition : RemoveSystemConfigurationObjectTask<AccountPartitionIdParameter, AccountPartition>
	{
		protected override ObjectId RootId
		{
			get
			{
				IConfigurationSession configurationSession = (IConfigurationSession)base.DataSession;
				return configurationSession.GetOrgContainerId().GetChildId(AccountPartition.AccountForestContainerName);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveAccountPartition(this.Identity.ToString());
			}
		}

		[Parameter]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		protected override void InternalProcessRecord()
		{
			if (!this.Force)
			{
				PartitionId partitionId = null;
				if (base.DataObject.TryGetPartitionId(out partitionId))
				{
					ADSessionSettings sessionSettings = ADSessionSettings.SessionSettingsFactory.Default.FromAllTenantsPartitionId(partitionId);
					ITenantConfigurationSession tenantConfigurationSession = DirectorySessionFactory.Default.CreateTenantConfigurationSession(ConsistencyMode.PartiallyConsistent, sessionSettings, 65, "InternalProcessRecord", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\SAFM\\RemoveAccountPartition.cs");
					ExchangeConfigurationUnit[] array = tenantConfigurationSession.Find<ExchangeConfigurationUnit>(null, QueryScope.SubTree, new NotFilter(new ExistsFilter(OrganizationSchema.SharedConfigurationInfo)), null, 1);
					if (array != null && array.Length != 0 && !base.ShouldContinue(Strings.ConfirmationRemoveAccountPartitionWithTenants(this.Identity.ToString())))
					{
						return;
					}
				}
			}
			base.InternalProcessRecord();
		}
	}
}
