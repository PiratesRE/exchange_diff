using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Services;

namespace Microsoft.Exchange.Data.ApplicationLogic.Directory
{
	internal class RecipientHelper
	{
		internal static SecurityIdentifier TryGetMasterAccountSid(ADRecipient adRecipient)
		{
			SecurityIdentifier result;
			if (adRecipient.IsLinked && adRecipient.MasterAccountSid != null && !RecipientHelper.IsWellKnownSid(adRecipient.MasterAccountSid))
			{
				RecipientHelper.Tracer.TraceDebug<ADRecipient, SecurityIdentifier>(0L, "RecipientIdentity {0} has MasterAccountSid: {1}", adRecipient, adRecipient.MasterAccountSid);
				result = adRecipient.MasterAccountSid;
			}
			else
			{
				RecipientHelper.Tracer.TraceDebug<ADRecipient>(0L, "RecipientIdentity {0} has no MasterAccountSid.", adRecipient);
				result = null;
			}
			return result;
		}

		internal static bool IsWellKnownSid(SecurityIdentifier sid)
		{
			bool result = false;
			foreach (WellKnownSidType wellKnownSidType in RecipientHelper.wellKnownSidTypes)
			{
				if (sid.IsWellKnown(wellKnownSidType))
				{
					RecipientHelper.Tracer.TraceDebug<SecurityIdentifier, WellKnownSidType>(0L, "sid {0} is well known sid: {1}", sid, wellKnownSidType);
					result = true;
					break;
				}
			}
			return result;
		}

		private static readonly Trace Tracer = ExTraceGlobals.CommonAlgorithmTracer;

		private static WellKnownSidType[] wellKnownSidTypes = (WellKnownSidType[])Enum.GetValues(typeof(WellKnownSidType));
	}
}
