using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "DatabaseAvailabilityGroup", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
	public sealed class RemoveDatabaseAvailabilityGroup : RemoveSystemConfigurationObjectTask<DatabaseAvailabilityGroupIdParameter, DatabaseAvailabilityGroup>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveDatabaseAvailabilityGroup(this.Identity.ToString());
			}
		}

		private void CheckServerDagRemovalMembership(DatabaseAvailabilityGroup dag)
		{
			int count = dag.Servers.Count;
			if (count > 0)
			{
				base.WriteError(new RemoveDagNeedsZeroServersException(count), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			DatabaseAvailabilityGroup dataObject = base.DataObject;
			DagTaskHelper.VerifyDagIsWithinScopes<DatabaseAvailabilityGroup>(this, dataObject, true);
			this.CheckServerDagRemovalMembership(dataObject);
			TaskLogger.LogExit();
		}

		protected override IConfigurable ResolveDataObject()
		{
			return base.GetDataObject<DatabaseAvailabilityGroup>(this.Identity, base.DataSession, null, new LocalizedString?(Strings.ErrorDagNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorDagNotUnique(this.Identity.ToString())));
		}
	}
}
