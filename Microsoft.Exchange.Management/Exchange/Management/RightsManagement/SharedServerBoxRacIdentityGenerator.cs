using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.OfflineRms;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.RightsManagementServices.Provider;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class SharedServerBoxRacIdentityGenerator : IPerTenantRMSTrustedPublishingDomainConfiguration
	{
		public SharedServerBoxRacIdentityGenerator(string slcCertChainCompressed, RMSTrustedPublishingDomain oldDefaultTPD, string sharedKey)
		{
			if (string.IsNullOrEmpty(slcCertChainCompressed))
			{
				throw new ArgumentNullException("slcCertChainCompressed");
			}
			this.compressedCertChain = slcCertChainCompressed;
			this.GetPrivateKeys(oldDefaultTPD, slcCertChainCompressed);
			this.originalSharedKey = sharedKey;
			this.compressedTrustedDomains = new List<string>(2);
			this.compressedTrustedDomains.Add(this.compressedCertChain);
			if (oldDefaultTPD != null)
			{
				this.compressedTrustedDomains.Add(oldDefaultTPD.SLCCertChain);
			}
		}

		public Uri IntranetLicensingUrl
		{
			get
			{
				return null;
			}
		}

		public Uri ExtranetLicensingUrl
		{
			get
			{
				return null;
			}
		}

		public Uri IntranetCertificationUrl
		{
			get
			{
				return null;
			}
		}

		public Uri ExtranetCertificationUrl
		{
			get
			{
				return null;
			}
		}

		public string CompressedSLCCertChain
		{
			get
			{
				return this.compressedCertChain;
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
				return SharedServerBoxRacIdentityGenerator.EmptyList;
			}
		}

		public IList<string> CompressedTrustedDomainChains
		{
			get
			{
				return this.compressedTrustedDomains;
			}
		}

		private bool HasCryptoModeChanged(RMSTrustedPublishingDomain oldDefaultTPD, string newSlcCertChainCompressed)
		{
			XrmlCertificateChain xrmlCertificateChain = RMUtil.DecompressSLCCertificate(oldDefaultTPD.SLCCertChain);
			XrmlCertificateChain xrmlCertificateChain2 = RMUtil.DecompressSLCCertificate(newSlcCertChainCompressed);
			return xrmlCertificateChain.GetCryptoMode() != xrmlCertificateChain2.GetCryptoMode();
		}

		private void GetPrivateKeys(RMSTrustedPublishingDomain oldDefaultTPD, string slcCertChainCompressed)
		{
			if (oldDefaultTPD == null || string.IsNullOrEmpty(oldDefaultTPD.PrivateKey) || this.HasCryptoModeChanged(oldDefaultTPD, slcCertChainCompressed))
			{
				this.resealKey = false;
				this.privateKeys = SharedServerBoxRacIdentityGenerator.EmptyPrivateKeys;
				return;
			}
			this.privateKeys = new Dictionary<string, PrivateKeyInformation>(1, StringComparer.OrdinalIgnoreCase);
			PrivateKeyInformation privateKeyInformation = new PrivateKeyInformation(oldDefaultTPD.KeyId, oldDefaultTPD.KeyIdType, oldDefaultTPD.KeyContainerName, oldDefaultTPD.KeyNumber, oldDefaultTPD.CSPName, oldDefaultTPD.CSPType, oldDefaultTPD.PrivateKey, false);
			this.privateKeys.Add(privateKeyInformation.Identity, privateKeyInformation);
			this.resealKey = true;
		}

		public string GenerateSharedKey()
		{
			if (this.resealKey && !string.IsNullOrEmpty(this.originalSharedKey))
			{
				return ServerManager.ResealRACKey(this, this.originalSharedKey);
			}
			return ServerManager.GenerateAndSealRACKey(this);
		}

		private static readonly Dictionary<string, PrivateKeyInformation> EmptyPrivateKeys = new Dictionary<string, PrivateKeyInformation>();

		private static readonly IList<string> EmptyList = new List<string>();

		private readonly string compressedCertChain;

		private Dictionary<string, PrivateKeyInformation> privateKeys;

		private readonly string originalSharedKey;

		private bool resealKey;

		private List<string> compressedTrustedDomains;
	}
}
