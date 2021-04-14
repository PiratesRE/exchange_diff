using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.StoreTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class PublicFolderPermissionInfo
	{
		public PublicFolderPermissionInfo(MailboxFolderPermission permissionEntry)
		{
			if (permissionEntry.User.ADRecipient == null)
			{
				this.User = new Identity(permissionEntry.User.ToString());
				this.Sid = "Unknown";
			}
			else
			{
				this.User = permissionEntry.User.ADRecipient.ToIdentity();
				SecurityIdentifier securityIdentifier = (SecurityIdentifier)permissionEntry.User.ADRecipient.propertyBag[IADSecurityPrincipalSchema.Sid];
				this.Sid = ((securityIdentifier == null) ? "Unknown" : securityIdentifier.Value);
			}
			List<string> list = new List<string>();
			if (permissionEntry.AccessRights.Count == 1 && permissionEntry.AccessRights[0].IsRole)
			{
				using (IEnumerator enumerator = Enum.GetValues(typeof(PublicFolderPermission)).GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						PublicFolderPermission publicFolderPermission = (PublicFolderPermission)obj;
						if (PublicFolderPermissionInfo.ContainsRight((PublicFolderPermission)permissionEntry.AccessRights[0].Permission, publicFolderPermission))
						{
							List<string> list2 = list;
							int num = (int)publicFolderPermission;
							list2.Add(num.ToString());
						}
					}
					goto IL_16C;
				}
			}
			foreach (MailboxFolderAccessRight mailboxFolderAccessRight in permissionEntry.AccessRights)
			{
				list.Add(mailboxFolderAccessRight.Permission.ToString());
			}
			IL_16C:
			this.AccessRights = list.ToArray();
		}

		[DataMember]
		public Identity User { get; set; }

		[DataMember]
		public string Sid { get; set; }

		[DataMember]
		public string[] AccessRights { get; set; }

		public static explicit operator PublicFolderPermissionInfo(MailboxFolderPermission folderPermission)
		{
			if (folderPermission != null)
			{
				return new PublicFolderPermissionInfo(folderPermission);
			}
			return null;
		}

		public static bool ContainsRight(PublicFolderPermission accessRights, PublicFolderPermission accessRight)
		{
			return (accessRights & accessRight) == accessRight;
		}
	}
}
