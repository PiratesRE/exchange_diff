using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class AddEventCounterBoundsToGlobalsTable : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return AddEventCounterBoundsToGlobalsTable.Instance.TestVersionIsReady(context, database);
		}

		private AddEventCounterBoundsToGlobalsTable() : base(new ComponentVersion(0, 10000), new ComponentVersion(0, 10001))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(database);
			globalsTable.EventCounterLowerBound.MinVersion = base.To.Value;
			globalsTable.EventCounterUpperBound.MinVersion = base.To.Value;
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
			StoreDatabase storeDatabase = container as StoreDatabase;
			if (storeDatabase.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
			{
				GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(storeDatabase);
				globalsTable.Table.AddColumn(context, globalsTable.EventCounterLowerBound);
				globalsTable.Table.AddColumn(context, globalsTable.EventCounterUpperBound);
				Column[] columnsToUpdate = new Column[]
				{
					globalsTable.EventCounterLowerBound,
					globalsTable.EventCounterUpperBound
				};
				object[] valuesToUpdate = new object[]
				{
					0L,
					1L
				};
				using (UpdateOperator updateOperator = Factory.CreateUpdateOperator(CultureHelper.DefaultCultureInfo, context, Factory.CreateTableOperator(CultureHelper.DefaultCultureInfo, context, globalsTable.Table, globalsTable.Table.PrimaryKeyIndex, null, null, null, 0, 1, KeyRange.AllRows, false, true), columnsToUpdate, valuesToUpdate, true))
				{
					int num = (int)updateOperator.ExecuteScalar();
				}
			}
		}

		public static AddEventCounterBoundsToGlobalsTable Instance = new AddEventCounterBoundsToGlobalsTable();
	}
}
