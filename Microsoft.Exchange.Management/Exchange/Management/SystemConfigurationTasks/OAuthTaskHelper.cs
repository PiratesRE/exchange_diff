using System;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Management.FederationProvisioning;
using Microsoft.Exchange.Management.ReportingTask;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Security.Dkm;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class OAuthTaskHelper
	{
		public static bool IsMultiTenancyEnabled
		{
			get
			{
				if (OAuthTaskHelper.tenancyEnabled == null)
				{
					lock (OAuthTaskHelper.lockObject)
					{
						if (OAuthTaskHelper.tenancyEnabled == null)
						{
							OAuthTaskHelper.tenancyEnabled = VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy;
						}
					}
				}
				return OAuthTaskHelper.tenancyEnabled.Enabled;
			}
		}

		public static MultiValuedProperty<byte[]> CertificateFromRawBytes(MultiValuedProperty<byte[]> rawBytes, Task.TaskErrorLoggingDelegate writeError)
		{
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (rawBytes == null)
			{
				return null;
			}
			byte[][] rawBytesArray = null;
			try
			{
				rawBytesArray = rawBytes.ToArray();
			}
			catch (InvalidOperationException innerException)
			{
				writeError(new TaskException(Strings.ErrorNotSupportedModifyMultivaluedProperties, innerException), ErrorCategory.InvalidArgument, null);
			}
			return OAuthTaskHelper.InternalCertificateFromRawBytes(rawBytesArray, writeError);
		}

		public static MultiValuedProperty<byte[]> CertificateFromBase64String(MultiValuedProperty<string> rawStrings, Task.TaskErrorLoggingDelegate writeError)
		{
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (rawStrings == null)
			{
				return null;
			}
			string[] rawStringArray = null;
			try
			{
				rawStringArray = rawStrings.ToArray();
			}
			catch (InvalidOperationException innerException)
			{
				writeError(new TaskException(Strings.ErrorNotSupportedModifyMultivaluedProperties, innerException), ErrorCategory.InvalidArgument, null);
			}
			return OAuthTaskHelper.InternalCertificateFromBase64String(rawStringArray, writeError);
		}

		private static MultiValuedProperty<byte[]> InternalCertificateFromRawBytes(byte[][] rawBytesArray, Task.TaskErrorLoggingDelegate writeError)
		{
			int num = rawBytesArray.Length;
			if (num > OAuthTaskHelper.MaxCertCount)
			{
				writeError(new TaskException(Strings.ErrorTooManyCertificates), ErrorCategory.InvalidArgument, null);
			}
			MultiValuedProperty<byte[]> result;
			try
			{
				byte[][] array = new byte[num][];
				for (int i = 0; i < num; i++)
				{
					array[i] = new X509Certificate2(rawBytesArray[i]).GetRawCertData();
				}
				result = new MultiValuedProperty<byte[]>(array);
			}
			catch (CryptographicException innerException)
			{
				writeError(new TaskException(Strings.ErrorInvalidCertificateData, innerException), ErrorCategory.InvalidArgument, null);
				throw;
			}
			return result;
		}

		private static MultiValuedProperty<byte[]> InternalCertificateFromBase64String(string[] rawStringArray, Task.TaskErrorLoggingDelegate writeError)
		{
			MultiValuedProperty<byte[]> result;
			try
			{
				result = OAuthTaskHelper.InternalCertificateFromRawBytes((from s in rawStringArray
				select Convert.FromBase64String(s)).ToArray<byte[]>(), writeError);
			}
			catch (ArgumentNullException innerException)
			{
				writeError(new TaskException(Strings.ErrorInvalidBase64String, innerException), ErrorCategory.InvalidArgument, null);
				throw;
			}
			catch (FormatException innerException2)
			{
				writeError(new TaskException(Strings.ErrorInvalidBase64String, innerException2), ErrorCategory.InvalidArgument, null);
				throw;
			}
			return result;
		}

		public static string EncryptSecretWithDKM(string secretInClearText, Task.TaskErrorLoggingDelegate writeError)
		{
			if (string.IsNullOrEmpty(secretInClearText))
			{
				throw new ArgumentNullException("secretInClearText");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			ExchangeGroupKey exchangeGroupKey = new ExchangeGroupKey(null, "Microsoft Exchange DKM");
			string result;
			try
			{
				result = exchangeGroupKey.ClearStringToEncryptedString(secretInClearText);
			}
			catch (Exception ex)
			{
				if (ex is CryptographicException || ex is InvalidDataException || exchangeGroupKey.IsDkmException(ex))
				{
					writeError(ex, ErrorCategory.InvalidData, null);
				}
				throw;
			}
			return result;
		}

		public static void ValidateApplicationRealmAndUniqueness(PartnerApplication partnerApplication, IConfigurationSession configSession, Task.TaskErrorLoggingDelegate writeError)
		{
			if (partnerApplication == null)
			{
				throw new ArgumentNullException("partnerApplication");
			}
			if (configSession == null)
			{
				throw new ArgumentNullException("configSession");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (!partnerApplication.IsModified(PartnerApplicationSchema.ApplicationIdentifier) && !partnerApplication.IsModified(PartnerApplicationSchema.Realm) && !partnerApplication.IsModified(PartnerApplicationSchema.IssuerIdentifier))
			{
				return;
			}
			if (OAuthCommon.IsRealmEmpty(partnerApplication.Realm) && !partnerApplication.UseAuthServer)
			{
				writeError(new TaskException(Strings.ErrorPartnerApplicationEmptyRealmWhenNotUseAuthServer), ErrorCategory.InvalidArgument, null);
			}
			ADObjectId containerId = PartnerApplication.GetContainerId(configSession);
			PartnerApplication[] source = configSession.Find<PartnerApplication>(containerId, QueryScope.OneLevel, new ComparisonFilter(ComparisonOperator.Equal, PartnerApplicationSchema.ApplicationIdentifier, partnerApplication.ApplicationIdentifier), null, ADGenericPagedReader<PartnerApplication>.DefaultPageSize);
			PartnerApplication partnerApplication2 = source.FirstOrDefault((PartnerApplication existingApp) => (OAuthCommon.IsRealmEmpty(partnerApplication.Realm) ? OAuthCommon.IsRealmEmpty(existingApp.Realm) : OAuthCommon.IsRealmMatch(existingApp.Realm, partnerApplication.Realm)) && !existingApp.Id.Equals(partnerApplication.Id));
			if (partnerApplication2 != null)
			{
				writeError(new TaskException(Strings.ErrorDuplicatePartnerApplication(partnerApplication2.Id.ToString())), ErrorCategory.InvalidArgument, null);
			}
			if (!string.IsNullOrEmpty(partnerApplication.IssuerIdentifier))
			{
				PartnerApplication partnerApplication3 = null;
				foreach (PartnerApplication partnerApplication4 in configSession.FindPaged<PartnerApplication>(containerId, QueryScope.OneLevel, null, null, ADGenericPagedReader<PartnerApplication>.DefaultPageSize))
				{
					if (partnerApplication4.IssuerIdentifier == partnerApplication.IssuerIdentifier && !partnerApplication4.Id.Equals(partnerApplication.Id))
					{
						partnerApplication3 = partnerApplication4;
						break;
					}
				}
				if (partnerApplication3 != null)
				{
					writeError(new TaskException(Strings.ErrorDuplicatePartnerApplication(partnerApplication3.Id.ToString())), ErrorCategory.InvalidArgument, null);
				}
			}
		}

		public static void ValidateAuthServerRealmAndUniqueness(AuthServer authServer, IConfigurationSession configSession, Task.TaskErrorLoggingDelegate writeError)
		{
			if (authServer == null)
			{
				throw new ArgumentNullException("authServer");
			}
			if (configSession == null)
			{
				throw new ArgumentNullException("configSession");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (!authServer.IsModified(AuthServerSchema.IssuerIdentifier) && !authServer.IsModified(AuthServerSchema.Realm))
			{
				return;
			}
			bool flag = OAuthCommon.IsRealmEmpty(authServer.Realm);
			bool flag2 = false;
			if (authServer.Type == AuthServerType.MicrosoftACS || authServer.Type == AuthServerType.AzureAD)
			{
				Guid guid;
				if (!OAuthTaskHelper.IsMultiTenancyEnabled)
				{
					if (flag || !Guid.TryParse(authServer.Realm, out guid))
					{
						flag2 = true;
					}
				}
				else if (!flag && !Guid.TryParse(authServer.Realm, out guid))
				{
					flag2 = true;
				}
			}
			if (flag2)
			{
				writeError(new TaskException(Strings.ErrorInvalidAuthServerRealm(authServer.Realm)), ErrorCategory.InvalidArgument, null);
			}
			ADObjectId containerId = AuthServer.GetContainerId(configSession);
			AuthServer[] array = configSession.Find<AuthServer>(containerId, QueryScope.OneLevel, null, null, ADGenericPagedReader<AuthServer>.DefaultPageSize);
			if (array == null || array.Length == 0)
			{
				return;
			}
			AuthServer authServer2 = array.FirstOrDefault((AuthServer existingAuthServer) => string.Equals(existingAuthServer.IssuerIdentifier, authServer.IssuerIdentifier, StringComparison.OrdinalIgnoreCase) && existingAuthServer.Type == authServer.Type && OAuthCommon.IsRealmMatchIncludingEmpty(existingAuthServer.Realm, authServer.Realm) && !existingAuthServer.Id.Equals(authServer.Id));
			if (authServer2 != null)
			{
				writeError(new TaskException(Strings.ErrorDuplicateAuthServer(authServer2.Id.ToString())), ErrorCategory.InvalidArgument, null);
			}
			if (authServer.Type != AuthServerType.MicrosoftACS && authServer.Type != AuthServerType.AzureAD)
			{
				return;
			}
			authServer2 = array.FirstOrDefault((AuthServer existingAuthServer) => existingAuthServer.Type == authServer.Type && OAuthCommon.IsRealmMatchIncludingEmpty(existingAuthServer.Realm, authServer.Realm) && string.Equals(existingAuthServer.IssuerIdentifier, authServer.IssuerIdentifier, StringComparison.OrdinalIgnoreCase) && !existingAuthServer.Id.Equals(authServer.Id));
			if (authServer2 != null)
			{
				writeError(new TaskException(flag ? Strings.ErrorExistingAuthServerWithEmptyRealm(authServer2.Id.ToString()) : Strings.ErrorExistingAuthServerWithSameRealm(authServer2.Id.ToString(), authServer.Realm)), ErrorCategory.InvalidArgument, null);
			}
		}

		public static void ValidateAuthServerAuthorizationEndpoint(AuthServer authServer, IConfigurationSession configSession, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError)
		{
			if (authServer == null)
			{
				throw new ArgumentNullException("authServer");
			}
			if (configSession == null)
			{
				throw new ArgumentNullException("configSession");
			}
			if (writeWarning == null)
			{
				throw new ArgumentNullException("writeWarning");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			ADObjectId containerId = AuthServer.GetContainerId(configSession);
			AuthServer[] array = configSession.Find<AuthServer>(containerId, QueryScope.OneLevel, null, null, ADGenericPagedReader<AuthServer>.DefaultPageSize);
			if (array == null || array.Length == 0)
			{
				return;
			}
			AuthServer authServer2 = array.FirstOrDefault((AuthServer existingAuthServer) => (existingAuthServer.Type == AuthServerType.ADFS || existingAuthServer.Type == AuthServerType.AzureAD) && existingAuthServer.IsDefaultAuthorizationEndpoint);
			if (authServer2 != null && authServer.IsDefaultAuthorizationEndpoint)
			{
				writeError(new TaskException(Strings.ErrorExistingAuthServerWithDefaultAuthorizationEndpoint(authServer2.Name)), ErrorCategory.InvalidArgument, null);
			}
			if (authServer2 == null && !authServer.IsDefaultAuthorizationEndpoint)
			{
				writeWarning(Strings.WarningNoAuthServerWithDefaultAuthorizationEndpoint);
			}
		}

		public static void FetchAuthMetadata(PartnerApplication partnerApplication, bool trustSslCert, bool updatePidOrRealmOrIssuer, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError)
		{
			if (partnerApplication == null)
			{
				throw new ArgumentNullException("partnerApplication");
			}
			if (writeWarning == null)
			{
				throw new ArgumentNullException("writeWarning");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			AuthMetadata authMetadata = OAuthTaskHelper.FetchAuthMetadata(partnerApplication.AuthMetadataUrl, trustSslCert, false, writeWarning, writeError);
			if (updatePidOrRealmOrIssuer)
			{
				partnerApplication.ApplicationIdentifier = authMetadata.ServiceName;
				partnerApplication.IssuerIdentifier = authMetadata.Issuer;
				partnerApplication.Realm = authMetadata.Realm;
			}
			else if (!OAuthCommon.IsIdMatch(partnerApplication.ApplicationIdentifier, authMetadata.ServiceName) || !OAuthCommon.IsRealmMatchIncludingEmpty(partnerApplication.Realm, authMetadata.Realm) || !string.Equals(partnerApplication.IssuerIdentifier, authMetadata.Issuer))
			{
				writeError(new TaskException(Strings.ErrorPidRealmIssuerDifferentFromMetadata(authMetadata.ServiceName, authMetadata.Realm, authMetadata.Issuer, partnerApplication.ApplicationIdentifier, partnerApplication.Realm, partnerApplication.IssuerIdentifier)), ErrorCategory.InvalidData, null);
			}
			partnerApplication.CertificateBytes = OAuthTaskHelper.InternalCertificateFromBase64String(authMetadata.CertificateStrings, writeError);
		}

		public static void FetchAuthMetadata(AuthServer authServer, bool trustSslCert, bool updateIdRealm, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError)
		{
			if (authServer == null)
			{
				throw new ArgumentNullException("authServer");
			}
			if (writeWarning == null)
			{
				throw new ArgumentNullException("writeWarning");
			}
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			AuthMetadata authMetadata = OAuthTaskHelper.FetchAuthMetadata(authServer.AuthMetadataUrl, trustSslCert, true, writeWarning, writeError);
			AuthMetadataParser.SetEndpointsIfWSFed(authMetadata, authServer.Type, authServer.AuthMetadataUrl);
			if (updateIdRealm)
			{
				authServer.IssuerIdentifier = authMetadata.ServiceName;
				authServer.Realm = authMetadata.Realm;
			}
			else if (!OAuthCommon.IsIdMatch(authServer.IssuerIdentifier, authMetadata.ServiceName) || !OAuthCommon.IsRealmMatchIncludingEmpty(authServer.Realm, authMetadata.Realm))
			{
				writeError(new TaskException(Strings.ErrorPidRealmDifferentFromMetadata(authMetadata.ServiceName, authMetadata.Realm, authServer.IssuerIdentifier, authServer.Realm)), ErrorCategory.InvalidData, null);
			}
			authServer.CertificateBytes = OAuthTaskHelper.InternalCertificateFromBase64String(authMetadata.CertificateStrings, writeError);
			authServer.TokenIssuingEndpoint = authMetadata.IssuingEndpoint;
			authServer.AuthorizationEndpoint = authMetadata.AuthorizationEndpoint;
		}

		private static AuthMetadata FetchAuthMetadata(string authMetadataUrl, bool trustSslCert, bool requireIssuingEndpoint, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError)
		{
			try
			{
				Uri uri = new Uri(authMetadataUrl);
				if (uri.Scheme != Uri.UriSchemeHttps)
				{
					writeWarning(Strings.WarningMetadataNotOverHttps(authMetadataUrl));
				}
			}
			catch (ArgumentNullException)
			{
				writeError(new TaskException(Strings.ErrorInvalidMetadataUrl(authMetadataUrl)), ErrorCategory.InvalidArgument, null);
			}
			catch (UriFormatException)
			{
				writeError(new TaskException(Strings.ErrorInvalidMetadataUrl(authMetadataUrl)), ErrorCategory.InvalidArgument, null);
			}
			AuthMetadata result = null;
			try
			{
				result = AuthMetadataClient.AcquireMetadata(authMetadataUrl, requireIssuingEndpoint, trustSslCert, true);
			}
			catch (AuthMetadataClientException exception)
			{
				writeError(exception, ErrorCategory.ResourceUnavailable, null);
			}
			catch (AuthMetadataParserException exception2)
			{
				writeError(exception2, ErrorCategory.ParserError, null);
			}
			return result;
		}

		public static void FixAuthMetadataUrl(AuthServer authServer, Task.TaskErrorLoggingDelegate writeError)
		{
			if (authServer.Type == AuthServerType.ADFS)
			{
				Uri uri = null;
				string authMetadataUrl = authServer.AuthMetadataUrl;
				try
				{
					uri = new Uri(authServer.AuthMetadataUrl);
				}
				catch (ArgumentNullException)
				{
					writeError(new TaskException(Strings.ErrorInvalidMetadataUrl(authMetadataUrl)), ErrorCategory.InvalidArgument, null);
				}
				catch (UriFormatException)
				{
					writeError(new TaskException(Strings.ErrorInvalidMetadataUrl(authMetadataUrl)), ErrorCategory.InvalidArgument, null);
				}
				if ((uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp) && uri.AbsolutePath == "/")
				{
					authServer.AuthMetadataUrl = new UriBuilder(uri)
					{
						Path = "federationmetadata/2007-06/federationmetadata.xml"
					}.ToString();
				}
			}
		}

		public static bool IsDatacenter()
		{
			return OAuthTaskHelper.IsMultiTenancyEnabled;
		}

		public static void ValidateLocalCertificate(string thumbprint, DateTime? futurePublishDate, bool skipAutomatedDeploymentChecks, Task.TaskErrorLoggingDelegate writeError)
		{
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (string.IsNullOrEmpty(thumbprint))
			{
				return;
			}
			X509Store x509Store = null;
			try
			{
				x509Store = new X509Store(StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.ReadOnly);
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
				if (x509Certificate2Collection.Count == 0)
				{
					writeError(new TaskException(Strings.ErrorThumbprintNotFound(thumbprint)), ErrorCategory.InvalidArgument, null);
				}
				ExchangeCertificate certificate = new ExchangeCertificate(x509Certificate2Collection[0]);
				OAuthTaskHelper.ValidateCertificate(certificate, futurePublishDate, skipAutomatedDeploymentChecks, writeError);
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
		}

		public static void ValidateRemoteCertificate(string server, string thumbprint, DateTime? futurePublishDate, bool skipAutomatedDeploymentChecks, Task.TaskErrorLoggingDelegate writeError)
		{
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (string.IsNullOrEmpty(thumbprint))
			{
				return;
			}
			ExchangeCertificate certificate = null;
			FederationTrustCertificateState federationTrustCertificateState = FederationCertificate.TestForCertificate(server, thumbprint, out certificate);
			if (federationTrustCertificateState == FederationTrustCertificateState.ServerUnreachable)
			{
				writeError(new TaskException(Strings.ErrorCannotContactServerForCert(server, thumbprint)), ErrorCategory.InvalidArgument, null);
			}
			else if (federationTrustCertificateState != FederationTrustCertificateState.Installed)
			{
				writeError(new TaskException(Strings.ErrorThumbprintNotFound(thumbprint)), ErrorCategory.InvalidArgument, null);
			}
			OAuthTaskHelper.ValidateCertificate(certificate, futurePublishDate, skipAutomatedDeploymentChecks, writeError);
		}

		public static void ValidateCertificate(ExchangeCertificate certificate, DateTime? futurePublishDate, bool skipAutomatedDeploymentChecks, Task.TaskErrorLoggingDelegate writeError)
		{
			if (writeError == null)
			{
				throw new ArgumentNullException("writeError");
			}
			if (certificate == null)
			{
				return;
			}
			try
			{
				ExchangeCertificateValidity exchangeCertificateValidity = ManageExchangeCertificate.ValidateExchangeCertificate(certificate, true);
				if (exchangeCertificateValidity != ExchangeCertificateValidity.Valid)
				{
					writeError(new TaskException(Strings.CertificateNotValidForExchange(certificate.Thumbprint, exchangeCertificateValidity.ToString())), ErrorCategory.InvalidArgument, null);
				}
				if (!skipAutomatedDeploymentChecks && !certificate.PrivateKeyExportable)
				{
					writeError(new TaskException(Strings.ErrorCertificateNotExportable(certificate.Thumbprint)), ErrorCategory.InvalidArgument, null);
				}
				if ((ExDateTime)certificate.NotAfter < ExDateTime.UtcNow)
				{
					writeError(new TaskException(Strings.ErrorCertificateHasExpired(certificate.Thumbprint)), ErrorCategory.InvalidArgument, null);
				}
				if ((ExDateTime)certificate.NotBefore > ExDateTime.UtcNow)
				{
					writeError(new TaskException(Strings.ErrorCertificateNotYetValid(certificate.Thumbprint)), ErrorCategory.InvalidArgument, null);
				}
				if (futurePublishDate != null && futurePublishDate != null && (ExDateTime)certificate.NotAfter <= (ExDateTime)futurePublishDate.Value.ToUniversalTime())
				{
					writeError(new TaskException(Strings.ErrorAuthNewCertificateExpire(certificate.Thumbprint)), ErrorCategory.InvalidArgument, null);
				}
			}
			catch (CryptographicException innerException)
			{
				writeError(new TaskException(Strings.ErrorFailedToValidateCertificate(certificate.Thumbprint), innerException), ErrorCategory.InvalidArgument, null);
			}
		}

		private static object lockObject = new object();

		private static IFeature tenancyEnabled;

		private static readonly int MaxCertCount = 10;
	}
}
