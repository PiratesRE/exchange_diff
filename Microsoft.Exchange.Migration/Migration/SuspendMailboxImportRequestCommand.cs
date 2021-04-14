using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SuspendMailboxImportRequestCommand : MrsAccessorCommand
	{
		public SuspendMailboxImportRequestCommand() : base("Suspend-MailboxImportRequest", SuspendMailboxImportRequestCommand.IgnoreExceptionTypes, SuspendMailboxImportRequestCommand.TransientExceptionTypes)
		{
		}

		public const string CmdletName = "Suspend-MailboxImportRequest";

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
