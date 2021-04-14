using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MarkAsReadWorkItem : WorkItem
	{
		public MarkAsReadWorkItem(IRuleEvaluationContext context, int actionIndex) : base(context, actionIndex)
		{
		}

		public override ExecutionStage Stage
		{
			get
			{
				return ExecutionStage.OnCreatedMessage;
			}
		}

		public override void Execute()
		{
			base.Context.TraceDebug("Mark-as-read action: Setting DeliverAsRead");
			base.Context.PropertiesForAllMessageCopies[MessageItemSchema.DeliverAsRead] = true;
		}
	}
}
