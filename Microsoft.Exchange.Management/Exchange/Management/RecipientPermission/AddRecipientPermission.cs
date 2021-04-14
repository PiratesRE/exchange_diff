using System;
using System.DirectoryServices;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.RecipientPermission
{
	[Cmdlet("Add", "RecipientPermission", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class AddRecipientPermission : SetRecipientPermissionTaskBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddRecipientPermission(this.Identity.ToString(), base.Trustee.ToString(), base.FormatMultiValuedProperty(base.AccessRights));
			}
		}

		protected override ActiveDirectorySecurityInheritance GetInheritanceType()
		{
			return ActiveDirectorySecurityInheritance.None;
		}

		protected override void ApplyModification(ActiveDirectoryAccessRule[] modifiedAces)
		{
			TaskLogger.LogEnter();
			DirectoryCommon.SetAces(new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), this.DataObject, modifiedAces);
			base.WriteResults(modifiedAces);
			TaskLogger.LogExit();
		}
	}
}
