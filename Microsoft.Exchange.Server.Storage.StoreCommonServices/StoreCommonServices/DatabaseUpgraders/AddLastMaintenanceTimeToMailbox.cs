using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class AddLastMaintenanceTimeToMailbox : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return AddLastMaintenanceTimeToMailbox.Instance.TestVersionIsReady(context, database);
		}

		private AddLastMaintenanceTimeToMailbox() : base(new ComponentVersion(0, 121), new ComponentVersion(0, 122))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
			MailboxTable mailboxTable = DatabaseSchema.MailboxTable(database);
			mailboxTable.LastMailboxMaintenanceTime.MinVersion = base.To.Value;
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
			StoreDatabase storeDatabase = container as StoreDatabase;
			if (storeDatabase.PhysicalDatabase.DatabaseType == DatabaseType.Jet)
			{
				MailboxTable mailboxTable = DatabaseSchema.MailboxTable(storeDatabase);
				mailboxTable.Table.AddColumn(context, mailboxTable.LastMailboxMaintenanceTime);
			}
		}

		public static AddLastMaintenanceTimeToMailbox Instance = new AddLastMaintenanceTimeToMailbox();
	}
}
