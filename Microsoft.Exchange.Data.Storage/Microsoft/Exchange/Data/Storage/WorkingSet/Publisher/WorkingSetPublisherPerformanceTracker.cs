using System;
using Microsoft.Exchange.Data.Storage.Optics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.WorkingSet.Publisher
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class WorkingSetPublisherPerformanceTracker : PerformanceTrackerBase, IWorkingSetPublisherPerformanceTracker, IMailboxPerformanceTracker, IPerformanceTracker
	{
		public WorkingSetPublisherPerformanceTracker(IMailboxSession mailboxSession) : base(mailboxSession)
		{
			this.OriginalMessageSender = string.Empty;
			this.OriginalMessageSenderRecipientType = string.Empty;
			this.OriginalMessageClass = string.Empty;
			this.OriginalMessageId = string.Empty;
			this.OriginalInternetMessageId = string.Empty;
			this.PublishedMessageId = string.Empty;
			this.PublishedIntnernetMessageId = string.Empty;
		}

		public string OriginalMessageSender { get; set; }

		public string OriginalMessageSenderRecipientType { get; set; }

		public string OriginalMessageClass { get; set; }

		public string OriginalMessageId { get; set; }

		public string OriginalInternetMessageId { get; set; }

		public int ParticipantsInOriginalMessage { get; set; }

		public string PublishedMessageId { get; set; }

		public string PublishedIntnernetMessageId { get; set; }

		public bool IsGroupParticipantAddedToParticipants { get; set; }

		public long EnsureGroupParticipantAddedMilliseconds { get; set; }

		public long DedupeParticipantsMilliseconds { get; set; }

		public void IncrementParticipantsAddedToPublishedMessage()
		{
			this.participantsAddedToPublishedMessage++;
		}

		public void IncrementParticipantsSkippedInPublishedMessage()
		{
			this.participantsSkippedInPublishedMessage++;
		}

		public bool HasWorkingSetUser { get; set; }

		public string Exception { get; set; }

		public ILogEvent GetLogEvent(string operationName)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("operationName", operationName);
			base.EnforceInternalState(PerformanceTrackerBase.InternalState.Stopped, "GetLogEvent");
			return new SchemaBasedLogEvent<WorkingSetPublisherLogSchema.OperationEnd>
			{
				{
					WorkingSetPublisherLogSchema.OperationEnd.OperationName,
					operationName
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.Elapsed,
					base.ElapsedTime.TotalMilliseconds
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.CPU,
					base.CpuTime.TotalMilliseconds
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.RPCCount,
					base.StoreRpcCount
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.RPCLatency,
					base.StoreRpcLatency.TotalMilliseconds
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.DirectoryCount,
					base.DirectoryCount
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.DirectoryLatency,
					base.DirectoryLatency.TotalMilliseconds
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.StoreTimeInServer,
					base.StoreTimeInServer.TotalMilliseconds
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.StoreTimeInCPU,
					base.StoreTimeInCPU.TotalMilliseconds
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.StorePagesRead,
					base.StorePagesRead
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.StorePagesPreRead,
					base.StorePagesPreread
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.StoreLogRecords,
					base.StoreLogRecords
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.StoreLogBytes,
					base.StoreLogBytes
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.OrigMsgSender,
					ExtensibleLogger.FormatPIIValue(this.OriginalMessageSender)
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.OrigMsgSndRcpType,
					this.OriginalMessageSenderRecipientType
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.OrigMsgClass,
					this.OriginalMessageClass
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.OrigMsgId,
					this.OriginalMessageId
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.OrigMsgInetId,
					this.OriginalInternetMessageId
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.PartOrigMsg,
					this.ParticipantsInOriginalMessage
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.GroupPart,
					this.IsGroupParticipantAddedToParticipants
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.EnsGroupPart,
					this.EnsureGroupParticipantAddedMilliseconds
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.DedupePart,
					this.DedupeParticipantsMilliseconds
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.PartAddedPub,
					this.participantsAddedToPublishedMessage
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.PartSkippedPub,
					this.participantsSkippedInPublishedMessage
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.PubMsgId,
					this.PublishedMessageId
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.PubMsgInetId,
					this.PublishedIntnernetMessageId
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.HasWorkingSet,
					this.HasWorkingSetUser
				},
				{
					WorkingSetPublisherLogSchema.OperationEnd.Exception,
					this.Exception
				}
			};
		}

		private int participantsAddedToPublishedMessage;

		private int participantsSkippedInPublishedMessage;
	}
}
