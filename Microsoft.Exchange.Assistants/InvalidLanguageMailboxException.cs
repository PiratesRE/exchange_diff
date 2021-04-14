using System;

namespace Microsoft.Exchange.Assistants
{
	internal class InvalidLanguageMailboxException : AIMailboxUnavailableException
	{
		public InvalidLanguageMailboxException(Exception innerException) : base(Strings.descInvalidLanguageMailboxException, innerException)
		{
		}
	}
}
