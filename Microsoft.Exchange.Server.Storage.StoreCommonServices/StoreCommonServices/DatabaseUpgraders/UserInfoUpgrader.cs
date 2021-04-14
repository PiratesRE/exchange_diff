using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class UserInfoUpgrader : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return UserInfoUpgrader.Instance.TestVersionIsReady(context, database);
		}

		public static void InitializeUpgraderAction(Action<Context> upgraderAction, Action<StoreDatabase> inMemorySchemaInitializationAction)
		{
			UserInfoUpgrader.upgraderAction = upgraderAction;
			UserInfoUpgrader.inMemorySchemaInitializationAction = inMemorySchemaInitializationAction;
		}

		private UserInfoUpgrader() : base(new ComponentVersion(0, 128), new ComponentVersion(0, 129))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			if (UserInfoUpgrader.inMemorySchemaInitializationAction != null)
			{
				UserInfoUpgrader.inMemorySchemaInitializationAction(database);
			}
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
			if (UserInfoUpgrader.upgraderAction != null)
			{
				UserInfoUpgrader.upgraderAction(context);
			}
		}

		public static UserInfoUpgrader Instance = new UserInfoUpgrader();

		private static Action<Context> upgraderAction;

		private static Action<StoreDatabase> inMemorySchemaInitializationAction;
	}
}
