using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	internal static class FederatedIdentityHelper
	{
		internal static FederatedIdentity GetFederatedIdentity(IFederatedIdentityParameters parameters)
		{
			OrganizationId organizationId = parameters.OrganizationId ?? OrganizationId.ForestWideOrgId;
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			FederatedIdentity federatedIdentity;
			if (organizationId.ConfigurationUnit == null)
			{
				ExTraceGlobals.FederatedIdentityTracer.TraceDebug<ADObjectId>(0L, "Handling user '{0}' as enterprise user.", parameters.ObjectId);
				federatedIdentity = FederatedIdentityHelper.GetFederatedIdentityForEnterprise(organizationIdCacheValue, parameters);
			}
			else
			{
				ExTraceGlobals.FederatedIdentityTracer.TraceDebug<ADObjectId>(0L, "Handling user '{0}' as tenant user.", parameters.ObjectId);
				federatedIdentity = FederatedIdentityHelper.GetFederatedIdentityForTenant(organizationIdCacheValue, parameters);
			}
			ExTraceGlobals.FederatedIdentityTracer.TraceDebug<ADObjectId, FederatedIdentity>(0L, "Federated identity for user '{0}' is: {1}", parameters.ObjectId, federatedIdentity);
			return federatedIdentity;
		}

		private static FederatedIdentity GetFederatedIdentityForEnterprise(OrganizationIdCacheValue organizationIdCacheValue, IFederatedIdentityParameters parameters)
		{
			string domain = organizationIdCacheValue.FederatedOrganizationId.AccountNamespaceWithWellKnownSubDomain.Domain;
			string text = parameters.ImmutableId;
			if (!string.IsNullOrEmpty(text))
			{
				string value = "@" + domain;
				if (!text.EndsWith(value, StringComparison.OrdinalIgnoreCase))
				{
					ExTraceGlobals.FederatedIdentityTracer.TraceError<ADObjectId, string, string>(0L, "User '{0}' has ImmutableId set to '{1}' but it doesn't match AccountNamespace '{2}' and it was ignored.", parameters.ObjectId, text, domain);
					text = null;
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				text = Convert.ToBase64String(parameters.ObjectId.ObjectGuid.ToByteArray()) + "@" + domain;
				ExTraceGlobals.FederatedIdentityTracer.TraceDebug<ADObjectId, string>(0L, "User '{0}' doesn't have ImmutableId set, generated one: {1}", parameters.ObjectId, text);
			}
			return new FederatedIdentity(text, IdentityType.ImmutableId);
		}

		private static FederatedIdentity GetFederatedIdentityForTenant(OrganizationIdCacheValue organizationIdCacheValue, IFederatedIdentityParameters parameters)
		{
			if (parameters.WindowsLiveID == SmtpAddress.Empty)
			{
				ExTraceGlobals.FederatedIdentityTracer.TraceError<ADObjectId>(0L, "User '{0}' is in a tenant but doesn't have WindowsLiveID set, so we cannot find if its namespace is federated or managed", parameters.ObjectId);
				throw new FederatedIdentityMisconfiguredException();
			}
			string domain = parameters.WindowsLiveID.Domain;
			AuthenticationType arg;
			if (!organizationIdCacheValue.NamespaceAuthenticationTypeHash.TryGetValue(domain, out arg))
			{
				ExTraceGlobals.FederatedIdentityTracer.TraceError<ADObjectId>(0L, "User '{0}' is in a tenant but cannot find AuthenticationType from the cache", parameters.ObjectId);
				throw new FederatedIdentityMisconfiguredException();
			}
			switch (arg)
			{
			case AuthenticationType.Managed:
				return new FederatedIdentity(parameters.WindowsLiveID.ToString(), IdentityType.UPN);
			case AuthenticationType.Federated:
			{
				string immutableIdPartial = parameters.ImmutableIdPartial;
				string text = parameters.ImmutableId;
				if (string.IsNullOrEmpty(text) && string.IsNullOrEmpty(immutableIdPartial))
				{
					ExTraceGlobals.FederatedIdentityTracer.TraceError<ADObjectId>(0L, "User '{0}' is in a federated namespace but doesn't have ImmutableId or OnPremisesObjectId", parameters.ObjectId);
					throw new FederatedIdentityMisconfiguredException();
				}
				if (string.IsNullOrEmpty(text))
				{
					text = immutableIdPartial + "@" + domain;
					ExTraceGlobals.FederatedIdentityTracer.TraceDebug<ADObjectId, string>(0L, "User '{0}' doesn't have ImmutableId set, defaulting to: {1}", parameters.ObjectId, text);
				}
				else
				{
					ExTraceGlobals.FederatedIdentityTracer.TraceDebug<ADObjectId, string>(0L, "User '{0}' has ImmutableId set: {1}", parameters.ObjectId, parameters.ImmutableId);
				}
				return new FederatedIdentity(text, IdentityType.ImmutableId);
			}
			default:
				ExTraceGlobals.FederatedIdentityTracer.TraceError<ADObjectId, AuthenticationType>(0L, "User '{0}' is in a tenant and its AuthenticationType is unknown: {1}", parameters.ObjectId, arg);
				throw new FederatedIdentityMisconfiguredException();
			}
		}
	}
}
