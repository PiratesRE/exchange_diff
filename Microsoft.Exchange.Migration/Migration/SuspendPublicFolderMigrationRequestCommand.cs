using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SuspendPublicFolderMigrationRequestCommand : MrsAccessorCommand
	{
		internal SuspendPublicFolderMigrationRequestCommand(MRSSubscriptionId identity) : base("Suspend-PublicFolderMailboxMigrationRequest", SuspendPublicFolderMigrationRequestCommand.IgnoreExceptionTypes, SuspendPublicFolderMigrationRequestCommand.TransientExceptionTypes)
		{
			base.Identity = identity;
		}

		public const string CmdletName = "Suspend-PublicFolderMailboxMigrationRequest";

		private static readonly Type[] IgnoreExceptionTypes = new Type[]
		{
			typeof(CannotSetCompletedPermanentException)
		};

		private static readonly Type[] TransientExceptionTypes = new Type[]
		{
			typeof(CannotSetCompletingPermanentException)
		};
	}
}
