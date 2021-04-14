using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class RemoveMoveRequestCommand : MrsAccessorCommand
	{
		internal RemoveMoveRequestCommand() : base("Remove-MoveRequest", RemoveMoveRequestCommand.IgnoreExceptionTypes, RemoveMoveRequestCommand.TransientExceptionTypes)
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
