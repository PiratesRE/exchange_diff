using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public class DatabaseSchemaBroken : Exception
	{
		public DatabaseSchemaBroken(string mdbName, string errorDetails) : base(string.Format("Database {0} has a broken schema. Error details are {1}", mdbName, errorDetails))
		{
		}

		public DatabaseSchemaBroken(string mdbName, string errorDetails, Exception innerException) : base(string.Format("Database {0} has a broken schema. Error details are {1}", mdbName, errorDetails), innerException)
		{
		}

		private const string DatabaseSchemaBrokenMessage = "Database {0} has a broken schema. Error details are {1}";
	}
}
