using System;

namespace Microsoft.Exchange.Clients.Security
{
	internal enum AccountLookupFailureReason
	{
		ADAccountNotFound,
		AccountDisabled,
		SharedMailboxAccountDisabled,
		NonUniqueRecipientException,
		CannotResolvePartitionException,
		CannotResolveTenantNameException,
		DataSourceOperationException,
		EmptySid,
		OrganizationNotFound,
		MailboxRecentlyCreated,
		MailboxSoftDeleted
	}
}
