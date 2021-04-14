using System;
using System.Diagnostics;
using Microsoft.Exchange.Search.Core.Diagnostics;

namespace Microsoft.Exchange.Search.Fast
{
	internal class TransportFlowStatistics
	{
		public object SyncRoot
		{
			[DebuggerStepThrough]
			get
			{
				return this.syncRoot;
			}
		}

		public TimeSpan TotalTimeProcessingMessages { get; private set; }

		public TimeSpan TimeInGetConnection { get; private set; }

		public TimeSpan TimeInPropertyBagLoad { get; private set; }

		public TimeSpan TimeInMessageItemConversion { get; private set; }

		public TimeSpan TimeDeterminingAgeOfItem { get; private set; }

		public TimeSpan TimeInMimeConversion { get; private set; }

		public TimeSpan TimeInShouldAnnotateMessage { get; private set; }

		public TimeSpan TimeProcessingFailedMessages { get; private set; }

		public TimeSpan TimeInQueue { get; private set; }

		public TimeSpan TimeInTransportRetriever { get; private set; }

		public TimeSpan TimeInDocParser { get; private set; }

		public TimeSpan TimeInWordbreaker { get; private set; }

		public TimeSpan TimeInNLGSubflow { get; private set; }

		public int TotalMessagesProcessed { get; private set; }

		public int MessagesSuccessfullyAnnotated { get; private set; }

		public int ConnectionLevelFailures { get; private set; }

		public int AnnotationSkipped { get; private set; }

		public int MessageLevelFailures { get; private set; }

		public void UpdateProcessingTimes(TimeSpan elapsed, TransportFlowStatistics.ProcessingStatus status)
		{
			lock (this.syncRoot)
			{
				this.TotalTimeProcessingMessages += elapsed;
				switch (status)
				{
				case TransportFlowStatistics.ProcessingStatus.Success:
					this.TotalMessagesProcessed++;
					this.MessagesSuccessfullyAnnotated++;
					break;
				case TransportFlowStatistics.ProcessingStatus.AnnotationSkipped:
					this.AnnotationSkipped++;
					break;
				case TransportFlowStatistics.ProcessingStatus.FailedToConnect:
					this.TotalMessagesProcessed++;
					this.ConnectionLevelFailures++;
					break;
				case TransportFlowStatistics.ProcessingStatus.FailedToProcess:
					this.TotalMessagesProcessed++;
					this.MessageLevelFailures++;
					this.TimeProcessingFailedMessages += elapsed;
					break;
				default:
					throw new ArgumentException("status");
				}
			}
		}

		public void UpdateOperatorTimings(TransportFlowOperatorTimings transportFlowOperationResult)
		{
			lock (this.syncRoot)
			{
				this.TimeInQueue += TimeSpan.FromMilliseconds((double)transportFlowOperationResult.TimeInQueueInMsec);
				this.TimeInDocParser += TimeSpan.FromMilliseconds((double)transportFlowOperationResult.TimeInDocParserInMsec);
				this.TimeInWordbreaker += TimeSpan.FromMilliseconds((double)transportFlowOperationResult.TimeInWordbreakerInMsec);
				this.TimeInNLGSubflow += TimeSpan.FromMilliseconds((double)transportFlowOperationResult.TimeInNLGSubflowInMsec);
				this.TimeInTransportRetriever += TimeSpan.FromMilliseconds((double)transportFlowOperationResult.TimeInTransportRetrieverInMsec);
			}
		}

		public void UpdateClientOperationTimings(ClientSideTimings clientSideTimings)
		{
			lock (this.syncRoot)
			{
				this.TimeInGetConnection += clientSideTimings.TimeInGetConnection;
				this.TimeInPropertyBagLoad += clientSideTimings.TimeInPropertyBagLoad;
				this.TimeInMessageItemConversion += clientSideTimings.TimeInMessageItemConversion;
				this.TimeDeterminingAgeOfItem += clientSideTimings.TimeDeterminingAgeOfItem;
				this.TimeInMimeConversion += clientSideTimings.TimeInMimeConversion;
				this.TimeInShouldAnnotateMessage += clientSideTimings.TimeInShouldAnnotateMessage;
			}
		}

		private readonly object syncRoot = new object();

		public enum ProcessingStatus
		{
			Success,
			AnnotationSkipped,
			FailedToConnect,
			FailedToProcess
		}
	}
}
