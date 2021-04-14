using System;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupEscalateItemPerformanceTracker : PerformanceTrackerBase, IGroupEscalateItemPerformanceTracker, IMailboxPerformanceTracker, IPerformanceTracker
	{
		public GroupEscalateItemPerformanceTracker(IMailboxSession mailboxSession) : base(mailboxSession)
		{
			this.OriginalMessageSender = string.Empty;
			this.OriginalMessageSenderRecipientType = string.Empty;
			this.OriginalMessageClass = string.Empty;
			this.OriginalMessageId = string.Empty;
			this.OriginalInternetMessageId = string.Empty;
		}

		public string OriginalMessageSender { get; set; }

		public string OriginalMessageSenderRecipientType { get; set; }

		public string OriginalMessageClass { get; set; }

		public string OriginalMessageId { get; set; }

		public string OriginalInternetMessageId { get; set; }

		public int ParticipantsInOriginalMessage { get; set; }

		public bool IsGroupParticipantAddedToReplyTo { get; set; }

		public bool IsGroupParticipantReplyToSkipped { get; set; }

		public bool IsGroupParticipantAddedToParticipants { get; set; }

		public long EnsureGroupParticipantAddedMilliseconds { get; set; }

		public long DedupeParticipantsMilliseconds { get; set; }

		public bool EscalateToYammer { get; set; }

		public long SendToYammerMilliseconds { get; set; }

		public void IncrementParticipantsAddedToEscalatedMessage()
		{
			this.participantsAddedToEscalatedMessage++;
		}

		public void IncrementParticipantsSkippedInEscalatedMessage()
		{
			this.participantsSkippedInEscalatedMessage++;
		}

		public bool HasEscalatedUser { get; set; }

		public bool UnsubscribeUrlInserted { get; set; }

		public long BuildUnsubscribeUrlMilliseconds { get; set; }

		public long LinkBodySize { get; set; }

		public long LinkOnBodyDetectionMilliseconds { get; set; }

		public long LinkInsertOnBodyMilliseconds { get; set; }

		public ILogEvent GetLogEvent(string operationName)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("operationName", operationName);
			base.EnforceInternalState(PerformanceTrackerBase.InternalState.Stopped, "GetLogEvent");
			return new SchemaBasedLogEvent<GroupEscalateItemLogSchema.OperationEnd>
			{
				{
					GroupEscalateItemLogSchema.OperationEnd.OperationName,
					operationName
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.Elapsed,
					base.ElapsedTime.TotalMilliseconds
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.CPU,
					base.CpuTime.TotalMilliseconds
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.RPCCount,
					base.StoreRpcCount
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.RPCLatency,
					base.StoreRpcLatency.TotalMilliseconds
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.DirectoryCount,
					base.DirectoryCount
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.DirectoryLatency,
					base.DirectoryLatency.TotalMilliseconds
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.StoreTimeInServer,
					base.StoreTimeInServer.TotalMilliseconds
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.StoreTimeInCPU,
					base.StoreTimeInCPU.TotalMilliseconds
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.StorePagesRead,
					base.StorePagesRead
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.StorePagesPreRead,
					base.StorePagesPreread
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.StoreLogRecords,
					base.StoreLogRecords
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.StoreLogBytes,
					base.StoreLogBytes
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.OrigMsgSender,
					ExtensibleLogger.FormatPIIValue(this.OriginalMessageSender)
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.OrigMsgSndRcpType,
					this.OriginalMessageSenderRecipientType
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.OrigMsgClass,
					this.OriginalMessageClass
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.OrigMsgId,
					this.OriginalMessageId
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.OrigMsgInetId,
					this.OriginalInternetMessageId
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.PartOrigMsg,
					this.ParticipantsInOriginalMessage
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.GroupReplyTo,
					this.IsGroupParticipantAddedToReplyTo
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.GroupPart,
					this.IsGroupParticipantAddedToParticipants
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.EnsGroupPart,
					this.EnsureGroupParticipantAddedMilliseconds
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.DedupePart,
					this.DedupeParticipantsMilliseconds
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.PartAddedEsc,
					this.participantsAddedToEscalatedMessage
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.PartSkippedEsc,
					this.participantsSkippedInEscalatedMessage
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.HasEscalated,
					this.HasEscalatedUser
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.GroupReplyToSkipped,
					this.IsGroupParticipantReplyToSkipped
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.SendToYammer,
					this.EscalateToYammer
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.SendToYammerMs,
					this.SendToYammerMilliseconds
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.UnsubscribeUrl,
					this.UnsubscribeUrlInserted
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.UnsubscribeUrlBuildMs,
					this.BuildUnsubscribeUrlMilliseconds
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.UnsubscribeBodySize,
					this.LinkBodySize
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.UnsubscribeUrlDetectionMs,
					this.LinkOnBodyDetectionMilliseconds
				},
				{
					GroupEscalateItemLogSchema.OperationEnd.UnsubscribeUrlInsertMs,
					this.LinkInsertOnBodyMilliseconds
				}
			};
		}

		private int participantsAddedToEscalatedMessage;

		private int participantsSkippedInEscalatedMessage;
	}
}
