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
	[Cmdlet("Set", "EdgeSyncServiceConfig", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetEdgeSyncServiceConfig : SetTopologySystemConfigurationObjectTask<EdgeSyncServiceConfigIdParameter, EdgeSyncServiceConfig>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetEdgeSyncServiceConfig(this.Identity.ToString());
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 44, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\transport\\EdgeSync\\SetEdgeSyncServiceConfig.cs");
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			ADObjectId siteId = this.DataObject.Id.AncestorDN(1);
			if (!EdgeSyncServiceConfig.ValidLogSizeCompatibility(this.DataObject.LogMaxFileSize, this.DataObject.LogMaxDirectorySize, siteId, (ITopologyConfigurationSession)base.DataSession))
			{
				base.WriteError(new InvalidOperationException(), ErrorCategory.InvalidOperation, this.DataObject);
			}
		}
	}
}
