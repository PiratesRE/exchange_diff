using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Security.ExternalAuthentication;
using Microsoft.Exchange.Net.WSTrust;

namespace Microsoft.Exchange.Data.Storage.Authentication
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class TargetUriResolver
	{
		public static TokenTarget Resolve(SmtpAddress smtpAddress, OrganizationId organizationId)
		{
			return TargetUriResolver.Resolve(smtpAddress.Domain.ToString(), organizationId);
		}

		public static TokenTarget Resolve(string domain, OrganizationId organizationId)
		{
			TokenTarget tokenTarget = TargetUriResolver.FromOrganizationRelationship(domain, organizationId);
			if (tokenTarget != null)
			{
				return tokenTarget;
			}
			tokenTarget = TargetUriViaGetFederationInformation.Singleton.Get(domain);
			if (tokenTarget != null)
			{
				return tokenTarget;
			}
			tokenTarget = TargetUriViaSCP.Singleton.Get(domain);
			if (tokenTarget != null)
			{
				return tokenTarget;
			}
			return null;
		}

		internal static void ClearCache()
		{
			TargetUriViaSCP.Singleton.Clear();
			TargetUriViaGetFederationInformation.Singleton.Clear();
			OrganizationIdCache.Singleton.Clear();
		}

		private static TokenTarget FromOrganizationRelationship(string domain, OrganizationId organizationId)
		{
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(organizationId);
			TargetUriResolver.Tracer.TraceDebug<string, OrganizationId>(0L, "Searching for OrganizationRelationship that matches domain {0} in organization {1}", domain, organizationId);
			OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(domain);
			if (organizationRelationship == null)
			{
				TargetUriResolver.Tracer.TraceError<string, OrganizationId>(0L, "Found no OrganizationRelationship that matches domain {0} in organization {1}", domain, organizationId);
				return null;
			}
			if (organizationRelationship.TargetApplicationUri == null)
			{
				TargetUriResolver.Tracer.TraceError<string, OrganizationId, ADObjectId>(0L, "Found OrganizationRelationship that matches domain {0} in organization {1}, but it has not TargetApplicationUri. OrganizationRelationship is {2}", domain, organizationId, organizationRelationship.Id);
				return null;
			}
			TokenTarget tokenTarget = organizationRelationship.GetTokenTarget();
			TargetUriResolver.Tracer.TraceDebug(0L, "Found OrganizationRelationship that matches domain {0} in organization {1}. Target is '{2}'. OrganizationRelationship is {3}", new object[]
			{
				domain,
				organizationId,
				tokenTarget,
				organizationRelationship.Id
			});
			return tokenTarget;
		}

		private static readonly Trace Tracer = ExTraceGlobals.TargetUriResolverTracer;
	}
}
