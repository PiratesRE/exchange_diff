using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OofHistoryCorruptionException : Exception
	{
		internal OofHistoryCorruptionException(string message) : base(message)
		{
		}
	}
}
