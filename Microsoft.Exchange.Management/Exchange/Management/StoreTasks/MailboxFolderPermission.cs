using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.StoreTasks
{
	[Serializable]
	public sealed class MailboxFolderPermission : ConfigurableObject
	{
		internal override ObjectSchema ObjectSchema
		{
			get
			{
				return MailboxFolderPermission.schema;
			}
		}

		public new ObjectId Identity
		{
			get
			{
				return (ObjectId)this[MailboxFolderPermissionSchema.Identity];
			}
			private set
			{
				this[MailboxFolderPermissionSchema.Identity] = value;
			}
		}

		public string FolderName
		{
			get
			{
				return (string)this[MailboxFolderPermissionSchema.FolderName];
			}
			private set
			{
				this[MailboxFolderPermissionSchema.FolderName] = value;
			}
		}

		public MailboxFolderUserId User
		{
			get
			{
				MailboxFolderUserId mailboxFolderUserId = (MailboxFolderUserId)this[MailboxFolderPermissionSchema.User];
				if (SuppressingPiiContext.NeedPiiSuppression)
				{
					switch (mailboxFolderUserId.UserType)
					{
					case MailboxFolderUserId.MailboxFolderUserType.Internal:
					case MailboxFolderUserId.MailboxFolderUserType.External:
					case MailboxFolderUserId.MailboxFolderUserType.Unknown:
						mailboxFolderUserId = null;
						break;
					}
				}
				return mailboxFolderUserId;
			}
			private set
			{
				this[MailboxFolderPermissionSchema.User] = value;
			}
		}

		public Collection<MailboxFolderAccessRight> AccessRights
		{
			get
			{
				return (Collection<MailboxFolderAccessRight>)this[MailboxFolderPermissionSchema.AccessRights];
			}
			set
			{
				MailboxFolderPermission.ValidateAccessRights(value);
				this[MailboxFolderPermissionSchema.AccessRights] = value;
			}
		}

		public MailboxFolderPermission() : base(new SimplePropertyBag(MailboxFolderPermissionSchema.Identity, MailboxFolderPermissionSchema.ObjectState, MailboxFolderPermissionSchema.ExchangeVersion))
		{
		}

		internal override ExchangeObjectVersion MaximumSupportedExchangeObjectVersion
		{
			get
			{
				return ExchangeObjectVersion.Exchange2010;
			}
		}

		internal static void ValidateAccessRights(IEnumerable<MailboxFolderAccessRight> accessRights)
		{
			if (accessRights != null)
			{
				bool flag = false;
				bool flag2 = false;
				foreach (MailboxFolderAccessRight mailboxFolderAccessRight in accessRights)
				{
					if (mailboxFolderAccessRight.IsRole)
					{
						if (flag2)
						{
							throw new ArgumentException(Strings.ErrorPrecannedRoleAndSpecificMailboxFolderPermission);
						}
						flag = true;
					}
					else
					{
						if (flag)
						{
							throw new ArgumentException(Strings.ErrorPrecannedRoleAndSpecificMailboxFolderPermission);
						}
						flag2 = true;
					}
				}
			}
		}

		internal static MailboxFolderPermission FromXsoPermission(string folderName, Permission permission, ObjectId mailboxFolderId)
		{
			Collection<MailboxFolderAccessRight> accessRights = MailboxFolderAccessRight.CreateMailboxFolderAccessRightCollection((int)permission.MemberRights);
			return new MailboxFolderPermission
			{
				FolderName = folderName,
				Identity = mailboxFolderId,
				User = MailboxFolderUserId.CreateFromSecurityPrincipal(permission.Principal),
				AccessRights = accessRights
			};
		}

		private static MailboxFolderPermissionSchema schema = ObjectSchema.GetInstance<MailboxFolderPermissionSchema>();
	}
}
