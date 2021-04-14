using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class AddMidsetDeletedDelta : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return AddMidsetDeletedDelta.Instance.TestVersionIsReady(context, database);
		}

		public static void InitializeUpgraderAction(Action<Context> upgraderAction, Action<StoreDatabase> inMemorySchemaInitializationAction)
		{
			AddMidsetDeletedDelta.upgraderAction = upgraderAction;
			AddMidsetDeletedDelta.inMemorySchemaInitializationAction = inMemorySchemaInitializationAction;
		}

		private AddMidsetDeletedDelta() : base(new ComponentVersion(0, 124), new ComponentVersion(0, 125))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			if (AddMidsetDeletedDelta.inMemorySchemaInitializationAction != null)
			{
				AddMidsetDeletedDelta.inMemorySchemaInitializationAction(database);
			}
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
			if (AddMidsetDeletedDelta.upgraderAction != null)
			{
				AddMidsetDeletedDelta.upgraderAction(context);
			}
		}

		public static AddMidsetDeletedDelta Instance = new AddMidsetDeletedDelta();

		private static Action<Context> upgraderAction;

		private static Action<StoreDatabase> inMemorySchemaInitializationAction;
	}
}
