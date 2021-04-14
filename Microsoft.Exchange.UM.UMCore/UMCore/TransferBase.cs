using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class TransferBase
	{
		internal TransferBase(BaseUMCallSession session, CallContext context)
		{
			this.session = session;
			this.context = context;
		}

		protected BaseUMCallSession Session
		{
			get
			{
				return this.session;
			}
		}

		protected CallContext Context
		{
			get
			{
				return this.context;
			}
		}

		internal abstract void Transfer();

		protected abstract PlatformSipUri GetReferredBySipUri();

		protected abstract PlatformSipUri GetReferTargetUri(PhoneNumber phone, PlatformSipUri refByUri);

		protected PlatformSipUri GetSipUriFromSubscriber(UMSubscriber user)
		{
			string extension = user.Extension;
			if (string.IsNullOrEmpty(extension))
			{
				PIIMessage[] data = new PIIMessage[]
				{
					PIIMessage.Create(PIIType._User, user),
					PIIMessage.Create(PIIType._PhoneNumber, user.Extension)
				};
				CallIdTracer.TraceError(ExTraceGlobals.CallSessionTracer, this, data, "GetReferredBySipUri: Invalid SIPResourceIdentifier, User=_User, Extension=_PhoneNumber, CallType={0}", new object[]
				{
					this.Context.CallType
				});
				throw new InvalidOperationException();
			}
			return Platform.Builder.CreateSipUri("SIP:" + extension.Trim());
		}

		protected PlatformSipUri GetReferTargetForPhoneNumbers(PhoneNumber phone, PlatformSipUri refByUri)
		{
			PlatformSipUri platformSipUri = Platform.Builder.CreateSipUri(SipUriScheme.Sip, phone.RenderUserPart(this.context.DialPlan), this.GetReferToHostPart(refByUri));
			platformSipUri.UserParameter = UserParameter.Phone;
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "GetReferTargetForPhoneNumbers(refertarget={0}):", new object[]
			{
				platformSipUri
			});
			return platformSipUri;
		}

		protected void FrameTransferTargetAndTransferForSIPNames(PhoneNumber phone)
		{
			PlatformSipUri referredBySipUri = this.GetReferredBySipUri();
			PlatformSipUri referTargetUri = this.GetReferTargetUri(phone, referredBySipUri);
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phone);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "FrameTransferTargetAndTransfer(input=_PhoneNumber): type={0} target={1}, referred-by={2}", new object[]
			{
				phone.UriType,
				referTargetUri,
				referredBySipUri
			});
			if (referredBySipUri != null)
			{
				List<PlatformSignalingHeader> list = new List<PlatformSignalingHeader>();
				list.Add(Platform.Builder.CreateSignalingHeader("Referred-By", "<" + referredBySipUri.ToString() + ">"));
				this.session.TransferAsync(referTargetUri, list);
				return;
			}
			this.session.TransferAsync(referTargetUri);
		}

		private string GetReferToHostPart(PlatformSipUri referredByUri)
		{
			string text;
			if (this.Context.DialPlan.URIType != UMUriType.SipName && SipRoutingHelper.UseGlobalSBCSettingsForOutbound(this.Context.GatewayConfig))
			{
				text = this.Context.GatewayConfig.Address.ToString();
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "GetReferToHostPart: SBC case.", new object[0]);
			}
			else if (referredByUri != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "GetReferToHostPart: Using referred-by's host part", new object[0]);
				text = referredByUri.Host;
			}
			else if (this.Context.FromUriOfCall != null && this.Context.DialPlan.URIType == UMUriType.SipName)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "GetReferToHostPart: Using from's host part", new object[0]);
				text = this.Context.FromUriOfCall.Host;
			}
			else
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "GetReferToHostPart: Using sip peer's end point", new object[0]);
				text = this.Context.ImmediatePeer.ToString();
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "GetReferToHostPart: returning hostPart={0}", new object[]
			{
				text
			});
			return text;
		}

		private CallContext context;

		private BaseUMCallSession session;
	}
}
