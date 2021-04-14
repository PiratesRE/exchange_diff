using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ResumePublicFolderMigrationRequestCommand : MrsAccessorCommand
	{
		internal ResumePublicFolderMigrationRequestCommand() : base("Resume-PublicFolderMailboxMigrationRequest", ResumePublicFolderMigrationRequestCommand.IgnoreExceptionTypes, null)
		{
		}

		public bool SuspendWhenReadyToComplete
		{
			set
			{
				base.AddParameter("SuspendWhenReadyToComplete", value);
			}
		}

		public const string CmdletName = "Resume-PublicFolderMailboxMigrationRequest";

		private static readonly Type[] IgnoreExceptionTypes = new Type[]
		{
			typeof(CannotSetCompletedPermanentException)
		};
	}
}
