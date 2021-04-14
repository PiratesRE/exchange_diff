using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SupervisedTransfer : TransferBase
	{
		internal SupervisedTransfer(BaseUMCallSession session, CallContext context, PhoneNumber number, UMSubscriber referrer) : base(session, context)
		{
			this.number = number;
			this.referrer = referrer;
		}

		internal override void Transfer()
		{
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, this.number);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "SupervisedTransfer to: _PhoneNumber.", new object[0]);
			if (base.Context.DialPlan.URIType == UMUriType.SipName)
			{
				base.FrameTransferTargetAndTransferForSIPNames(this.number);
				return;
			}
			if (SipRoutingHelper.UseGlobalSBCSettingsForOutbound(base.Context.GatewayConfig))
			{
				base.Session.TransferAsync(base.GetReferTargetForPhoneNumbers(this.number, null));
				return;
			}
			base.Session.TransferAsync();
		}

		protected override PlatformSipUri GetReferredBySipUri()
		{
			return base.GetSipUriFromSubscriber(this.referrer);
		}

		protected override PlatformSipUri GetReferTargetUri(PhoneNumber phone, PlatformSipUri refByUri)
		{
			return null;
		}

		private UMSubscriber referrer;

		private PhoneNumber number;
	}
}
