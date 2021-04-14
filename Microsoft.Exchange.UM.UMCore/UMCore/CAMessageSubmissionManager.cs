using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class CAMessageSubmissionManager : ActivityManager
	{
		internal CAMessageSubmissionManager(ActivityManager manager, ActivityManagerConfig config) : base(manager, config)
		{
			this.mbxRecipient = (manager.CallSession.CurrentCallContext.CalleeInfo as UMMailboxRecipient);
		}

		protected bool IsMailboxOverQuota
		{
			get
			{
				return this.isMailboxOverQuota;
			}
		}

		protected bool PipelineHealthy
		{
			get
			{
				return this.pipelineHealthy;
			}
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "CAMessageSubmissionManager class asked to ExecuteAction={0}.", new object[]
			{
				action
			});
			string input;
			if (string.Equals(action, "isQuotaExceeded", StringComparison.OrdinalIgnoreCase))
			{
				input = this.IsMailboxQuotaExceeded(vo);
			}
			else if (string.Equals(action, "isPipelineHealthy", StringComparison.OrdinalIgnoreCase))
			{
				input = this.IsPipelineHealthy(vo);
			}
			else if (string.Equals(action, "canAnnonLeaveMessage", StringComparison.OrdinalIgnoreCase))
			{
				input = this.CanAnnonLeaveMessage(vo);
			}
			else
			{
				if (!string.Equals(action, "HandleFailedTransfer", StringComparison.OrdinalIgnoreCase))
				{
					return base.ExecuteAction(action, vo);
				}
				input = this.HandleFailedTransfer(vo);
			}
			return base.CurrentActivity.GetTransition(input);
		}

		internal virtual string HandleFailedTransfer(BaseUMCallSession callSession)
		{
			return null;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<CAMessageSubmissionManager>(this);
		}

		private string IsMailboxQuotaExceeded(BaseUMCallSession vo)
		{
			string result = null;
			if (vo.CurrentCallContext.IsDiagnosticCall)
			{
				return "quotaNotExceeded";
			}
			if (vo.CurrentCallContext.UmSubscriberData != null && vo.CurrentCallContext.UmSubscriberData.IsQuotaExceeded)
			{
				this.isMailboxOverQuota = true;
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_QuotaExceededFailedSubmit, null, new object[]
				{
					vo.CurrentCallContext.CalleeInfo,
					vo.CallId
				});
				PIIMessage data = PIIMessage.Create(PIIType._Callee, vo.CurrentCallContext.CalleeInfo);
				CallIdTracer.TraceWarning(ExTraceGlobals.VoiceMailTracer, this, data, "Quota Exceeded. Failed to submit message for user _Callee.", new object[0]);
			}
			else
			{
				result = "quotaNotExceeded";
			}
			return result;
		}

		private string IsPipelineHealthy(BaseUMCallSession vo)
		{
			string result = null;
			if (vo.CurrentCallContext.IsDiagnosticCall)
			{
				return "pipelineHealthy";
			}
			PipelineSubmitStatus pipelineSubmitStatus = PipelineDispatcher.Instance.CanSubmitWorkItem((this.mbxRecipient != null) ? this.mbxRecipient.ADUser.ServerLegacyDN : "af360a7e-e6d4-494a-ac69-6ae14896d16b", (this.mbxRecipient != null) ? this.mbxRecipient.ADUser.DistinguishedName : "455e5330-ce1f-48d1-b6b1-2e318d2ff2c4", PipelineDispatcher.ThrottledWorkItemType.NonCDRWorkItem);
			this.pipelineHealthy = (pipelineSubmitStatus == PipelineSubmitStatus.Ok);
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "IsPipelineHealthy {0}  SubmitStatus {1}", new object[]
			{
				this.pipelineHealthy,
				pipelineSubmitStatus
			});
			if (this.pipelineHealthy)
			{
				result = "pipelineHealthy";
				UMEventNotificationHelper.PublishUMSuccessEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.UMPipelineFull.ToString());
			}
			else
			{
				vo.IncrementCounter(CallAnswerCounters.CallsMissedBecausePipelineNotHealthy);
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_FailedSubmitSincePipelineIsFull, null, new object[]
				{
					vo.CurrentCallContext.CalleeInfo.ADRecipient.DistinguishedName,
					vo.CallId
				});
				CallIdTracer.TraceWarning(ExTraceGlobals.VoiceMailTracer, this, "IsPipelineHealthy {0}", new object[]
				{
					this.pipelineHealthy
				});
				if (pipelineSubmitStatus == PipelineSubmitStatus.PipelineFull)
				{
					UMEventNotificationHelper.PublishUMFailureEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.UMPipelineFull.ToString());
				}
			}
			return result;
		}

		private string CanAnnonLeaveMessage(BaseUMCallSession vo)
		{
			string result = null;
			UMSubscriber umsubscriber = this.mbxRecipient as UMSubscriber;
			if (umsubscriber == null || vo.CurrentCallContext.IsDiagnosticCall || umsubscriber.CanAnonymousCallersLeaveMessage)
			{
				result = "annonCanLeaveMessage";
			}
			return result;
		}

		private bool isMailboxOverQuota;

		private bool pipelineHealthy = true;

		private UMMailboxRecipient mbxRecipient;
	}
}
