using System;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.StoreIntegrityCheck
{
	public static class Globals
	{
		public static void Initialize()
		{
			InMemoryJobStorage.Initialize();
			JobScheduler.Initialize();
		}

		public static void Terminate()
		{
		}

		public static void DatabaseMounting(Context context, StoreDatabase database, bool readOnly)
		{
			InMemoryJobStorage.MountEventHandler(context, database);
			JobScheduler.MountEventHandler(context, database, readOnly);
		}

		public static void DatabaseMounted(Context context, StoreDatabase database)
		{
		}

		public static void DatabaseDismounting(Context context, StoreDatabase database)
		{
			InMemoryJobStorage.DismountEventHandler(database);
			JobScheduler.DismountEventHandler(database);
		}
	}
}
