using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class UnifiedMailbox : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return UnifiedMailbox.Instance.TestVersionIsReady(context, database);
		}

		public static void InitializeUpgraderAction(Action<Context> upgraderAction, Action<StoreDatabase> inMemorySchemaInitializationAction)
		{
			UnifiedMailbox.upgraderAction = upgraderAction;
			UnifiedMailbox.inMemorySchemaInitializationAction = inMemorySchemaInitializationAction;
		}

		private UnifiedMailbox() : base(new ComponentVersion(0, 126), new ComponentVersion(0, 127))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			MailboxIdentityTable mailboxIdentityTable = DatabaseSchema.MailboxIdentityTable(database);
			mailboxIdentityTable.NextMessageDocumentId.MinVersion = base.To.Value;
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(database);
			mailboxTable.MailboxPartitionNumber.MinVersion = base.To.Value;
			mailboxTable.UnifiedMailboxGuid.MinVersion = base.To.Value;
			mailboxTable.UnifiedMailboxGuidIndex.MinVersion = base.To.Value;
			if (UnifiedMailbox.inMemorySchemaInitializationAction != null)
			{
				UnifiedMailbox.inMemorySchemaInitializationAction(database);
			}
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
			StoreDatabase storeDatabase = container as StoreDatabase;
			if (storeDatabase.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
			{
				MailboxIdentityTable mailboxIdentityTable = DatabaseSchema.MailboxIdentityTable(storeDatabase);
				mailboxIdentityTable.Table.AddColumn(context, mailboxIdentityTable.NextMessageDocumentId);
				MailboxTable mailboxTable = DatabaseSchema.MailboxTable(storeDatabase);
				mailboxTable.Table.AddColumn(context, mailboxTable.MailboxPartitionNumber);
				mailboxTable.Table.AddColumn(context, mailboxTable.UnifiedMailboxGuid);
				mailboxTable.Table.CreateIndex(context, mailboxTable.UnifiedMailboxGuidIndex, null);
				if (UnifiedMailbox.upgraderAction != null)
				{
					UnifiedMailbox.upgraderAction(context);
				}
			}
		}

		public static UnifiedMailbox Instance = new UnifiedMailbox();

		private static Action<Context> upgraderAction;

		private static Action<StoreDatabase> inMemorySchemaInitializationAction;
	}
}
