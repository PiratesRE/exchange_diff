using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ResumeSyncRequestCommand : MrsAccessorCommand
	{
		public ResumeSyncRequestCommand() : base("Resume-SyncRequest", ResumeSyncRequestCommand.IgnoreExceptionTypes, null)
		{
		}

		public const string CmdletName = "Resume-SyncRequest";

		private static readonly Type[] IgnoreExceptionTypes = new Type[]
		{
			typeof(CannotSetCompletedPermanentException)
		};
	}
}
