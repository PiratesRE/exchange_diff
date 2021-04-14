using System;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices.MailboxUpgraders
{
	public sealed class UpgradeReceiveFolderContent : SchemaUpgrader
	{
		public static bool IsReady(Context context, Mailbox mailbox)
		{
			return UpgradeReceiveFolderContent.Instance.TestVersionIsReady(context, mailbox);
		}

		public static void InitializeUpgraderAction(Action<Context, Mailbox> upgraderAction)
		{
			UpgradeReceiveFolderContent.upgraderAction = upgraderAction;
		}

		private UpgradeReceiveFolderContent() : base(new ComponentVersion(0, 10000), new ComponentVersion(0, 10001))
		{
		}

		public override void InitInMemoryDatabaseSchema(Context context, StoreDatabase database)
		{
		}

		public override void PerformUpgrade(Context context, ISchemaVersion container)
		{
			if (UpgradeReceiveFolderContent.upgraderAction != null)
			{
				UpgradeReceiveFolderContent.upgraderAction(context, (Mailbox)container);
			}
		}

		public static UpgradeReceiveFolderContent Instance = new UpgradeReceiveFolderContent();

		private static Action<Context, Mailbox> upgraderAction;
	}
}
