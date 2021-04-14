using System;
using System.Text;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Data.Storage.MailboxRules
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class OofReplyWorkItem : WorkItem
	{
		public OofReplyWorkItem(IRuleEvaluationContext context, byte[] messageTemplateEntryId, int actionIndex) : base(context, actionIndex)
		{
			this.messageTemplateEntryId = messageTemplateEntryId;
		}

		public override ExecutionStage Stage
		{
			get
			{
				return ExecutionStage.OnPromotedMessage | ExecutionStage.OnDeliveredMessage;
			}
		}

		public override void Execute()
		{
			if (!this.ShouldExecuteOnThisStage)
			{
				return;
			}
			if (base.Context.DetectLoop())
			{
				string[] valueOrDefault = base.Context.Message.GetValueOrDefault<string[]>(MessageItemSchema.XLoop, null);
				if (valueOrDefault == null || valueOrDefault.Length != 1)
				{
					return;
				}
				base.Context.TraceDebug("Sending OOF even though loop was detected due to 1 XLoop header in the message.");
			}
			if (string.IsNullOrEmpty(base.Context.SenderAddress))
			{
				base.Context.TraceDebug("Sender address was empty, this OOF reply work item is not executed.");
				return;
			}
			MapiFolder nonIpmSubtreeFolder = null;
			MapiFolder oofHistoryFolder = null;
			OofHistory oofHistory = null;
			try
			{
				MailboxSession mailboxSession = base.Context.StoreSession as MailboxSession;
				bool shouldSendOofReply = true;
				bool shouldKeepOofHistory = true;
				if ((base.Rule.StateFlags & RuleStateFlags.KeepOOFHistory) == (RuleStateFlags)0)
				{
					base.Context.TraceError("Rule does not have 'keep OOF history' flag set, OOF history operation skipped.");
					shouldKeepOofHistory = false;
				}
				if (shouldKeepOofHistory && mailboxSession == null)
				{
					base.Context.TraceError("This is a public folder delivery, OOF history operation skipped.");
					shouldKeepOofHistory = false;
				}
				if (shouldKeepOofHistory && mailboxSession.MailboxOwner.MailboxInfo.Location.ServerVersion < 1937801494)
				{
					base.Context.TraceError("Recipient's mailbox doesn't support OOF history.");
					shouldKeepOofHistory = false;
				}
				if (string.IsNullOrEmpty(base.Context.SenderAddress))
				{
					base.Context.TraceError("Sender address was empty, OOF history operation skipped.");
					shouldKeepOofHistory = false;
				}
				if (shouldKeepOofHistory)
				{
					RuleUtil.RunMapiCode(ServerStrings.UpdateOOFHistoryOperation, delegate(object[] param0)
					{
						MapiStore mapiStore = this.Context.StoreSession.Mailbox.MapiStore;
						byte[] globalRuleId = mapiStore.GlobalIdFromId(this.Rule.ID);
						byte[] bytes = Encoding.ASCII.GetBytes(this.Context.SenderAddress);
						oofHistory = new OofHistory(bytes, globalRuleId, this.Context);
						if (!oofHistory.TryInitialize())
						{
							shouldKeepOofHistory = false;
							return;
						}
						if (mailboxSession.IsMailboxOof())
						{
							shouldSendOofReply = oofHistory.ShouldSendOofReply();
							this.Context.TraceDebug<bool>("Should send automatic reply: {0}.", shouldSendOofReply);
							return;
						}
						this.Context.TraceDebug("Mailbox is not OOF, not sending oof reply");
					}, new object[0]);
				}
				if (!shouldSendOofReply)
				{
					if (!ObjectClass.IsOfClass(base.Context.Message.ClassName, "IPM.Note.Microsoft.Approval.Request"))
					{
						base.Context.TraceDebug("Automatic reply will not be sent.");
						return;
					}
					base.Context.TraceDebug("Sending automatic reply anyway - the incoming message is an approval request.");
				}
				if (!base.Context.LimitChecker.DoesExceedAutoReplyLimit())
				{
					using (MessageItem messageItem = base.OpenMessage(this.messageTemplateEntryId))
					{
						using (MessageItem messageItem2 = RuleMessageUtils.CreateOofReply(base.Context.Message, messageItem, base.Context.StoreSession.PreferedCulture, new InboundConversionOptions(base.Context.StoreSession.GetADRecipientSession(true, ConsistencyMode.IgnoreInvalid), base.Context.DefaultDomainName), base.Context.XLoopValue))
						{
							messageItem2[ItemSchema.SpamConfidenceLevel] = -1;
							messageItem2.AutoResponseSuppress = AutoResponseSuppress.All;
							base.Context.SetMailboxOwnerAsSender(messageItem2);
							base.SetRecipientsResponsibility(messageItem2);
							base.SubmitMessage(messageItem2);
						}
					}
					if (shouldKeepOofHistory && oofHistory != null)
					{
						RuleUtil.RunMapiCode(ServerStrings.AppendOOFHistoryEntry, delegate(object[] param0)
						{
							oofHistory.AppendEntry();
						}, new object[0]);
					}
				}
			}
			finally
			{
				if (oofHistory != null)
				{
					RuleUtil.RunMapiCode(ServerStrings.UnlockOOFHistory, delegate(object[] param0)
					{
						oofHistory.Dispose();
					}, new object[0]);
				}
				if (oofHistoryFolder != null)
				{
					RuleUtil.RunMapiCode(ServerStrings.DisposeOOFHistoryFolder, delegate(object[] param0)
					{
						oofHistoryFolder.Dispose();
					}, new object[0]);
				}
				if (nonIpmSubtreeFolder != null)
				{
					RuleUtil.RunMapiCode(ServerStrings.DisposeNonIPMFolder, delegate(object[] param0)
					{
						nonIpmSubtreeFolder.Dispose();
					}, new object[0]);
				}
			}
		}

		private const int MinSupportedMailboxServerVersion = 1937801494;

		private byte[] messageTemplateEntryId;
	}
}
