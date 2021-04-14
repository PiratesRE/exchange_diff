using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Win32;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public sealed class OwaRegistryKeys
	{
		internal static string DefaultTempFolderLocation
		{
			get
			{
				return OwaRegistryKeys.defaultTempFolderLocation;
			}
		}

		public static string OwaBasicVersion
		{
			get
			{
				return (string)OwaRegistryKeys.GetValue(OwaRegistryKeys.owaBasicVersionKey);
			}
		}

		public static bool AllowInternalUntrustedCerts
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.allowInternalUntrustedCertsKey);
			}
		}

		public static bool AllowProxyingWithoutSsl
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.allowProxyingWithoutSslKey);
			}
		}

		public static int MaxRecipientsPerMessage
		{
			get
			{
				return (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.maxRecipientsPerMessageKey);
			}
		}

		public static int WebReadyDocumentViewingRecycleByConversions
		{
			get
			{
				return (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.webReadyDocumentViewingRecycleByConversionsKey);
			}
		}

		public static int WebReadyDocumentViewingExcelRowsPerPage
		{
			get
			{
				return (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.webReadyDocumentViewingExcelRowsPerPageKey);
			}
		}

		public static int WebReadyDocumentViewingMaxDocumentInputSize
		{
			get
			{
				return (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.webReadyDocumentViewingMaxDocumentInputSizeKey);
			}
		}

		public static int WebReadyDocumentViewingMaxDocumentOutputSize
		{
			get
			{
				return (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.webReadyDocumentViewingMaxDocumentOutputSizeKey);
			}
		}

		public static bool WebReadyDocumentViewingWithInlineImage
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.webReadyDocumentViewingWithInlineImageKey);
			}
		}

		public static string WebReadyDocumentViewingTempFolderLocation
		{
			get
			{
				return (string)OwaRegistryKeys.GetValue(OwaRegistryKeys.webReadyDocumentViewingTempFolderLocationKey);
			}
		}

		public static int WebReadyDocumentViewingCacheDiskQuota
		{
			get
			{
				return (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.webReadyDocumentViewingCacheDiskQuotaKey);
			}
		}

		public static int WebReadyDocumentViewingConversionTimeout
		{
			get
			{
				return (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.webReadyDocumentViewingConversionTimeoutKey);
			}
		}

		public static int WebReadyDocumentViewingMemoryLimitInMB
		{
			get
			{
				return (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.webReadyDocumentViewingMemoryLimitInMBKey);
			}
		}

		public static bool ForceSMimeClientUpgrade
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.forceSMimeClientUpgradeKey);
			}
		}

		public static bool CheckCRLOnSend
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.checkCRLOnSendKey);
			}
		}

		public static int DLExpansionTimeout
		{
			get
			{
				int num = (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.dlExpansionTimeoutKey);
				if (num < 0)
				{
					return 0;
				}
				return num;
			}
		}

		public static bool UseSecondaryProxiesWhenFindingCertificates
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.useSecondaryProxiesWhenFindingCertificatesKey);
			}
		}

		public static int CRLConnectionTimeout
		{
			get
			{
				int num = (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.clrConnectionTimeoutKey);
				if (num < 5000)
				{
					return 5000;
				}
				return num;
			}
		}

		public static int CRLRetrievalTimeout
		{
			get
			{
				int num = (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.clrRetrievalTimeoutKey);
				if (num < 0)
				{
					return 5000;
				}
				return num;
			}
		}

		public static bool DisableCRLCheck
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.disableCRLCheckKey);
			}
		}

		public static bool AlwaysSign
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.alwaysSignKey);
			}
		}

		public static bool AlwaysEncrypt
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.alwaysEncryptKey);
			}
		}

		public static bool ClearSign
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.clearSignKey);
			}
		}

		public static bool IncludeCertificateChainWithoutRootCertificate
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.includeCertificateChainWithoutRootCertificateKey);
			}
		}

		public static bool IncludeCertificateChainAndRootCertificate
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.includeCertificateChainAndRootCertificateKey);
			}
		}

		public static bool EncryptTemporaryBuffers
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.encryptTemporaryBuffersKey);
			}
		}

		public static bool SignedEmailCertificateInclusion
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.signedEmailCertificateInclusionKey);
			}
		}

		public static int BccEncryptedEmailForking
		{
			get
			{
				int num = (int)OwaRegistryKeys.GetValue(OwaRegistryKeys.bccEncryptedEmailForkingKey);
				if (num < 0 || num > 2)
				{
					return (int)OwaRegistryKeys.bccEncryptedEmailForkingKey.DefaultValue;
				}
				return num;
			}
		}

		public static bool IncludeSMIMECapabilitiesInMessage
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.includeSMIMECapabilitiesInMessageKey);
			}
		}

		public static bool CopyRecipientHeaders
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.copyRecipientHeadersKey);
			}
		}

		public static bool OnlyUseSmartCard
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.onlyUseSmartCardKey);
			}
		}

		public static bool TripleWrapSignedEncryptedMail
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.tripleWrapSignedEncryptedMailKey);
			}
		}

		public static bool UseKeyIdentifier
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.useKeyIdentifierKey);
			}
		}

		public static string EncryptionAlgorithms
		{
			get
			{
				return (string)OwaRegistryKeys.GetValue(OwaRegistryKeys.encryptionAlgorithmsKey);
			}
		}

		public static string SigningAlgorithms
		{
			get
			{
				return (string)OwaRegistryKeys.GetValue(OwaRegistryKeys.signingAlgorithmsKey);
			}
		}

		public static bool AllowUserChoiceOfSigningCertificate
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.allowUserChoiceOfSigningCertificateKey);
			}
		}

		public static string SenderCertificateAttributesToDisplay
		{
			get
			{
				return (string)OwaRegistryKeys.GetValue(OwaRegistryKeys.senderCertificateAttributesToDisplayKey);
			}
		}

		public static bool UseEmbeddedMessageFileNameAsAttachmentName
		{
			get
			{
				return (bool)OwaRegistryKeys.GetValue(OwaRegistryKeys.useEmbeddedMessageFileNameAsAttachmentNameKey);
			}
		}

		public static string IMImplementationDLLPath
		{
			get
			{
				return (string)OwaRegistryKeys.GetValue(OwaRegistryKeys.implementationDLLPathKey);
			}
		}

		public static string IMImplementationDLLPathKey
		{
			get
			{
				return OwaRegistryKeys.implementationDLLPathKey.Name;
			}
		}

		public static void Initialize()
		{
			ExTraceGlobals.CoreCallTracer.TraceDebug(0L, "OwaRegistryKeys.Initialize");
			for (int i = 0; i < OwaRegistryKeys.keyPaths.Length; i++)
			{
				using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(OwaRegistryKeys.keyPaths[i]))
				{
					if (registryKey != null)
					{
						foreach (OwaRegistryKey owaRegistryKey in OwaRegistryKeys.keys[i])
						{
							OwaRegistryKeys.keyValueCache[owaRegistryKey] = OwaRegistryKeys.ReadKeyValue(registryKey, owaRegistryKey);
						}
					}
				}
			}
		}

		private static object ReadKeyValue(RegistryKey keyContainer, OwaRegistryKey owaKey)
		{
			ExTraceGlobals.CoreTracer.TraceDebug<string>(0L, "Reading registry key \"{0}\"", owaKey.Name);
			object obj;
			if (owaKey.Type == typeof(int))
			{
				obj = keyContainer.GetValue(owaKey.Name, owaKey.DefaultValue);
				if (obj.GetType() != typeof(int))
				{
					obj = null;
				}
			}
			else if (owaKey.Type == typeof(string))
			{
				obj = keyContainer.GetValue(owaKey.Name, owaKey.DefaultValue);
				if (obj.GetType() != typeof(string))
				{
					obj = null;
				}
			}
			else
			{
				if (!(owaKey.Type == typeof(bool)))
				{
					return null;
				}
				object value = keyContainer.GetValue(owaKey.Name, owaKey.DefaultValue);
				if (value.GetType() != typeof(int))
				{
					obj = null;
				}
				else
				{
					obj = ((int)value != 0);
				}
			}
			if (obj == null)
			{
				ExTraceGlobals.CoreTracer.TraceDebug(0L, "Couldn't find key or key format/type was incorrect, using default value");
				obj = owaKey.DefaultValue;
			}
			ExTraceGlobals.CoreTracer.TraceDebug<string, object>(0L, "Configuration registry key \"{0}\" read with value=\"{1}\"", owaKey.Name, obj);
			return obj;
		}

		private static object GetValue(OwaRegistryKey key)
		{
			object result = null;
			if (OwaRegistryKeys.keyValueCache.TryGetValue(key, out result))
			{
				return result;
			}
			return key.DefaultValue;
		}

		internal const int DefaultRecycleByConversions = 150;

		internal const int DefaultExcelRowsPerPage = 200;

		internal const int DefaultMaxDocumentInputSize = 5000;

		internal const int DefaultMaxDocumentOutputSize = 5000;

		internal const bool DefaultWithInlineImage = true;

		internal const int DefaultMemoryLimitInMB = 200;

		internal const int DefaultCacheDiskQuota = 1000;

		internal const int DefaultConversionTimeout = 20;

		private static readonly string smimeKeyPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA\\SMime";

		internal static readonly string IMKeyPath = "SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA\\InstantMessaging";

		private static readonly string OwaSetupInstallKeyPath = "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Setup";

		private static readonly string[] keyPaths = new string[]
		{
			"SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA",
			"SYSTEM\\CurrentControlSet\\Services\\MSExchange OWA\\WebReadyDocumentViewing",
			OwaRegistryKeys.IMKeyPath,
			OwaRegistryKeys.smimeKeyPath,
			OwaRegistryKeys.OwaSetupInstallKeyPath
		};

		private static string defaultTempFolderLocation = Path.GetTempPath();

		private static OwaRegistryKey allowInternalUntrustedCertsKey = new OwaRegistryKey("AllowInternalUntrustedCerts", typeof(bool), true);

		private static OwaRegistryKey allowProxyingWithoutSslKey = new OwaRegistryKey("AllowProxyingWithoutSsl", typeof(bool), false);

		private static OwaRegistryKey maxRecipientsPerMessageKey = new OwaRegistryKey("MaxRecipientsPerMessage", typeof(int), 2000);

		private static OwaRegistryKey webReadyDocumentViewingRecycleByConversionsKey = new OwaRegistryKey("RecycleByConversions", typeof(int), 150);

		private static OwaRegistryKey webReadyDocumentViewingExcelRowsPerPageKey = new OwaRegistryKey("ExcelRowsPerPage", typeof(int), 200);

		private static OwaRegistryKey webReadyDocumentViewingMaxDocumentInputSizeKey = new OwaRegistryKey("MaxDocumentInputSize", typeof(int), 5000);

		private static OwaRegistryKey webReadyDocumentViewingMaxDocumentOutputSizeKey = new OwaRegistryKey("MaxDocumentOutputSize", typeof(int), 5000);

		private static OwaRegistryKey webReadyDocumentViewingWithInlineImageKey = new OwaRegistryKey("WebReadyDocumentViewingWithInlineImage", typeof(bool), true);

		private static OwaRegistryKey webReadyDocumentViewingTempFolderLocationKey = new OwaRegistryKey("TempFolderLocation", typeof(string), OwaRegistryKeys.DefaultTempFolderLocation);

		private static OwaRegistryKey webReadyDocumentViewingCacheDiskQuotaKey = new OwaRegistryKey("CacheDiskQuota", typeof(int), 1000);

		private static OwaRegistryKey webReadyDocumentViewingConversionTimeoutKey = new OwaRegistryKey("ConversionTimeout", typeof(int), 20);

		private static OwaRegistryKey webReadyDocumentViewingMemoryLimitInMBKey = new OwaRegistryKey("MemoryLimitInMB", typeof(int), 200);

		private static OwaRegistryKey forceSMimeClientUpgradeKey = new OwaRegistryKey("ForceSMimeClientUpgrade", typeof(bool), true);

		private static OwaRegistryKey checkCRLOnSendKey = new OwaRegistryKey("CheckCRLOnSend", typeof(bool), false);

		private static OwaRegistryKey dlExpansionTimeoutKey = new OwaRegistryKey("DLExpansionTimeout", typeof(int), 60000);

		private static OwaRegistryKey useSecondaryProxiesWhenFindingCertificatesKey = new OwaRegistryKey("UseSecondaryProxiesWhenFindingCertificates", typeof(bool), true);

		private static OwaRegistryKey clrConnectionTimeoutKey = new OwaRegistryKey("CRLConnectionTimeout", typeof(int), 60000);

		private static OwaRegistryKey clrRetrievalTimeoutKey = new OwaRegistryKey("CRLRetrievalTimeout", typeof(int), 10000);

		private static OwaRegistryKey disableCRLCheckKey = new OwaRegistryKey("DisableCRLCheck", typeof(bool), false);

		private static OwaRegistryKey alwaysSignKey = new OwaRegistryKey("AlwaysSign", typeof(bool), false);

		private static OwaRegistryKey alwaysEncryptKey = new OwaRegistryKey("AlwaysEncrypt", typeof(bool), false);

		private static OwaRegistryKey clearSignKey = new OwaRegistryKey("ClearSign", typeof(bool), true);

		private static OwaRegistryKey includeCertificateChainWithoutRootCertificateKey = new OwaRegistryKey("IncludeCertificateChainWithoutRootCertificate", typeof(bool), false);

		private static OwaRegistryKey includeCertificateChainAndRootCertificateKey = new OwaRegistryKey("IncludeCertificateChainAndRootCertificate", typeof(bool), false);

		private static OwaRegistryKey encryptTemporaryBuffersKey = new OwaRegistryKey("EncryptTemporaryBuffers", typeof(bool), true);

		private static OwaRegistryKey signedEmailCertificateInclusionKey = new OwaRegistryKey("SignedEmailCertificateInclusion", typeof(bool), true);

		private static OwaRegistryKey bccEncryptedEmailForkingKey = new OwaRegistryKey("BccEncryptedEmailForking", typeof(int), 0);

		private static OwaRegistryKey includeSMIMECapabilitiesInMessageKey = new OwaRegistryKey("IncludeSMIMECapabilitiesInMessage", typeof(bool), false);

		private static OwaRegistryKey copyRecipientHeadersKey = new OwaRegistryKey("CopyRecipientHeaders", typeof(bool), false);

		private static OwaRegistryKey onlyUseSmartCardKey = new OwaRegistryKey("OnlyUseSmartCard", typeof(bool), false);

		private static OwaRegistryKey tripleWrapSignedEncryptedMailKey = new OwaRegistryKey("TripleWrapSignedEncryptedMail", typeof(bool), true);

		private static OwaRegistryKey useKeyIdentifierKey = new OwaRegistryKey("UseKeyIdentifier", typeof(bool), false);

		private static OwaRegistryKey encryptionAlgorithmsKey = new OwaRegistryKey("EncryptionAlgorithms", typeof(string), string.Empty);

		private static OwaRegistryKey signingAlgorithmsKey = new OwaRegistryKey("SigningAlgorithms", typeof(string), string.Empty);

		private static OwaRegistryKey allowUserChoiceOfSigningCertificateKey = new OwaRegistryKey("AllowUserChoiceOfSigningCertificate", typeof(bool), false);

		private static OwaRegistryKey senderCertificateAttributesToDisplayKey = new OwaRegistryKey("SenderCertificateAttributesToDisplay", typeof(string), string.Empty);

		private static OwaRegistryKey useEmbeddedMessageFileNameAsAttachmentNameKey = new OwaRegistryKey("UseEmbeddedMessageFileNameAsAttachmentName", typeof(bool), false);

		private static OwaRegistryKey implementationDLLPathKey = new OwaRegistryKey("ImplementationDLLPath", typeof(string), string.Empty);

		private static OwaRegistryKey owaBasicVersionKey = new OwaRegistryKey("OwaBasicVersion", typeof(string), string.Empty);

		private static OwaRegistryKey[] owaKeys = new OwaRegistryKey[]
		{
			OwaRegistryKeys.allowInternalUntrustedCertsKey,
			OwaRegistryKeys.allowProxyingWithoutSslKey,
			OwaRegistryKeys.maxRecipientsPerMessageKey,
			OwaRegistryKeys.owaBasicVersionKey
		};

		private static OwaRegistryKey[] owaWebReadyDocumentViewingKeys = new OwaRegistryKey[]
		{
			OwaRegistryKeys.webReadyDocumentViewingRecycleByConversionsKey,
			OwaRegistryKeys.webReadyDocumentViewingExcelRowsPerPageKey,
			OwaRegistryKeys.webReadyDocumentViewingMaxDocumentInputSizeKey,
			OwaRegistryKeys.webReadyDocumentViewingMaxDocumentOutputSizeKey,
			OwaRegistryKeys.webReadyDocumentViewingWithInlineImageKey,
			OwaRegistryKeys.webReadyDocumentViewingTempFolderLocationKey,
			OwaRegistryKeys.webReadyDocumentViewingCacheDiskQuotaKey,
			OwaRegistryKeys.webReadyDocumentViewingConversionTimeoutKey,
			OwaRegistryKeys.webReadyDocumentViewingMemoryLimitInMBKey
		};

		private static OwaRegistryKey[] owaSMimeKeys = new OwaRegistryKey[]
		{
			OwaRegistryKeys.forceSMimeClientUpgradeKey,
			OwaRegistryKeys.checkCRLOnSendKey,
			OwaRegistryKeys.dlExpansionTimeoutKey,
			OwaRegistryKeys.useSecondaryProxiesWhenFindingCertificatesKey,
			OwaRegistryKeys.clrConnectionTimeoutKey,
			OwaRegistryKeys.clrRetrievalTimeoutKey,
			OwaRegistryKeys.disableCRLCheckKey,
			OwaRegistryKeys.alwaysSignKey,
			OwaRegistryKeys.alwaysEncryptKey,
			OwaRegistryKeys.clearSignKey,
			OwaRegistryKeys.includeCertificateChainWithoutRootCertificateKey,
			OwaRegistryKeys.includeCertificateChainAndRootCertificateKey,
			OwaRegistryKeys.encryptTemporaryBuffersKey,
			OwaRegistryKeys.signedEmailCertificateInclusionKey,
			OwaRegistryKeys.bccEncryptedEmailForkingKey,
			OwaRegistryKeys.includeSMIMECapabilitiesInMessageKey,
			OwaRegistryKeys.copyRecipientHeadersKey,
			OwaRegistryKeys.onlyUseSmartCardKey,
			OwaRegistryKeys.tripleWrapSignedEncryptedMailKey,
			OwaRegistryKeys.useKeyIdentifierKey,
			OwaRegistryKeys.encryptionAlgorithmsKey,
			OwaRegistryKeys.signingAlgorithmsKey,
			OwaRegistryKeys.allowUserChoiceOfSigningCertificateKey,
			OwaRegistryKeys.useEmbeddedMessageFileNameAsAttachmentNameKey,
			OwaRegistryKeys.senderCertificateAttributesToDisplayKey
		};

		private static OwaRegistryKey[] owaIMKeys = new OwaRegistryKey[]
		{
			OwaRegistryKeys.implementationDLLPathKey
		};

		private static readonly OwaRegistryKey[] owaSetupKeys = new OwaRegistryKey[]
		{
			OwaRegistryKeys.owaBasicVersionKey
		};

		private static OwaRegistryKey[][] keys = new OwaRegistryKey[][]
		{
			OwaRegistryKeys.owaKeys,
			OwaRegistryKeys.owaWebReadyDocumentViewingKeys,
			OwaRegistryKeys.owaIMKeys,
			OwaRegistryKeys.owaSMimeKeys,
			OwaRegistryKeys.owaSetupKeys
		};

		private static Dictionary<OwaRegistryKey, object> keyValueCache = new Dictionary<OwaRegistryKey, object>(OwaRegistryKeys.keys.Length);
	}
}
