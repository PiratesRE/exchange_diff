using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Cmdlet("Get", "MailboxFolderPermission", DefaultParameterSetName = "Identity")]
	public class GetMailboxFolderPermission : GetTenantXsoObjectWithFolderIdentityTaskBase<MailboxFolder>
	{
		[Parameter(Mandatory = false)]
		public MailboxFolderUserIdParameter User
		{
			get
			{
				return (MailboxFolderUserIdParameter)base.Fields["User"];
			}
			set
			{
				base.Fields["User"] = value;
			}
		}

		protected virtual ObjectId ResolvedObjectId
		{
			get
			{
				return this.Identity.InternalMailboxFolderId;
			}
		}

		protected sealed override IConfigDataProvider CreateSession()
		{
			ADUser mailboxOwner = this.PrepareMailboxUser();
			base.InnerMailboxFolderDataProvider = new MailboxFolderDataProvider(base.OrgWideSessionSettings, mailboxOwner, "Get-MailboxFolderPermission");
			if (this.User != null)
			{
				this.mailboxUserId = this.User.ResolveMailboxFolderUserId(base.InnerMailboxFolderDataProvider.MailboxSession);
			}
			return base.InnerMailboxFolderDataProvider;
		}

		protected override ObjectId RootId
		{
			get
			{
				return null;
			}
		}

		protected override bool DeepSearch
		{
			get
			{
				return false;
			}
		}

		protected override void WriteResult(IConfigurable dataObject)
		{
			TaskLogger.LogEnter(new object[]
			{
				dataObject.Identity,
				dataObject
			});
			MailboxFolder mailboxFolder = (MailboxFolder)dataObject;
			MailboxSession mailboxSession = base.InnerMailboxFolderDataProvider.MailboxSession;
			VersionedId internalFolderIdentity = mailboxFolder.InternalFolderIdentity;
			int num = 0;
			using (Folder folder = Folder.Bind(mailboxSession, internalFolderIdentity))
			{
				PermissionSet permissionSet = folder.GetPermissionSet();
				if (permissionSet.DefaultPermission != null && (this.mailboxUserId == null || this.mailboxUserId.Equals(permissionSet.DefaultPermission.Principal)))
				{
					base.WriteResult(MailboxFolderPermission.FromXsoPermission(folder.DisplayName, permissionSet.DefaultPermission, this.ResolvedObjectId));
					num++;
				}
				if (permissionSet.AnonymousPermission != null && (this.mailboxUserId == null || this.mailboxUserId.Equals(permissionSet.AnonymousPermission.Principal)))
				{
					base.WriteResult(MailboxFolderPermission.FromXsoPermission(folder.DisplayName, permissionSet.AnonymousPermission, this.ResolvedObjectId));
					num++;
				}
				foreach (Permission permission in permissionSet)
				{
					if (this.mailboxUserId == null || this.mailboxUserId.Equals(permission.Principal))
					{
						base.WriteResult(MailboxFolderPermission.FromXsoPermission(folder.DisplayName, permission, this.ResolvedObjectId));
						num++;
					}
				}
			}
			if (this.mailboxUserId != null && num == 0)
			{
				throw new UserNotFoundInPermissionEntryException(this.mailboxUserId.ToString());
			}
			TaskLogger.LogExit();
		}

		private MailboxFolderUserId mailboxUserId;
	}
}
