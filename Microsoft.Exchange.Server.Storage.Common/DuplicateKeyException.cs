using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class DuplicateKeyException : NonFatalDatabaseException
	{
		public DuplicateKeyException(string message) : base(message)
		{
		}

		public DuplicateKeyException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
