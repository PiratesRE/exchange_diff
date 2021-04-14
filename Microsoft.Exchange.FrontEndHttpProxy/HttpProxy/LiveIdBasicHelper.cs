using System;
using System.Security.Principal;
using System.Web;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.HttpProxy;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.HttpProxy
{
	internal static class LiveIdBasicHelper
	{
		public static IIdentity GetCallerIdentity(HttpContext httpContext)
		{
			ADRawEntry callerAdEntry = LiveIdBasicHelper.GetCallerAdEntry(httpContext);
			SecurityIdentifier securityIdentifier = callerAdEntry[ADMailboxRecipientSchema.Sid] as SecurityIdentifier;
			OrganizationId organizationId = (OrganizationId)callerAdEntry[ADObjectSchema.OrganizationId];
			return new GenericSidIdentity(securityIdentifier.ToString(), "LiveIdBasic", securityIdentifier, organizationId.PartitionId.ToString());
		}

		private static ADRawEntry GetCallerAdEntry(HttpContext httpContext)
		{
			if (!httpContext.Items.Contains(Constants.CallerADRawEntryKeyName))
			{
				CommonAccessToken commonAccessToken = httpContext.Items["Item-CommonAccessToken"] as CommonAccessToken;
				if (commonAccessToken == null)
				{
					throw new InvalidOperationException("CAT token not present - cannot lookup LiveIdBasic user's AD entry.");
				}
				ADRawEntry value = null;
				LatencyTracker latencyTracker = (LatencyTracker)httpContext.Items[Constants.LatencyTrackerContextKeyName];
				LiveIdBasicTokenAccessor accessor = LiveIdBasicTokenAccessor.Attach(commonAccessToken);
				if (accessor.TokenType == AccessTokenType.LiveIdBasic)
				{
					ExTraceGlobals.VerboseTracer.TraceDebug<string, string>(0L, "[Extensions::GetFullCallerIdentity] Calling AD to convert PUID {0} for LiveIdMemberName {1} to SID to construct GenericSidIdentity.", accessor.Puid, accessor.LiveIdMemberName);
					ITenantRecipientSession session = DirectoryHelper.GetTenantRecipientSessionFromSmtpOrLiveId(latencyTracker, accessor.LiveIdMemberName, false);
					value = DirectoryHelper.InvokeAccountForest(latencyTracker, () => session.FindUniqueEntryByNetID(accessor.Puid, null, UserBasedAnchorMailbox.ADRawEntryPropertySet));
				}
				httpContext.Items[Constants.CallerADRawEntryKeyName] = value;
			}
			return (ADRawEntry)httpContext.Items[Constants.CallerADRawEntryKeyName];
		}
	}
}
