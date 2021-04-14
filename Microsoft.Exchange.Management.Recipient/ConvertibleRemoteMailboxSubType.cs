using System;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	public enum ConvertibleRemoteMailboxSubType : long
	{
		Regular = 2147483648L,
		Room = 8589934592L,
		Equipment = 17179869184L,
		Shared = 34359738368L
	}
}
