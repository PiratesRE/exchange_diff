using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class DatabaseSchema
	{
		private DatabaseSchema(StoreDatabase database)
		{
			this.extendedPropertyNameMappingTable = new ExtendedPropertyNameMappingTable();
			database.PhysicalDatabase.AddTableMetadata(this.extendedPropertyNameMappingTable.Table);
			this.replidGuidMapTable = new ReplidGuidMapTable();
			database.PhysicalDatabase.AddTableMetadata(this.replidGuidMapTable.Table);
			this.mailboxIdentityTable = new MailboxIdentityTable();
			database.PhysicalDatabase.AddTableMetadata(this.mailboxIdentityTable.Table);
			this.mailboxTable = new MailboxTable();
			database.PhysicalDatabase.AddTableMetadata(this.mailboxTable.Table);
			this.globalsTable = new GlobalsTable();
			database.PhysicalDatabase.AddTableMetadata(this.globalsTable.Table);
			this.upgradeHistoryTable = new UpgradeHistoryTable();
			database.PhysicalDatabase.AddTableMetadata(this.upgradeHistoryTable.Table);
			this.timedEventsTable = new TimedEventsTable();
			database.PhysicalDatabase.AddTableMetadata(this.timedEventsTable.Table);
			this.fullTextIndexTableFunctionTableFunction = new FullTextIndexTableFunctionTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.fullTextIndexTableFunctionTableFunction.TableFunction);
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
			Table table = databaseSchema.extendedPropertyNameMappingTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.extendedPropertyNameMappingTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.extendedPropertyNameMappingTable = null;
			}
			table = databaseSchema.replidGuidMapTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.replidGuidMapTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.replidGuidMapTable = null;
			}
			table = databaseSchema.mailboxIdentityTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.mailboxIdentityTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.mailboxIdentityTable = null;
			}
			table = databaseSchema.mailboxTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.mailboxTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.mailboxTable = null;
			}
			table = databaseSchema.globalsTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.globalsTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.globalsTable = null;
			}
			table = databaseSchema.upgradeHistoryTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.upgradeHistoryTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.upgradeHistoryTable = null;
			}
			table = databaseSchema.timedEventsTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.timedEventsTable.PostMountInitialize(currentSchemaVersion);
				return;
			}
			database.PhysicalDatabase.RemoveTableMetadata(table.Name);
			databaseSchema.timedEventsTable = null;
		}

		public static ExtendedPropertyNameMappingTable ExtendedPropertyNameMappingTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.extendedPropertyNameMappingTable;
		}

		public static ReplidGuidMapTable ReplidGuidMapTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.replidGuidMapTable;
		}

		public static MailboxIdentityTable MailboxIdentityTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.mailboxIdentityTable;
		}

		public static MailboxTable MailboxTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.mailboxTable;
		}

		public static GlobalsTable GlobalsTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.globalsTable;
		}

		public static UpgradeHistoryTable UpgradeHistoryTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.upgradeHistoryTable;
		}

		public static TimedEventsTable TimedEventsTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.timedEventsTable;
		}

		public static FullTextIndexTableFunctionTableFunction FullTextIndexTableFunctionTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.fullTextIndexTableFunctionTableFunction;
		}

		private static int databaseSchemaDataSlot = -1;

		private ExtendedPropertyNameMappingTable extendedPropertyNameMappingTable;

		private ReplidGuidMapTable replidGuidMapTable;

		private MailboxIdentityTable mailboxIdentityTable;

		private MailboxTable mailboxTable;

		private GlobalsTable globalsTable;

		private UpgradeHistoryTable upgradeHistoryTable;

		private TimedEventsTable timedEventsTable;

		private FullTextIndexTableFunctionTableFunction fullTextIndexTableFunctionTableFunction;
	}
}
