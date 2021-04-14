using System;

namespace Microsoft.Exchange.Assistants
{
	internal class AmbiguousAliasMailboxException : AIMailboxUnavailableException
	{
		public AmbiguousAliasMailboxException(Exception innerException) : base(Strings.descAmbiguousAliasMailboxException, innerException)
		{
		}
	}
}
