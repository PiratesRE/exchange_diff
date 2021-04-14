using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class AsyncMessageCleanup : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return AsyncMessageCleanup.Instance.TestVersionIsReady(context, database);
		}

		private AsyncMessageCleanup() : base(new ComponentVersion(0, 123), new ComponentVersion(0, 124))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
		}

		public static AsyncMessageCleanup Instance = new AsyncMessageCleanup();
	}
}
