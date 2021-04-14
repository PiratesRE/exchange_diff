using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ResolveCallerStage : SynchronousPipelineStageBase, IUMNetworkResource
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
				return TimeSpan.FromMinutes(1.0);
			}
		}

		internal static void ResolveCaller(PipelineContext pipelineContext)
		{
			ExAssert.RetailAssert(pipelineContext is IUMCAMessage && pipelineContext is IUMResolveCaller, "ResolveCallerStage must operate on PipelineContext which implements IUMCAMessage and IUMResolveCaller. {0}", new object[]
			{
				pipelineContext.GetType().ToString()
			});
			IUMCAMessage iumcamessage = (IUMCAMessage)pipelineContext;
			IUMResolveCaller iumresolveCaller = (IUMResolveCaller)pipelineContext;
			iumresolveCaller.ContactInfo = new DefaultContactInfo();
			UMSubscriber umsubscriber = iumcamessage.CAMessageRecipient as UMSubscriber;
			bool flag = false;
			PIIMessage piimessage = PIIMessage.Create(PIIType._PhoneNumber, pipelineContext.CallerId.ToDial);
			if (!string.IsNullOrEmpty(pipelineContext.CallerAddress))
			{
				IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromUmUser(iumcamessage.CAMessageRecipient);
				IADOrgPerson iadorgPerson = iadrecipientLookup.LookupBySmtpAddress(pipelineContext.CallerAddress) as IADOrgPerson;
				if (iadorgPerson != null)
				{
					iumresolveCaller.ContactInfo = new ADContactInfo(iadorgPerson, umsubscriber.DialPlan, pipelineContext.CallerId);
					PIIMessage[] data = new PIIMessage[]
					{
						piimessage,
						PIIMessage.Create(PIIType._PII, pipelineContext.CallerAddress)
					};
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, data, "ResolveCaller:SMTPLookup: callerId:_PhoneNumber callerAddress_PII ADOrgPerson:{0}", new object[]
					{
						iadorgPerson
					});
					flag = true;
				}
			}
			if (!flag)
			{
				ContactInfo contactInfo = ContactInfo.FindContactByCallerId(umsubscriber, pipelineContext.CallerId);
				if (contactInfo != null)
				{
					flag = true;
					iumresolveCaller.ContactInfo = contactInfo;
					PIIMessage[] data2 = new PIIMessage[]
					{
						piimessage,
						PIIMessage.Create(PIIType._UserDisplayName, iumresolveCaller.ContactInfo.DisplayName)
					};
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, data2, "ResolveCaller::Contact Search() callerId:_PhoneNumber Contact: '_UserDisplayName'", new object[0]);
				}
			}
			if (GlobCfg.EnableCallerIdDisplayNameResolution && !flag && !string.IsNullOrEmpty(pipelineContext.CallerIdDisplayName))
			{
				iumresolveCaller.ContactInfo = new CallerNameDisplayContactInfo(pipelineContext.CallerIdDisplayName);
				PIIMessage[] data3 = new PIIMessage[]
				{
					piimessage,
					PIIMessage.Create(PIIType._UserDisplayName, iumresolveCaller.ContactInfo.DisplayName)
				};
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, null, data3, "ResolveCaller::CallerID:_PhoneNumber resolved using CallerNameDisplay: '_UserDisplayName'", new object[0]);
			}
			CallContext.UpdateCountersAndPercentages(flag, GeneralCounters.CallerResolutionsSucceeded, GeneralCounters.CallerResolutionsAttempted, GeneralCounters.PercentageSuccessfulCallerResolutions, GeneralCounters.PercentageSuccessfulCallerResolutions_Base);
		}

		internal override bool ShouldRunStage(PipelineWorkItem workItem)
		{
			IUMResolveCaller iumresolveCaller = workItem.Message as IUMResolveCaller;
			return iumresolveCaller.ContactInfo == null;
		}

		protected override StageRetryDetails InternalGetRetrySchedule()
		{
			return new StageRetryDetails(StageRetryDetails.FinalAction.SkipStage);
		}

		protected override void InternalDoSynchronousWork()
		{
			try
			{
				ResolveCallerStage.ResolveCaller(base.WorkItem.Message);
			}
			catch (Exception ex)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "Got exception while resolving caller e={0}", new object[]
				{
					ex
				});
				CallContext.UpdateCountersAndPercentages(false, GeneralCounters.CallerResolutionsSucceeded, GeneralCounters.CallerResolutionsAttempted, GeneralCounters.PercentageSuccessfulCallerResolutions, GeneralCounters.PercentageSuccessfulCallerResolutions_Base);
				throw;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<ResolveCallerStage>(this);
		}
	}
}
