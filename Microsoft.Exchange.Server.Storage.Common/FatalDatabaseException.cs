using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class FatalDatabaseException : Exception
	{
		public FatalDatabaseException(string message) : base(message)
		{
		}

		public FatalDatabaseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
