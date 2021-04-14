using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	public static class Globals
	{
		public static void Initialize(DatabaseType databaseTypeToUse, Factory.JetHADatabaseCreator haCreator)
		{
			Connection.Initialize();
			Factory.Initialize(databaseTypeToUse, haCreator);
		}

		public static void Terminate()
		{
		}
	}
}
