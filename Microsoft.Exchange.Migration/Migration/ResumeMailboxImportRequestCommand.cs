using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ResumeMailboxImportRequestCommand : MrsAccessorCommand
	{
		public ResumeMailboxImportRequestCommand() : base("Resume-MailboxImportRequest", ResumeMailboxImportRequestCommand.IgnoreExceptionTypes, null)
		{
		}

		private static readonly Type[] IgnoreExceptionTypes = new Type[]
		{
			typeof(CannotSetCompletedPermanentException)
		};
	}
}
