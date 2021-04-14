using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Add", "MailboxFolderPermission", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class AddMailboxFolderPermission : SetMailboxFolderPermissionBase
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

		protected virtual ObjectId ResolvedObjectId
		{
			get
			{
				return this.Identity.InternalMailboxFolderId;
			}
		}

		protected virtual bool IsPublicFolderIdentity
		{
			get
			{
				return false;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddMailboxFolderPermission(this.Identity.ToString(), base.User.ToString(), base.FormatMultiValuedProperty(this.AccessRights));
			}
		}

		protected override void ValidateMailboxFolderUserId(MailboxFolderUserId mailboxFolderUserId)
		{
			base.ValidateMailboxFolderUserId(mailboxFolderUserId);
			if (mailboxFolderUserId.UserType == MailboxFolderUserId.MailboxFolderUserType.Internal && !ADRecipient.IsValidRecipient(mailboxFolderUserId.ADRecipient, !this.IsPublicFolderIdentity))
			{
				throw new InvalidInternalUserIdException(base.User.ToString());
			}
			if (mailboxFolderUserId.UserType == MailboxFolderUserId.MailboxFolderUserType.Unknown)
			{
				throw new InvalidExternalUserIdException(base.User.ToString());
			}
		}

		internal override bool InternalProcessPermissions(Folder folder)
		{
			MemberRights memberRights = (MemberRights)MailboxFolderAccessRight.CalculateMemberRights(this.AccessRights, folder.ClassName == "IPF.Appointment");
			PermissionSet permissionSet = folder.GetPermissionSet();
			Permission permission = null;
			try
			{
				if (base.MailboxFolderUserId.UserType == MailboxFolderUserId.MailboxFolderUserType.Default)
				{
					if (permissionSet.DefaultPermission != null && permissionSet.DefaultPermission.MemberRights != MemberRights.None)
					{
						throw new UserAlreadyExistsInPermissionEntryException(base.MailboxFolderUserId.ToString());
					}
					permissionSet.SetDefaultPermission(memberRights);
					permission = permissionSet.DefaultPermission;
				}
				else if (base.MailboxFolderUserId.UserType == MailboxFolderUserId.MailboxFolderUserType.Anonymous)
				{
					if (permissionSet.AnonymousPermission != null && permissionSet.AnonymousPermission.MemberRights != MemberRights.None)
					{
						throw new UserAlreadyExistsInPermissionEntryException(base.MailboxFolderUserId.ToString());
					}
					permissionSet.SetAnonymousPermission(memberRights);
					permission = permissionSet.AnonymousPermission;
				}
				else
				{
					PermissionSecurityPrincipal securityPrincipal = base.MailboxFolderUserId.ToSecurityPrincipal();
					Permission entry = permissionSet.GetEntry(securityPrincipal);
					if (entry != null)
					{
						throw new UserAlreadyExistsInPermissionEntryException(base.MailboxFolderUserId.ToString());
					}
					permission = permissionSet.AddEntry(securityPrincipal, memberRights);
				}
			}
			catch (ArgumentOutOfRangeException exception)
			{
				base.WriteError(exception, (ErrorCategory)1003, this.Identity);
				return false;
			}
			base.WriteObject(MailboxFolderPermission.FromXsoPermission(folder.DisplayName, permission, this.ResolvedObjectId));
			return true;
		}
	}
}
