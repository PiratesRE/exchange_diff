using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class FixReceiveFolderPK : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return FixReceiveFolderPK.Instance.TestVersionIsReady(context, database);
		}

		public static void InitializeUpgraderAction(Action<Context, StoreDatabase> upgraderAction, Action<Context, StoreDatabase> inMemorySchemaInitializationAction)
		{
			FixReceiveFolderPK.upgraderAction = upgraderAction;
			FixReceiveFolderPK.inMemorySchemaInitializationAction = inMemorySchemaInitializationAction;
		}

		private FixReceiveFolderPK() : base(new ComponentVersion(0, 10000), new ComponentVersion(0, 10001))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			if (FixReceiveFolderPK.inMemorySchemaInitializationAction != null)
			{
				FixReceiveFolderPK.inMemorySchemaInitializationAction(context, database);
			}
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
			if (FixReceiveFolderPK.upgraderAction != null)
			{
				FixReceiveFolderPK.upgraderAction(context, (StoreDatabase)container);
			}
		}

		public static FixReceiveFolderPK Instance = new FixReceiveFolderPK();

		private static Action<Context, StoreDatabase> upgraderAction;

		private static Action<Context, StoreDatabase> inMemorySchemaInitializationAction;
	}
}
