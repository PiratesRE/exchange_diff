using System;

namespace Microsoft.Exchange.Management.SystemManager.WinForms
{
	public enum UserRange : short
	{
		AllUsersWithoutMailAttributes = 3,
		AccountDisabledUsers = 1,
		AccountEnabledUsers,
		AllUsersWithMailAttributes = 192,
		MailEnabledUsers = 128,
		MailboxUsers = 64
	}
}
