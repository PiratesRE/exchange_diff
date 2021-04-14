using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SuspendMoveRequestCommand : MrsAccessorCommand
	{
		public SuspendMoveRequestCommand() : base("Suspend-MoveRequest", SuspendMoveRequestCommand.IgnoreExceptionTypes, SuspendMoveRequestCommand.TransientExceptionTypes)
		{
		}

		public const string CmdletName = "Suspend-MoveRequest";

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
