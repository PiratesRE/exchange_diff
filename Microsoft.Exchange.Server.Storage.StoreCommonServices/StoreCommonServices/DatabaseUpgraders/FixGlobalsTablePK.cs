using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.PhysicalAccessJet;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class FixGlobalsTablePK : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return FixGlobalsTablePK.Instance.TestVersionIsReady(context, database);
		}

		private FixGlobalsTablePK() : base(new ComponentVersion(0, 10000), new ComponentVersion(0, 10001))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(database);
			if (globalsTable.GlobalsPK.MaxVersion != base.From.Value)
			{
				globalsTable.GlobalsPK.MaxVersion = base.From.Value;
				globalsTable.NewGlobalsPK.MinVersion = base.To.Value;
			}
			if (database.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
			{
				JetConnection jetConnection = context.GetConnection() as JetConnection;
				if (jetConnection.IsIndexCreated(globalsTable.Table, globalsTable.Table.Name, globalsTable.Table.TableClass, globalsTable.NewGlobalsPK.Name))
				{
					globalsTable.Table.SetPrimaryKeyIndexForUpgraders(globalsTable.NewGlobalsPK);
				}
			}
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
			StoreDatabase storeDatabase = container as StoreDatabase;
			if (storeDatabase.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
			{
				GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(storeDatabase);
				List<Column> list = new List<Column>(new Column[]
				{
					globalsTable.VersionName,
					globalsTable.DatabaseVersion,
					globalsTable.Inid,
					globalsTable.LastMaintenanceTask,
					globalsTable.ExtensionBlob
				});
				if (AddEventCounterBoundsToGlobalsTable.IsReady(context, storeDatabase))
				{
					list.Add(globalsTable.EventCounterLowerBound);
					list.Add(globalsTable.EventCounterUpperBound);
				}
				if (AddDatabaseDsGuidToGlobalsTable.IsReady(context, storeDatabase))
				{
					list.Add(globalsTable.DatabaseDsGuid);
				}
				Guid guid = Guid.Empty;
				long num = 0L;
				long num2 = 1L;
				string @string;
				int @int;
				long int2;
				int int3;
				byte[] binary;
				using (TableOperator tableOperator = Factory.CreateTableOperator(CultureHelper.DefaultCultureInfo, context, globalsTable.Table, globalsTable.GlobalsPK, list, null, null, 0, 1, KeyRange.AllRows, false, true))
				{
					using (Reader reader = tableOperator.ExecuteReader(false))
					{
						reader.Read();
						@string = reader.GetString(globalsTable.VersionName);
						@int = reader.GetInt32(globalsTable.DatabaseVersion);
						int2 = reader.GetInt64(globalsTable.Inid);
						int3 = reader.GetInt32(globalsTable.LastMaintenanceTask);
						binary = reader.GetBinary(globalsTable.ExtensionBlob);
						if (AddEventCounterBoundsToGlobalsTable.IsReady(context, storeDatabase))
						{
							num = reader.GetInt64(globalsTable.EventCounterLowerBound);
							num2 = reader.GetInt64(globalsTable.EventCounterUpperBound);
						}
						if (AddDatabaseDsGuidToGlobalsTable.IsReady(context, storeDatabase))
						{
							guid = reader.GetGuid(globalsTable.DatabaseDsGuid);
						}
					}
				}
				Factory.DeleteTable(context, globalsTable.Table.Name);
				globalsTable.Table.SetPrimaryKeyIndexForUpgraders(globalsTable.NewGlobalsPK);
				globalsTable.Table.CreateTable(context, base.To.Value);
				JetConnection jetConnection = context.GetConnection() as JetConnection;
				jetConnection.DeleteIndex(globalsTable.Table.Name, globalsTable.Table.TableClass, globalsTable.GlobalsPK.Name);
				this.InitInMemoryDatabaseSchema(context, storeDatabase);
				using (DataRow dataRow = Factory.CreateDataRow(context.Culture, context, globalsTable.Table, false, new ColumnValue[]
				{
					new ColumnValue(globalsTable.VersionName, @string),
					new ColumnValue(globalsTable.DatabaseVersion, @int),
					new ColumnValue(globalsTable.Inid, int2),
					new ColumnValue(globalsTable.LastMaintenanceTask, int3),
					new ColumnValue(globalsTable.ExtensionBlob, binary)
				}))
				{
					if (AddEventCounterBoundsToGlobalsTable.IsReady(context, storeDatabase))
					{
						dataRow.SetValue(context, globalsTable.EventCounterLowerBound, num);
						dataRow.SetValue(context, globalsTable.EventCounterUpperBound, num2);
					}
					if (AddDatabaseDsGuidToGlobalsTable.IsReady(context, storeDatabase))
					{
						dataRow.SetValue(context, globalsTable.DatabaseDsGuid, guid);
					}
					dataRow.Flush(context);
				}
			}
		}

		public override void TransactionAborted(Context context, ISchemaVersion container)
		{
			StoreDatabase database = container as StoreDatabase;
			GlobalsTable globalsTable = DatabaseSchema.GlobalsTable(database);
			globalsTable.Table.SetPrimaryKeyIndexForUpgraders(globalsTable.GlobalsPK);
		}

		public static FixGlobalsTablePK Instance = new FixGlobalsTablePK();
	}
}
