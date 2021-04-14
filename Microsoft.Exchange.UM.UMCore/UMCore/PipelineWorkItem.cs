using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PipelineWorkItem : DisposableBase
	{
		private PipelineWorkItem(PipelineContext msg, Guid workId)
		{
			this.workId = workId;
			this.message = msg;
			this.statisticsLogRow.SentTime = this.Message.SentTime.UniversalTime;
			this.statisticsLogRow.WorkId = this.WorkId;
			this.statisticsLogRow.MessageType = this.Message.MessageType;
			this.InitializePipeline();
			this.creationTime = new ExDateTime(ExTimeZone.CurrentTimeZone, File.GetCreationTime(this.HeaderFilename));
		}

		internal PipelineStageBase CurrentStage
		{
			get
			{
				return this.myPipeline[this.pipelineStageNum];
			}
		}

		internal TimeSpan ExpectedRunTime
		{
			get
			{
				return this.expectedRunTime;
			}
		}

		internal bool SLARecorded { get; set; }

		internal bool IsRejected { get; set; }

		internal Guid WorkId
		{
			get
			{
				return this.workId;
			}
		}

		internal string HeaderFilename
		{
			get
			{
				return this.message.HeaderFileName;
			}
		}

		internal PipelineContext Message
		{
			get
			{
				return this.message;
			}
		}

		internal bool IsComplete
		{
			get
			{
				return this.pipelineStageNum >= this.myPipeline.Count;
			}
		}

		internal bool IsRunning
		{
			get
			{
				return this.isRunning;
			}
			set
			{
				this.isRunning = value;
			}
		}

		internal bool HeaderFileExists
		{
			get
			{
				bool result;
				try
				{
					result = File.Exists(this.HeaderFilename);
				}
				catch (IOException)
				{
					result = false;
				}
				return result;
			}
		}

		internal TimeSpan TimeInQueue
		{
			get
			{
				return ExDateTime.Now.Subtract(this.creationTime);
			}
		}

		internal TranscriptionContext TranscriptionContext
		{
			get
			{
				return this.transcriptionContext;
			}
		}

		internal PipelineStatisticsLogger.PipelineStatisticsLogRow PipelineStatisticsLogRow
		{
			get
			{
				return this.statisticsLogRow;
			}
		}

		public override bool Equals(object obj)
		{
			PipelineWorkItem pipelineWorkItem = obj as PipelineWorkItem;
			return pipelineWorkItem != null && this.WorkId.Equals(pipelineWorkItem.WorkId);
		}

		public override int GetHashCode()
		{
			return this.WorkId.GetHashCode();
		}

		public PipelineDispatcher.WIThrottleData GetThrottlingData()
		{
			if (this.message != null)
			{
				return this.message.GetThrottlingData();
			}
			return null;
		}

		internal static bool TryCreate(FileInfo diskQueueItem, Guid workId, out PipelineWorkItem workItem)
		{
			workItem = null;
			PipelineContext pipelineContext = null;
			try
			{
				pipelineContext = PipelineContext.FromHeaderFile(diskQueueItem.FullName);
				if (pipelineContext != null)
				{
					if (pipelineContext.ProcessedCount >= PipelineWorkItem.ProcessedCountMax)
					{
						throw new ReachMaxProcessedTimesException(PipelineWorkItem.ProcessedCountMax.ToString(CultureInfo.InvariantCulture));
					}
					workItem = new PipelineWorkItem(pipelineContext, workId);
				}
			}
			finally
			{
				if (workItem == null && pipelineContext != null)
				{
					pipelineContext.Dispose();
					pipelineContext = null;
				}
			}
			return null != workItem;
		}

		internal void AdvanceToNextStage()
		{
			do
			{
				this.pipelineStageNum++;
			}
			while (!this.IsComplete && !this.CurrentStage.ShouldRunStage(this));
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing)
			{
				if (this.myPipeline != null)
				{
					foreach (PipelineStageBase pipelineStageBase in this.myPipeline)
					{
						pipelineStageBase.Dispose();
					}
				}
				if (this.message != null)
				{
					this.message.Dispose();
				}
				TranscriptionStage.UpdateBacklog(-this.transcriptionContext.BacklogContribution);
				this.transcriptionContext.BacklogContribution = TimeSpan.Zero;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PipelineWorkItem>(this);
		}

		private void InitializePipeline()
		{
			this.myPipeline = new List<PipelineStageBase>(this.message.Pipeline.Count + 1);
			for (int i = 0; i < this.message.Pipeline.Count; i++)
			{
				IPipelineStageFactory factory = this.message.Pipeline[i];
				this.AddStage(factory);
			}
			this.AddStage(PipelineWorkItem.PostCompletionStage.Factory);
		}

		private void AddStage(IPipelineStageFactory factory)
		{
			PipelineStageBase pipelineStageBase = factory.CreateStage(this);
			this.expectedRunTime += pipelineStageBase.ExpectedRunTime;
			this.myPipeline.Add(pipelineStageBase);
		}

		internal static readonly int ProcessedCountMax = 6;

		private Guid workId = Guid.Empty;

		private int pipelineStageNum;

		private bool isRunning;

		private PipelineContext message;

		private List<PipelineStageBase> myPipeline;

		private TranscriptionContext transcriptionContext = new TranscriptionContext();

		private PipelineStatisticsLogger.PipelineStatisticsLogRow statisticsLogRow = new PipelineStatisticsLogger.PipelineStatisticsLogRow();

		private ExDateTime creationTime;

		private TimeSpan expectedRunTime;

		private class PostCompletionStage : SynchronousPipelineStageBase, IUMNetworkResource
		{
			public static IPipelineStageFactory Factory
			{
				get
				{
					return PipelineWorkItem.PostCompletionStage.factory;
				}
			}

			internal override PipelineDispatcher.PipelineResourceType ResourceType
			{
				get
				{
					return PipelineDispatcher.PipelineResourceType.NetworkBound;
				}
			}

			public string NetworkResourceId
			{
				get
				{
					return base.WorkItem.Message.GetMailboxServerId();
				}
			}

			internal override TimeSpan ExpectedRunTime
			{
				get
				{
					return TimeSpan.FromMinutes(1.0);
				}
			}

			protected override StageRetryDetails InternalGetRetrySchedule()
			{
				return new StageRetryDetails(StageRetryDetails.FinalAction.SkipStage);
			}

			protected override void InternalDoSynchronousWork()
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "PostCompletionStage - InternalDoSynchronousWork", new object[0]);
				base.WorkItem.Message.PostCompletion();
			}

			protected override DisposeTracker InternalGetDisposeTracker()
			{
				return DisposeTracker.Get<PipelineWorkItem.PostCompletionStage>(this);
			}

			protected override void InternalDispose(bool disposing)
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "PostCompletionStage.InternalDispose", new object[0]);
					MessageItem messageToSubmit = base.WorkItem.Message.MessageToSubmit;
					if (messageToSubmit != null)
					{
						CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "PostCompletionStage - Disposing message item", new object[0]);
						messageToSubmit.Dispose();
					}
				}
				base.InternalDispose(disposing);
			}

			private static IPipelineStageFactory factory = new PipelineStageFactory<PipelineWorkItem.PostCompletionStage>();
		}
	}
}
