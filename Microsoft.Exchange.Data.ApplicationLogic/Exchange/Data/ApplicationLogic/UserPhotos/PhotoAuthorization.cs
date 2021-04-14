using System;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.ApplicationLogic.UserPhotos
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PhotoAuthorization
	{
		public PhotoAuthorization(LazyLookupTimeoutCache<OrganizationId, OrganizationIdCacheValue> organizationConfigCache, ITracer upstreamTracer)
		{
			ArgumentValidator.ThrowIfNull("organizationConfigCache", organizationConfigCache);
			this.tracer = ExTraceGlobals.UserPhotosTracer.Compose(upstreamTracer);
			this.organizationConfigCache = organizationConfigCache;
		}

		public void Authorize(PhotoPrincipal requestor, PhotoPrincipal target)
		{
			if (requestor == null)
			{
				throw new ArgumentNullException("requestor");
			}
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			if (requestor.IsSame(target))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Photo authorization: authorized: requestor and target are same principal.");
				return;
			}
			if (requestor.InSameOrganization(target))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Photo authorization: authorized: requestor and target are in same organization.");
				return;
			}
			if (this.PhotoSharingEnabled(requestor, target))
			{
				this.tracer.TraceDebug((long)this.GetHashCode(), "Photo authorization: authorized: photo sharing enabled.");
				return;
			}
			this.tracer.TraceDebug((long)this.GetHashCode(), "Photo authorization: ACCESS DENIED.");
			throw new AccessDeniedException(Strings.UserPhotoAccessDenied);
		}

		private bool PhotoSharingEnabled(PhotoPrincipal requestor, PhotoPrincipal target)
		{
			OrganizationIdCacheValue organizationIdCacheValue = this.organizationConfigCache.Get((target.OrganizationId == null) ? OrganizationId.ForestWideOrgId : target.OrganizationId);
			if (organizationIdCacheValue == null)
			{
				this.tracer.TraceError((long)this.GetHashCode(), "Photo authorization: target organization's configuration not available in cache.");
				return false;
			}
			foreach (string domain in requestor.GetEmailAddressDomains())
			{
				OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(domain);
				if (organizationRelationship != null && organizationRelationship.Enabled && organizationRelationship.PhotosEnabled)
				{
					return true;
				}
			}
			return false;
		}

		private readonly ITracer tracer = ExTraceGlobals.UserPhotosTracer;

		private readonly LazyLookupTimeoutCache<OrganizationId, OrganizationIdCacheValue> organizationConfigCache;
	}
}
