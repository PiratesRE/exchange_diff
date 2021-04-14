using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Mime;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Data.Transport.Email;
using Microsoft.Exchange.Flighting;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.VariantConfiguration;
using Microsoft.Win32;

namespace Microsoft.Exchange.Transport.RightsManagement
{
	internal static class E4eHelper
	{
		internal static E4eEncryptionHelper GetE4eEncryptionHelper(MiniRecipient miniRecipient)
		{
			E4eEncryptionHelper instance = E4eEncryptionHelper.Instance;
			string s = string.Empty;
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(miniRecipient.GetContext(null), null, null);
			IVersion version = snapshot.E4E.Version;
			if (version != null)
			{
				s = version.VersionNum;
			}
			int num;
			if (int.TryParse(s, out num))
			{
				if (num == 1)
				{
					instance = E4eEncryptionHelper.Instance;
				}
				else if (num == 2)
				{
					instance = E4eEncryptionHelperV2.Instance;
				}
			}
			return instance;
		}

		internal static E4eDecryptionHelper GetE4eDecryptionHelper(string messageVersion)
		{
			if (string.IsNullOrWhiteSpace(messageVersion))
			{
				return E4eDecryptionHelper.Instance;
			}
			int num;
			if (!int.TryParse(messageVersion, out num))
			{
				throw new E4eException(string.Format("messageVersion: {0} could not be converted to an integer.", messageVersion));
			}
			if (num == 1)
			{
				return E4eDecryptionHelper.Instance;
			}
			if (num == 2)
			{
				return E4eDecryptionHelperV2.Instance;
			}
			throw new E4eException(string.Format("messageVersion: {0} is not valid.", messageVersion));
		}

		internal static void RunUnderExceptionHandler(string messageId, E4eHelper.E4eDelegate method, E4eHelper.CompleteProcessDelegate completeProcessDelegate, out Exception exception, out bool isTransientException)
		{
			exception = null;
			isTransientException = false;
			try
			{
				method();
			}
			catch (E4eException ex)
			{
				exception = ex;
				isTransientException = false;
			}
			catch (InvalidRpmsgFormatException ex2)
			{
				exception = ex2;
				isTransientException = false;
			}
			catch (RightsManagementException ex3)
			{
				exception = ex3;
				if (!ex3.IsPermanent)
				{
					isTransientException = true;
				}
			}
			catch (ExchangeConfigurationException ex4)
			{
				exception = ex4;
				isTransientException = true;
			}
			catch (CryptographicException ex5)
			{
				exception = ex5;
				isTransientException = false;
			}
			catch (SecurityException ex6)
			{
				exception = ex6;
				isTransientException = false;
			}
			catch (FormatException ex7)
			{
				exception = ex7;
				isTransientException = false;
			}
			catch (EncoderFallbackException ex8)
			{
				exception = ex8;
				isTransientException = false;
			}
			catch (TransientException ex9)
			{
				exception = ex9;
				isTransientException = true;
			}
			catch (MessageConversionException ex10)
			{
				exception = ex10;
				isTransientException = false;
			}
			catch (Exception ex11)
			{
				E4eLog.Instance.LogError(messageId, "Encountered a unknown exception: {0}. Re-throwing the exception.", new object[]
				{
					ex11.ToString()
				});
				if (completeProcessDelegate != null)
				{
					completeProcessDelegate(null);
				}
				throw;
			}
		}

		internal static MailDirectionality GetDirectionality(MailItem mailItem)
		{
			TransportMailItemWrapper transportMailItemWrapper = mailItem as TransportMailItemWrapper;
			if (transportMailItemWrapper != null && transportMailItemWrapper.TransportMailItem != null)
			{
				return transportMailItemWrapper.TransportMailItem.Directionality;
			}
			return MailDirectionality.Undefined;
		}

		internal static string GetP2From(EmailMessage emailMessage)
		{
			if (emailMessage.From == null)
			{
				return string.Empty;
			}
			return emailMessage.From.SmtpAddress;
		}

