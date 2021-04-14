using System;
using System.Security.Principal;

namespace Microsoft.Exchange.Security
{
	public static class SecurityIdentity
	{
		public static SecurityIdentifier GetGroupSecurityIdentifier(Guid mailboxGuid, SecurityIdentity.GroupMailboxMemberType groupMailboxMemberType)
		{
			byte[] array = mailboxGuid.ToByteArray();
			if (array.Length != 16)
			{
				throw new ArgumentException(string.Format("Mailformed mailbox guid {0}", mailboxGuid));
			}
			string sddlForm = string.Concat(new object[]
			{
				"S-1-8-",
				BitConverter.ToUInt32(array, 0),
				"-",
				BitConverter.ToUInt32(array, 4),
				"-",
				BitConverter.ToUInt32(array, 8),
				"-",
				BitConverter.ToUInt32(array, 12),
				"-",
				(int)groupMailboxMemberType
			});
			return new SecurityIdentifier(sddlForm);
		}

		public enum GroupMailboxMemberType
		{
			Owner,
			Member
		}
	}
}
