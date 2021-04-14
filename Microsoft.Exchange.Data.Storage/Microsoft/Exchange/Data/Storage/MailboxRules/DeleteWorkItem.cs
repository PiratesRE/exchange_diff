using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DeleteWorkItem : WorkItem
	{
		public DeleteWorkItem(IRuleEvaluationContext context, int actionIndex) : base(context, actionIndex)
		{
		}

		public override bool ShouldExecuteOnThisStage
		{
			get
			{
				return base.ShouldExecuteOnThisStage || ExecutionStage.OnPublicFolderBefore == base.Context.ExecutionStage;
			}
		}

		public override ExecutionStage Stage
		{
			get
			{
				return ExecutionStage.OnPromotedMessage | ExecutionStage.OnPublicFolderBefore;
			}
		}

		public override void Execute()
		{
			if (!this.ShouldExecuteOnThisStage)
			{
				return;
			}
			base.Context.TraceDebug("Delete action: Creating not-read notification.");
			using (MessageItem messageItem = this.CreateNrn())
			{
				base.Context.TraceDebug("Delete action: Preparing not-read notification.");
				base.Context.SetMailboxOwnerAsSender(messageItem);
				base.SetRecipientsResponsibility(messageItem);
				base.Context.TraceDebug("Delete action: Submitting not-read notification.");
				base.SubmitMessage(messageItem);
				base.Context.TraceDebug("Delete action: Not-read notification submitted.");
			}
		}

		internal MessageItem CreateNrn()
		{
			return RuleMessageUtils.CreateNotReadNotification(base.Context.Message);
		}
	}
}
