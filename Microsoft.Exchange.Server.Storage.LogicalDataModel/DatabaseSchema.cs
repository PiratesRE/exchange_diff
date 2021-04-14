using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LogicalDataModel
{
	public class DatabaseSchema
	{
		private DatabaseSchema(StoreDatabase database)
		{
			this.deliveredToTable = new DeliveredToTable();
			database.PhysicalDatabase.AddTableMetadata(this.deliveredToTable.Table);
			this.eventsTable = new EventsTable();
			database.PhysicalDatabase.AddTableMetadata(this.eventsTable.Table);
			this.attachmentTable = new AttachmentTable();
			database.PhysicalDatabase.AddTableMetadata(this.attachmentTable.Table);
			this.folderTable = new FolderTable();
			database.PhysicalDatabase.AddTableMetadata(this.folderTable.Table);
			this.inferenceLogTable = new InferenceLogTable();
			database.PhysicalDatabase.AddTableMetadata(this.inferenceLogTable.Table);
			this.messageTable = new MessageTable();
			database.PhysicalDatabase.AddTableMetadata(this.messageTable.Table);
			this.perUserTable = new PerUserTable();
			database.PhysicalDatabase.AddTableMetadata(this.perUserTable.Table);
			this.receiveFolderTable = new ReceiveFolderTable();
			database.PhysicalDatabase.AddTableMetadata(this.receiveFolderTable.Table);
			this.receiveFolder2Table = new ReceiveFolder2Table();
			database.PhysicalDatabase.AddTableMetadata(this.receiveFolder2Table.Table);
			this.searchQueueTable = new SearchQueueTable();
			database.PhysicalDatabase.AddTableMetadata(this.searchQueueTable.Table);
			this.tombstoneTable = new TombstoneTable();
			database.PhysicalDatabase.AddTableMetadata(this.tombstoneTable.Table);
			this.watermarksTable = new WatermarksTable();
			database.PhysicalDatabase.AddTableMetadata(this.watermarksTable.Table);
			this.userInfoTable = new UserInfoTable();
			database.PhysicalDatabase.AddTableMetadata(this.userInfoTable.Table);
			this.attachmentTableFunctionTableFunction = new AttachmentTableFunctionTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.attachmentTableFunctionTableFunction.TableFunction);
			this.recipientTableFunctionTableFunction = new RecipientTableFunctionTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.recipientTableFunctionTableFunction.TableFunction);
			this.conversationMembersBlobTableFunction = new ConversationMembersBlobTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.conversationMembersBlobTableFunction.TableFunction);
			this.folderHierarchyBlobTableFunction = new FolderHierarchyBlobTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.folderHierarchyBlobTableFunction.TableFunction);
			this.searchResultsTableFunction = new SearchResultsTableFunction();
			database.PhysicalDatabase.AddTableMetadata(this.searchResultsTableFunction.TableFunction);
		}

		internal static void Initialize()
		{
			if (DatabaseSchema.databaseSchemaDataSlot == -1)
			{
				DatabaseSchema.databaseSchemaDataSlot = StoreDatabase.AllocateComponentDataSlot();
			}
		}

		internal static void MountEventHandlerForFullTextIndex(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			databaseSchema.messageTable.SetupFullTextIndex(database);
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
			Table table = databaseSchema.deliveredToTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.deliveredToTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.deliveredToTable = null;
			}
			table = databaseSchema.eventsTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.eventsTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.eventsTable = null;
			}
			table = databaseSchema.attachmentTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.attachmentTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.attachmentTable = null;
			}
			table = databaseSchema.folderTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.folderTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.folderTable = null;
			}
			table = databaseSchema.inferenceLogTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.inferenceLogTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.inferenceLogTable = null;
			}
			table = databaseSchema.messageTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.messageTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.messageTable = null;
			}
			table = databaseSchema.perUserTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.perUserTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.perUserTable = null;
			}
			table = databaseSchema.receiveFolderTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.receiveFolderTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.receiveFolderTable = null;
			}
			table = databaseSchema.receiveFolder2Table.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.receiveFolder2Table.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.receiveFolder2Table = null;
			}
			table = databaseSchema.searchQueueTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.searchQueueTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.searchQueueTable = null;
			}
			table = databaseSchema.tombstoneTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.tombstoneTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.tombstoneTable = null;
			}
			table = databaseSchema.watermarksTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.watermarksTable.PostMountInitialize(currentSchemaVersion);
			}
			else
			{
				database.PhysicalDatabase.RemoveTableMetadata(table.Name);
				databaseSchema.watermarksTable = null;
			}
			table = databaseSchema.userInfoTable.Table;
			if (table.MinVersion <= currentSchemaVersion.Value && currentSchemaVersion.Value <= table.MaxVersion)
			{
				databaseSchema.userInfoTable.PostMountInitialize(currentSchemaVersion);
				return;
			}
			database.PhysicalDatabase.RemoveTableMetadata(table.Name);
			databaseSchema.userInfoTable = null;
		}

		public static DeliveredToTable DeliveredToTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.deliveredToTable;
		}

		public static EventsTable EventsTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.eventsTable;
		}

		public static AttachmentTable AttachmentTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.attachmentTable;
		}

		public static FolderTable FolderTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.folderTable;
		}

		public static InferenceLogTable InferenceLogTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.inferenceLogTable;
		}

		public static MessageTable MessageTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.messageTable;
		}

		public static PerUserTable PerUserTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.perUserTable;
		}

		public static ReceiveFolderTable ReceiveFolderTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.receiveFolderTable;
		}

		public static ReceiveFolder2Table ReceiveFolder2Table(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.receiveFolder2Table;
		}

		public static SearchQueueTable SearchQueueTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.searchQueueTable;
		}

		public static TombstoneTable TombstoneTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.tombstoneTable;
		}

		public static WatermarksTable WatermarksTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.watermarksTable;
		}

		public static UserInfoTable UserInfoTable(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.userInfoTable;
		}

		public static AttachmentTableFunctionTableFunction AttachmentTableFunctionTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.attachmentTableFunctionTableFunction;
		}

		public static RecipientTableFunctionTableFunction RecipientTableFunctionTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.recipientTableFunctionTableFunction;
		}

		public static ConversationMembersBlobTableFunction ConversationMembersBlobTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.conversationMembersBlobTableFunction;
		}

		public static FolderHierarchyBlobTableFunction FolderHierarchyBlobTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.folderHierarchyBlobTableFunction;
		}

		public static SearchResultsTableFunction SearchResultsTableFunction(StoreDatabase database)
		{
			DatabaseSchema databaseSchema = (DatabaseSchema)database.ComponentData[DatabaseSchema.databaseSchemaDataSlot];
			return databaseSchema.searchResultsTableFunction;
		}

		private static int databaseSchemaDataSlot = -1;

		private DeliveredToTable deliveredToTable;

		private EventsTable eventsTable;

		private AttachmentTable attachmentTable;

		private FolderTable folderTable;

		private InferenceLogTable inferenceLogTable;

		private MessageTable messageTable;

		private PerUserTable perUserTable;

		private ReceiveFolderTable receiveFolderTable;

		private ReceiveFolder2Table receiveFolder2Table;

		private SearchQueueTable searchQueueTable;

		private TombstoneTable tombstoneTable;

		private WatermarksTable watermarksTable;

		private UserInfoTable userInfoTable;

		private AttachmentTableFunctionTableFunction attachmentTableFunctionTableFunction;

		private RecipientTableFunctionTableFunction recipientTableFunctionTableFunction;

		private ConversationMembersBlobTableFunction conversationMembersBlobTableFunction;

		private FolderHierarchyBlobTableFunction folderHierarchyBlobTableFunction;

		private SearchResultsTableFunction searchResultsTableFunction;
	}
}
