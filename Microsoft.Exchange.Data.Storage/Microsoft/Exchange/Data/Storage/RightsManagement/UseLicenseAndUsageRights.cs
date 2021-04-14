using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UseLicenseAndUsageRights
	{
		internal UseLicenseAndUsageRights(string useLicense, ContentRight usageRights, ExDateTime expiryTime, byte[] drmPropsSignature, OrganizationId organizationId, string publishingLicense, Uri licensingUri)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("useLicense", useLicense);
			ArgumentValidator.ThrowIfNull("drmPropsSignature", drmPropsSignature);
			ArgumentValidator.ThrowIfNull("organizationId", organizationId);
			ArgumentValidator.ThrowIfNullOrEmpty("publishingLicense", publishingLicense);
			ArgumentValidator.ThrowIfNull("licensingUri", licensingUri);
			this.UseLicense = useLicense;
			this.UsageRights = usageRights;
			this.ExpiryTime = expiryTime;
			this.DRMPropsSignature = drmPropsSignature;
			this.OrganizationId = organizationId;
			this.PublishingLicense = publishingLicense;
			this.LicensingUri = licensingUri;
		}

		public readonly string UseLicense;

		public readonly ContentRight UsageRights;

		public readonly ExDateTime ExpiryTime;

		public readonly byte[] DRMPropsSignature;

		public readonly OrganizationId OrganizationId;

		public readonly string PublishingLicense;

		public readonly Uri LicensingUri;
	}
}
