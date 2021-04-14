using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public class DatabaseSchema
	{
		private DatabaseSchema(StoreDatabase database)
		{
			this.pseudoIndexDefinitionTable = new PseudoIndexDefinitionTable();
			database.PhysicalDatabase.AddTableMetadata(this.pseudoIndexDefinitionTable.Table);
			this.pseudoIndexControlTable = new PseudoIndexControlTable();
			database.PhysicalDatabase.AddTableMetadata(this.pseudoIndexControlTable.Table);
			this.pseudoIndexMaintenanceTable = new PseudoIndexMaintenanceTable();
			database.PhysicalDatabase.AddTableMetadata(this.pseudoIndexMaintenanceTable.Table);
			this.indexExplosionTableFunctionTableFunction = new IndexExplosionTableFunctionTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.indexExplosionTableFunctionTableFunction.TableFunction);
			this.columnMappingBlobTableFunction = new ColumnMappingBlobTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.columnMappingBlobTableFunction.TableFunction);
			this.conditionalIndexMappingBlobTableFunction = new ConditionalIndexMappingBlobTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.conditionalIndexMappingBlobTableFunction.TableFunction);
			this.indexDefinitionBlobTableFunction = new IndexDefinitionBlobTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.indexDefinitionBlobTableFunction.TableFunction);
			this.columnMappingBlobHeaderTableFunction = new ColumnMappingBlobHeaderTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.columnMappingBlobHeaderTableFunction.TableFunction);
			this.indexDefinitionBlobHeaderTableFunction = new IndexDefinitionBlobHeaderTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.indexDefinitionBlobHeaderTableFunction.TableFunction);
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
			Table table = databaseSchema.pseudoIndexDefinitionTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.pseudoIndexDefinitionTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.pseudoIndexDefinitionTable = null;
			}
			table = databaseSchema.pseudoIndexControlTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.pseudoIndexControlTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.pseudoIndexControlTable = null;
			}
			table = databaseSchema.pseudoIndexMaintenanceTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.pseudoIndexMaintenanceTable.PostMountInitialize(currentSchemaVersion);
				return;
			}
			database.PhysicalDatabase.RemoveTableMetadata(table.Name);
			databaseSchema.pseudoIndexMaintenanceTable = null;
		}

		public static PseudoIndexDefinitionTable PseudoIndexDefinitionTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.pseudoIndexDefinitionTable;
		}

		public static PseudoIndexControlTable PseudoIndexControlTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.pseudoIndexControlTable;
		}

		public static PseudoIndexMaintenanceTable PseudoIndexMaintenanceTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.pseudoIndexMaintenanceTable;
		}

		public static IndexExplosionTableFunctionTableFunction IndexExplosionTableFunctionTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.indexExplosionTableFunctionTableFunction;
		}

		public static ColumnMappingBlobTableFunction ColumnMappingBlobTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.columnMappingBlobTableFunction;
		}

		public static ConditionalIndexMappingBlobTableFunction ConditionalIndexMappingBlobTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.conditionalIndexMappingBlobTableFunction;
		}

		public static IndexDefinitionBlobTableFunction IndexDefinitionBlobTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.indexDefinitionBlobTableFunction;
		}

		public static ColumnMappingBlobHeaderTableFunction ColumnMappingBlobHeaderTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.columnMappingBlobHeaderTableFunction;
		}

		public static IndexDefinitionBlobHeaderTableFunction IndexDefinitionBlobHeaderTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.indexDefinitionBlobHeaderTableFunction;
		}

		private static int databaseSchemaDataSlot = -1;

		private PseudoIndexDefinitionTable pseudoIndexDefinitionTable;

		private PseudoIndexControlTable pseudoIndexControlTable;

		private PseudoIndexMaintenanceTable pseudoIndexMaintenanceTable;

		private IndexExplosionTableFunctionTableFunction indexExplosionTableFunctionTableFunction;

		private ColumnMappingBlobTableFunction columnMappingBlobTableFunction;

		private ConditionalIndexMappingBlobTableFunction conditionalIndexMappingBlobTableFunction;

		private IndexDefinitionBlobTableFunction indexDefinitionBlobTableFunction;

		private ColumnMappingBlobHeaderTableFunction columnMappingBlobHeaderTableFunction;

		private IndexDefinitionBlobHeaderTableFunction indexDefinitionBlobHeaderTableFunction;
	}
}
