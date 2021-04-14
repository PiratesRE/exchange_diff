using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "StampGroup", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
	public sealed class RemoveStampGroup : RemoveSystemConfigurationObjectTask<StampGroupIdParameter, StampGroup>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveDatabaseAvailabilityGroup(this.Identity.ToString());
			}
		}

		private void CheckServerDagRemovalMembership(StampGroup stampGroup)
		{
			int count = stampGroup.Servers.Count;
			if (count > 0)
			{
				base.WriteError(new RemoveDagNeedsZeroServersException(count), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			StampGroup dataObject = base.DataObject;
			this.CheckServerDagRemovalMembership(dataObject);
			TaskLogger.LogExit();
		}

		protected override IConfigurable ResolveDataObject()
		{
			return base.GetDataObject<StampGroup>(this.Identity, base.DataSession, null, new LocalizedString?(Strings.ErrorDagNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorDagNotUnique(this.Identity.ToString())));
		}
	}
}
