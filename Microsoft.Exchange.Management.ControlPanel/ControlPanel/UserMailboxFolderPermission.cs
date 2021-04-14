using System;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Management.StoreTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UserMailboxFolderPermission : MailboxFolderPermissionRow
	{
		public UserMailboxFolderPermission(MailboxFolderPermission permission) : base(permission)
		{
		}

		[DataMember]
		public string ChangePermissionsForUser
		{
			get
			{
				return string.Format(OwaOptionStrings.ChangePermissions, base.User);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}

		[DataMember]
		public string ReadAccessRights
		{
			get
			{
				if (!base.IsSupported)
				{
					return null;
				}
				return Enum.GetName(typeof(MailboxFolderPermissionRole), base.MailboxFolderPermission.AccessRights.First<MailboxFolderAccessRight>().Permission);
			}
			private set
			{
				throw new NotSupportedException();
			}
		}
	}
}