		internal static string GetCurrentSender(MailItem mailItem)
		{
			string text = mailItem.FromAddress.ToString();
			if (string.IsNullOrWhiteSpace(text) || mailItem.FromAddress.Equals(RoutingAddress.NullReversePath))
			{
				E4eLog.Instance.LogInfo(mailItem.Message.MessageId, "P1.From Address is not found for sender, using P2.From Address", new object[0]);
				text = E4eHelper.GetP2From(mailItem.Message);
			}
			return text;
		}

		internal static string GetOriginalSender(MailItem mailItem)
		{
			Header xheader = Utils.GetXHeader(mailItem.Message, "X-MS-Exchange-Organization-E4eMessageOriginalSender");
			string text = string.Empty;
			if (xheader != null)
			{
				text = xheader.Value;
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				text = E4eHelper.GetCurrentSender(mailItem);
			}
			return text;
		}

		internal static string GetCurrentSender(IReadOnlyMailItem mailItem)
		{
			string text = mailItem.From.ToString();
			if (string.IsNullOrWhiteSpace(text) || mailItem.From.Equals(RoutingAddress.NullReversePath))
			{
				E4eLog.Instance.LogInfo(mailItem.Message.MessageId, "P1.From Address is not found for sender, using P2.From Address", new object[0]);
				text = E4eHelper.GetP2From(mailItem.Message);
			}
			return text;
		}

		internal static string GetOriginalSender(IReadOnlyMailItem mailItem)
		{
			Header xheader = Utils.GetXHeader(mailItem.Message, "X-MS-Exchange-Organization-E4eMessageOriginalSender");
			string text = string.Empty;
			if (xheader != null)
			{
				text = xheader.Value;
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				text = E4eHelper.GetCurrentSender(mailItem);
			}
			return text;
		}

		internal static OrganizationId GetOriginalSenderOrgId(MailItem mailItem)
		{
			Header xheader = Utils.GetXHeader(mailItem.Message, "X-MS-Exchange-Organization-E4eMessageOriginalSenderOrgId");
			OrganizationId organizationId = null;
			if (xheader != null)
			{
				organizationId = E4eHelper.FromBase64String(xheader.Value);
			}
			if (organizationId == null)
			{
				organizationId = Utils.OrgIdFromMailItem(mailItem);
			}
			return organizationId;
		}

		internal static void GetCultureInfo(EmailMessage message, out string charsetName, out CultureInfo cultureInfo, out Encoding encoding)
		{
			if (!Utils.TryGetCultureInfoAndEncoding(message, out charsetName, out cultureInfo, out encoding))
			{
				throw new MessageConversionException(Strings.InvalidCharset(message.Body.CharsetName), false);
			}
		}

		internal static void LogAllE4eHeaders(EmailMessage message, string agentIdentifier)
		{
			E4eHelper.LogHeaderValue(message, "X-MS-Exchange-Organization-E4eMessageOriginalSender", agentIdentifier);
			E4eHelper.LogHeaderValue(message, "X-MS-Exchange-Organization-E4eMessageOriginalSenderOrgId", agentIdentifier);
			E4eHelper.LogHeaderValue(message, "X-MS-Exchange-Organization-E4eEncryptMessage", agentIdentifier);
			E4eHelper.LogHeaderValue(message, "X-MS-Exchange-Organization-E4eMessageEncrypted", agentIdentifier);
			E4eHelper.LogHeaderValue(message, "X-MS-Exchange-Organization-E4eHtmlFileGenerated", agentIdentifier);
			E4eHelper.LogHeaderValue(message, "X-MS-Exchange-Organization-E4eDecryptMessage", agentIdentifier);
			E4eHelper.LogHeaderValue(message, "X-MS-Exchange-Organization-E4eMessageDecrypted", agentIdentifier);
			E4eHelper.LogHeaderValue(message, "X-MS-Exchange-Organization-E4ePortal", agentIdentifier);
			E4eHelper.LogHeaderValue(message, "X-MS-Exchange-Organization-E4eReEncryptMessage", agentIdentifier);
			E4eHelper.LogHeaderValue(message, "X-MS-Exchange-OMEMessageEncrypted", agentIdentifier);
		}

