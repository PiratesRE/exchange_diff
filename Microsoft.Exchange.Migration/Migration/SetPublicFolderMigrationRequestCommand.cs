using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SetPublicFolderMigrationRequestCommand : NewPublicFolderMigrationRequestCommandBase
	{
		public SetPublicFolderMigrationRequestCommand(ISubscriptionId id) : base("Set-PublicFolderMailboxMigrationRequest", SetPublicFolderMigrationRequestCommand.ExceptionsToIgnore)
		{
			MigrationUtil.ThrowOnNullArgument(id, "id");
			base.Identity = id;
		}

		public const string CmdletName = "Set-PublicFolderMailboxMigrationRequest";

		private static readonly Type[] ExceptionsToIgnore = new Type[]
		{
			typeof(CannotSetCompletedPermanentException)
		};
	}
}
