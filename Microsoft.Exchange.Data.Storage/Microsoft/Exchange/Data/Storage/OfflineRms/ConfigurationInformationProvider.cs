using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.RightsManagementServices.Core;
using Microsoft.RightsManagementServices.Provider;

namespace Microsoft.Exchange.Data.Storage.OfflineRms
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ConfigurationInformationProvider : IConfigurationInformationProvider
	{
		public ConfigurationInformationProvider(IPerTenantRMSTrustedPublishingDomainConfiguration perTenantconfig)
		{
			this.serverLicensorCertificate = new ConfigurationInformationProvider.ServerLicensorCertInformation(ConfigurationInformationProvider.GetTrustedDomainChainArrayFromCompressedString(perTenantconfig.CompressedSLCCertChain));
			this.serverLicensorCertificate.IsValidated = true;
			List<string[]> list = new List<string[]>(perTenantconfig.CompressedTrustedDomainChains.Count);
			foreach (string compressedCertChainString in perTenantconfig.CompressedTrustedDomainChains)
			{
				list.Add(ConfigurationInformationProvider.GetTrustedDomainChainArrayFromCompressedString(compressedCertChainString));
			}
			this.trustedUserDomains = new ConfigurationInformationProvider.TrustedDomainInformation(list);
			this.trustedUserDomains.IsValidated = true;
			this.trustedPublishingDomains = this.trustedUserDomains;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (string compressedTemplateString in perTenantconfig.CompressedRMSTemplates)
			{
				string templateFromCompressedString = ConfigurationInformationProvider.GetTemplateFromCompressedString(compressedTemplateString);
				Guid templateGuidFromLicense;
				try
				{
					templateGuidFromLicense = DrmClientUtils.GetTemplateGuidFromLicense(templateFromCompressedString);
				}
				catch (RightsManagementException ex)
				{
					throw new ConfigurationProviderException(true, "ConfigurationInformationProvider failed to parse template data for tenant ", ex);
				}
				dictionary.Add(templateGuidFromLicense.ToString("B"), templateFromCompressedString);
			}
			this.rightsTemplateInformation = new ConfigurationInformationProvider.RightsTemplateInformation(dictionary);
			this.licensingIntranetDistributionPointUrl = perTenantconfig.IntranetLicensingUrl;
			this.licensingExtranetDistributionPointUrl = perTenantconfig.ExtranetLicensingUrl;
			this.certificationIntranetDistributionPointUrl = perTenantconfig.IntranetCertificationUrl;
			this.certificationExtranetDistributionPointUrl = perTenantconfig.ExtranetCertificationUrl;
		}

		public IServerLicensorCertInformation ServerLicensorCertificate
		{
			get
			{
				return this.serverLicensorCertificate;
			}
		}

		public ITrustedDomainInformation TrustedUserDomains
		{
			get
			{
				return this.trustedUserDomains;
			}
		}

		public ITrustedDomainInformation TrustedPublishingDomains
		{
			get
			{
				return this.trustedPublishingDomains;
			}
		}

		public IRightsTemplateInformation RightsTemplateList
		{
			get
			{
				return this.rightsTemplateInformation;
			}
		}

		public TimeSpan RightsAccountCertificateValidityTime
		{
			get
			{
				return ConfigurationInformationProvider.RightsAccountCertificateValidityTimeSpan;
			}
		}

		public Uri LicensingIntranetDistributionPointUrl
		{
			get
			{
				return this.licensingIntranetDistributionPointUrl;
			}
		}

		public Uri LicensingExtranetDistributionPointUrl
		{
			get
			{
				return this.licensingExtranetDistributionPointUrl;
			}
		}

		public Uri CertificationIntranetDistributionPointUrl
		{
			get
			{
				return this.certificationIntranetDistributionPointUrl;
			}
		}

		public Uri CertificationExtranetDistributionPointUrl
		{
			get
			{
				return this.certificationExtranetDistributionPointUrl;
			}
		}

		private static string GetTemplateFromCompressedString(string compressedTemplateString)
		{
			string result;
			try
			{
				RmsTemplateType rmsTemplateType;
				result = RMUtil.DecompressTemplate(compressedTemplateString, out rmsTemplateType);
			}
			catch (FormatException ex)
			{
				throw new ConfigurationProviderException(true, ex);
			}
			catch (InvalidRpmsgFormatException ex2)
			{
				throw new ConfigurationProviderException(true, ex2);
			}
			return result;
		}

		private static string[] GetTrustedDomainChainArrayFromCompressedString(string compressedCertChainString)
		{
			string[] result;
			try
			{
				XrmlCertificateChain xrmlCertificateChain = RMUtil.DecompressSLCCertificate(compressedCertChainString);
				result = xrmlCertificateChain.ToStringArray();
			}
			catch (FormatException ex)
			{
				throw new ConfigurationProviderException(true, "ConfigurationInformationProvider failed to read TPD from compressed form", ex);
			}
			catch (InvalidRpmsgFormatException ex2)
			{
				throw new ConfigurationProviderException(true, "ConfigurationInformationProvider failed to read TPD from compressed form", ex2);
			}
			return result;
		}

		private static readonly TimeSpan RightsAccountCertificateValidityTimeSpan = TimeSpan.FromDays(365.0);

		private readonly IServerLicensorCertInformation serverLicensorCertificate;

		private readonly ITrustedDomainInformation trustedUserDomains;

		private readonly ITrustedDomainInformation trustedPublishingDomains;

		private readonly IRightsTemplateInformation rightsTemplateInformation;

		private readonly Uri licensingIntranetDistributionPointUrl;

		private readonly Uri licensingExtranetDistributionPointUrl;

		private readonly Uri certificationIntranetDistributionPointUrl;

		private readonly Uri certificationExtranetDistributionPointUrl;

		private class ServerLicensorCertInformation : IServerLicensorCertInformation
		{
			public ServerLicensorCertInformation(string[] xrmlChain)
			{
				if (xrmlChain == null || xrmlChain.Length == 0)
				{
					throw new ArgumentNullException("xrmlChain is null");
				}
				this.certificateChain = new XrmlCertificateChain(xrmlChain);
			}

			public XrmlCertificateChain CertificateChain
			{
				get
				{
					return this.certificateChain;
				}
			}

			public ISizeTraceableItem Issuer
			{
				get
				{
					return this.issuer;
				}
				set
				{
					this.issuer = value;
				}
			}

			public ISizeTraceableItem IssuerAsPrincipal
			{
				get
				{
					return this.issuerAsPrincipal;
				}
				set
				{
					this.issuerAsPrincipal = value;
				}
			}

			public bool IsValidated
			{
				get
				{
					return this.validated;
				}
				set
				{
					this.validated = value;
				}
			}

			private readonly XrmlCertificateChain certificateChain;

			private ISizeTraceableItem issuer;

			private ISizeTraceableItem issuerAsPrincipal;

			private bool validated;
		}

		private class TrustedDomainInformation : ITrustedDomainInformation
		{
			public TrustedDomainInformation(IList<string[]> trustedDomainChains)
			{
				this.trustedDomains = new List<XrmlCertificateChain>(trustedDomainChains.Count);
				foreach (string[] array in trustedDomainChains)
				{
					this.trustedDomains.Add(new XrmlCertificateChain(array));
				}
			}

			public List<XrmlCertificateChain> TrustedDomains
			{
				get
				{
					return this.trustedDomains;
				}
			}

			public ISizeTraceableItem TrustedPrincipalList
			{
				get
				{
					return this.trustedPrincipals;
				}
				set
				{
					this.trustedPrincipals = value;
				}
			}

			public bool IsValidated
			{
				get
				{
					return this.validated;
				}
				set
				{
					this.validated = value;
				}
			}

			private readonly List<XrmlCertificateChain> trustedDomains;

			private ISizeTraceableItem trustedPrincipals;

			private bool validated;
		}

		private class RightsTemplateInformation : IRightsTemplateInformation
		{
			public RightsTemplateInformation(Dictionary<string, string> rightTemplates)
			{
				this.rightTemplates = rightTemplates;
			}

			public Dictionary<string, string> RightsTemplates
			{
				get
				{
					return this.rightTemplates;
				}
			}

			public VerifiedRightsTemplateItems VerifiedItems
			{
				get
				{
					return this.verifiedRightsTemplateItems;
				}
				set
				{
					this.verifiedRightsTemplateItems = value;
				}
			}

			private Dictionary<string, string> rightTemplates;

			private VerifiedRightsTemplateItems verifiedRightsTemplateItems;
		}
	}
}
