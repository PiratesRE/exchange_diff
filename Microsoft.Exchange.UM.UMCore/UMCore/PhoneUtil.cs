using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PhoneUtil
	{
		internal static void SetTransferTargetPhone(ActivityManager manager, TransferExtension ext, PhoneNumber phone)
		{
			PhoneUtil.SetTransferTargetPhone(manager, ext, phone, null);
		}

		internal static void SetTransferTargetPhone(ActivityManager manager, TransferExtension ext, PhoneNumber phone, ContactInfo targetContactInfo)
		{
			PIIMessage[] data = new PIIMessage[]
			{
				PIIMessage.Create(PIIType._PhoneNumber, phone),
				PIIMessage.Create(PIIType._PII, targetContactInfo)
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.AutoAttendantTracer, manager, data, "Setting transfer extension = _PhoneNumber Type = {0}. TargetContactInfo = _PII", new object[]
			{
				ext
			});
			manager.TargetPhoneNumber = phone;
			manager.WriteVariable("targetContactInfo", targetContactInfo);
			manager.WriteVariable("transferExtension", ext);
		}
	}
}
