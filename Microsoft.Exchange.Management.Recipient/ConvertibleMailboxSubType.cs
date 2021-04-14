using System;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public enum ConvertibleMailboxSubType : long
	{
		Regular = 1L,
		Room = 16L,
		Equipment = 32L,
		Shared = 4L
	}
}
