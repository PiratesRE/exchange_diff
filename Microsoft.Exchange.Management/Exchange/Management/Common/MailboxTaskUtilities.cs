using System;
using System.Security;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Extensions;

namespace Microsoft.Exchange.Management.Common
{
	public static class MailboxTaskUtilities
	{
		public static SecureString GetRandomPassword(string name, string samAccountName)
		{
			return MailboxTaskUtilities.GetRandomPassword(name, samAccountName, 128);
		}

		public static SecureString GetRandomPassword(string name, string samAccountName, int length)
		{
			string randomPassword = PasswordHelper.GetRandomPassword(name, samAccountName, length);
			return randomPassword.ConvertToSecureString();
		}
	}
}
