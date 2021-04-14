using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class EnableAddingSpecialFolders : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return EnableAddingSpecialFolders.Instance.TestVersionIsReady(context, database);
		}

		private EnableAddingSpecialFolders() : base(new ComponentVersion(0, 130), new ComponentVersion(0, 131))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
		}

		public static EnableAddingSpecialFolders Instance = new EnableAddingSpecialFolders();
	}
}
