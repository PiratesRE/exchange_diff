using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RbacTasks
{
	[Cmdlet("Remove", "ManagementRoleEntry", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveManagementRoleEntry : AddRemoveManagementRoleEntryActionBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveManagementRoleEntry(this.removedEntry.ToString(), this.DataObject.Id.ToString());
			}
		}

		protected override bool IsTopLevelUnscopedRoleModificationAllowed()
		{
			return true;
		}

		protected override void InternalApplyChangeAndValidate()
		{
			TaskLogger.LogEnter();
			this.removedEntry = RoleHelper.GetMandatoryRoleEntry(this.DataObject, this.Identity.CmdletOrScriptName, this.Identity.PSSnapinName, new Task.TaskErrorLoggingDelegate(base.WriteError));
			this.InternalAddRemoveRoleEntry(this.DataObject.RoleEntries);
			TaskLogger.LogExit();
		}

		protected override void InternalAddRemoveRoleEntry(MultiValuedProperty<RoleEntry> roleEntries)
		{
			roleEntries.Remove(this.removedEntry);
		}

		protected override string GetRoleEntryString()
		{
			return this.removedEntry.ToString();
		}

		protected override LocalizedException GetRoleSaveException(string roleEntry, string role, string exception)
		{
			return new ExRBACSaveRemoveRoleEntry(roleEntry, role, exception);
		}

		private RoleEntry removedEntry;
	}
}
