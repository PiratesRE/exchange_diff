using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class BlindTransferToPhone : BlindTransferBase
	{
		internal BlindTransferToPhone(BaseUMCallSession session, CallContext context, PhoneNumber number) : base(session, context, number)
		{
		}

		protected override PlatformSipUri GetReferTargetUri(PhoneNumber phone, PlatformSipUri refByUri)
		{
			if (phone.UriType == UMUriType.SipName)
			{
				return Platform.Builder.CreateSipUri("SIP:" + phone.ToDial);
			}
			return base.GetReferTargetForPhoneNumbers(phone, refByUri);
		}

		protected override PlatformSipUri GetReferredBySipUri()
		{
			PlatformSipUri platformSipUri = null;
			if (base.Context.CallType == 3)
			{
				platformSipUri = base.GetSipUriFromSubscriber(base.Context.CallerInfo);
			}
			else if (!string.IsNullOrEmpty(base.Context.OCFeature.ReferredBy))
			{
				PlatformSignalingHeader platformSignalingHeader = Platform.Builder.CreateSignalingHeader("Referred-By", base.Context.OCFeature.ReferredBy);
				platformSipUri = platformSignalingHeader.ParseUri();
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, "GetReferredBySipUri: returning {0} for call type {1}", new object[]
			{
				platformSipUri,
				base.Context.CallType
			});
			return platformSipUri;
		}
	}
}
