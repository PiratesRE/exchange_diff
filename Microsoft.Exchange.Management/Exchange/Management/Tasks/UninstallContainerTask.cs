using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("uninstall", "Container", SupportsShouldProcess = true)]
	public sealed class UninstallContainerTask : RemoveSystemConfigurationObjectTask<ContainerIdParameter, Container>
	{
		[Parameter]
		public SwitchParameter Recursive
		{
			get
			{
				return (SwitchParameter)(base.Fields["Recursive"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Recursive"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			PartitionId partitionId = (this.Identity.InternalADObjectId != null) ? this.Identity.InternalADObjectId.GetPartitionId() : PartitionId.LocalForest;
			ADSessionSettings sessionSettings = ADSessionSettings.FromAccountPartitionWideScopeSet(partitionId);
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession((base.ServerSettings == null) ? null : base.ServerSettings.PreferredGlobalCatalog(partitionId.ForestFQDN), false, ConsistencyMode.PartiallyConsistent, sessionSettings, 49, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\UninstallContainerTask.cs");
		}

		protected override void InternalValidate()
		{
			try
			{
				base.InternalValidate();
			}
			catch (ManagementObjectNotFoundException ex)
			{
				base.WriteWarning(ex.Message);
				this.validationFailed = true;
			}
		}

		protected override void InternalProcessRecord()
		{
			if (!this.validationFailed)
			{
				if (!this.Recursive)
				{
					base.InternalProcessRecord();
					return;
				}
				base.WriteWarning("Recursive");
				((IConfigurationSession)base.DataSession).DeleteTree(base.DataObject, delegate(ADTreeDeleteNotFinishedException de)
				{
					if (de != null)
					{
						base.WriteVerbose(de.LocalizedString);
					}
				});
			}
		}

		private bool validationFailed;
	}
}
