using System;
using System.IdentityModel.Selectors;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Description;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.Security.RightsManagement;

namespace Microsoft.Exchange.Data.Storage.RightsManagement
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SamlClientCredentials : ClientCredentials
	{
		public SamlClientCredentials(LicenseIdentity identity, OrganizationId organizationId, Uri targetUri, Offer offer, IRmsLatencyTracker latencyTracker)
		{
			if (identity == null)
			{
				throw new ArgumentNullException("identity");
			}
			if (targetUri == null)
			{
				throw new ArgumentNullException("targetUri");
			}
			if (offer == null)
			{
				throw new ArgumentNullException("offer");
			}
			base.SupportInteractive = false;
			this.Identity = identity;
			this.OrganizationId = organizationId;
			this.TargetUri = targetUri;
			this.Offer = offer;
			base.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
		}

		protected SamlClientCredentials(SamlClientCredentials other) : base(other)
		{
		}

		protected override ClientCredentials CloneCore()
		{
			return new SamlClientCredentials(this);
		}

		public override SecurityTokenManager CreateSecurityTokenManager()
		{
			if (this.cachedSecurityTokenManager == null)
			{
				this.cachedSecurityTokenManager = new SamlSecurityTokenManager(this);
			}
			return this.cachedSecurityTokenManager;
		}

		internal LicenseIdentity Identity;

		internal OrganizationId OrganizationId;

		internal Uri TargetUri;

		internal Offer Offer;

		internal IRmsLatencyTracker RmsLatencyTracker;

		private SamlSecurityTokenManager cachedSecurityTokenManager;
	}
}
