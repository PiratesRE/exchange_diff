using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.PSDirectInvoke;
using Microsoft.Exchange.Management.RecipientTasks;
using Microsoft.Exchange.UnifiedGroups;

namespace Microsoft.Exchange.FederatedDirectory
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class DeleteGroupMailboxTask : UnifiedGroupsTask
	{
		public DeleteGroupMailboxTask(ADUser accessingUser, ExchangePrincipal accessingPrincipal, IRecipientSession adSession) : base(accessingUser, adSession)
		{
			this.accessingPrincipal = accessingPrincipal;
		}

		public string ExternalDirectoryObjectId { get; set; }

		public string SmtpAddress { get; set; }

		protected override string TaskName
		{
			get
			{
				return "DeleteGroupMailboxTask";
			}
		}

		protected override void RunInternal()
		{
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.ExchangeDelete;
			UnifiedGroupsTask.Tracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.RunInternal: Deleting group in Exchange: {1}", base.ActivityId, this.ExternalDirectoryObjectId ?? this.SmtpAddress);
			this.DeleteGroupMailbox();
			UnifiedGroupsTask.Tracer.TraceDebug<Guid>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.RunInternal: Finished deleting group in Exchange", base.ActivityId);
			FederatedDirectoryLogger.AppendToLog(new SchemaBasedLogEvent<FederatedDirectoryLogSchema.TraceTag>
			{
				{
					FederatedDirectoryLogSchema.TraceTag.TaskName,
					this.TaskName
				},
				{
					FederatedDirectoryLogSchema.TraceTag.ActivityId,
					base.ActivityId
				},
				{
					FederatedDirectoryLogSchema.TraceTag.CurrentAction,
					base.CurrentAction
				},
				{
					FederatedDirectoryLogSchema.TraceTag.Message,
					string.Format("Deleted group mailbox in Exchange. {0}", this.ExternalDirectoryObjectId ?? this.SmtpAddress)
				}
			});
			base.CurrentAction = UnifiedGroupsTask.UnifiedGroupsAction.Completed;
		}

		private void DeleteGroupMailbox()
		{
			using (PSLocalTask<RemoveGroupMailbox, object> pslocalTask = CmdletTaskFactory.Instance.CreateRemoveGroupMailboxTask(this.accessingPrincipal))
			{
				pslocalTask.CaptureAdditionalIO = true;
				pslocalTask.Task.ExecutingUser = new RecipientIdParameter(this.accessingPrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
				pslocalTask.Task.Identity = new MailboxIdParameter(this.ExternalDirectoryObjectId ?? this.SmtpAddress);
				UnifiedGroupsTask.Tracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "ActivityId={0}. {1}", base.ActivityId, new PSLocalTaskLogging.RemoveGroupMailboxToString(pslocalTask.Task).ToString());
				pslocalTask.Task.Execute();
				UnifiedGroupsTask.Tracer.TraceDebug<Guid, string>((long)this.GetHashCode(), "ActivityId={0}. {1}", base.ActivityId, new PSLocalTaskLogging.TaskOutputToString(pslocalTask.AdditionalIO).ToString());
				if (pslocalTask.Error != null)
				{
					UnifiedGroupsTask.Tracer.TraceError<Guid, string>((long)this.GetHashCode(), "ActivityId={0}. DeleteUnifiedGroupTask.DeleteGroupMailbox() failed: {1}", base.ActivityId, pslocalTask.ErrorMessage);
					throw new ExchangeAdaptorException(Strings.GroupMailboxFailedDelete(this.ExternalDirectoryObjectId ?? this.SmtpAddress, pslocalTask.ErrorMessage));
				}
			}
		}

		private readonly ExchangePrincipal accessingPrincipal;
	}
}
