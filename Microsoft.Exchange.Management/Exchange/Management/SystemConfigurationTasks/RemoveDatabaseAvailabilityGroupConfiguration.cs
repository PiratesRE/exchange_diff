using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "DatabaseAvailabilityGroupConfiguration", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
	public sealed class RemoveDatabaseAvailabilityGroupConfiguration : RemoveSystemConfigurationObjectTask<DatabaseAvailabilityGroupConfigurationIdParameter, DatabaseAvailabilityGroupConfiguration>
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveDatabaseAvailabilityGroupConfiguration(this.Identity.ToString());
			}
		}

		private void CheckDagConfigurationRemovalMembership(DatabaseAvailabilityGroupConfiguration dagConfig)
		{
			int count = dagConfig.Dags.Count;
			if (count > 0)
			{
				base.WriteError(new RemoveDagConfigurationNeedsZeroDagsException(count), ErrorCategory.InvalidArgument, null);
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			DatabaseAvailabilityGroupConfiguration dataObject = base.DataObject;
			this.CheckDagConfigurationRemovalMembership(dataObject);
			TaskLogger.LogExit();
		}

		protected override IConfigurable ResolveDataObject()
		{
			return base.GetDataObject<DatabaseAvailabilityGroupConfiguration>(this.Identity, base.DataSession, null, new LocalizedString?(Strings.ErrorDagNotFound(this.Identity.ToString())), new LocalizedString?(Strings.ErrorDagNotUnique(this.Identity.ToString())));
		}
	}
}
