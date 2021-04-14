using System;
using System.Management.Automation;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Cmdlet("Resume", "PublicFolderMailboxMigrationRequest", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class ResumePublicFolderMailboxMigrationRequest : ResumeRequest<PublicFolderMailboxMigrationRequestIdParameter>
	{
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public override SwitchParameter SuspendWhenReadyToComplete
		{
			get
			{
				return base.SuspendWhenReadyToComplete;
			}
			set
			{
				base.SuspendWhenReadyToComplete = value;
			}
		}

		protected override void ModifyRequest(TransactionalRequestJob requestJob)
		{
			base.ModifyRequest(requestJob);
			if (base.IsFieldSet("SuspendWhenReadyToComplete"))
			{
				requestJob.SuspendWhenReadyToComplete = this.SuspendWhenReadyToComplete;
				requestJob.PreventCompletion = this.SuspendWhenReadyToComplete;
				requestJob.AllowedToFinishMove = !this.SuspendWhenReadyToComplete;
			}
		}

		protected override void CheckIndexEntry()
		{
		}

		private const string TaskNoun = "PublicFolderMailboxMigrationRequest";
	}
}
