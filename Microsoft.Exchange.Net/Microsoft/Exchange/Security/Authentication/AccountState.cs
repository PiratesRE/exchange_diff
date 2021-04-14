using System;

namespace Microsoft.Exchange.Security.Authentication
{
	public enum AccountState
	{
		AccountEnabled,
		AccountDisabled,
		PasswordExpired,
		AccountDeleted,
		MailboxDisabled,
		ProtocolDisabled
	}
}
