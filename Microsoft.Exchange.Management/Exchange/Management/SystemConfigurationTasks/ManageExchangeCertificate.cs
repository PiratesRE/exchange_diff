using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Metabase;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.MessageSecurity.EdgeSync;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Security.Cryptography;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public static class ManageExchangeCertificate
	{
		internal static void ProcessRpcError(RpcException e, string server, out LocalizedException outException, out ErrorCategory outCategory)
		{
			if (e.ErrorCode == 1753)
			{
				outException = new LocalizedException(Strings.RpcNotRegistered(server));
				outCategory = ErrorCategory.ResourceUnavailable;
				return;
			}
			if (e.ErrorCode == 1722)
			{
				outException = new LocalizedException(Strings.RpcUnavailable(server));
				outCategory = ErrorCategory.InvalidOperation;
				return;
			}
			Win32Exception ex = new Win32Exception(e.ErrorCode);
			outException = new LocalizedException(Strings.GenericRpcError(server, ex.Message), ex);
			outCategory = ErrorCategory.InvalidOperation;
		}

		internal static void ThrowLocalizedException(RpcException e, string server)
		{
			LocalizedException ex;
			ErrorCategory errorCategory;
			ManageExchangeCertificate.ProcessRpcError(e, server, out ex, out errorCategory);
			throw ex;
		}

		internal static void WriteRpcError(RpcException e, string server, Task.TaskErrorLoggingDelegate errorHandler)
		{
			LocalizedException exception;
			ErrorCategory category;
			ManageExchangeCertificate.ProcessRpcError(e, server, out exception, out category);
			errorHandler(exception, category, null);
		}

		internal static ExchangeCertificateValidity ValidateExchangeCertificate(X509Certificate2 cert, bool ignoreAccessible)
		{
			if (cert == null)
			{
				throw new ArgumentNullException("cert");
			}
			if (!cert.HasPrivateKey)
			{
				return ExchangeCertificateValidity.PrivateKeyMissing;
			}
			string keyAlgorithm = cert.GetKeyAlgorithm();
			bool flag = string.Equals(keyAlgorithm, WellKnownOid.X957Sha1Dsa.Value, StringComparison.OrdinalIgnoreCase);
			if (!string.Equals(keyAlgorithm, WellKnownOid.RsaRsa.Value, StringComparison.OrdinalIgnoreCase) && !flag)
			{
				return ExchangeCertificateValidity.KeyAlgorithmUnsupported;
			}
			foreach (X509Extension x509Extension in cert.Extensions)
			{
				try
				{
					X509KeyUsageExtension x509KeyUsageExtension = x509Extension as X509KeyUsageExtension;
					if (x509KeyUsageExtension != null)
					{
						X509KeyUsageFlags keyUsages = x509KeyUsageExtension.KeyUsages;
						bool flag2 = false;
						if (keyUsages == X509KeyUsageFlags.None)
						{
							flag2 = true;
						}
						else if ((keyUsages & (X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature)) != X509KeyUsageFlags.None)
						{
							flag2 = true;
						}
						if (!flag2)
						{
							return ExchangeCertificateValidity.SigningNotSupported;
						}
					}
				}
				catch (CryptographicException)
				{
					return ExchangeCertificateValidity.KeyUsageCorrupted;
				}
				try
				{
					X509EnhancedKeyUsageExtension x509EnhancedKeyUsageExtension = x509Extension as X509EnhancedKeyUsageExtension;
					if (x509EnhancedKeyUsageExtension != null && x509EnhancedKeyUsageExtension.EnhancedKeyUsages.Count > 0 && x509EnhancedKeyUsageExtension.EnhancedKeyUsages[WellKnownOid.PkixKpServerAuth.Value] == null)
					{
						return ExchangeCertificateValidity.PkixKpServerAuthNotFoundInEnhancedKeyUsage;
					}
				}
				catch (CryptographicException)
				{
					return ExchangeCertificateValidity.EnhancedKeyUsageCorrupted;
				}
			}
			if (TlsCertificateInfo.IsCNGProvider(cert))
			{
				return ManageExchangeCertificate.CheckCNGSettings(cert);
			}
			AsymmetricAlgorithm privateKey;
			try
			{
				privateKey = cert.PrivateKey;
			}
			catch (CryptographicException)
			{
				return ExchangeCertificateValidity.PrivateKeyNotAccessible;
			}
			ICspAsymmetricAlgorithm cspAsymmetricAlgorithm = privateKey as ICspAsymmetricAlgorithm;
			if (cspAsymmetricAlgorithm == null)
			{
				return ExchangeCertificateValidity.PrivateKeyUnsupportedAlgorithm;
			}
			CspKeyContainerInfo cspKeyContainerInfo = cspAsymmetricAlgorithm.CspKeyContainerInfo;
			if (cspKeyContainerInfo.Protected)
			{
				return ExchangeCertificateValidity.CspKeyContainerInfoProtected;
			}
			if (cspKeyContainerInfo.HardwareDevice && cspKeyContainerInfo.Removable)
			{
				return ExchangeCertificateValidity.CspKeyContainerInfoRemovableDevice;
			}
			if (!ignoreAccessible && !cspKeyContainerInfo.Accessible)
			{
				return ExchangeCertificateValidity.CspKeyContainerInfoNotAccessible;
			}
			switch (cspKeyContainerInfo.KeyNumber)
			{
			case KeyNumber.Exchange:
			case KeyNumber.Signature:
			{
				AsymmetricAlgorithm key = cert.PublicKey.Key;
				if (key.KeySize < 1024)
				{
					return ExchangeCertificateValidity.PublicKeyUnsupportedSize;
				}
				return ExchangeCertificateValidity.Valid;
			}
			default:
				return ExchangeCertificateValidity.CspKeyContainerInfoUnknownKeyNumber;
			}
		}

		internal static ExchangeCertificateValidity CheckCNGSettings(X509Certificate2 certificate)
		{
			string providerName;
			string keyName;
			TlsCertificateInfo.GetProviderInfo(certificate, out providerName, out keyName);
			SafeNCryptHandle safeNCryptHandle;
			int num = CngNativeMethods.NCryptOpenStorageProvider(out safeNCryptHandle, providerName, 0U);
			if (num != 0)
			{
				throw new CryptographicException(num);
			}
			using (safeNCryptHandle)
			{
				uint valueSize = (uint)Marshal.SizeOf(typeof(uint));
				uint num2;
				uint num3;
				num = CngNativeMethods.NCryptGetProperty(safeNCryptHandle, "Impl Type", out num2, valueSize, out num3, (CngNativeMethods.PropertyOptions)0U);
				if (num != 0)
				{
					throw new CryptographicException(num);
				}
				CngNativeMethods.ImplemenationType implemenationType = (CngNativeMethods.ImplemenationType)num2;
				if ((implemenationType & CngNativeMethods.ImplemenationType.Hardware) == CngNativeMethods.ImplemenationType.Hardware && (implemenationType & CngNativeMethods.ImplemenationType.Removable) == CngNativeMethods.ImplemenationType.Removable)
				{
					return ExchangeCertificateValidity.CspKeyContainerInfoRemovableDevice;
				}
				CngNativeMethods.KeyOptions options = CngNativeMethods.KeyOptions.MachineKeyset | CngNativeMethods.KeyOptions.Silent;
				SafeNCryptHandle safeNCryptHandle3;
				num = CngNativeMethods.NCryptOpenKey(safeNCryptHandle, out safeNCryptHandle3, keyName, 0U, options);
				if (num != 0)
				{
					if (num == -2146893802)
					{
						return ExchangeCertificateValidity.PrivateKeyNotAccessible;
					}
					if (num == -2146893790)
					{
						return ExchangeCertificateValidity.CspKeyContainerInfoProtected;
					}
					throw new CryptographicException(num);
				}
				else
				{
					using (safeNCryptHandle3)
					{
						num = CngNativeMethods.NCryptGetProperty(safeNCryptHandle3, "Length", out num2, valueSize, out num3, (CngNativeMethods.PropertyOptions)0U);
						if (num != 0)
						{
							throw new CryptographicException(num);
						}
						if (num2 < 1024U)
						{
							return ExchangeCertificateValidity.PublicKeyUnsupportedSize;
						}
					}
				}
			}
			return ExchangeCertificateValidity.Valid;
		}

		internal static void EnsureValidExchangeCertificate(X509Certificate2 cert, bool ignoreAccessible)
		{
			ExchangeCertificateValidity exchangeCertificateValidity = ManageExchangeCertificate.ValidateExchangeCertificate(cert, ignoreAccessible);
			if (exchangeCertificateValidity != ExchangeCertificateValidity.Valid)
			{
				throw new CertificateNotValidForExchangeException(cert.Thumbprint, exchangeCertificateValidity.ToString());
			}
		}

		internal static bool IsCertEnabledForNetworkService(ExchangeCertificate cert)
		{
			if (cert.AccessRules != null)
			{
				foreach (AccessRule accessRule in cert.AccessRules)
				{
					CryptoKeyAccessRule cryptoKeyAccessRule = (CryptoKeyAccessRule)accessRule;
					if (cryptoKeyAccessRule.IdentityReference == ManageExchangeCertificate.NetworkServiceIdentityReference && cryptoKeyAccessRule.AccessControlType == AccessControlType.Allow && (cryptoKeyAccessRule.CryptoKeyRights & CryptoKeyRights.GenericRead) != (CryptoKeyRights)0)
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		internal static bool IsServerRoleSupported(Server server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			return server.IsEdgeServer || server.IsHubTransportServer || server.IsUnifiedMessagingServer || server.IsClientAccessServer || server.IsMailboxServer || server.IsFfoWebServiceRole || server.IsCafeServer || server.IsFrontendTransportServer || server.IsOSPRole;
		}

		internal static string UnifyThumbprintFormat(string thumbprint)
		{
			if (thumbprint == null)
			{
				return null;
			}
			if (thumbprint.Length > 3 && thumbprint[2] == ' ')
			{
				return thumbprint.Replace(" ", null);
			}
			return thumbprint;
		}

		internal static Server FindLocalServer(ITopologyConfigurationSession systemConfigDataSession)
		{
			Server result;
			try
			{
				result = systemConfigDataSession.FindLocalServer();
			}
			catch (LocalServerNotFoundException)
			{
				throw;
			}
			catch (Exception innerException)
			{
				string text = NativeHelpers.GetLocalComputerFqdn(false);
				if (text == null)
				{
					text = string.Empty;
				}
				throw new LocalServerNotFoundException(text, innerException);
			}
			return result;
		}

		internal static string WrapCertificateRequestWithPemTags(string base64Request)
		{
			return string.Format(CultureInfo.InvariantCulture, "{1}{0}{2}{3}{0}", new object[]
			{
				Environment.NewLine,
				"-----BEGIN NEW CERTIFICATE REQUEST-----",
				base64Request,
				"-----END NEW CERTIFICATE REQUEST-----"
			});
		}

		internal static bool CheckCnIsFQDN(string subjectName)
		{
			if (string.IsNullOrEmpty(subjectName))
			{
				return false;
			}
			char[] separator = new char[]
			{
				','
			};
			char[] separator2 = new char[]
			{
				'='
			};
			string[] array = subjectName.Split(separator, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				string[] array3 = text.Split(separator2, StringSplitOptions.RemoveEmptyEntries);
				if (array3.Length == 2 && array3[0].Trim().Equals("CN", StringComparison.OrdinalIgnoreCase))
				{
					string text2 = array3[1].Trim();
					if (!string.IsNullOrEmpty(text2) && (Dns.IsValidName(text2) || (Dns.IsValidWildcardName(text2) && text2.Length > 2)))
					{
						return true;
					}
				}
			}
			return false;
		}

		internal static void AddUniqueDomain(IList<string> domains, string name)
		{
			if (!domains.Contains(name))
			{
				domains.Add(name);
			}
		}

		internal static void AddUniqueDomainIfValid(IList<string> domains, string name)
		{
			if (ManageExchangeCertificate.IsDomainValidForCertificate(name) && !domains.Contains(name))
			{
				domains.Add(name);
			}
		}

		internal static bool IsDomainValidForCertificate(string domain)
		{
			return Dns.IsValidName(domain) || Dns.IsValidWildcardName(domain);
		}

		internal static X509Certificate2 FindImportedCertificate(X509Certificate2Collection certs)
		{
			if (certs.Count == 1)
			{
				return certs[0];
			}
			foreach (X509Certificate2 x509Certificate in certs)
			{
				foreach (X509Extension x509Extension in x509Certificate.Extensions)
				{
					X509KeyUsageExtension x509KeyUsageExtension = x509Extension as X509KeyUsageExtension;
					if (x509KeyUsageExtension != null && (x509KeyUsageExtension.KeyUsages & X509KeyUsageFlags.KeyCertSign) == X509KeyUsageFlags.None)
					{
						return x509Certificate;
					}
				}
			}
			return null;
		}

		internal static string GetPopFQDN(ITopologyConfigurationSession session)
		{
			PopImapAdConfiguration popImapAdConfiguration = PopImapAdConfiguration.FindOne<Pop3AdConfiguration>(session);
			if (popImapAdConfiguration == null)
			{
				return null;
			}
			return popImapAdConfiguration.X509CertificateName;
		}

		internal static string GetIMapFQDN(ITopologyConfigurationSession session)
		{
			PopImapAdConfiguration popImapAdConfiguration = PopImapAdConfiguration.FindOne<Imap4AdConfiguration>(session);
			if (popImapAdConfiguration == null)
			{
				return null;
			}
			return popImapAdConfiguration.X509CertificateName;
		}

		internal static int CompareByNotBefore(ExchangeCertificate x, ExchangeCertificate y)
		{
			if (x == null)
			{
				if (y == null)
				{
					return 0;
				}
				return -1;
			}
			else
			{
				if (y == null)
				{
					return 1;
				}
				return y.NotBefore.CompareTo(x.NotBefore);
			}
		}

		internal static IEnumerable<string> DomainsToList(MultiValuedProperty<SmtpDomain> domains)
		{
			List<string> list = new List<string>(domains.Count);
			foreach (SmtpDomain smtpDomain in domains)
			{
				list.Add(smtpDomain.ToString());
			}
			return list;
		}

		internal static Dictionary<AllowedServices, LocalizedString> EnableForServices(X509Certificate2 cert, AllowedServices services, bool requireSsl, ITopologyConfigurationSession dataSession, Server server, List<LocalizedString> warningList, bool allowConfirmation, bool forceNetworkService)
		{
			return ManageExchangeCertificate.EnableForServices(cert, services, null, requireSsl, dataSession, server, warningList, allowConfirmation, forceNetworkService);
		}

		internal static Dictionary<AllowedServices, LocalizedString> EnableForServices(X509Certificate2 cert, AllowedServices services, string websiteName, bool requireSsl, ITopologyConfigurationSession dataSession, Server server, List<LocalizedString> warningList, bool allowConfirmation, bool forceNetworkService)
		{
			Dictionary<AllowedServices, LocalizedString> dictionary = new Dictionary<AllowedServices, LocalizedString>(3);
			if (dataSession == null)
			{
				throw new ArgumentNullException("dataSession");
			}
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if ((services & AllowedServices.IIS) != AllowedServices.None)
			{
				if (allowConfirmation && !IisUtility.SslRequiredOnTheRoot(null) && requireSsl)
				{
					dictionary[AllowedServices.IIS] = Strings.ConfirmEnforceRequireSslOnRoot;
				}
				else
				{
					IisUtility.SetSslCertificateByName(websiteName, cert, requireSsl);
				}
			}
			if ((services & AllowedServices.POP) != AllowedServices.None || (services & AllowedServices.IMAP) != AllowedServices.None || (services & AllowedServices.SMTP) != AllowedServices.None || forceNetworkService)
			{
				AccessRule rule = new CryptoKeyAccessRule(new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null), CryptoKeyRights.GenericRead, AccessControlType.Allow);
				try
				{
					TlsCertificateInfo.AddAccessRule(cert, rule);
				}
				catch (CryptographicException innerException)
				{
					throw new AddAccessRuleCryptographicException(cert.Thumbprint, innerException);
				}
				catch (ArgumentException innerException2)
				{
					throw new AddAccessRuleArgumentException(cert.Thumbprint, innerException2);
				}
				catch (UnauthorizedAccessException innerException3)
				{
					throw new AddAccessRuleUnauthorizedAccessException(cert.Thumbprint, innerException3);
				}
				catch (COMException innerException4)
				{
					throw new AddAccessRuleCOMException(cert.Thumbprint, innerException4);
				}
			}
			if ((services & AllowedServices.SMTP) != AllowedServices.None)
			{
				ManageExchangeCertificate.WarnIfNotBestMatch(new ExchangeCertificate(cert), dataSession, server, warningList);
				LocalizedString localizedString = ManageExchangeCertificate.UpdateActiveDirectory(cert, dataSession, server, warningList, allowConfirmation);
				if (localizedString != LocalizedString.Empty)
				{
					dictionary[AllowedServices.SMTP] = localizedString;
				}
			}
			if ((services & AllowedServices.POP) != AllowedServices.None)
			{
				ManageExchangeCertificate.SetPop3Certificate(cert, dataSession, warningList);
			}
			if ((services & AllowedServices.IMAP) != AllowedServices.None)
			{
				ManageExchangeCertificate.SetImap4Certificate(cert, dataSession, warningList);
			}
			if ((services & AllowedServices.UM) != AllowedServices.None)
			{
				ManageExchangeCertificate.SetUMCertificate(cert, server, dataSession, allowConfirmation, dictionary, warningList);
			}
			if ((services & AllowedServices.UMCallRouter) != AllowedServices.None)
			{
				ManageExchangeCertificate.SetUMCallRouterCertificate(cert, server, dataSession, allowConfirmation, dictionary, warningList);
			}
			if (dictionary.Count <= 0)
			{
				return null;
			}
			return dictionary;
		}

		internal static LocalizedString UpdateActiveDirectory(X509Certificate2 certificate, IConfigurationSession systemConfiguration, Server server, List<LocalizedString> warningList, bool allowConfirmation)
		{
			X509Certificate2 internalTransportCertificate = ExchangeCertificate.GetInternalTransportCertificate(server);
			if (internalTransportCertificate != null)
			{
				if (string.Equals(internalTransportCertificate.Thumbprint, certificate.Thumbprint, StringComparison.OrdinalIgnoreCase))
				{
					return LocalizedString.Empty;
				}
				if (allowConfirmation)
				{
					return Strings.ConfirmOverwriteInternalTransportCertificate(internalTransportCertificate.Thumbprint, internalTransportCertificate.NotAfter, certificate.Thumbprint, certificate.NotAfter);
				}
			}
			server.InternalTransportCertificate = certificate.Export(X509ContentType.SerializedCert);
			systemConfiguration.Save(server);
			if (server.IsHubTransportServer)
			{
				ManageExchangeCertificate.ReEncryptEdgeSyncCredentials(server, internalTransportCertificate, certificate);
				systemConfiguration.Save(server);
			}
			else if (server.IsEdgeServer && warningList != null)
			{
				warningList.Add(Strings.InternalTransportCertificateUpdatedOnEdge);
			}
			return LocalizedString.Empty;
		}

		private static void ReEncryptEdgeSyncCredentials(Server server, X509Certificate2 oldCertificate, X509Certificate2 newCertificate)
		{
			if (server.EdgeSyncCredentials == null || server.EdgeSyncCredentials.Count == 0)
			{
				return;
			}
			if (oldCertificate == null)
			{
				throw new InvalidOperationException(Strings.InternalTransportCertificateCorruptedInADOnHub);
			}
			if (TlsCertificateInfo.IsCNGProvider(newCertificate))
			{
				throw new InvalidOperationException(Strings.InternalTransportCertificateMustBeCAPICertificate(newCertificate.Thumbprint));
			}
			oldCertificate = ExchangeCertificate.GetCertificateFromStore(StoreName.My, oldCertificate.Thumbprint);
			if (oldCertificate == null)
			{
				throw new InvalidOperationException(Strings.InternalTransportCertificateCorruptedInADOnHub);
			}
			EdgeSyncCredential[] array = new EdgeSyncCredential[server.EdgeSyncCredentials.Count];
			using (RSACryptoServiceProvider rsacryptoServiceProvider = (RSACryptoServiceProvider)oldCertificate.PrivateKey)
			{
				for (int i = 0; i < server.EdgeSyncCredentials.Count; i++)
				{
					array[i] = EdgeSyncCredential.DeserializeEdgeSyncCredential(server.EdgeSyncCredentials[i]);
					try
					{
						array[i].EncryptedESRAPassword = rsacryptoServiceProvider.Decrypt(array[i].EncryptedESRAPassword, false);
					}
					catch (CryptographicException)
					{
						throw new InvalidOperationException(Strings.InternalTransportCertificateCorruptedInADOnHub);
					}
				}
			}
			using (RSACryptoServiceProvider rsacryptoServiceProvider2 = newCertificate.PublicKey.Key as RSACryptoServiceProvider)
			{
				if (rsacryptoServiceProvider2 != null)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (array[j].EncryptedESRAPassword != null)
						{
							array[j].EncryptedESRAPassword = rsacryptoServiceProvider2.Encrypt(array[j].EncryptedESRAPassword, false);
							server.EdgeSyncCredentials[j] = EdgeSyncCredential.SerializeEdgeSyncCredential(array[j]);
						}
					}
				}
			}
		}

		private static void WarnIfNotBestMatch(ExchangeCertificate certificate, IConfigurationSession session, Server server, List<LocalizedString> warningList)
		{
			if (warningList == null)
			{
				return;
			}
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			try
			{
				x509Store.Open(OpenFlags.ReadOnly);
				using (ChainEngine chainEngine = new ChainEngine())
				{
					IEnumerable<ManageExchangeCertificate.FqdnConnectors> connectorFQDNs = ManageExchangeCertificate.GetConnectorFQDNs(session, server);
					foreach (ManageExchangeCertificate.FqdnConnectors fqdnConnectors in connectorFQDNs)
					{
						X509Certificate2 x509Certificate;
						if (ManageExchangeCertificate.CertificateHasLowerPrecedence(x509Store, chainEngine, fqdnConnectors.Fqdn, certificate, out x509Certificate))
						{
							if (!new ExchangeCertificate(x509Certificate).IsSelfSigned)
							{
								warningList.Add(Strings.WarnCertificateWillNotBeUsedBestIsPKI(x509Certificate.Thumbprint, fqdnConnectors.Fqdn, fqdnConnectors.Connectors));
							}
							else
							{
								warningList.Add(Strings.WarnCertificateWillNotBeUsed(x509Certificate.Thumbprint, fqdnConnectors.Fqdn, fqdnConnectors.Connectors));
							}
						}
					}
				}
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
		}

		private static void SetPop3Certificate(X509Certificate2 cert, ITopologyConfigurationSession dataSession, List<LocalizedString> warningList)
		{
			ManageExchangeCertificate.SetPopImapCertificate(cert, PopImapAdConfiguration.FindOne<Pop3AdConfiguration>(dataSession), dataSession, warningList);
		}

		private static void SetImap4Certificate(X509Certificate2 cert, ITopologyConfigurationSession dataSession, List<LocalizedString> warningList)
		{
			ManageExchangeCertificate.SetPopImapCertificate(cert, PopImapAdConfiguration.FindOne<Imap4AdConfiguration>(dataSession), dataSession, warningList);
		}

		private static void SetPopImapCertificate(X509Certificate2 cert, PopImapAdConfiguration popImapAdConfiguration, IConfigurationSession dataSession, List<LocalizedString> warningList)
		{
			if (popImapAdConfiguration != null)
			{
				IList<string> fqdns = TlsCertificateInfo.GetFQDNs(cert);
				if (fqdns.Count == 0)
				{
					return;
				}
				popImapAdConfiguration.X509CertificateName = fqdns[0];
				try
				{
					dataSession.Save(popImapAdConfiguration);
				}
				catch (DataValidationException)
				{
					if (warningList != null)
					{
						warningList.Add(Strings.WarnInvalidCertificateForProtocol(cert.Thumbprint, fqdns[0], popImapAdConfiguration.ProtocolName.Remove(popImapAdConfiguration.ProtocolName.Length - 1)));
					}
				}
			}
		}

		private static void SetUMCallRouterCertificate(X509Certificate2 cert, Server server, IConfigurationSession dataSession, bool allowConfirmation, Dictionary<AllowedServices, LocalizedString> confirmationStrings, List<LocalizedString> warningList)
		{
			if (!server.IsCafeServer)
			{
				throw new CafeRoleNotInstalledException(cert.Thumbprint, server.Name);
			}
			SIPFEServerConfiguration sipfeserverConfiguration = SIPFEServerConfiguration.Find(server, dataSession);
			if (sipfeserverConfiguration.UMStartupMode == UMStartupMode.TCP)
			{
				throw new CannotAssignCertificateToUMCRException(cert.Thumbprint);
			}
			if (CertificateUtils.IsExpired(cert))
			{
				throw new CertificateExpiredException(cert.Thumbprint);
			}
			if (allowConfirmation)
			{
				confirmationStrings[AllowedServices.UMCallRouter] = Strings.ConfirmEnableCertForUMCR(cert.Thumbprint);
				return;
			}
			bool flag = false;
			if (string.IsNullOrEmpty(sipfeserverConfiguration.UMCertificateThumbprint))
			{
				flag = true;
			}
			ManageExchangeCertificate.CopyCertToRootStoreIfNeeded(cert, server);
			sipfeserverConfiguration.UMCertificateThumbprint = cert.Thumbprint;
			dataSession.Save(sipfeserverConfiguration);
			if (warningList != null && flag)
			{
				warningList.Add(Strings.ExchangeCertificateUpdatedForUMCR(server.Name));
			}
		}

		private static void SetUMCertificate(X509Certificate2 cert, Server server, IConfigurationSession dataSession, bool allowConfirmation, Dictionary<AllowedServices, LocalizedString> confirmationStrings, List<LocalizedString> warningList)
		{
			UMServer umserver = new UMServer(server);
			if (!server.IsUnifiedMessagingServer)
			{
				throw new UMRoleNotInstalledException(cert.Thumbprint, server.Name);
			}
			if (umserver.UMStartupMode == UMStartupMode.TCP)
			{
				throw new CannotAssignCertificateToUMException(cert.Thumbprint);
			}
			if (CertificateUtils.IsExpired(cert))
			{
				throw new CertificateExpiredException(cert.Thumbprint);
			}
			if (allowConfirmation)
			{
				confirmationStrings[AllowedServices.UM] = Strings.ConfirmEnableCertForUM(cert.Thumbprint);
				return;
			}
			bool flag = false;
			if (string.IsNullOrEmpty(umserver.UMCertificateThumbprint))
			{
				flag = true;
			}
			ManageExchangeCertificate.CopyCertToRootStoreIfNeeded(cert, server);
			umserver.UMCertificateThumbprint = cert.Thumbprint;
			dataSession.Save(server);
			if (warningList != null && flag)
			{
				warningList.Add(Strings.ExchangeCertificateUpdatedForUM(server.Name));
			}
		}

		private static void CopyCertToRootStoreIfNeeded(X509Certificate2 cert, Server server)
		{
			try
			{
				if (CertificateUtils.IsSelfSignedCertificate(cert))
				{
					CertificateUtils.CopyCertToRootStore(cert);
				}
			}
			catch (CryptographicException innerException)
			{
				throw new UnableToAddCertificateToRootStoreException(cert.Thumbprint, server.Name, innerException);
			}
			catch (SecurityException innerException2)
			{
				throw new UnableToAddCertificateToRootStoreException(cert.Thumbprint, server.Name, innerException2);
			}
		}

		private static bool CertificateHasLowerPrecedence(X509Store store, ChainEngine engine, string fqdn, X509Certificate2 certificate, out X509Certificate2 best)
		{
			CertificateSelectionOption options = CertificateSelectionOption.WildcardAllowed | CertificateSelectionOption.PreferedNonSelfSigned;
			string[] names = new string[]
			{
				fqdn
			};
			IEnumerable<X509Certificate2> enumerable = TlsCertificateInfo.FindAll(store, names, options, engine, out best);
			bool flag = false;
			foreach (X509Certificate2 x509Certificate in enumerable)
			{
				if (string.Equals(certificate.Thumbprint, x509Certificate.Thumbprint, StringComparison.OrdinalIgnoreCase))
				{
					flag = true;
					break;
				}
			}
			return flag && best != null && !string.Equals(best.Thumbprint, certificate.Thumbprint, StringComparison.OrdinalIgnoreCase);
		}

		private static IEnumerable<ManageExchangeCertificate.FqdnConnectors> GetConnectorFQDNs(IConfigurationSession session, Server server)
		{
			if (server == null)
			{
				return new ManageExchangeCertificate.FqdnConnectors[0];
			}
			ManageExchangeCertificate.FqdnConnectorsMap fqdnConnectorsMap = new ManageExchangeCertificate.FqdnConnectorsMap();
			ADPagedReader<ReceiveConnector> adpagedReader = session.FindAllPaged<ReceiveConnector>();
			foreach (ReceiveConnector receiveConnector in adpagedReader)
			{
				if (!string.IsNullOrEmpty(receiveConnector.Fqdn) && server.Identity.Equals(receiveConnector.Server) && (receiveConnector.AuthMechanism & AuthMechanisms.Tls) != AuthMechanisms.None)
				{
					fqdnConnectorsMap.Add(receiveConnector.Fqdn, receiveConnector.Name);
				}
			}
			ADPagedReader<SmtpSendConnectorConfig> adpagedReader2 = session.FindAllPaged<SmtpSendConnectorConfig>();
			foreach (SmtpSendConnectorConfig smtpSendConnectorConfig in adpagedReader2)
			{
				if (!string.IsNullOrEmpty(smtpSendConnectorConfig.Fqdn) && smtpSendConnectorConfig.DNSRoutingEnabled && smtpSendConnectorConfig.DomainSecureEnabled && smtpSendConnectorConfig.SourceTransportServers.Contains(server.Id))
				{
					fqdnConnectorsMap.Add(smtpSendConnectorConfig.Fqdn, smtpSendConnectorConfig.Name);
				}
			}
			return fqdnConnectorsMap;
		}

		internal const string AutoDiscoverPrefix = "autodiscover.";

		internal const int CertificateLifetimeInMonths = 60;

		private static readonly IdentityReference NetworkServiceIdentityReference = new SecurityIdentifier(WellKnownSidType.NetworkServiceSid, null).Translate(typeof(NTAccount));

		public delegate bool ShouldContinueDelegate(LocalizedString message);

		private sealed class FqdnConnectorsMap : IEnumerable<ManageExchangeCertificate.FqdnConnectors>, IEnumerable
		{
			public FqdnConnectorsMap()
			{
				this.map = new Dictionary<string, LinkedList<string>>(StringComparer.OrdinalIgnoreCase);
			}

			public void Add(string fqdn, string connectorName)
			{
				LinkedList<string> linkedList;
				if (this.map.TryGetValue(fqdn, out linkedList))
				{
					linkedList.AddLast(connectorName);
					return;
				}
				this.map[fqdn] = new LinkedList<string>(new string[]
				{
					connectorName
				});
			}

			public IEnumerator<ManageExchangeCertificate.FqdnConnectors> GetEnumerator()
			{
				foreach (KeyValuePair<string, LinkedList<string>> entry in this.map)
				{
					KeyValuePair<string, LinkedList<string>> keyValuePair = entry;
					string key = keyValuePair.Key;
					KeyValuePair<string, LinkedList<string>> keyValuePair2 = entry;
					yield return new ManageExchangeCertificate.FqdnConnectors(key, keyValuePair2.Value);
				}
				yield break;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			private Dictionary<string, LinkedList<string>> map;
		}

		private sealed class FqdnConnectors
		{
			public FqdnConnectors(string fqdn, IEnumerable<string> connectors)
			{
				this.Fqdn = fqdn;
				StringBuilder stringBuilder = new StringBuilder();
				foreach (string value in connectors)
				{
					if (stringBuilder.Length > 0)
					{
						stringBuilder.Append(", ");
					}
					stringBuilder.Append(value);
				}
				this.Connectors = stringBuilder.ToString();
			}

			public readonly string Fqdn;

			public readonly string Connectors;
		}
	}
}
