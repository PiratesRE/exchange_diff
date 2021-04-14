using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.RightsManagementServices.Provider;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class PerTenantRMSTrustedPublishingDomainConfiguration : TenantConfigurationCacheableItem<RMSTrustedPublishingDomain>, IPerTenantRMSTrustedPublishingDomainConfiguration
	{
		public override long ItemSize
		{
			get
			{
				return (long)this.estimatedSize;
			}
		}

		public Uri IntranetLicensingUrl
		{
			get
			{
				return this.intranetLicensingUrl;
			}
		}

		public Uri ExtranetLicensingUrl
		{
			get
			{
				return this.extranetLicensingUrl;
			}
		}

		public Uri IntranetCertificationUrl
		{
			get
			{
				return this.intranetCertificationUrl;
			}
		}

		public Uri ExtranetCertificationUrl
		{
			get
			{
				return this.extranetCertificationUrl;
			}
		}

		public string CompressedSLCCertChain
		{
			get
			{
				return this.compressedSLCCertChain;
			}
		}

		public int ActiveCryptoMode
		{
			get
			{
				return this.activeCryptoMode;
			}
		}

		public Dictionary<string, PrivateKeyInformation> PrivateKeys
		{
			get
			{
				return this.privateKeys;
			}
		}

		public IList<string> CompressedRMSTemplates
		{
			get
			{
				return this.compressedRMSTemplates;
			}
		}

		public IList<string> CompressedTrustedDomainChains
		{
			get
			{
				return this.compressedTrustedDomainChains;
			}
		}

		public override void ReadData(IConfigurationSession session)
		{
			RMSTrustedPublishingDomain[] array = session.Find<RMSTrustedPublishingDomain>(null, QueryScope.SubTree, null, null, 0);
			if (array == null || array.Length == 0)
			{
				throw new RightsManagementServerException(ServerStrings.FailedToLocateTPDConfig(session.SessionSettings.CurrentOrganizationId.ToString()), false);
			}
			this.compressedTrustedDomainChains = new List<string>(array.Length);
			this.compressedRMSTemplates = new List<string>();
			this.privateKeys = new Dictionary<string, PrivateKeyInformation>(array.Length, StringComparer.OrdinalIgnoreCase);
			foreach (RMSTrustedPublishingDomain rmstrustedPublishingDomain in array)
			{
				if (string.IsNullOrEmpty(rmstrustedPublishingDomain.SLCCertChain))
				{
					throw new DataValidationException(new PropertyValidationError(new LocalizedString("SLCCertChain is null from AD for tenant " + base.OrganizationId), RMSTrustedPublishingDomainSchema.SLCCertChain, null));
				}
				if (string.IsNullOrEmpty(rmstrustedPublishingDomain.PrivateKey))
				{
					throw new DataValidationException(new PropertyValidationError(new LocalizedString("PrivateKey is null from AD for tenant " + base.OrganizationId), RMSTrustedPublishingDomainSchema.PrivateKey, null));
				}
				if (string.IsNullOrEmpty(rmstrustedPublishingDomain.KeyId))
				{
					throw new DataValidationException(new PropertyValidationError(new LocalizedString("KeyId is null from AD for tenant " + base.OrganizationId), RMSTrustedPublishingDomainSchema.KeyId, null));
				}
				if (string.IsNullOrEmpty(rmstrustedPublishingDomain.KeyIdType))
				{
					throw new DataValidationException(new PropertyValidationError(new LocalizedString("KeyIdType is null from AD for tenant " + base.OrganizationId), RMSTrustedPublishingDomainSchema.KeyIdType, null));
				}
				if (rmstrustedPublishingDomain.IntranetLicensingUrl == null || string.IsNullOrEmpty(rmstrustedPublishingDomain.IntranetLicensingUrl.OriginalString))
				{
					throw new DataValidationException(new PropertyValidationError(new LocalizedString("IntranetLicensingUrl is null from AD for tenant " + base.OrganizationId), RMSTrustedPublishingDomainSchema.IntranetLicensingUrl, null));
				}
				if (rmstrustedPublishingDomain.ExtranetLicensingUrl == null || string.IsNullOrEmpty(rmstrustedPublishingDomain.ExtranetLicensingUrl.OriginalString))
				{
					throw new DataValidationException(new PropertyValidationError(new LocalizedString("ExtranetLicensingUrl is null from AD for tenant " + base.OrganizationId), RMSTrustedPublishingDomainSchema.ExtranetLicensingUrl, null));
				}
				if (rmstrustedPublishingDomain.Default)
				{
					this.intranetLicensingUrl = rmstrustedPublishingDomain.IntranetLicensingUrl;
					this.estimatedSize += rmstrustedPublishingDomain.IntranetLicensingUrl.OriginalString.Length * 2;
					this.extranetLicensingUrl = rmstrustedPublishingDomain.ExtranetLicensingUrl;
					this.estimatedSize += rmstrustedPublishingDomain.ExtranetLicensingUrl.OriginalString.Length * 2;
					if (rmstrustedPublishingDomain.IntranetCertificationUrl != null && !string.IsNullOrEmpty(rmstrustedPublishingDomain.IntranetCertificationUrl.OriginalString))
					{
						this.intranetCertificationUrl = rmstrustedPublishingDomain.IntranetCertificationUrl;
						this.estimatedSize += rmstrustedPublishingDomain.IntranetCertificationUrl.OriginalString.Length * 2;
					}
					if (rmstrustedPublishingDomain.ExtranetCertificationUrl != null && !string.IsNullOrEmpty(rmstrustedPublishingDomain.ExtranetCertificationUrl.OriginalString))
					{
						this.extranetCertificationUrl = rmstrustedPublishingDomain.ExtranetCertificationUrl;
						this.estimatedSize += rmstrustedPublishingDomain.ExtranetCertificationUrl.OriginalString.Length * 2;
					}
					this.compressedSLCCertChain = rmstrustedPublishingDomain.SLCCertChain;
					this.estimatedSize += rmstrustedPublishingDomain.SLCCertChain.Length * 2;
					this.activeCryptoMode = PerTenantRMSTrustedPublishingDomainConfiguration.CryptoModeFromCompressedSLC(this.compressedSLCCertChain);
				}
				if (rmstrustedPublishingDomain.RMSTemplates != null && rmstrustedPublishingDomain.RMSTemplates.Count > 0)
				{
					foreach (string text in rmstrustedPublishingDomain.RMSTemplates)
					{
						if (string.IsNullOrEmpty(text))
						{
							throw new DataValidationException(new PropertyValidationError(new LocalizedString("Template contains empty string for " + base.OrganizationId), RMSTrustedPublishingDomainSchema.ExtranetLicensingUrl, null));
						}
						this.CompressedRMSTemplates.Add(text);
						this.estimatedSize += text.Length;
					}
				}
				PrivateKeyInformation privateKeyInformation = new PrivateKeyInformation(rmstrustedPublishingDomain.KeyId, rmstrustedPublishingDomain.KeyIdType, rmstrustedPublishingDomain.KeyContainerName, rmstrustedPublishingDomain.KeyNumber, rmstrustedPublishingDomain.CSPName, rmstrustedPublishingDomain.CSPType, rmstrustedPublishingDomain.PrivateKey, rmstrustedPublishingDomain.Default);
				this.estimatedSize += 8;
				this.estimatedSize += rmstrustedPublishingDomain.KeyId.Length * 2;
				this.estimatedSize += rmstrustedPublishingDomain.KeyIdType.Length * 2;
				this.estimatedSize += rmstrustedPublishingDomain.PrivateKey.Length * 2;
				if (!string.IsNullOrEmpty(rmstrustedPublishingDomain.CSPName))
				{
					this.estimatedSize += rmstrustedPublishingDomain.CSPName.Length * 2;
				}
				if (!string.IsNullOrEmpty(rmstrustedPublishingDomain.KeyContainerName))
				{
					this.estimatedSize += rmstrustedPublishingDomain.KeyContainerName.Length * 2;
				}
				this.privateKeys[privateKeyInformation.Identity] = privateKeyInformation;
				this.compressedTrustedDomainChains.Add(rmstrustedPublishingDomain.SLCCertChain);
				this.estimatedSize += rmstrustedPublishingDomain.SLCCertChain.Length * 2;
			}
		}

		private static int CryptoModeFromCompressedSLC(string compressedSLCCertChain)
		{
			XrmlCertificateChain xrmlCertificateChain = RMUtil.DecompressSLCCertificate(compressedSLCCertChain);
			return xrmlCertificateChain.GetCryptoMode();
		}

		private Uri intranetLicensingUrl;

		private Uri extranetLicensingUrl;

		private Uri intranetCertificationUrl;

		private Uri extranetCertificationUrl;

		private string compressedSLCCertChain;

		private int activeCryptoMode = 1;

		private Dictionary<string, PrivateKeyInformation> privateKeys;

		private List<string> compressedRMSTemplates;

		private List<string> compressedTrustedDomainChains;

		private int estimatedSize;
	}
}
