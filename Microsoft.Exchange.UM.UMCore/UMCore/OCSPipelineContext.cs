using System;
using System.IO;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.OCS;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class OCSPipelineContext : PipelineContext, IUMCAMessage, IUMResolveCaller
	{
		internal OCSPipelineContext(string xmlData)
		{
			this.xmlData = xmlData;
			base.MessageType = "OCSNotification";
		}

		protected OCSPipelineContext(UserNotificationEvent evt, string xmlData) : base(new SubmissionHelper(evt.CallId, evt.CallerId, evt.Subscriber.ADRecipient.Guid, evt.Subscriber.DisplayName, evt.Subscriber.TelephonyCulture.ToString(), null, null, evt.TenantGuid))
		{
			this.notifEvent = evt;
			base.MessageType = "OCSNotification";
			this.xmlData = xmlData;
			base.MessageID = Util.GenerateMessageIdFromSeed(evt.CallId);
			base.SentTime = evt.Time;
			base.InitializeCallerIdAndTryGetDialPlan(this.CAMessageRecipient);
		}

		public UMRecipient CAMessageRecipient
		{
			get
			{
				if (this.notifEvent == null)
				{
					return null;
				}
				return this.notifEvent.Subscriber;
			}
		}

		public bool CollectMessageForAnalysis
		{
			get
			{
				return false;
			}
		}

		public ContactInfo ContactInfo
		{
			get
			{
				return this.contactInfo;
			}
			set
			{
				this.contactInfo = value;
			}
		}

		internal override Pipeline Pipeline
		{
			get
			{
				return TextPipeline.Instance;
			}
		}

		public override void PrepareUnProtectedMessage()
		{
			if (this.notifEvent.Subscriber.IsMissedCallNotificationEnabled)
			{
				base.PrepareUnProtectedMessage();
			}
		}

		public override string GetMailboxServerId()
		{
			return base.GetMailboxServerIdHelper();
		}

		public override string GetRecipientIdForThrottling()
		{
			return base.GetRecipientIdHelper();
		}

		public override void PostCompletion()
		{
			if (this.notifEvent.Subscriber.IsMissedCallNotificationEnabled)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "OCSPipelineContext.PostCompletion - Incrementing OCS user event notifications counter", new object[0]);
				Util.IncrementCounter(GeneralCounters.OCSUserEventNotifications);
			}
			base.PostCompletion();
		}

		internal static OCSPipelineContext Deserialize(string xmlData)
		{
			OCSPipelineContext result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				UserNotificationEvent userNotificationEvent = UserNotificationEvent.Deserialize(xmlData);
				disposeGuard.Add<UserNotificationEvent>(userNotificationEvent);
				OCSPipelineContext ocspipelineContext = new OCSPipelineContext(userNotificationEvent, xmlData);
				disposeGuard.Add<OCSPipelineContext>(ocspipelineContext);
				PipelineSubmitStatus pipelineSubmitStatus = PipelineDispatcher.Instance.CanSubmitWorkItem(userNotificationEvent.Subscriber.ADUser.ServerLegacyDN, userNotificationEvent.Subscriber.ADUser.DistinguishedName, PipelineDispatcher.ThrottledWorkItemType.NonCDRWorkItem);
				if (pipelineSubmitStatus != PipelineSubmitStatus.Ok)
				{
					string distinguishedName = userNotificationEvent.Subscriber.ADUser.DistinguishedName;
					throw new PipelineFullException(distinguishedName);
				}
				disposeGuard.Success();
				result = ocspipelineContext;
			}
			return result;
		}

		internal override void WriteCustomHeaderFields(StreamWriter headerStream)
		{
			headerStream.WriteLine("OCSNotificationData : " + this.xmlData);
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "OCSPipelineContext.Dispose() called", new object[0]);
					if (this.notifEvent != null)
					{
						this.notifEvent.Dispose();
						this.notifEvent = null;
					}
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<OCSPipelineContext>(this);
		}

		protected override void SetMessageProperties()
		{
			base.SetMessageProperties();
			this.notifEvent.RenderMessage(base.MessageToSubmit, this.ContactInfo);
		}

		protected override void WriteCommonHeaderFields(StreamWriter headerStream)
		{
		}

		private string xmlData;

		private UserNotificationEvent notifEvent;

		private ContactInfo contactInfo;
	}
}
