using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.StoreDriverDelivery;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.MailboxTransport.StoreDriverCommon;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal class ApprovalRequestUpdater
	{
		public static ApprovalRequestUpdater.Result TryUpdateExistingApprovalRequest(MessageItem updateMessage)
		{
			updateMessage.Load(ApprovalRequestUpdater.ApprovalRequestUpdateProperties);
			int? valueAsNullable = updateMessage.GetValueAsNullable<int>(MessageItemSchema.ApprovalDecision);
			string valueOrDefault = updateMessage.GetValueOrDefault<string>(MessageItemSchema.ApprovalDecisionMaker);
			string valueOrDefault2 = updateMessage.GetValueOrDefault<string>(MessageItemSchema.ApprovalRequestMessageId);
			ExDateTime? valueAsNullable2 = updateMessage.GetValueAsNullable<ExDateTime>(MessageItemSchema.ApprovalDecisionTime);
			if (valueAsNullable == null)
			{
				ApprovalRequestUpdater.diag.TraceDebug(0L, "Invalid update becasue there's no decision.");
				return ApprovalRequestUpdater.Result.InvalidUpdateMessage;
			}
			if (!ApprovalRequestUpdater.IsDecisionExpiry(valueAsNullable.Value) && string.IsNullOrEmpty(valueOrDefault))
			{
				ApprovalRequestUpdater.diag.TraceDebug(0L, "Invalid update becasue there's no decisionMaker");
				return ApprovalRequestUpdater.Result.InvalidUpdateMessage;
			}
			if (string.IsNullOrEmpty(valueOrDefault2))
			{
				ApprovalRequestUpdater.diag.TraceDebug(0L, "Invalid update becasue there's no messageId");
				return ApprovalRequestUpdater.Result.InvalidUpdateMessage;
			}
			if (valueAsNullable2 == null)
			{
				ApprovalRequestUpdater.diag.TraceDebug(0L, "Invalid update becasue there's no decisionTime");
				return ApprovalRequestUpdater.Result.InvalidUpdateMessage;
			}
			return ApprovalRequestUpdater.FindAndUpdateExistingApprovalRequest(updateMessage, valueAsNullable.Value, valueOrDefault, valueAsNullable2.Value, valueOrDefault2);
		}

		private static ApprovalRequestUpdater.Result FindAndUpdateExistingApprovalRequest(MessageItem updateMessage, int decision, string decisionMaker, ExDateTime decisionTime, string messageId)
		{
			bool updated = false;
			string local;
			string domain;
			if (!FindMessageUtils.TryParseMessageId(messageId, out local, out domain))
			{
				return ApprovalRequestUpdater.Result.InvalidUpdateMessage;
			}
			ApprovalRequestUpdater.diag.TraceDebug<string>(0L, "Update approval request: messageid={0}", messageId);
			MessageStatus messageStatus = StorageExceptionHandler.RunUnderExceptionHandler(ApprovalRequestUpdater.MessageConverterInstance, delegate
			{
				MailboxSession mailboxSession = (MailboxSession)updateMessage.Session;
				StoreObjectId storeObjectId = null;
				for (int i = 0; i < 25; i++)
				{
					string internetMessageId = ApprovalRequestWriter.FormatApprovalRequestMessageId(local, i, domain, false);
					IStorePropertyBag[] array = AllItemsFolderHelper.FindItemsFromInternetId(mailboxSession, internetMessageId, new PropertyDefinition[]
					{
						ItemSchema.Id
					});
					if (array != null && array.Length > 0)
					{
						ApprovalRequestUpdater.diag.TraceDebug<int>(0L, "Found {0} to update, picking the first.", array.Length);
						storeObjectId = ((VersionedId)array[0][ItemSchema.Id]).ObjectId;
						break;
					}
				}
				if (storeObjectId != null)
				{
					using (MessageItem messageItem = MessageItem.Bind(mailboxSession, storeObjectId))
					{
						if (ApprovalRequestUpdater.VerifyAndUpdateApprovalRequest(mailboxSession, updateMessage.Sender, decision, decisionMaker, decisionTime, messageItem))
						{
							ConflictResolutionResult conflictResolutionResult = messageItem.Save(SaveMode.ResolveConflicts);
							if (conflictResolutionResult.SaveStatus != SaveResult.Success)
							{
								ApprovalRequestUpdater.diag.TraceDebug<string, SaveResult>(0L, "Saving message: {0}, resulted in an update conflict ({1}). Ignored", messageId, conflictResolutionResult.SaveStatus);
							}
							AggregateOperationResult aggregateOperationResult = mailboxSession.Delete(DeleteItemFlags.MoveToDeletedItems, new StoreId[]
							{
								storeObjectId
							});
							if (aggregateOperationResult.OperationResult != OperationResult.Succeeded)
							{
								ApprovalRequestUpdater.diag.TraceDebug<string, OperationResult>(0L, "Delete message: {0}, resulted in failure {1} Ignored", messageId, aggregateOperationResult.OperationResult);
							}
							updated = true;
						}
					}
				}
			});
			if (!updated)
			{
				ApprovalRequestUpdater.diag.TraceDebug<string>(0L, "Couldn't find message: {0}, ignored", messageId);
				return ApprovalRequestUpdater.Result.NotFound;
			}
			if (MessageStatus.Success != messageStatus)
			{
				ApprovalRequestUpdater.diag.TraceDebug<string, string>(0L, "Message ({0}) processing was not successful ({1}), ignoring..", messageId, (messageStatus.Exception == null) ? "NULL exception" : messageStatus.Exception.Message);
				return ApprovalRequestUpdater.Result.SaveConflict;
			}
			return ApprovalRequestUpdater.Result.UpdatedSucessfully;
		}

		private static bool VerifyAndUpdateApprovalRequest(MailboxSession session, Participant updateMessageSender, int decision, string decisionMaker, ExDateTime decisionTime, MessageItem approvalRequest)
		{
			if (!object.Equals("IPM.Note.Microsoft.Approval.Request", approvalRequest.ClassName))
			{
				ApprovalRequestUpdater.diag.TraceDebug(0L, "not a approval request, ignore.");
				return false;
			}
			if (!Participant.HasSameEmail(updateMessageSender, approvalRequest.Sender))
			{
				ApprovalRequestUpdater.diag.TraceDebug(0L, "not the same sender, ignore.");
				return false;
			}
			approvalRequest.OpenAsReadWrite();
			if (ApprovalRequestUpdater.IsDecisionExpiry(decision))
			{
				approvalRequest[MessageItemSchema.ExpiryTime] = decisionTime;
			}
			else
			{
				approvalRequest[MessageItemSchema.ApprovalDecision] = decision;
				approvalRequest[MessageItemSchema.ApprovalDecisionMaker] = decisionMaker;
				approvalRequest[MessageItemSchema.ApprovalDecisionTime] = decisionTime;
			}
			return true;
		}

		private static bool IsDecisionExpiry(int decision)
		{
			return decision < 1;
		}

		private static readonly PropertyDefinition[] ApprovalRequestUpdateProperties = new PropertyDefinition[]
		{
			MessageItemSchema.ApprovalRequestMessageId,
			MessageItemSchema.ApprovalDecisionTime,
			MessageItemSchema.ApprovalDecisionMaker,
			MessageItemSchema.ApprovalDecision
		};

		private static readonly IMessageConverter MessageConverterInstance = new ApprovalRequestUpdater.MessageConverter();

		private static readonly Trace diag = ExTraceGlobals.ApprovalAgentTracer;

		public enum Result
		{
			UpdatedSucessfully,
			NotFound,
			InvalidUpdateMessage,
			SaveConflict
		}

		private class MessageConverter : IMessageConverter
		{
			public string Description
			{
				get
				{
					return "ApprovalAgent.ApprovalRequestUpdater";
				}
			}

			public bool IsOutbound
			{
				get
				{
					return false;
				}
			}

			public Trace Tracer
			{
				get
				{
					return ApprovalRequestUpdater.diag;
				}
			}

			public void LogMessage(Exception exception)
			{
			}
		}
	}
}