		internal static void LogHeaderValue(EmailMessage message, string headerName, string agentIdentifier)
		{
			Header xheader = Utils.GetXHeader(message, headerName);
			if (xheader == null)
			{
				E4eLog.Instance.LogInfo(message.MessageId, "{0}Header '{1}' -- not found.", new object[]
				{
					agentIdentifier,
					headerName
				});
				return;
			}
			if (xheader.Value == null)
			{
				E4eLog.Instance.LogInfo(message.MessageId, "{0}Header '{1}' -- value is null", new object[]
				{
					agentIdentifier,
					headerName
				});
				return;
			}
			E4eLog.Instance.LogInfo(message.MessageId, "{0}Header '{1}' -- has value '{2}'", new object[]
			{
				agentIdentifier,
				headerName,
				xheader.Value
			});
		}

		internal static OrganizationId GetOriginalSenderOrgId(IReadOnlyMailItem mailItem)
		{
			Header xheader = Utils.GetXHeader(mailItem.Message, "X-MS-Exchange-Organization-E4eMessageOriginalSenderOrgId");
			OrganizationId organizationId = null;
			if (xheader != null)
			{
				organizationId = E4eHelper.FromBase64String(xheader.Value);
			}
			if (organizationId == null)
			{
				organizationId = mailItem.OrganizationId;
			}
			return organizationId;
		}

		internal static bool IsHeaderSetToTrue(EmailMessage emailMessage, string headerName)
		{
			Header xheader = Utils.GetXHeader(emailMessage, headerName);
			return xheader != null && xheader.Value.Equals("true", StringComparison.OrdinalIgnoreCase);
		}

		internal static void RemoveHeader(EmailMessage emailMessage, string headerName)
		{
			emailMessage.MimeDocument.RootPart.Headers.RemoveAll(headerName);
		}

		internal static void OverrideMime(MailItem mailItem, EmailMessage emailMessage)
		{
			try
			{
				using (Stream mimeWriteStream = mailItem.GetMimeWriteStream())
				{
					emailMessage.MimeDocument.RootPart.WriteTo(mimeWriteStream);
				}
			}
			finally
			{
				if (emailMessage != null)
				{
					((IDisposable)emailMessage).Dispose();
				}
			}
		}

