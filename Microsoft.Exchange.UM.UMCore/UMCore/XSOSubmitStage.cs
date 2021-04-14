using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class XSOSubmitStage : SubmitStage, IUMNetworkResource
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
			XSOVoiceMessagePipelineContext xsovoiceMessagePipelineContext = base.WorkItem.Message as XSOVoiceMessagePipelineContext;
			ExAssert.RetailAssert(xsovoiceMessagePipelineContext != null, "XSOSubmitStage must operate on PipelineContext which is an instance of XSOVoiceMessagePipelineContext");
			UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = null;
			try
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Inside XSOSubmitStage", new object[0]);
				mailboxSessionLock = xsovoiceMessagePipelineContext.Caller.CreateSessionLock();
				if (mailboxSessionLock.Session == null || mailboxSessionLock.UnderlyingStoreRPCSessionDisconnected)
				{
					throw new WorkItemNeedsToBeRequeuedException();
				}
				base.MessageToSubmit.Send();
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Successfully submitted the message via xso.", new object[0]);
			}
			finally
			{
				if (mailboxSessionLock != null)
				{
					mailboxSessionLock.Dispose();
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<XSOSubmitStage>(this);
		}
	}
}
