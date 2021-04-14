using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class HealthCheckStage : SynchronousPipelineStageBase, IUMNetworkResource
	{
		internal override PipelineDispatcher.PipelineResourceType ResourceType
		{
			get
			{
				if (this.resourceType == null)
				{
					this.resourceType = new PipelineDispatcher.PipelineResourceType?(this.HealthContext.ResourceBeingChecked);
				}
				return this.resourceType.Value;
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

		private HealthCheckPipelineContext HealthContext
		{
			get
			{
				return (HealthCheckPipelineContext)base.WorkItem.Message;
			}
		}

		protected override void InternalDoSynchronousWork()
		{
			this.HealthContext.Passed(this.ResourceType);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "HealthCheckStage passed for {0}", new object[]
			{
				this.ResourceType
			});
		}

		private PipelineDispatcher.PipelineResourceType? resourceType = null;
	}
}
