using System;
using System.Globalization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.WorkingSet.SignalApiEx;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;
using Microsoft.Exchange.WorkingSet.SignalApi;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents.WorkingSetActionProcessors
{
	internal class AddExchangeItemProcessor : AbstractActionProcessor
	{
		public AddExchangeItemProcessor(ActionProcessorFactory actionProcessorFactory) : base(actionProcessorFactory)
		{
		}

		public override void Process(StoreDriverDeliveryEventArgsImpl argsImpl, Action action, int traceId)
		{
			AbstractActionProcessor.Tracer.TraceDebug((long)traceId, "WorkingSetAgent.AddExchangeItemProcessor.Process: entering");
			MailboxSession mailboxSession = argsImpl.MailboxSession;
			string workingSetSourcePartition = WorkingSetUtils.GetWorkingSetSourcePartition(action.Item);
			string workingSetSourcePartitionInternal = WorkingSetUtils.GetWorkingSetSourcePartitionInternal(action.Item);
			StoreObjectId workingSetPartitionFolderId = WorkingSetUtils.GetWorkingSetPartitionFolderId(mailboxSession, workingSetSourcePartition, workingSetSourcePartitionInternal);
			using (Item item = ((ExchangeItem)action.Item).Item)
			{
				using (MessageItem messageItem = MessageItem.Create(mailboxSession, workingSetPartitionFolderId))
				{
					messageItem.ClassName = item.ClassName;
					Body.CopyBody(item.Body, messageItem.Body, CultureInfo.InvariantCulture, false, false);
					Item.CopyItemContent(item, messageItem);
					if (workingSetSourcePartition != null)
					{
						messageItem.WorkingSetSourcePartition = workingSetSourcePartition;
					}
					WorkingSetSource sourceSystemCode = (WorkingSetSource)action.Item.SourceSystemCode;
					WorkingSetFlags workingSetFlags = WorkingSetUtils.GetWorkingSetFlags(sourceSystemCode);
					messageItem.WorkingSetId = action.Item.Identifier;
					messageItem.WorkingSetSource = sourceSystemCode;
					messageItem.WorkingSetFlags = workingSetFlags;
					PolicyTagHelper.SetRetentionProperties(messageItem, ExDateTime.UtcNow.AddDays(30.0), 30);
					messageItem.AutoResponseSuppress = AutoResponseSuppress.All;
					messageItem.Save(SaveMode.NoConflictResolutionForceSave);
					AbstractActionProcessor.Tracer.TraceDebug((long)traceId, "WorkingSetAgent.AddExchangeItemProcessor.Process: workingSetSource - {0}, workingSetFlags - {1}, workingSetSoucePartition - {2}, folder - {3}", new object[]
					{
						sourceSystemCode,
						workingSetFlags,
						workingSetSourcePartition,
						workingSetPartitionFolderId
					});
				}
			}
			WorkingSet.AddExchangeItem.Increment();
			WorkingSet.AddItem.Increment();
			argsImpl.MailRecipient.DsnRequested = DsnRequestedFlags.Never;
			throw new SmtpResponseException(AddExchangeItemProcessor.processedAddExchangeItemSignalMailResponse, this.actionProcessorFactory.WorkingSetAgent.Name);
		}

		private static readonly SmtpResponse processedAddExchangeItemSignalMailResponse = new SmtpResponse("250", "2.1.6", new string[]
		{
			"WorkingSetAgent; Exchange item added, suppressing delivery Signal mail"
		});
	}
}
