using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class PipelineStageBase : DisposableBase
	{
		internal PipelineStageBase()
		{
		}

		internal bool MarkedForLastChanceHandling { get; set; }

		internal PipelineWorkItem WorkItem
		{
			get
			{
				return this.workItem;
			}
		}

		internal StageCompletionCallback StageCompletionCallback
		{
			get
			{
				return this.callback;
			}
		}

		internal abstract PipelineDispatcher.PipelineResourceType ResourceType { get; }

		internal abstract TimeSpan ExpectedRunTime { get; }

		internal virtual StageRetryDetails RetrySchedule
		{
			get
			{
				if (this.retrySchedule == null)
				{
					this.retrySchedule = this.InternalGetRetrySchedule();
				}
				return this.retrySchedule;
			}
		}

		internal virtual bool ShouldRunStage(PipelineWorkItem workItem)
		{
			return true;
		}

		internal virtual void Initialize(PipelineWorkItem workItem)
		{
			this.workItem = workItem;
		}

		internal void DispatchWorkAsync(StageCompletionCallback completionCallback)
		{
			this.callback = completionCallback;
			this.RetrySchedule.UpgdateStageRunTimestamp();
			this.InternalDispatchWorkAsync();
		}

		protected virtual void ReportFailure()
		{
		}

		protected virtual StageRetryDetails InternalGetRetrySchedule()
		{
			return new StageRetryDetails(StageRetryDetails.FinalAction.DropMessage, TimeSpan.FromMinutes(10.0), TimeSpan.FromDays(1.0));
		}

		protected abstract void InternalDispatchWorkAsync();

		protected override void InternalDispose(bool disposing)
		{
		}

		private PipelineWorkItem workItem;

		private StageCompletionCallback callback;

		private StageRetryDetails retrySchedule;
	}
}
