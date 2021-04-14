using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.StoreTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class MailboxFolderPermissionRow : BaseRow
	{
		public MailboxFolderPermissionRow(MailboxFolderPermission permission) : base(new MailboxFolderPermissionIdentity(permission), permission)
		{
			this.MailboxFolderPermission = permission;
		}

		public MailboxFolderPermission MailboxFolderPermission { get; private set; }

		public Identity MailboxFolderId
		{
			get
			{
				return ((MailboxFolderPermissionIdentity)base.Identity).MailboxFolderId;
			}
			set
			{
				((MailboxFolderPermissionIdentity)base.Identity).MailboxFolderId = value;
			}
		}

		[DataMember]
		public string User
		{
			get
			{
				return base.Identity.DisplayName;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public bool IsSupported
		{
			get
			{
				if (this.MailboxFolderPermission.AccessRights.Count != 1)
				{
					return false;
				}
				MailboxFolderAccessRight[] array = this.MailboxFolderPermission.AccessRights.ToArray<MailboxFolderAccessRight>();
				int i = 0;
				while (i < array.Length)
				{
					MailboxFolderAccessRight mailboxFolderAccessRight = array[i];
					bool result;
					if (!mailboxFolderAccessRight.IsRole)
					{
						result = false;
					}
					else
					{
						MailboxFolderPermissionRole permission = (MailboxFolderPermissionRole)mailboxFolderAccessRight.Permission;
						if (permission == MailboxFolderPermissionRole.Reviewer || permission == MailboxFolderPermissionRole.AvailabilityOnly || permission == MailboxFolderPermissionRole.LimitedDetails)
						{
							i++;
							continue;
						}
						result = false;
					}
					return result;
				}
				return true;
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string AccessRights
		{
			get
			{
				string[] value = Array.ConvertAll<MailboxFolderAccessRight, string>(this.MailboxFolderPermission.AccessRights.ToArray<MailboxFolderAccessRight>(), delegate(MailboxFolderAccessRight x)
				{
					if (!x.IsRole)
					{
						return (x.Permission == 0) ? OwaOptionStrings.NoneAccessRightRole : OwaOptionStrings.CustomAccessRightRole;
					}
					return LocalizedDescriptionAttribute.FromEnumForOwaOption(typeof(MailboxFolderPermissionRole), x.Permission);
				});
				return string.Join(",", value);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		public bool IsAnonymousOrDefault
		{
			get
			{
				switch (this.MailboxFolderPermission.User.UserType)
				{
				case MailboxFolderUserId.MailboxFolderUserType.Default:
				case MailboxFolderUserId.MailboxFolderUserType.Anonymous:
					return true;
				default:
					return false;
				}
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
