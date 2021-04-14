using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RemoveMailboxImportRequestCommand : MrsAccessorCommand
	{
		internal RemoveMailboxImportRequestCommand() : base("Remove-MailboxImportRequest", RemoveMailboxImportRequestCommand.IgnoreExceptionTypes, RemoveMailboxImportRequestCommand.TransientExceptionTypes)
		{
		}

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
