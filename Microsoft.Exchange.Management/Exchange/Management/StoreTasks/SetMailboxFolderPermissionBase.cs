using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;

namespace Microsoft.Exchange.Management.StoreTasks
{
	public abstract class SetMailboxFolderPermissionBase : SetTenantXsoObjectWithFolderIdentityTaskBase<MailboxFolder>
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

		protected RecipientTypeDetails UserRecipientTypeDetails
		{
			get
			{
				if (this.targetUser == null)
				{
					return RecipientTypeDetails.None;
				}
				return this.targetUser.RecipientTypeDetails;
			}
		}

		private protected MailboxFolderUserId MailboxFolderUserId
		{
			protected get
			{
				return this.mailboxFolderUserId;
			}
			private set
			{
				this.ValidateMailboxFolderUserId(value);
				this.mailboxFolderUserId = value;
			}
		}

		protected sealed override IConfigDataProvider CreateSession()
		{
			this.targetUser = this.PrepareMailboxUser();
			base.InnerMailboxFolderDataProvider = new MailboxFolderDataProvider(base.OrgWideSessionSettings, this.targetUser, "SetMailboxFolderPermissionBase");
			this.MailboxFolderUserId = this.User.ResolveMailboxFolderUserId(base.InnerMailboxFolderDataProvider.MailboxSession);
			return base.InnerMailboxFolderDataProvider;
		}

		protected virtual void ValidateMailboxFolderUserId(MailboxFolderUserId mailboxFolderUserId)
		{
		}

		internal Permission GetTargetPermission(PermissionSet permissionSet)
		{
			Permission result = null;
			switch (this.MailboxFolderUserId.UserType)
			{
			case MailboxFolderUserId.MailboxFolderUserType.Default:
				return permissionSet.DefaultPermission;
			case MailboxFolderUserId.MailboxFolderUserType.Anonymous:
				return permissionSet.AnonymousPermission;
			case MailboxFolderUserId.MailboxFolderUserType.Internal:
			case MailboxFolderUserId.MailboxFolderUserType.External:
			{
				PermissionSecurityPrincipal securityPrincipal = this.MailboxFolderUserId.ToSecurityPrincipal();
				return permissionSet.GetEntry(securityPrincipal);
			}
			}
			foreach (Permission permission in permissionSet)
			{
				if (this.MailboxFolderUserId.Equals(permission.Principal))
				{
					result = permission;
					break;
				}
			}
			return result;
		}

		internal abstract bool InternalProcessPermissions(Folder folder);

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			MailboxFolder dataObject = this.DataObject;
			MailboxSession mailboxSession = base.InnerMailboxFolderDataProvider.MailboxSession;
			VersionedId internalFolderIdentity = dataObject.InternalFolderIdentity;
			using (Folder folder = Folder.Bind(mailboxSession, internalFolderIdentity))
			{
				if (this.InternalProcessPermissions(folder))
				{
					folder.Save();
				}
			}
			TaskLogger.LogExit();
		}

		private ADUser targetUser;

		private MailboxFolderUserId mailboxFolderUserId;
	}
}
