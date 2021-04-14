using System;
using System.Security.Principal;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Availability;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.InfoWorker.Availability
{
	internal static class LocalForestConfiguration
	{
		internal static bool IsValidPrincipal(ClientSecurityContext callingPrincipal, ADRecipient userOrGroupAccount)
		{
			bool flag = false;
			if (userOrGroupAccount == null)
			{
				LocalForestConfiguration.ConfigurationTracer.TraceError(0L, "Unable to get the configured account object for checking validity of calling principal.");
				return false;
			}
			if (userOrGroupAccount.RecipientType != RecipientType.User && userOrGroupAccount.RecipientType != RecipientType.UserMailbox && userOrGroupAccount.RecipientType != RecipientType.Group)
			{
				LocalForestConfiguration.ConfigurationTracer.TraceError<ADRecipient>(0L, "The configured account {0} object is neither a user nor a group.", userOrGroupAccount);
				return false;
			}
			if (userOrGroupAccount.RecipientType != RecipientType.Group)
			{
				flag = object.Equals(callingPrincipal.UserSid, ((ADUser)userOrGroupAccount).Sid);
				if (!flag)
				{
					LocalForestConfiguration.ConfigurationTracer.TraceError<SecurityIdentifier, SecurityIdentifier>(0L, "[LocalForestConfiguration::IsValidPrincipal] The caller (SID={0}) is not the same as the expected user (SID={1}).", callingPrincipal.UserSid, ((ADUser)userOrGroupAccount).Sid);
				}
			}
			else
			{
				IdentityReferenceCollection groups = callingPrincipal.GetGroups();
				if (groups != null)
				{
					flag = groups.Contains(((ADGroup)userOrGroupAccount).Sid);
				}
				if (!flag)
				{
					LocalForestConfiguration.ConfigurationTracer.TraceError<SecurityIdentifier, SecurityIdentifier>(0L, "[LocalForestConfiguration::IsValidPrincipal] The caller (SID={0}) does not have the expected group membership (SID={1}).", callingPrincipal.UserSid, ((ADGroup)userOrGroupAccount).Sid);
				}
			}
			return flag;
		}

		internal static bool HasOrgWideAccess(OrganizationId organizationId, ClientSecurityContext callingPrincipal)
		{
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			ADRecipient availabilityConfigOrgWideAccount = organizationIdCacheValue.GetAvailabilityConfigOrgWideAccount();
			LocalForestConfiguration.ConfigurationTracer.TraceDebug<string, string>(0L, "Org Wide Account {0} configured for organization {1}.", (availabilityConfigOrgWideAccount == null) ? "null" : availabilityConfigOrgWideAccount.ToString(), (organizationId == null) ? "ForestWide" : organizationId.ToString());
			return LocalForestConfiguration.IsValidPrincipal(callingPrincipal, availabilityConfigOrgWideAccount);
		}

		internal static bool HasPerUserAccess(OrganizationId organizationId, ClientSecurityContext callingPrincipal)
		{
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			ADRecipient availabilityConfigPerUserAccount = organizationIdCacheValue.GetAvailabilityConfigPerUserAccount();
			LocalForestConfiguration.ConfigurationTracer.TraceDebug<string, string>(0L, "PerUser Access Account {0} configured for organization {1}.", (availabilityConfigPerUserAccount == null) ? "null" : availabilityConfigPerUserAccount.ToString(), (organizationId == null) ? "ForestWide" : organizationId.ToString());
			return LocalForestConfiguration.IsValidPrincipal(callingPrincipal, availabilityConfigPerUserAccount);
		}

		private static readonly Trace ConfigurationTracer = ExTraceGlobals.ConfigurationTracer;
	}
}
