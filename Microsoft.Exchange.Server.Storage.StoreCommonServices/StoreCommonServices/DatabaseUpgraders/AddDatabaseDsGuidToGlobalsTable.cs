using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class AddDatabaseDsGuidToGlobalsTable : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return AddDatabaseDsGuidToGlobalsTable.Instance.TestVersionIsReady(context, database);
		}

		private AddDatabaseDsGuidToGlobalsTable() : base(new ComponentVersion(0, 129), new ComponentVersion(0, 130))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(database);
			globalsTable.DatabaseDsGuid.MinVersion = base.To.Value;
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
			StoreDatabase storeDatabase = container as StoreDatabase;
			if (storeDatabase.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
			{
				GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(storeDatabase);
				globalsTable.Table.AddColumn(context, globalsTable.DatabaseDsGuid);
				Column[] columnsToUpdate = new Column[]
				{
					globalsTable.DatabaseDsGuid
				};
				List<object> list = new List<object>(1);
				list.Add(storeDatabase.MdbGuid);
				using (UpdateOperator updateOperator = Factory.CreateUpdateOperator(CultureHelper.DefaultCultureInfo, context, Factory.CreateTableOperator(CultureHelper.DefaultCultureInfo, context, globalsTable.Table, globalsTable.Table.PrimaryKeyIndex, null, null, null, 0, 1, KeyRange.AllRows, false, true), columnsToUpdate, list, true))
				{
					int num = (int)updateOperator.ExecuteScalar();
				}
			}
		}

		public static AddDatabaseDsGuidToGlobalsTable Instance = new AddDatabaseDsGuidToGlobalsTable();
	}
}
