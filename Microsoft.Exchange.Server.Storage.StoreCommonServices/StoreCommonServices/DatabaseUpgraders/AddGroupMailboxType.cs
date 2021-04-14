using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.DatabaseUpgraders
{
	public sealed class AddGroupMailboxType : SchemaUpgrader
	{
		public static bool IsReady(Context context, StoreDatabase database)
		{
			return AddGroupMailboxType.Instance.TestVersionIsReady(context, database);
		}

		private AddGroupMailboxType() : base(new ComponentVersion(0, 125), new ComponentVersion(0, 126))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
		}

		public static AddGroupMailboxType Instance = new AddGroupMailboxType();
	}
}
