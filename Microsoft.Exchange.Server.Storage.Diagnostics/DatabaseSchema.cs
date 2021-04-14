using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.Diagnostics
{
	public class DatabaseSchema
	{
		private DatabaseSchema(StoreDatabase database)
		{
			this.mSysObjectsTable = new MSysObjectsTable();
			database.PhysicalDatabase.AddTableMetadata(this.mSysObjectsTable.Table);
			this.mSysObjidsTable = new MSysObjidsTable();
			database.PhysicalDatabase.AddTableMetadata(this.mSysObjidsTable.Table);
			this.catalogTableFunction = new CatalogTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.catalogTableFunction.TableFunction);
			this.spaceUsageTableFunction = new SpaceUsageTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.spaceUsageTableFunction.TableFunction);
		}

		internal static void Initialize()
		{
			if (DatabaseSchema.databaseSchemaDataSlot == -1)
			{
				DatabaseSchema.databaseSchemaDataSlot = StoreDatabase.AllocateComponentDataSlot();
			}
		}

		internal static void Initialize(StoreDatabase database)
		{
			database.ComponentData[DatabaseSchema.databaseSchemaDataSlot] = new DatabaseSchema(database);
		}

		internal static void PostMountInitialize(Context context, StoreDatabase database)
		{
			if (database.PhysicalDatabase.DatabaseType != DatabaseType.Jet)
			{
				return;
			}
			ComponentVersion currentSchemaVersion = database.GetCurrentSchemaVersion(context);
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			Table table = databaseSchema.mSysObjectsTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.mSysObjectsTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.mSysObjectsTable = null;
			}
			table = databaseSchema.mSysObjidsTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.mSysObjidsTable.PostMountInitialize(currentSchemaVersion);
				return;
			}
			database.PhysicalDatabase.RemoveTableMetadata(table.Name);
			databaseSchema.mSysObjidsTable = null;
		}

		public static MSysObjectsTable MSysObjectsTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.mSysObjectsTable;
		}

		public static MSysObjidsTable MSysObjidsTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.mSysObjidsTable;
		}

		public static CatalogTableFunction CatalogTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.catalogTableFunction;
		}

		public static SpaceUsageTableFunction SpaceUsageTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.spaceUsageTableFunction;
		}

		private static int databaseSchemaDataSlot = -1;

		private MSysObjectsTable mSysObjectsTable;

		private MSysObjidsTable mSysObjidsTable;

		private CatalogTableFunction catalogTableFunction;

		private SpaceUsageTableFunction spaceUsageTableFunction;
	}
}
