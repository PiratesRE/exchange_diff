using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class AddUpgradeHistoryTable : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return AddUpgradeHistoryTable.Instance.TestVersionIsReady(context, database);
		}

		private AddUpgradeHistoryTable() : base(new ComponentVersion(0, 122), new ComponentVersion(0, 123))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			UpgradeHistoryTable upgradeHistoryTable = DatabaseSchema.UpgradeHistoryTable(database);
			upgradeHistoryTable.Table.MinVersion = base.To.Value;
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
			StoreDatabase database = container as StoreDatabase;
			UpgradeHistoryTable upgradeHistoryTable = DatabaseSchema.UpgradeHistoryTable(database);
			upgradeHistoryTable.Table.CreateTable(context, base.To.Value);
		}

		public static AddUpgradeHistoryTable Instance = new AddUpgradeHistoryTable();
	}
}
