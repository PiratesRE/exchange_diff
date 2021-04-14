using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class RemoveFolderIdsetIn : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return RemoveFolderIdsetIn.Instance.TestVersionIsReady(context, database);
		}

		private RemoveFolderIdsetIn() : base(new ComponentVersion(0, 127), new ComponentVersion(0, 128))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
		}

		public static RemoveFolderIdsetIn Instance = new RemoveFolderIdsetIn();
	}
}
