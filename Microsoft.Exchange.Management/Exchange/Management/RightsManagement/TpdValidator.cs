using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.OfflineRms;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Dkm;
using Microsoft.RightsManagementServices.Provider;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TpdValidator
	{
		public TpdValidator(bool internalLicensingEnabled, Uri intranetLicensingUrl, Uri extranetLicensingUrl, Uri intranetCertificationUrl, Uri extranetCertificationUrl, SwitchParameter rmsOnlineSwitch, SwitchParameter defaultSwitch, SwitchParameter refreshTemplatesSwitch)
		{
			this.internalLicensingEnabled = internalLicensingEnabled;
			this.intranetLicensingUrl = intranetLicensingUrl;
			this.extranetLicensingUrl = extranetLicensingUrl;
			this.intranetCertificationUrl = intranetCertificationUrl;
			this.extranetCertificationUrl = extranetCertificationUrl;
			this.rmsOnlineSwitch = rmsOnlineSwitch;
			this.defaultSwitch = defaultSwitch;
			this.refreshTemplatesSwitch = refreshTemplatesSwitch;
		}

		public string ValidateTpdSuitableForImport(TrustedDocDomain tpd, string tpdName, out object failureTarget, IConfigurationSession configurationSession = null, string existingTpdKeyId = null, string existingTpdKeyType = null, Uri existingTpdIntranetLicensingUrl = null, Uri existingTpdExtranetLicensingUrl = null, SecureString tpdFilePassword = null)
		{
			RmsUtil.ThrowIfParameterNull(tpd, "tpd");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
			RmsUtil.ThrowIfKeyInformationInvalid(tpd, tpdName, out failureTarget);
			RmsUtil.ThrowIfSlcCertificateChainInvalid(tpd, tpdName, out failureTarget);
			RmsUtil.ThrowIfTpdCspDoesNotMatchCryptoMode(tpd, tpdName, out failureTarget);
			RmsUtil.ThrowIfTpdUsesUnauthorizedCryptoModeOnFips(tpd, tpdName, out failureTarget);
			string result;
			using (TrustedPublishingDomainPrivateKeyProvider trustedPublishingDomainPrivateKeyProvider = this.CreatePrivateKeyProvider(tpdName, tpd.m_ttdki, tpdFilePassword, out result, out failureTarget))
			{
				TrustedPublishingDomainImportUtilities tpdImportUtilities = this.CreateTpdImportUtilities(tpd, trustedPublishingDomainPrivateKeyProvider);
				RmsUtil.ThrowIfSlcCertificateDoesNotChainToProductionHeirarchyCertificate(tpdImportUtilities, tpdName, out failureTarget);
				if (this.refreshTemplatesSwitch)
				{
					RmsUtil.ThrowIfUrlWasSpecified(this.intranetLicensingUrl, this.refreshTemplatesSwitch, out failureTarget);
					RmsUtil.ThrowIfUrlWasSpecified(this.extranetLicensingUrl, this.refreshTemplatesSwitch, out failureTarget);
					RmsUtil.ThrowIfUrlWasSpecified(this.intranetCertificationUrl, this.refreshTemplatesSwitch, out failureTarget);
					RmsUtil.ThrowIfUrlWasSpecified(this.extranetCertificationUrl, this.refreshTemplatesSwitch, out failureTarget);
					RmsUtil.ThrowIfDefaultWasSpecified(this.defaultSwitch, out failureTarget);
					RmsUtil.ThrowIfImportedKeyIdAndTypeDoNotMatchExistingTPD(tpdName, tpd.m_ttdki.strID, existingTpdKeyId, out failureTarget);
					RmsUtil.ThrowIfImportedKeyIdAndTypeDoNotMatchExistingTPD(tpdName, tpd.m_ttdki.strIDType, existingTpdKeyType, out failureTarget);
				}
				else
				{
					RmsUtil.ThrowIfTpdDoesNotHavePrivateKeyIfInternalLicensingEnabled(tpd, tpdName, this.internalLicensingEnabled, out failureTarget);
					if (!this.rmsOnlineSwitch)
					{
						RmsUtil.ThrowIfImportedTPDsKeyIdIsNotUnique(configurationSession, tpd.m_ttdki.strID, tpd.m_ttdki.strIDType, out failureTarget);
					}
					RmsUtil.ThrowIfIsNotWellFormedRmServiceUrl(this.intranetLicensingUrl, out failureTarget);
					RmsUtil.ThrowIfIsNotWellFormedRmServiceUrl(this.extranetLicensingUrl, out failureTarget);
					RmsUtil.ThrowIfIsNotWellFormedRmServiceUrl(this.intranetCertificationUrl, out failureTarget);
					RmsUtil.ThrowIfIsNotWellFormedRmServiceUrl(this.extranetCertificationUrl, out failureTarget);
				}
				RmsUtil.ThrowIfRightsTemplatesInvalid(tpd.m_astrRightsTemplates, tpdName, tpdImportUtilities, this.refreshTemplatesSwitch ? existingTpdIntranetLicensingUrl : this.intranetLicensingUrl, this.refreshTemplatesSwitch ? existingTpdExtranetLicensingUrl : this.extranetLicensingUrl, out failureTarget);
			}
			return result;
		}

		private TrustedPublishingDomainPrivateKeyProvider CreatePrivateKeyProvider(string tpdName, KeyInformation keyInfo, SecureString tpdFilePassword, out string dkmEncryptedPrivateKey, out object failureTarget)
		{
			dkmEncryptedPrivateKey = null;
			failureTarget = null;
			if (!this.refreshTemplatesSwitch && !string.IsNullOrEmpty(keyInfo.strEncryptedPrivateKey))
			{
				return this.CreateKeyProviderAndDkmProtectKey(tpdName, keyInfo, tpdFilePassword, out dkmEncryptedPrivateKey, out failureTarget);
			}
			return null;
		}

		private TrustedPublishingDomainPrivateKeyProvider CreateKeyProviderAndDkmProtectKey(string tpdName, KeyInformation keyInfo, SecureString tpdFilePassword, out string dkmEncryptedPrivateKey, out object failureTarget)
		{
			failureTarget = null;
			byte[] bytes = this.DecryptPrivateKey(keyInfo, tpdFilePassword);
			ExchangeGroupKey exchangeGroupKey = new ExchangeGroupKey(null, "Microsoft Exchange DKM");
			Exception ex;
			if (!exchangeGroupKey.TryByteArrayToEncryptedString(bytes, out dkmEncryptedPrivateKey, out ex))
			{
				failureTarget = tpdName;
				throw new FailedToDkmProtectPrivateKeyException(ex);
			}
			Dictionary<string, PrivateKeyInformation> dictionary = new Dictionary<string, PrivateKeyInformation>(1, StringComparer.OrdinalIgnoreCase);
			PrivateKeyInformation privateKeyInformation = new PrivateKeyInformation(keyInfo.strID, keyInfo.strIDType, keyInfo.strKeyContainerName, keyInfo.nKeyNumber, keyInfo.strCSPName, keyInfo.nCSPType, dkmEncryptedPrivateKey, true);
			dictionary.Add(privateKeyInformation.Identity, privateKeyInformation);
			return new TrustedPublishingDomainPrivateKeyProvider(null, dictionary);
		}

		protected virtual byte[] DecryptPrivateKey(KeyInformation keyInfo, SecureString tpdFilePassword)
		{
			IPrivateKeyDecryptor privateKeyDecryptor = this.CreatePrivateKeyDecryptor(tpdFilePassword);
			byte[] result;
			try
			{
				result = privateKeyDecryptor.Decrypt(keyInfo.strEncryptedPrivateKey);
			}
			catch (PrivateKeyDecryptionFailedException e)
			{
				throw new FailedToDecryptPrivateKeyException(e);
			}
			return result;
		}

		protected virtual TrustedPublishingDomainImportUtilities CreateTpdImportUtilities(TrustedDocDomain tpd, TrustedPublishingDomainPrivateKeyProvider privateKeyProvider)
		{
			return RmsUtil.CreateTpdImportUtilities(new XrmlCertificateChain(tpd.m_strLicensorCertChain), privateKeyProvider);
		}

		private IPrivateKeyDecryptor CreatePrivateKeyDecryptor(SecureString tpdFilePassword)
		{
			if (this.rmsOnlineSwitch)
			{
				return new RmsOnlinePrivateKeyDecryptor();
			}
			return new OnPremisePrivateKeyDecryptor(tpdFilePassword);
		}

		private readonly bool internalLicensingEnabled;

		private readonly Uri intranetLicensingUrl;

		private readonly Uri extranetLicensingUrl;

		private readonly Uri intranetCertificationUrl;

		private readonly Uri extranetCertificationUrl;

		private readonly SwitchParameter rmsOnlineSwitch;

		private readonly SwitchParameter defaultSwitch;

		private readonly SwitchParameter refreshTemplatesSwitch;
	}
}
