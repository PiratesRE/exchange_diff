using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SubscriberAccessUtils
	{
		public static UMRecipient ResolveCaller(PhoneNumber callerId, UMRecipient callee, UMDialPlan dp)
		{
			UMRecipient result = null;
			try
			{
				PIIMessage data = PIIMessage.Create(PIIType._PhoneNumber, callerId.ToDial);
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, data, "SubscriberAccessUtils::ResolveCaller(): using number = _PhoneNumber", new object[0]);
				ADContactInfo adcontactInfo = null;
				UMSubscriber umsubscriber = callee as UMSubscriber;
				if (umsubscriber != null)
				{
					ADContactInfo.TryFindCallerByCallerId(umsubscriber, callerId, out adcontactInfo);
				}
				else
				{
					ADContactInfo.TryFindUmSubscriberByCallerId(dp, callerId, out adcontactInfo);
				}
				if (adcontactInfo != null)
				{
					result = UMRecipient.Factory.FromADRecipient<UMRecipient>(adcontactInfo.ADOrgPerson as ADRecipient);
				}
				else
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, data, "SubscriberAccessUtils::ResolveCallerFromCallerId():Failed to find the caller for callerid: '_PhoneNumber'.", new object[0]);
				}
			}
			catch (LocalizedException ex)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.CallSessionTracer, null, "SubscriberAccessUtils::ResolveCallerFromCallerId():Failed to find the caller : {0}.", new object[]
				{
					ex
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_UnableToResolveCallerToSubscriber, null, new object[]
				{
					callerId.ToDial,
					ex.Message
				});
			}
			return result;
		}
	}
}
