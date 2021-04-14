using System;
using System.Globalization;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class BlindTransferToHost : BlindTransferBase
	{
		internal BlindTransferToHost(BaseUMCallSession session, CallContext context, PhoneNumber number, PlatformSipUri referredByUri) : base(session, context, number)
		{
			this.referredByUri = referredByUri;
		}

		protected override PlatformSipUri GetReferredBySipUri()
		{
			return this.referredByUri;
		}

		protected override PlatformSipUri GetReferTargetUri(PhoneNumber phone, PlatformSipUri refByUri)
		{
			PlatformSipUri platformSipUri = Platform.Builder.CreateSipUri(string.Format(CultureInfo.InvariantCulture, "sip:{0}", new object[]
			{
				phone.ToDial
			}));
			if (!string.IsNullOrEmpty(platformSipUri.User) && UtilityMethods.IsAnonymousNumber(platformSipUri.User))
			{
				platformSipUri.User = "Anonymous";
			}
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, phone);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "TransferToHost::GetReferTargetUri() phone = _PhoneNumber returning {0}", new object[]
			{
				platformSipUri
			});
			return platformSipUri;
		}

		private PlatformSipUri referredByUri;
	}
}
