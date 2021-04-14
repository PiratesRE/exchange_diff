using System;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics.Components.Security;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.OAuth
{
	internal static class OAuthActAsUserExtensions
	{
		internal static SecurityIdentifier GetMasterAccountSidIfAvailable(this OAuthActAsUser oauthActAsUser)
		{
			return oauthActAsUser.GetMasterAccountSidIfAvailable(OAuthActAsUserExtensions.useMasterAccountSid.Value);
		}

		internal static SecurityIdentifier GetMasterAccountSidIfAvailable(this OAuthActAsUser oauthActAsUser, bool useMasterAccountSid)
		{
			SecurityIdentifier result = null;
			if (oauthActAsUser != null)
			{
				result = oauthActAsUser.Sid;
				if (useMasterAccountSid && OAuthActAsUserExtensions.IsValidMasterAccountSid(oauthActAsUser.MasterAccountSid))
				{
					result = oauthActAsUser.MasterAccountSid;
				}
			}
			return result;
		}

		private static bool IsValidMasterAccountSid(SecurityIdentifier masterAccountSid)
		{
			return masterAccountSid != null && !masterAccountSid.Equals(OAuthActAsUserExtensions.selfSidSentinel);
		}

		private static BoolAppSettingsEntry useMasterAccountSid = new BoolAppSettingsEntry("OAuthHttpModule.UseMasterAccountSid", false, ExTraceGlobals.OAuthTracer);

		private static readonly SecurityIdentifier selfSidSentinel = new SecurityIdentifier(WellKnownSidType.SelfSid, null);
	}
}
