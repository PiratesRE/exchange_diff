using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class TombstoneTableDiagnostics : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return TombstoneTableDiagnostics.Instance.TestVersionIsReady(context, database);
		}

		public static void InitializeUpgraderAction(Action<Context> upgraderAction, Action<StoreDatabase> inMemorySchemaInitializationAction)
		{
			TombstoneTableDiagnostics.upgraderAction = upgraderAction;
			TombstoneTableDiagnostics.inMemorySchemaInitializationAction = inMemorySchemaInitializationAction;
		}

		private TombstoneTableDiagnostics() : base(new ComponentVersion(0, 10000), new ComponentVersion(0, 10001))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			if (TombstoneTableDiagnostics.inMemorySchemaInitializationAction != null)
			{
				TombstoneTableDiagnostics.inMemorySchemaInitializationAction(database);
			}
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
			if (TombstoneTableDiagnostics.upgraderAction != null)
			{
				TombstoneTableDiagnostics.upgraderAction(context);
			}
		}

		public static TombstoneTableDiagnostics Instance = new TombstoneTableDiagnostics();

		private static Action<Context> upgraderAction;

		private static Action<StoreDatabase> inMemorySchemaInitializationAction;
	}
}
