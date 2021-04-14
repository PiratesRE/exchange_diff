using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[Serializable]
	public class RequestStatisticsBase : RequestJobBase
	{
		public RequestStatisticsBase()
		{
			this.report = null;
			this.positionInQueue = LocalizedString.Empty;
		}

		internal RequestStatisticsBase(SimpleProviderPropertyBag propertyBag) : base(propertyBag)
		{
			this.report = null;
			this.positionInQueue = LocalizedString.Empty;
		}

		public virtual ByteQuantifiedSize? BytesTransferred
		{
			get
			{
				if (base.ProgressTracker != null)
				{
					return new ByteQuantifiedSize?(new ByteQuantifiedSize(base.ProgressTracker.BytesTransferred));
				}
				return null;
			}
		}

		public virtual ByteQuantifiedSize? BytesTransferredPerMinute
		{
			get
			{
				if (base.ProgressTracker != null)
				{
					return new ByteQuantifiedSize?(new ByteQuantifiedSize(base.ProgressTracker.BytesPerMinute));
				}
				return null;
			}
		}

		public virtual ulong? ItemsTransferred
		{
			get
			{
				if (base.ProgressTracker != null)
				{
					return new ulong?(base.ProgressTracker.ItemsTransferred);
				}
				return null;
			}
		}

		public virtual LocalizedString PositionInQueue
		{
			get
			{
				return this.positionInQueue;
			}
			internal set
			{
				this.positionInQueue = value;
			}
		}

		public virtual Report Report
		{
			get
			{
				return this.report;
			}
			internal set
			{
				this.report = value;
			}
		}

		internal virtual void UpdateThroughputFromMoveRequestInfo(MoveRequestInfo moveRequestInfo)
		{
			if (moveRequestInfo != null && this.IsRequestActive())
			{
				base.ProgressTracker = moveRequestInfo.ProgressTracker;
				base.BadItemsEncountered = moveRequestInfo.BadItemsEncountered;
				base.PercentComplete = moveRequestInfo.PercentComplete;
			}
		}

		private bool IsRequestActive()
		{
			return base.IdleTime < TimeSpan.FromMinutes(60.0) && base.RequestJobState == JobProcessingState.InProgress && (base.Status == RequestStatus.InProgress || base.Status == RequestStatus.CompletionInProgress);
		}

		internal virtual void UpdateMessageFromMoveRequestInfo(MoveRequestInfo moveRequestInfo)
		{
			if (moveRequestInfo != null && !moveRequestInfo.Message.IsEmpty)
			{
				base.Message = (base.Message.IsEmpty ? moveRequestInfo.Message : ServerStrings.CompositeError(base.Message, moveRequestInfo.Message));
			}
		}

		internal void PopulateDiagnosticInfo(RequestStatisticsDiagnosticArgument arguments, string jobPickupFailureMessage)
		{
			RequestJobDiagnosticInfoXML requestJobDiagnosticInfoXML = new RequestJobDiagnosticInfoXML
			{
				PoisonCount = base.PoisonCount,
				LastPickupTime = (base.LastPickupTime ?? DateTime.MinValue),
				IsCanceled = base.CancelRequest,
				RetryCount = base.RetryCount,
				TotalRetryCount = base.TotalRetryCount,
				DomainController = base.DomainControllerToUpdate,
				SkippedItems = base.SkippedItemCounts,
				FailureHistory = base.FailureHistory
			};
			if (base.TimeTracker != null)
			{
				requestJobDiagnosticInfoXML.DoNotPickUntil = (base.TimeTracker.GetTimestamp(RequestJobTimestamp.DoNotPickUntil) ?? DateTime.MinValue);
				requestJobDiagnosticInfoXML.LastProgressTime = (base.TimeTracker.GetTimestamp(RequestJobTimestamp.LastProgressCheckpoint) ?? DateTime.MinValue);
				requestJobDiagnosticInfoXML.TimeTracker = base.TimeTracker.GetDiagnosticInfo(arguments);
			}
			if (base.ProgressTracker != null)
			{
				requestJobDiagnosticInfoXML.ProgressTracker = base.ProgressTracker.GetDiagnosticInfo(arguments);
			}
			if (!string.IsNullOrEmpty(jobPickupFailureMessage))
			{
				requestJobDiagnosticInfoXML.JobPickupFailureMessage = jobPickupFailureMessage;
			}
			base.DiagnosticInfo = requestJobDiagnosticInfoXML.Serialize(true);
		}

		private Report report;

		private LocalizedString positionInQueue;
	}
}
