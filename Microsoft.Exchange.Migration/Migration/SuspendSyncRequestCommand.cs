using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SuspendSyncRequestCommand : MrsAccessorCommand
	{
		public SuspendSyncRequestCommand() : base("Suspend-SyncRequest", SuspendSyncRequestCommand.IgnoreExceptionTypes, SuspendSyncRequestCommand.TransientExceptionTypes)
		{
		}

		public const string CmdletName = "Suspend-SyncRequest";

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
