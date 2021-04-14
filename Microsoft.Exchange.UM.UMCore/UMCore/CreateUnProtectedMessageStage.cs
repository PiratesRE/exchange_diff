using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class CreateUnProtectedMessageStage : SynchronousPipelineStageBase, IUMNetworkResource
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
				return TimeSpan.FromMinutes(3.0);
			}
		}

		protected override void InternalDoSynchronousWork()
		{
			IUMCreateMessage message = base.WorkItem.Message;
			ExAssert.RetailAssert(message != null, "CreateProtectedMessageStage must operate on PipelineContext which implements IUMCreateMessage");
			message.PrepareUnProtectedMessage();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CreateUnProtectedMessageStage>(this);
		}
	}
}
