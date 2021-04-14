using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CreateProtectedMessageStage : SynchronousPipelineStageBase, IUMNetworkResource
	{
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
				return TimeSpan.FromMinutes(5.0);
			}
		}

		protected override void ReportFailure()
		{
			IUMCreateMessage message = base.WorkItem.Message;
			ExAssert.RetailAssert(message != null, "CreateProtectedMessageStage must operate on PipelineContext which implements IUMCreateMessage");
			message.PrepareNDRForFailureToGenerateProtectedMessage();
		}

		protected override StageRetryDetails InternalGetRetrySchedule()
		{
			return new StageRetryDetails(StageRetryDetails.FinalAction.SkipStage, TimeSpan.FromMinutes(10.0), TimeSpan.FromDays(1.0));
		}

		protected override void InternalDoSynchronousWork()
		{
			IUMCreateMessage message = base.WorkItem.Message;
			ExAssert.RetailAssert(message != null, "CreateProtectedMessageStage must operate on PipelineContext which implements IUMCreateMessage");
			message.PrepareProtectedMessage();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CreateProtectedMessageStage>(this);
		}
	}
}
