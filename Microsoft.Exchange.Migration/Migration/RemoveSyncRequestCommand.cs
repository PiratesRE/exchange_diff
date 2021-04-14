using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RemoveSyncRequestCommand : MrsAccessorCommand
	{
		internal RemoveSyncRequestCommand() : base("Remove-SyncRequest", RemoveSyncRequestCommand.IgnoreExceptionTypes, RemoveSyncRequestCommand.TransientExceptionTypes)
		{
		}

		public const string CmdletName = "Remove-SyncRequest";

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
