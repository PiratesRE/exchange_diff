using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.OfflineRms;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.RightsManagementServices.Core;
using Microsoft.RightsManagementServices.Online;
using Microsoft.RightsManagementServices.Provider;
using Microsoft.Win32;

namespace Microsoft.Exchange.Management.RightsManagement
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal static class RmsUtil
	{
		public static void ThrowIfParameterNull(object param, string paramName)
		{
			if (param == null)
			{
				throw new ArgumentNullException(paramName);
			}
		}

		public static void ThrowIfStringParameterNullOrEmpty(string s, string paramName)
		{
			if (string.IsNullOrEmpty(s))
			{
				throw new ArgumentException(paramName);
			}
		}

		public static void ThrowIfGuidEmpty(Guid guid, string paramName)
		{
			if (guid.Equals(Guid.Empty))
			{
				throw new ArgumentException(paramName);
			}
		}

		public static void ThrowIfTenantInfoisNull(TenantInfo[] tenantInfo, Guid externalDirectoryOrgId)
		{
			if (tenantInfo == null)
			{
				throw new ImportTpdException(string.Format("RMS Online returned a null TenantInfo reference for tenant with external directory organization ID {0}", externalDirectoryOrgId), null);
			}
		}

		public static void ThrowIfZeroOrMultipleTenantInfoObjectsReturned(TenantInfo[] tenantInfo, Guid externalDirectoryOrgId)
		{
			if (tenantInfo.Length == 0 || tenantInfo.Length > 1)
			{
				throw new ImportTpdException(string.Format("RMS Online returned zero or multiple TenantInfo objects when exactly one was requested for tenant with external directory organization ID {0}", externalDirectoryOrgId), null);
			}
		}

		public static void ThrowIfErrorInfoObjectReturned(TenantInfo tenantInfo, Guid externalDirectoryOrgId)
		{
			if (tenantInfo.ErrorInfo != null)
			{
				throw new RmsOnlineImportTpdException(string.Format("RMS Online returned an error for tenant with external directory organization ID {0}", externalDirectoryOrgId), tenantInfo.ErrorInfo.ErrorCode);
			}
		}

		public static void ThrowIfTenantInfoDoesNotIncludeActiveTPD(TenantInfo tenantInfo, Guid externalDirectoryOrgId)
		{
			if (tenantInfo.ActivePublishingDomain == null)
			{
				throw new ImportTpdException(string.Format("RMS Online returned a TenantInfo containing no active TPD for tenant with external directory organization ID {0}", externalDirectoryOrgId), null);
			}
		}

		public static void ThrowIfTpdDoesNotIncludeKeyInformation(TrustedDocDomain tpd, Guid externalDirectoryOrgId)
		{
			if (tpd.m_ttdki == null)
			{
				throw new ImportTpdException(string.Format("RMS Online returned a TPD containing no key information for tenant with external directory organization ID {0}", externalDirectoryOrgId), null);
			}
		}

		public static void ThrowIfTpdDoesNotIncludeSLC(TrustedDocDomain tpd, Guid externalDirectoryOrgId)
		{
			if (tpd.m_strLicensorCertChain == null)
			{
				throw new ImportTpdException(string.Format("RMS Online returned a TPD containing no SLC for tenant with external directory organization ID {0}", externalDirectoryOrgId), null);
			}
		}

		public static void ThrowIfTpdDoesNotIncludeTemplates(TrustedDocDomain tpd, Guid externalDirectoryOrgId)
		{
			if (tpd.m_astrRightsTemplates == null)
			{
				throw new ImportTpdException(string.Format("RMS Online returned a TPD containing no templates for tenant with external directory organization ID {0}", externalDirectoryOrgId), null);
			}
		}

		public static void ThrowIfTenantInfoDoesNotIncludeLicensingUrls(TenantInfo tenantInfo, Guid externalDirectoryOrgId)
		{
			if (null == tenantInfo.LicensingIntranetDistributionPointUrl || null == tenantInfo.LicensingExtranetDistributionPointUrl)
			{
				throw new ImportTpdException(string.Format("RMS Online did not return intranet/extranet licensing URLs for tenant with external directory organization ID {0}", externalDirectoryOrgId), null);
			}
		}

		public static void ThrowIfTenantInfoDoesNotIncludeCertificationUrls(TenantInfo tenantInfo, Guid externalDirectoryOrgId)
		{
			if (null == tenantInfo.CertificationIntranetDistributionPointUrl || null == tenantInfo.CertificationExtranetDistributionPointUrl)
			{
				throw new ImportTpdException(string.Format("RMS Online did not return intranet/extranet certification URLs for tenant with external directory organization ID {0}", externalDirectoryOrgId), null);
			}
		}

		public static void ThrowIfClientCredentialsIsNull(TenantManagementServiceClient proxy)
		{
			if (proxy.ClientCredentials == null)
			{
				throw new ImportTpdException("proxy.ClientCredentials is unexpectedly null", null);
			}
		}

		public static void ThrowIfCertificateCollectionIsNullOrEmpty(X509Certificate2Collection certificates, string exceptionText)
		{
			if (certificates == null || certificates.Count == 0)
			{
				throw new ImportTpdException(exceptionText, null);
			}
		}

		public static void ThrowIfKeyInformationInvalid(TrustedDocDomain tpd, string tpdName, out object failureTarget)
		{
			RmsUtil.ThrowIfParameterNull(tpd, "tpd");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
			if (tpd.m_ttdki == null)
			{
				failureTarget = tpdName;
				throw new NoKeyInformationInImportedTrustedPublishingDomainException();
			}
			RmsUtil.ThrowIfKeyIdInvalid(tpd.m_ttdki, tpdName, out failureTarget);
			RmsUtil.ThrowIfKeyTypeInvalid(tpd.m_ttdki, tpdName, out failureTarget);
		}

		public static void ThrowIfSlcCertificateChainInvalid(TrustedDocDomain tpd, string tpdName, out object failureTarget)
		{
			RmsUtil.ThrowIfParameterNull(tpd, "tpd");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
			failureTarget = null;
			if (tpd.m_strLicensorCertChain == null || tpd.m_strLicensorCertChain.Length == 0 || string.IsNullOrEmpty(tpd.m_strLicensorCertChain[0]))
			{
				failureTarget = tpdName;
				throw new NoSLCCertChainInImportedTrustedPublishingDomainException();
			}
		}

		public static void ThrowIfTpdCspDoesNotMatchCryptoMode(TrustedDocDomain tpd, string tpdName, out object failureTarget)
		{
			RmsUtil.ThrowIfParameterNull(tpd, "tpd");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
			failureTarget = null;
			int cryptoMode = RmsUtil.CryptoModeFromTpd(tpd);
			RmsUtil.CSP_TYPE csp_TYPE;
			if (!RmsUtil.TryCspEnumFromInteger(tpd.m_ttdki.nCSPType, out csp_TYPE))
			{
				failureTarget = tpdName;
				throw new InvalidCspForCryptoModeInImportedTrustedPublishingDomainException(csp_TYPE.ToString(), cryptoMode);
			}
			switch (cryptoMode)
			{
			case 1:
				if (csp_TYPE != RmsUtil.CSP_TYPE.PROV_RSA_FULL && csp_TYPE != RmsUtil.CSP_TYPE.PROV_RSA_AES)
				{
					failureTarget = tpdName;
					throw new InvalidCspForCryptoModeInImportedTrustedPublishingDomainException(csp_TYPE.ToString(), cryptoMode);
				}
				break;
			case 2:
				if (csp_TYPE != RmsUtil.CSP_TYPE.PROV_RSA_AES)
				{
					failureTarget = tpdName;
					throw new InvalidCspForCryptoModeInImportedTrustedPublishingDomainException(csp_TYPE.ToString(), cryptoMode);
				}
				break;
			default:
				failureTarget = tpdName;
				throw new InvalidCspForCryptoModeInImportedTrustedPublishingDomainException(csp_TYPE.ToString(), cryptoMode);
			}
		}

		public static void ThrowIfTpdUsesUnauthorizedCryptoModeOnFips(TrustedDocDomain tpd, string tpdName, out object failureTarget)
		{
			RmsUtil.ThrowIfParameterNull(tpd, "tpd");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
			failureTarget = null;
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("System\\CurrentControlSet\\Control\\Lsa\\FIPSAlgorithmPolicy\\", false))
			{
				object value;
				if (registryKey != null && (value = registryKey.GetValue("Enabled")) != null && (int)value == 1)
				{
					int num = RmsUtil.CryptoModeFromTpd(tpd);
					if (num == 1)
					{
						failureTarget = tpdName;
						throw new InvalidFipsCryptoModeInImportedTrustedPublishingDomainException(num);
					}
				}
			}
		}

		public static void ThrowIfSlcCertificateDoesNotChainToProductionHeirarchyCertificate(TrustedPublishingDomainImportUtilities tpdImportUtilities, string tpdName, out object failureTarget)
		{
			RmsUtil.ThrowIfParameterNull(tpdImportUtilities, "tpdImportUtilities");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
			try
			{
				failureTarget = null;
				tpdImportUtilities.ValidateTrustedPublishingDomain();
			}
			catch (ValidationException ex)
			{
				failureTarget = tpdName;
				throw new FailedToValidateSLCCertChainException(ex.ErrorCode);
			}
		}

		public static void ThrowIfUrlWasSpecified(Uri url, SwitchParameter refreshTemplatesSwitch, out object failureTarget)
		{
			failureTarget = null;
			if (null != url)
			{
				failureTarget = refreshTemplatesSwitch;
				throw new RmsUrlsCannotBeSetException();
			}
		}

		public static void ThrowIfDefaultWasSpecified(SwitchParameter defaultSwitch, out object failureTarget)
		{
			failureTarget = null;
			if (defaultSwitch)
			{
				failureTarget = defaultSwitch;
				throw new CannotSetDefaultTPDException();
			}
		}

		public static void ThrowIfImportedKeyIdAndTypeDoNotMatchExistingTPD(string tpdName, string importedKeyIdOrType, string existingKeyIdOrType, out object failureTarget)
		{
			RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(importedKeyIdOrType, "importedKeyIdOrType");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(existingKeyIdOrType, "existingKeyIdOrType");
			failureTarget = null;
			if (!string.Equals(importedKeyIdOrType, existingKeyIdOrType, StringComparison.OrdinalIgnoreCase))
			{
				failureTarget = existingKeyIdOrType;
				throw new KeyNoMatchException(tpdName);
			}
		}

		public static void ThrowIfTpdDoesNotHavePrivateKeyIfInternalLicensingEnabled(TrustedDocDomain tpd, string tpdName, bool internalLicensingEnabled, out object failureTarget)
		{
			RmsUtil.ThrowIfParameterNull(tpd, "tpd");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
			RmsUtil.ThrowIfParameterNull(tpd.m_ttdki, "tpd.m_ttdki");
			failureTarget = null;
			if (internalLicensingEnabled && string.IsNullOrEmpty(tpd.m_ttdki.strEncryptedPrivateKey))
			{
				failureTarget = tpdName;
				throw new NoPrivateKeyInImportedTrustedPublishingDomainException();
			}
		}

		public static void ThrowIfIsNotWellFormedRmServiceUrl(Uri url, out object failureTarget)
		{
			failureTarget = null;
			if (null != url && !RMUtil.IsWellFormedRmServiceUrl(url))
			{
				failureTarget = url;
				throw new RmsUrlIsInvalidException(url);
			}
		}

		public static void ThrowIfRightsTemplatesInvalid(IEnumerable<string> templates, string tpdName, TrustedPublishingDomainImportUtilities tpdImportUtilities, Uri intranetLicensingUrl, Uri extranetLicensingUrl, out object failureTarget)
		{
			failureTarget = null;
			if (templates != null)
			{
				RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
				RmsUtil.ThrowIfParameterNull(tpdImportUtilities, "tpdImportUtilities");
				RmsUtil.ThrowIfParameterNull(intranetLicensingUrl, "intranetLicensingUrl");
				RmsUtil.ThrowIfParameterNull(extranetLicensingUrl, "extranetLicensingUrl");
				foreach (string template in templates)
				{
					Uri templateDistributionPoint;
					Uri templateDistributionPoint2;
					Guid templateGuid;
					try
					{
						DrmClientUtils.ParseTemplate(template, out templateDistributionPoint, out templateDistributionPoint2, out templateGuid);
					}
					catch (RightsManagementException innerException)
					{
						failureTarget = tpdName;
						throw new InvalidTemplateException(innerException);
					}
					RmsUtil.ThrowIfRightsTemplateInvalid(tpdImportUtilities, tpdName, template, templateGuid, out failureTarget);
					RmsUtil.ThrowIfTemplateDistributionPointInvalid(templateDistributionPoint, RmsUtil.TemplateDistributionPointType.Intranet, templateGuid, intranetLicensingUrl, extranetLicensingUrl, out failureTarget);
					RmsUtil.ThrowIfTemplateDistributionPointInvalid(templateDistributionPoint2, RmsUtil.TemplateDistributionPointType.Extranet, templateGuid, intranetLicensingUrl, extranetLicensingUrl, out failureTarget);
				}
			}
		}

		public static void ThrowIfImportedTPDsKeyIdIsNotUnique(IConfigurationSession session, string keyIdBeingImported, string keyIdTypeBeingImported, out object failureTarget)
		{
			RmsUtil.ThrowIfParameterNull(session, "session");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(keyIdBeingImported, "keyIdBeingImported");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(keyIdTypeBeingImported, "keyIdTypeBeingImported");
			failureTarget = null;
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, RMSTrustedPublishingDomainSchema.KeyId, keyIdBeingImported),
				new ComparisonFilter(ComparisonOperator.Equal, RMSTrustedPublishingDomainSchema.KeyIdType, keyIdTypeBeingImported)
			});
			if (RmsUtil.TPDExists(session, filter))
			{
				failureTarget = keyIdBeingImported;
				throw new DuplicateTPDKeyIdException(keyIdTypeBeingImported, keyIdBeingImported);
			}
		}

		public static bool TPDExists(IConfigurationSession session, string keyIdBeingImported, string keyIdTypeBeingImported)
		{
			RmsUtil.ThrowIfParameterNull(session, "session");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(keyIdBeingImported, "keyIdBeingImported");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(keyIdTypeBeingImported, "keyIdTypeBeingImported");
			QueryFilter filter = new AndFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, RMSTrustedPublishingDomainSchema.KeyId, keyIdBeingImported),
				new ComparisonFilter(ComparisonOperator.Equal, RMSTrustedPublishingDomainSchema.KeyIdType, keyIdTypeBeingImported)
			});
			return RmsUtil.TPDExists(session, filter);
		}

		public static bool TPDExists(IConfigurationSession session, QueryFilter filter = null)
		{
			RmsUtil.ThrowIfParameterNull(session, "session");
			RMSTrustedPublishingDomain[] array = session.Find<RMSTrustedPublishingDomain>(null, QueryScope.SubTree, filter, null, 1);
			return array.Length != 0;
		}

		public static bool IsKnownException(Exception exception)
		{
			return exception is RightsManagementException || exception is ExchangeConfigurationException;
		}

		public static bool AreRmsOnlinePreRequisitesMet(IRMConfiguration irmConfiguration)
		{
			RmsUtil.ThrowIfParameterNull(irmConfiguration, "irmConfiguration");
			return RMUtil.IsWellFormedRmServiceUrl(irmConfiguration.RMSOnlineKeySharingLocation);
		}

		public static string TemplateNamesFromTemplateArray(string[] templateXrMLArray)
		{
			RmsUtil.ThrowIfParameterNull(templateXrMLArray, "templateXrMLArray");
			List<string> list = new List<string>();
			foreach (string templateXrml in templateXrMLArray)
			{
				RmsTemplate rmsTemplate = RmsTemplate.CreateServerTemplateFromTemplateDefinition(templateXrml, RmsTemplateType.Archived);
				list.Add(rmsTemplate.Name);
			}
			return string.Join(", ", list.ToArray());
		}

		public static bool TryExtractDecryptionCertificateSKIFromEncryptedXml(string encryptedData, out string requiredCertificateSKI, out Exception exception)
		{
			RmsUtil.ThrowIfParameterNull(encryptedData, "encryptedData");
			requiredCertificateSKI = null;
			exception = null;
			try
			{
				XmlDocument xmlDocument = new SafeXmlDocument();
				xmlDocument.LoadXml(encryptedData);
				using (XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("X509SKI"))
				{
					if (elementsByTagName.Count > 0)
					{
						byte[] value = Convert.FromBase64String(elementsByTagName[0].InnerText);
						requiredCertificateSKI = BitConverter.ToString(value);
						return true;
					}
				}
				exception = new XmlException("X509SKI node not found in encrypted XML document");
			}
			catch (FormatException ex)
			{
				exception = ex;
			}
			catch (XmlException ex2)
			{
				exception = ex2;
			}
			return false;
		}

		public static TrustedPublishingDomainImportUtilities CreateTpdImportUtilities(XrmlCertificateChain slcCertificate, TrustedPublishingDomainPrivateKeyProvider privateKeyProvider)
		{
			RmsUtil.ThrowIfParameterNull(slcCertificate, "slcCertificate");
			if (privateKeyProvider == null)
			{
				return new TrustedPublishingDomainImportUtilities(slcCertificate);
			}
			return new TrustedPublishingDomainImportUtilities(slcCertificate, privateKeyProvider);
		}

		public static TrustedDocDomain ConvertFromRmsOnlineTrustedDocDomain(TrustedDocDomain rmsoTPD)
		{
			RmsUtil.ThrowIfParameterNull(rmsoTPD, "rmsoTPD");
			return new TrustedDocDomain
			{
				m_ttdki = RmsUtil.ConvertFromRmsOnlineKeyInformation(rmsoTPD.m_ttdki),
				m_strLicensorCertChain = rmsoTPD.m_strLicensorCertChain,
				m_astrRightsTemplates = rmsoTPD.m_astrRightsTemplates
			};
		}

		public static bool TryExtractUrlsFromTenantConfiguration(XElement tenantConfigurationElement, out Uri intranetCertificationUrl, out Uri extranetCertificationUrl, out Uri intranetLicensingUrl, out Uri extranetLicensingUrl, out Exception exception)
		{
			RmsUtil.ThrowIfParameterNull(tenantConfigurationElement, "tenantConfigurationElement");
			intranetCertificationUrl = null;
			extranetCertificationUrl = null;
			intranetLicensingUrl = null;
			extranetLicensingUrl = null;
			exception = null;
			try
			{
				XmlReader reader = tenantConfigurationElement.CreateReader();
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(reader);
				XmlNode xmlNode = xmlDocument.SelectSingleNode("/TenantConfiguration/CertificationIntranetDistributionPointUrl");
				XmlNode xmlNode2 = xmlDocument.SelectSingleNode("/TenantConfiguration/CertificationExtranetDistributionPointUrl");
				XmlNode xmlNode3 = xmlDocument.SelectSingleNode("/TenantConfiguration/LicensingIntranetDistributionPointUrl");
				XmlNode xmlNode4 = xmlDocument.SelectSingleNode("/TenantConfiguration/LicensingExtranetDistributionPointUrl");
				if (xmlNode != null && xmlNode2 != null && xmlNode3 != null && xmlNode4 != null)
				{
					intranetCertificationUrl = new Uri(xmlNode.InnerText);
					extranetCertificationUrl = new Uri(xmlNode2.InnerText);
					intranetLicensingUrl = new Uri(xmlNode3.InnerText);
					extranetLicensingUrl = new Uri(xmlNode4.InnerText);
					return true;
				}
				exception = new XmlException("Unable to extract certification/licensing URLs from TenantConfiguration XML");
			}
			catch (XmlException ex)
			{
				exception = ex;
			}
			catch (UriFormatException ex2)
			{
				exception = ex2;
			}
			return false;
		}

		public static string GenerateRmsOnlineTpdName(string existingDefaultTpdName, string newTpdNameRoot)
		{
			RmsUtil.ThrowIfStringParameterNullOrEmpty(newTpdNameRoot, "newTpdNameRoot");
			if (string.IsNullOrEmpty(existingDefaultTpdName))
			{
				return string.Format("{0}{1}{2}", newTpdNameRoot, " - ", "1");
			}
			int num = 0;
			if (existingDefaultTpdName.Length > " - ".Length && string.Compare(existingDefaultTpdName, 0, newTpdNameRoot, 0, newTpdNameRoot.Length, true) == 0)
			{
				int num2 = existingDefaultTpdName.LastIndexOf(" - ", StringComparison.Ordinal);
				if (-1 != num2 && existingDefaultTpdName.Length > num2 + " - ".Length)
				{
					int.TryParse(existingDefaultTpdName.Substring(num2 + " - ".Length), out num);
				}
			}
			return string.Format("{0}{1}{2}", newTpdNameRoot, " - ", num + 1);
		}

		public static Guid GetExternalDirectoryOrgIdThrowOnFailure(IConfigurationSession session, OrganizationId orgId)
		{
			Guid externalOrganizationId = Guid.Empty;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ExchangeConfigurationUnit exchangeConfigurationUnit = session.Read<ExchangeConfigurationUnit>(orgId.ConfigurationUnit);
				if (exchangeConfigurationUnit != null)
				{
					Guid.TryParse(exchangeConfigurationUnit.ExternalDirectoryOrganizationId, out externalOrganizationId);
				}
			});
			if (!adoperationResult.Succeeded || externalOrganizationId == Guid.Empty)
			{
				throw new ImportTpdException("Unable to lookup ExternalDirectoryOrganizationId for organization", null);
			}
			return externalOrganizationId;
		}

		private static void ThrowIfKeyIdInvalid(KeyInformation keyInfo, string tpdName, out object failureTarget)
		{
			RmsUtil.ThrowIfParameterNull(keyInfo, "keyInfo");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
			failureTarget = null;
			if (string.IsNullOrEmpty(keyInfo.strID))
			{
				failureTarget = tpdName;
				throw new NoKeyIDInImportedTrustedPublishingDomainException();
			}
		}

		private static void ThrowIfKeyTypeInvalid(KeyInformation keyInfo, string tpdName, out object failureTarget)
		{
			RmsUtil.ThrowIfParameterNull(keyInfo, "keyInfo");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
			failureTarget = null;
			if (string.IsNullOrEmpty(keyInfo.strIDType))
			{
				failureTarget = tpdName;
				throw new NoKeyIDTypeInImportedTrustedPublishingDomainException();
			}
		}

		private static void ThrowIfRightsTemplateInvalid(TrustedPublishingDomainImportUtilities tpdImportUtilities, string tpdName, string template, Guid templateGuid, out object failureTarget)
		{
			RmsUtil.ThrowIfParameterNull(tpdImportUtilities, "tpdImportUtilities");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(tpdName, "tpdName");
			RmsUtil.ThrowIfStringParameterNullOrEmpty(template, "template");
			failureTarget = null;
			if (Guid.Empty == templateGuid)
			{
				failureTarget = tpdName;
				throw new InvalidTemplateException();
			}
			try
			{
				tpdImportUtilities.ValidateRightsTemplate(template);
			}
			catch (ValidationException ex)
			{
				failureTarget = tpdName;
				throw new FailedToValidateTemplateException(templateGuid, ex.ErrorCode);
			}
		}

		private static void ThrowIfTemplateDistributionPointInvalid(Uri templateDistributionPoint, RmsUtil.TemplateDistributionPointType templateDistributionPointType, Guid templateGuid, Uri intranetLicensingUrl, Uri extranetLicensingUrl, out object failureTarget)
		{
			RmsUtil.ThrowIfParameterNull(templateDistributionPointType, "templateDistributionPointType");
			RmsUtil.ThrowIfParameterNull(intranetLicensingUrl, "intranetLicensingUrl");
			RmsUtil.ThrowIfParameterNull(extranetLicensingUrl, "extranetLicensingUrl");
			failureTarget = null;
			if (templateDistributionPoint != null && Uri.Compare(templateDistributionPoint, intranetLicensingUrl, UriComponents.SchemeAndServer, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) != 0 && Uri.Compare(templateDistributionPoint, extranetLicensingUrl, UriComponents.SchemeAndServer, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase) != 0)
			{
				Uri uri = (templateDistributionPointType == RmsUtil.TemplateDistributionPointType.Intranet) ? intranetLicensingUrl : extranetLicensingUrl;
				failureTarget = uri;
				throw new FailedToMatchTemplateDistributionPointToLicensingUriException(templateGuid, templateDistributionPoint, uri);
			}
		}

		private static KeyInformation ConvertFromRmsOnlineKeyInformation(KeyInformation rmsoKeyInfo)
		{
			RmsUtil.ThrowIfParameterNull(rmsoKeyInfo, "rmsoKeyInfo");
			return new KeyInformation
			{
				strID = rmsoKeyInfo.strID,
				strIDType = rmsoKeyInfo.strIDType,
				nCSPType = rmsoKeyInfo.nCSPType,
				strCSPName = rmsoKeyInfo.strCSPName,
				strKeyContainerName = rmsoKeyInfo.strKeyContainerName,
				nKeyNumber = rmsoKeyInfo.nKeyNumber,
				strEncryptedPrivateKey = rmsoKeyInfo.strEncryptedPrivateKey
			};
		}

		private static bool TryCspEnumFromInteger(int cspTypeIndex, out RmsUtil.CSP_TYPE cspTypeEnum)
		{
			cspTypeEnum = RmsUtil.CSP_TYPE.PROV_CSP_UNKNOWN;
			if (Enum.IsDefined(typeof(RmsUtil.CSP_TYPE), cspTypeIndex))
			{
				cspTypeEnum = (RmsUtil.CSP_TYPE)cspTypeIndex;
				return true;
			}
			return false;
		}

		private static int CryptoModeFromTpd(TrustedDocDomain tpd)
		{
			string compressedCerts = RMUtil.CompressSLCCertificateChain(tpd.m_strLicensorCertChain);
			XrmlCertificateChain xrmlCertificateChain = RMUtil.DecompressSLCCertificate(compressedCerts);
			return xrmlCertificateChain.GetCryptoMode();
		}

		private const string intranetCertificationUrlXpath = "/TenantConfiguration/CertificationIntranetDistributionPointUrl";

		private const string extranetCertificationUrlXpath = "/TenantConfiguration/CertificationExtranetDistributionPointUrl";

		private const string intranetLicensingUrlXpath = "/TenantConfiguration/LicensingIntranetDistributionPointUrl";

		private const string extranetLicensingXpath = "/TenantConfiguration/LicensingExtranetDistributionPointUrl";

		private const string FIPSRegistryKeyLocation = "System\\CurrentControlSet\\Control\\Lsa\\FIPSAlgorithmPolicy\\";

		private enum CSP_TYPE
		{
			PROV_CSP_UNKNOWN,
			PROV_RSA_FULL,
			PROV_RSA_SIG,
			PROV_DSS,
			PROV_FORTEZZA,
			PROV_MS_EXCHANGE,
			PROV_SSL,
			PROV_RSA_SCHANNEL = 12,
			PROV_DSS_DH,
			PROV_EC_ECDSA_SIG,
			PROV_EC_ECNRA_SIG,
			PROV_EC_ECDSA_FULL,
			PROV_EC_ECNRA_FULL,
			PROV_DH_SCHANNEL,
			PROV_SPYRUS_LYNKS = 20,
			PROV_RNG,
			PROV_INTEL_SEC,
			PROV_REPLACE_OWF,
			PROV_RSA_AES
		}

		public enum TemplateDistributionPointType
		{
			Intranet,
			Extranet
		}
	}
}
