using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class CouldNotCreateDatabaseException : Exception
	{
		public CouldNotCreateDatabaseException(string message) : base(message)
		{
		}

		public CouldNotCreateDatabaseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