		internal static byte[] ReadStreamToEnd(Stream stream)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				stream.CopyTo(memoryStream);
				result = memoryStream.ToArray();
			}
			return result;
		}

		internal static string ToBase64String(OrganizationId orgId)
		{
			return Convert.ToBase64String(orgId.GetBytes(Encoding.UTF8));
		}

		internal static OrganizationId FromBase64String(string orgId)
		{
			OrganizationId result;
			try
			{
				if (!OrganizationId.TryCreateFromBytes(Convert.FromBase64String(orgId), Encoding.UTF8, out result))
				{
					throw new E4eException("Could not create OrganizationId from base64 string.");
				}
			}
			catch (ADTransientException innerException)
			{
				throw new E4eException("Could not create OrganizationId from base64 string.", innerException);
			}
			return result;
		}

		internal static bool IsFlightingFeatureEnabledForSender(MailItem mailItem, string originalSenderAddress, OrganizationId originalSenderOrganizationId)
		{
			return E4eHelper.IsFlightingFeatureEnabled(E4eHelper.GetMiniRecipient(mailItem, originalSenderAddress, originalSenderOrganizationId));
		}

		internal static bool IsFlightingFeatureEnabled(MiniRecipient miniRecipient)
		{
			bool result = false;
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(miniRecipient.GetContext(null), null, null);
			IFeature e4E = snapshot.E4E.E4E;
			if (e4E != null)
			{
				result = e4E.Enabled;
			}
			return result;
		}

		internal static bool IsOTPEnabledForSender(string originalSenderAddress, OrganizationId originalSenderOrganizationId)
		{
			return E4eHelper.IsOTPEnabled(E4eHelper.CreateMiniRecipient(originalSenderAddress, originalSenderOrganizationId));
		}

		internal static bool IsOTPEnabled(MiniRecipient miniRecipient)
		{
			bool result = false;
			VariantConfigurationSnapshot snapshot = VariantConfiguration.GetSnapshot(miniRecipient.GetContext(null), null, null);
			IFeature otp = snapshot.E4E.OTP;
			if (otp != null)
			{
				result = otp.Enabled;
			}
			return result;
		}

		internal static MiniRecipient GetMiniRecipient(MailItem mailItem, string originalSenderAddress, OrganizationId originalSenderOrganizationId)
		{
			ProxyAddress proxyAddress = ProxyAddress.Parse(originalSenderAddress);
			ADRecipientCache<TransportMiniRecipient> adrecipientCache = (ADRecipientCache<TransportMiniRecipient>)mailItem.RecipientCache;
			Result<TransportMiniRecipient> result = adrecipientCache.FindAndCacheRecipient(proxyAddress);
			MiniRecipient result2;
			if (result.Data == null)
			{
				E4eLog.Instance.LogInfo(mailItem.Message.MessageId, "Unable to find transport mini recipient for sender: {0}.", new object[]
				{
					originalSenderAddress
				});
				result2 = E4eHelper.CreateMiniRecipient(originalSenderAddress, originalSenderOrganizationId);
			}
			else
			{
				result2 = result.Data;
			}
			return result2;
		}

		internal static MiniRecipient CreateMiniRecipient(string originalSenderAddress, OrganizationId originalSenderOrganizationId)
		{
			MiniRecipient miniRecipient = new MiniRecipient();
			miniRecipient[MiniRecipientSchema.UserPrincipalName] = originalSenderAddress;
			miniRecipient[ADObjectSchema.OrganizationId] = originalSenderOrganizationId;
			miniRecipient[MiniRecipientSchema.Languages] = new MultiValuedProperty<CultureInfo>();
			return miniRecipient;
		}

		internal static string GetDefaultAcceptedDomainName(OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			string text;
			if (E4eHelper.defaultAcceptedDomainTable.TryGetValue(organizationId, out text))
			{
				return text;
			}
			IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 763, "GetDefaultAcceptedDomainName", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\RightsManagement\\E4eHelper.cs");
			Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain defaultAcceptedDomain = tenantOrTopologyConfigurationSession.GetDefaultAcceptedDomain();
			if (defaultAcceptedDomain != null)
			{
				text = defaultAcceptedDomain.DomainName.ToString();
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				E4eHelper.defaultAcceptedDomainTable.Add(organizationId, text);
			}
			return text;
		}

		internal static InboundConversionOptions GetInboundConversionOptions(OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			InboundConversionOptions inboundConversionOptions = new InboundConversionOptions(E4eHelper.GetDefaultAcceptedDomainName(organizationId));
			inboundConversionOptions.LoadPerOrganizationCharsetDetectionOptions(organizationId);
			return inboundConversionOptions;
		}

		internal static OutboundConversionOptions GetOutboundConversionOptions(OrganizationId organizationId)
		{
			if (organizationId == null)
			{
				throw new ArgumentNullException("organizationId");
			}
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId), 818, "GetOutboundConversionOptions", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\RightsManagement\\E4eHelper.cs");
			return new OutboundConversionOptions(E4eHelper.GetDefaultAcceptedDomainName(organizationId))
			{
				ClearCategories = false,
				AllowPartialStnefConversion = true,
				DemoteBcc = true,
				UserADSession = tenantOrRootOrgRecipientSession
			};
		}

		internal static string GetCertificateName()
		{
			if (!string.IsNullOrEmpty(E4eHelper.cachedE4eCertificateName))
			{
				return E4eHelper.cachedE4eCertificateName;
			}
			string result;
			lock (E4eHelper.cacheLock)
			{
				if (!string.IsNullOrEmpty(E4eHelper.cachedE4eCertificateName))
				{
					result = E4eHelper.cachedE4eCertificateName;
				}
				else
				{
					object obj2 = E4eHelper.ReadRegistryKey("SOFTWARE\\Microsoft\\ExchangeLabs", "E4eCertificateDistinguishedName");
					if (obj2 == null)
					{
						result = "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";
					}
					else
					{
						string value = obj2.ToString();
						if (string.IsNullOrEmpty(value))
						{
							result = "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";
						}
						else
						{
							E4eHelper.cachedE4eCertificateName = value;
							result = E4eHelper.cachedE4eCertificateName;
						}
					}
				}
			}
			return result;
		}

		internal static object ReadRegistryKey(string keyPath, string valueName)
		{
			using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(keyPath))
			{
				if (registryKey != null)
				{
					return registryKey.GetValue(valueName, null);
				}
			}
			return null;
		}

		internal static void GetTransportPLAndULAndLicenseUri(MailItem mailItem, out string publishingLicense, out string useLicense, out Uri licenseUri)
		{
			object obj;
			mailItem.Properties.TryGetValue("Microsoft.Exchange.Encryption.TransportDecryptionPL", out obj);
			publishingLicense = (string)obj;
			mailItem.Properties.TryGetValue("Microsoft.Exchange.Encryption.TransportDecryptionUL", out obj);
			useLicense = (string)obj;
			licenseUri = null;
			if (mailItem.Properties.TryGetValue("Microsoft.Exchange.Encryption.TransportDecryptionLicenseUri", out obj) && obj != null)
			{
				Uri.TryCreate((string)obj, UriKind.Absolute, out licenseUri);
			}
		}

		internal static void GetTransportPLAndULAndLicenseUri(IReadOnlyMailItem mailItem, out string publishingLicense, out string useLicense, out Uri licenseUri)
		{
			mailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.Encryption.TransportDecryptionPL", out publishingLicense);
			mailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.Encryption.TransportDecryptionUL", out useLicense);
			licenseUri = null;
			string text;
			if (mailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.Encryption.TransportDecryptionLicenseUri", out text) && !string.IsNullOrWhiteSpace(text))
			{
				Uri.TryCreate(text, UriKind.Absolute, out licenseUri);
			}
		}

		internal static void SetTransportPLAndULAndLicenseUri(MailItem mailItem, string publishingLicense, string useLicense, Uri licenseUri)
		{
			mailItem.Properties["Microsoft.Exchange.Encryption.TransportDecryptionPL"] = publishingLicense;
			mailItem.Properties["Microsoft.Exchange.Encryption.TransportDecryptionUL"] = useLicense;
			mailItem.Properties["Microsoft.Exchange.Encryption.TransportDecryptionLicenseUri"] = ((licenseUri == null) ? string.Empty : licenseUri.OriginalString);
		}

		internal static bool IsPipelineDecrypted(IReadOnlyMailItem mailItem)
		{
			string value = null;
			mailItem.ExtendedProperties.TryGetValue<string>("Microsoft.Exchange.Encryption.TransportDecryptionPL", out value);
			return !string.IsNullOrWhiteSpace(value);
		}

		internal static bool IsPipelineDecrypted(MailItem mailItem)
		{
			object obj;
			mailItem.Properties.TryGetValue("Microsoft.Exchange.Encryption.TransportDecryptionPL", out obj);
			string value = (string)obj;
			return !string.IsNullOrWhiteSpace(value);
		}

		internal const string EncryptedAttachmentFileName = "message.html";

		internal const string VersionInputName = "version";

		internal const string MetadataInputName = "metadata";

		internal const string SignatureInputName = "signature";

		internal const string CertificateInputName = "certificate";

		internal const string OTPButtonInputName = "OTPButton";

		internal const string RpmsgInputName = "rpmsg";

		internal const int MaxDeferrals = 3;

		internal const int MetadataLength = 5;

		internal const char MetadataSplitChar = '|';

		internal const string MicrosoftHostedKeyPath = "SOFTWARE\\Microsoft\\ExchangeLabs";

		private const string DefaultCertificateName = "CN=auth.outlook.com, OU=Exchange, O=Microsoft Corporation, L=Redmond, S=Washington, C=US";

		private static string cachedE4eCertificateName = string.Empty;

		private static object cacheLock = new object();

		private static MruDictionaryCache<OrganizationId, string> defaultAcceptedDomainTable = new MruDictionaryCache<OrganizationId, string>(5, 50000, 5);

		internal delegate void E4eDelegate();

		internal delegate void CompleteProcessDelegate(AgentAsyncState agentAsyncState);

		internal enum MetaDataIndex
		{
			OriginalSender,
			Recipient,
			OriginalSenderOrgId,
			SentTime,
			MessageId
		}
	}
}
