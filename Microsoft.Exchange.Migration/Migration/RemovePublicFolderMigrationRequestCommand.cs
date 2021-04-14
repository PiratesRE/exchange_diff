using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RemovePublicFolderMigrationRequestCommand : MrsAccessorCommand
	{
		internal RemovePublicFolderMigrationRequestCommand(MRSSubscriptionId identity) : base("Remove-PublicFolderMailboxMigrationRequest", RemovePublicFolderMigrationRequestCommand.IgnoreExceptionTypes, RemovePublicFolderMigrationRequestCommand.TransientExceptionTypes)
		{
			base.Identity = identity;
		}

		public const string CmdletName = "Remove-PublicFolderMailboxMigrationRequest";

		private static readonly Type[] IgnoreExceptionTypes = new Type[]
		{
			typeof(ManagementObjectNotFoundException)
		};

		private static readonly Type[] TransientExceptionTypes = new Type[]
		{
			typeof(CannotSetCompletingPermanentException)
		};
	}
}
