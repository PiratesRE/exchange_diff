using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ResumeMoveRequestCommand : MrsAccessorCommand
	{
		public ResumeMoveRequestCommand() : base("Resume-MoveRequest", ResumeMoveRequestCommand.IgnoreExceptionTypes, null)
		{
		}

		public bool SuspendWhenReadyToComplete
		{
			set
			{
				base.AddParameter("SuspendWhenReadyToComplete", value);
			}
		}

		private static readonly Type[] IgnoreExceptionTypes = new Type[]
		{
			typeof(CannotSetCompletedPermanentException)
		};
	}
}
