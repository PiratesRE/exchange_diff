using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Set", "MailboxFolderPermission", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetMailboxFolderPermission : SetMailboxFolderPermissionBase
	{
		[Parameter(Mandatory = true)]
		public MailboxFolderAccessRight[] AccessRights
		{
			get
			{
				return (MailboxFolderAccessRight[])base.Fields["AccessRights"];
			}
			set
			{
				MailboxFolderPermission.ValidateAccessRights(value);
				base.Fields["AccessRights"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetMailboxFolderPermission(this.Identity.ToString(), base.User.ToString(), base.FormatMultiValuedProperty(this.AccessRights));
			}
		}

		internal override bool InternalProcessPermissions(Folder folder)
		{
			PermissionSet permissionSet = folder.GetPermissionSet();
			Permission targetPermission = base.GetTargetPermission(permissionSet);
			if (targetPermission == null)
			{
				throw new UserNotFoundInPermissionEntryException(base.MailboxFolderUserId.ToString());
			}
			MemberRights memberRights = (MemberRights)MailboxFolderAccessRight.CalculateMemberRights(this.AccessRights, folder.ClassName == "IPF.Appointment");
			if (targetPermission.MemberRights == memberRights)
			{
				this.WriteWarning(Strings.WarningMailboxFolderPermissionUnchanged(this.DataObject.Identity.ToString()));
				return false;
			}
			try
			{
				targetPermission.MemberRights = memberRights;
			}
			catch (ArgumentOutOfRangeException exception)
			{
				base.WriteError(exception, (ErrorCategory)1003, this.Identity);
			}
			return true;
		}
	}
}
