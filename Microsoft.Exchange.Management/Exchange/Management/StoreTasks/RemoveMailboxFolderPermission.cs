using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Remove", "MailboxFolderPermission", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public class RemoveMailboxFolderPermission : SetMailboxFolderPermissionBase
	{
		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveMailboxFolderPermission(this.Identity.ToString(), base.User.ToString());
			}
		}

		protected override void ValidateMailboxFolderUserId(MailboxFolderUserId mailboxFolderUserId)
		{
			base.ValidateMailboxFolderUserId(mailboxFolderUserId);
			if ((mailboxFolderUserId.UserType == MailboxFolderUserId.MailboxFolderUserType.Default || mailboxFolderUserId.UserType == MailboxFolderUserId.MailboxFolderUserType.Anonymous) && base.UserRecipientTypeDetails != RecipientTypeDetails.PublicFolderMailbox)
			{
				throw new CannotRemoveSpecialUserException();
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
			if (base.MailboxFolderUserId.UserType == MailboxFolderUserId.MailboxFolderUserType.Default)
			{
				permissionSet.SetDefaultPermission(MemberRights.None);
			}
			else if (base.MailboxFolderUserId.UserType == MailboxFolderUserId.MailboxFolderUserType.Anonymous)
			{
				permissionSet.SetAnonymousPermission(MemberRights.None);
			}
			else
			{
				permissionSet.RemoveEntry(targetPermission.Principal);
			}
			return true;
		}
	}
}
