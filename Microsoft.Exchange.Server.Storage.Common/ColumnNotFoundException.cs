using System;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class ColumnNotFoundException : NonFatalDatabaseException
	{
		public ColumnNotFoundException(string message) : base(message)
		{
		}

		public ColumnNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
