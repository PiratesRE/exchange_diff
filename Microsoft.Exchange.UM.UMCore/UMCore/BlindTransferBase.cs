using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class BlindTransferBase : TransferBase
	{
		protected BlindTransferBase(BaseUMCallSession session, CallContext context, PhoneNumber number) : base(session, context)
		{
			this.number = number;
		}

		protected PhoneNumber Number
		{
			get
			{
				return this.number;
			}
		}

		internal override void Transfer()
		{
			PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, this.number);
			CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, this, data, "BlindTransfer to: _PhoneNumber.", new object[0]);
			if (base.Context.DialPlan.URIType == UMUriType.SipName || this.number.UriType == UMUriType.SipName)
			{
				base.FrameTransferTargetAndTransferForSIPNames(this.number);
				return;
			}
			base.Session.TransferAsync(base.GetReferTargetForPhoneNumbers(this.number, null));
		}

		private PhoneNumber number;
	}
}
