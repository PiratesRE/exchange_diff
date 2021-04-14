using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class DuplicateColumnException : NonFatalDatabaseException
	{
		public DuplicateColumnException(string message) : base(message)
		{
		}

		public DuplicateColumnException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
