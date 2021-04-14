using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.EdgeSync
{
	[Cmdlet("Set", "EdgeSyncMservConnector", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetEdgeSyncMservConnector : SetTopologySystemConfigurationObjectTask<EdgeSyncMservConnectorIdParameter, EdgeSyncMservConnector>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetEdgeSyncMservConnector(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 45, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\SetEdgeSyncMservConnector.cs");
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (this.DataObject.PrimaryLeaseLocation != null && !Utils.IsLeaseDirectoryValidPath(this.DataObject.PrimaryLeaseLocation))
			{
				base.WriteError(new InvalidOperationException(Strings.InvalidPrimaryLeaseLocation), ErrorCategory.InvalidOperation, this.DataObject);
			}
			if (this.DataObject.BackupLeaseLocation != null && !Utils.IsLeaseDirectoryValidPath(this.DataObject.BackupLeaseLocation))
			{
				base.WriteError(new InvalidOperationException(Strings.InvalidBackupLeaseLocation), ErrorCategory.InvalidOperation, this.DataObject);
			}
		}
	}
}
