using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Microsoft.Exchange.Diagnostics.Components.Net;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal static class TlsCertificateInfo
	{
		public static X509Certificate2 FindCertificate(string name, bool wildcardAllowed)
		{
			return TlsCertificateInfo.FindCertificate(new string[]
			{
				name
			}, wildcardAllowed);
		}

		public static X509Certificate2 FindCertificate(IEnumerable<string> names, bool wildcardAllowed)
		{
			CertificateSelectionOption options = wildcardAllowed ? CertificateSelectionOption.WildcardAllowed : CertificateSelectionOption.None;
			return TlsCertificateInfo.FindCertificate(names, options);
		}

		public static X509Certificate2 FindCertificate(string name, CertificateSelectionOption options)
		{
			return TlsCertificateInfo.FindCertificate(new string[]
			{
				name
			}, options);
		}

		public static X509Certificate2 FindCertificate(X509Store store, X509FindType type, object findValue)
		{
			X509Certificate2Collection x509Certificate2Collection = store.Certificates.Find(type, findValue, false);
			if (x509Certificate2Collection.Count <= 0)
			{
				return null;
			}
			if (!TlsCertificateInfo.TlsCertSearcher.IsValidServerKey(x509Certificate2Collection[0]))
			{
				return null;
			}
			return x509Certificate2Collection[0];
		}

		public static X509Certificate2 FindCertificate(IEnumerable<string> names, CertificateSelectionOption options)
		{
			X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
			X509Certificate2 result;
			using (ChainEngine chainEngine = new ChainEngine())
			{
				x509Store.Open(OpenFlags.ReadOnly);
				try
				{
					result = TlsCertificateInfo.FindCertificate(x509Store, names, options, chainEngine);
				}
				finally
				{
					x509Store.Close();
				}
			}
			return result;
		}

		public static X509Certificate2 FindCertificate(X509Store store, IEnumerable<string> names, CertificateSelectionOption options, ChainEngine engine)
		{
			return TlsCertificateInfo.FindCertificate(store, names, options, WildcardMatchType.MultiLevel, engine);
		}

		public static X509Certificate2 FindCertificate(X509Store store, IEnumerable<string> names, CertificateSelectionOption options, WildcardMatchType wildcardMatchType, ChainEngine engine)
		{
			ExTraceGlobals.CertificateTracer.Information((long)names.GetHashCode(), "Finding an appropriate TLS certificate.");
			return new TlsCertificateInfo.TlsCertSearcher(store, engine)
			{
				Options = options,
				WildCardMatchType = wildcardMatchType
			}.Search(names);
		}

		public static X509Certificate2 FindCertificate(X509Store store, IEnumerable<string> names, CertificateSelectionOption options, WildcardMatchType wildcardMatchType, ChainEngine engine, TlsCertificateInfo.FilterCert filterCert, string subject, string issuer)
		{
			ExTraceGlobals.CertificateTracer.Information((long)names.GetHashCode(), "Finding an appropriate TLS certificate.");
			return new TlsCertificateInfo.TlsCertSearcher(store, engine)
			{
				Options = options,
				WildCardMatchType = wildcardMatchType
			}.Search(names, filterCert, subject, issuer);
		}

		internal static X509Certificate2 FindFirstCertWithSubjectDistinguishedName(string certificateSubject)
		{
			return TlsCertificateInfo.FindFirstCertWithSubjectDistinguishedName(certificateSubject, true);
		}

		internal static X509Certificate2 FindFirstCertWithSubjectDistinguishedName(string certificateSubject, bool checkForValid)
		{
			X509Certificate2Collection x509Certificate2Collection = TlsCertificateInfo.FindAllCertWithSubjectDistinguishedName(certificateSubject, checkForValid);
			if (x509Certificate2Collection == null || x509Certificate2Collection.Count == 0)
			{
				throw new ArgumentException(NetException.CertificateSubjectNotFound(certificateSubject), "certificateSubject");
			}
			return x509Certificate2Collection[0];
		}

		internal static X509Certificate2 FindCertByThumbprint(string certificateThumbprint)
		{
			X509Certificate2Collection x509Certificate2Collection = null;
			X509Store x509Store = null;
			if (string.IsNullOrEmpty(certificateThumbprint))
			{
				throw new ArgumentException(NetException.EmptyCertThumbprint, "certificateThumbprint");
			}
			try
			{
				x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.OpenExistingOnly);
				x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, certificateThumbprint, true);
				if (x509Certificate2Collection == null || x509Certificate2Collection.Count == 0)
				{
					throw new ArgumentException(NetException.CertificateThumbprintNotFound(certificateThumbprint), "certificateThumbprint");
				}
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
			return x509Certificate2Collection[0];
		}

		internal static bool TryFindFirstCertWithSubjectAndIssuerDistinguishedName(string certificateSubject, string certificateIssuer, out X509Certificate2 tlsCertificate)
		{
			return TlsCertificateInfo.TryFindFirstCertWithSubjectAndIssuerDistinguishedName(certificateSubject, certificateIssuer, true, out tlsCertificate);
		}

		internal static bool TryFindFirstCertWithSubjectAndIssuerDistinguishedName(string certificateSubject, string certificateIssuer, bool checkForValid, out X509Certificate2 tlsCertificate)
		{
			X509Certificate2Collection x509Certificate2Collection = TlsCertificateInfo.FindAllCertWithSubjectDistinguishedName(certificateSubject, checkForValid);
			tlsCertificate = null;
			bool result = false;
			if (x509Certificate2Collection != null && x509Certificate2Collection.Count > 0)
			{
				if (!string.IsNullOrEmpty(certificateIssuer))
				{
					foreach (X509Certificate2 x509Certificate in x509Certificate2Collection)
					{
						if (certificateIssuer.Equals(x509Certificate.IssuerName.Name, StringComparison.OrdinalIgnoreCase))
						{
							result = true;
							tlsCertificate = x509Certificate;
							break;
						}
					}
				}
				else
				{
					tlsCertificate = x509Certificate2Collection[0];
					result = true;
				}
			}
			return result;
		}

		internal static X509Certificate2Collection FindAllCertWithSubjectDistinguishedName(string certificateSubject)
		{
			return TlsCertificateInfo.FindAllCertWithSubjectDistinguishedName(certificateSubject, true);
		}

		internal static X509Certificate2Collection FindAllCertWithSubjectDistinguishedName(string certificateSubject, bool checkForValid)
		{
			X509Certificate2Collection result = null;
			X509Store x509Store = null;
			if (string.IsNullOrEmpty(certificateSubject))
			{
				throw new ArgumentException(NetException.EmptyCertSubject, "certificateSubject");
			}
			try
			{
				x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.OpenExistingOnly);
				result = x509Store.Certificates.Find(X509FindType.FindBySubjectDistinguishedName, certificateSubject, checkForValid);
			}
			finally
			{
				if (x509Store != null)
				{
					x509Store.Close();
				}
			}
			return result;
		}

		public static IEnumerable<X509Certificate2> FindAll(X509Store store, IEnumerable<string> names, CertificateSelectionOption options, ChainEngine engine, out X509Certificate2 best)
		{
			return new TlsCertificateInfo.TlsCertSearcher(store, engine)
			{
				Options = options
			}.FindAll(names, out best);
		}

		public static List<X509Certificate2> EmptyFilterCert(string subject, string issuer, List<X509Certificate2> certList)
		{
			return certList;
		}

		public static List<X509Certificate2> FilterCertBySubjectAndIssuerExceptSubjectCN(string subject, string issuer, List<X509Certificate2> certList)
		{
			List<X509Certificate2> list = new List<X509Certificate2>();
			foreach (X509Certificate2 x509Certificate in certList)
			{
				if (TlsCertificateInfo.SubjectMatichingExceptCN(x509Certificate.Subject, subject) && (string.IsNullOrEmpty(issuer) || string.Equals(x509Certificate.Issuer, issuer, StringComparison.OrdinalIgnoreCase)))
				{
					list.Add(x509Certificate);
				}
			}
			return list;
		}

		public static X509Certificate2 CreateSelfSignCertificate(IEnumerable<string> subjectAlternativeNames, TimeSpan validFor, CertificateCreationOption options)
		{
			return TlsCertificateInfo.CreateSelfSignCertificate(subjectAlternativeNames, validFor, options, 2048);
		}

		public static X509Certificate2 CreateSelfSignCertificate(IEnumerable<string> subjectAlternativeNames, TimeSpan validFor, CertificateCreationOption options, int keySizeRequested)
		{
			return TlsCertificateInfo.CreateSelfSignCertificate(null, subjectAlternativeNames, validFor, options, keySizeRequested, null);
		}

		public static bool IsValidKeySize(int keySize)
		{
			return keySize == 2048 || keySize == 4096 || keySize == 1024;
		}

		public static X509Certificate2 CreateSelfSignCertificate(X500DistinguishedName subject, IEnumerable<string> subjectAlternativeNames, TimeSpan validFor, CertificateCreationOption options, int keySizeRequested, string friendlyName, bool ephemeral)
		{
			return TlsCertificateInfo.CreateSelfSignCertificate(subject, subjectAlternativeNames, validFor, options, keySizeRequested, friendlyName, ephemeral, null);
		}

		public static X509Certificate2 CreateSelfSignCertificate(X500DistinguishedName subject, IEnumerable<string> subjectAlternativeNames, TimeSpan validFor, CertificateCreationOption options, int keySizeRequested, string friendlyName, bool ephemeral, string subjectKeyIdentifier)
		{
			Oid signatureAlgorithm = ((options & CertificateCreationOption.DSSProvider) == CertificateCreationOption.None) ? WellKnownOid.Sha256Rsa : WellKnownOid.X957Sha1Dsa;
			return TlsCertificateInfo.CreateSelfSignCertificate(subject, subjectAlternativeNames, validFor, options, keySizeRequested, friendlyName, ephemeral, subjectKeyIdentifier, signatureAlgorithm);
		}

		public static X509Certificate2 CreateSelfSignCertificate(X500DistinguishedName subject, IEnumerable<string> subjectAlternativeNames, TimeSpan validFor, CertificateCreationOption options, int keySizeRequested, string friendlyName, bool ephemeral, string subjectKeyIdentifier, Oid signatureAlgorithm)
		{
			if (!TlsCertificateInfo.IsValidKeySize(keySizeRequested))
			{
				throw new ArgumentException(string.Format("Invalid TLS Certificate Key Size ({0}).", keySizeRequested), "keySizeRequested");
			}
			if ((options & CertificateCreationOption.RSAProvider) != CertificateCreationOption.None && (options & CertificateCreationOption.DSSProvider) != CertificateCreationOption.None)
			{
				throw new ArgumentException(NetException.DSSAndRSA, "options");
			}
			if ((options & CertificateCreationOption.Exportable) != CertificateCreationOption.None && (options & CertificateCreationOption.Archivable) != CertificateCreationOption.None)
			{
				throw new ArgumentException(NetException.ExportAndArchive, "options");
			}
			int num = 0;
			if (subjectAlternativeNames != null)
			{
				foreach (string name in subjectAlternativeNames)
				{
					if (!TlsCertificateInfo.IsValidDnsName(name))
					{
						throw new ArgumentException(NetException.InvalidFQDN(name), "subjectAlternativeNames");
					}
					num++;
				}
			}
			if (num == 0)
			{
				if (subject == null)
				{
					throw new ArgumentException(NetException.EmptyFQDNList, "subjectAlternativeNames");
				}
				subjectAlternativeNames = null;
			}
			if (validFor <= TimeSpan.Zero)
			{
				throw new ArgumentException(NetException.InvalidDuration, "validFor");
			}
			X500DistinguishedName x500DistinguishedName = subject ?? TlsCertificateInfo.GetDefaultSubjectName(subjectAlternativeNames);
			if (x500DistinguishedName == null)
			{
				x500DistinguishedName = new X500DistinguishedName("cn=" + Environment.MachineName, X500DistinguishedNameFlags.UseUTF8Encoding);
			}
			CapiNativeMethods.CryptKeyProvInfo keyProvInfo = default(CapiNativeMethods.CryptKeyProvInfo);
			bool flag = (options & CertificateCreationOption.DSSProvider) == CertificateCreationOption.None;
			if (flag)
			{
				keyProvInfo.ProviderType = CapiNativeMethods.ProviderType.RsaSChannel;
				keyProvInfo.KeySpec = CapiNativeMethods.KeySpec.KeyExchange;
			}
			else
			{
				keyProvInfo.ProviderType = CapiNativeMethods.ProviderType.DssDiffieHellman;
				keyProvInfo.KeySpec = CapiNativeMethods.KeySpec.Signature;
			}
			keyProvInfo.Flags = CapiNativeMethods.AcquireContext.MachineKeyset;
			SafeCryptProvHandle invalidHandle = SafeCryptProvHandle.InvalidHandle;
			for (;;)
			{
				keyProvInfo.ContainerName = Guid.NewGuid().ToString();
				if (CapiNativeMethods.CryptAcquireContext(out invalidHandle, keyProvInfo.ContainerName, keyProvInfo.ProviderName, keyProvInfo.ProviderType, keyProvInfo.Flags | CapiNativeMethods.AcquireContext.Silent))
				{
					invalidHandle.Close();
				}
				else
				{
					if (!CapiNativeMethods.CryptAcquireContext(out invalidHandle, keyProvInfo.ContainerName, keyProvInfo.ProviderName, keyProvInfo.ProviderType, keyProvInfo.Flags | CapiNativeMethods.AcquireContext.NewKeyset | CapiNativeMethods.AcquireContext.Silent))
					{
						break;
					}
					SafeCryptProvHandle invalidHandle2 = SafeCryptProvHandle.InvalidHandle;
					CapiNativeMethods.KeySpec algid = flag ? CapiNativeMethods.KeySpec.KeyExchange : CapiNativeMethods.KeySpec.Signature;
					uint num2 = (uint)(flag ? (keySizeRequested << 16) : 67108864);
					if ((options & CertificateCreationOption.Exportable) == CertificateCreationOption.Exportable)
					{
						num2 |= 1U;
					}
					if ((options & CertificateCreationOption.Archivable) == CertificateCreationOption.Archivable)
					{
						num2 |= 16384U;
					}
					if (!CapiNativeMethods.CryptGenKey(invalidHandle, algid, num2, ref invalidHandle2))
					{
						goto Block_20;
					}
					invalidHandle2.Close();
				}
				if (!invalidHandle.IsInvalid)
				{
					goto Block_21;
				}
			}
			throw new CryptographicException(Marshal.GetLastWin32Error());
			Block_20:
			throw new CryptographicException(Marshal.GetLastWin32Error());
			Block_21:
			DateTime utcNow = DateTime.UtcNow;
			DateTime toTime = utcNow + validFor;
			X509ExtensionCollection x509ExtensionCollection = new X509ExtensionCollection();
			X509KeyUsageExtension extension = flag ? new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment | X509KeyUsageFlags.DigitalSignature, true) : new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true);
			x509ExtensionCollection.Add(extension);
			if (subjectAlternativeNames != null)
			{
				X509SubjectAltNameExtension extension2 = new X509SubjectAltNameExtension(subjectAlternativeNames, false);
				x509ExtensionCollection.Add(extension2);
			}
			X509EnhancedKeyUsageExtension extension3 = new X509EnhancedKeyUsageExtension(new OidCollection
			{
				WellKnownOid.PkixKpServerAuth
			}, false);
			x509ExtensionCollection.Add(extension3);
			X509BasicConstraintsExtension extension4 = new X509BasicConstraintsExtension(false, false, 0, true);
			x509ExtensionCollection.Add(extension4);
			if (!string.IsNullOrEmpty(subjectKeyIdentifier))
			{
				X509SubjectKeyIdentifierExtension extension5 = new X509SubjectKeyIdentifierExtension(subjectKeyIdentifier, false);
				x509ExtensionCollection.Add(extension5);
			}
			X509Certificate2 x509Certificate = CapiNativeMethods.CreateSelfSignCertificate(invalidHandle, x500DistinguishedName, 0U, keyProvInfo, signatureAlgorithm.Value, utcNow, toTime, x509ExtensionCollection, friendlyName ?? "Microsoft Exchange");
			if (!ephemeral)
			{
				X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.ReadWrite);
				x509Store.Add(x509Certificate);
				x509Store.Close();
			}
			return x509Certificate;
		}

		public static bool TryInstallCertificateInTrustedRootCA(X509Certificate2 certificate)
		{
			X509Store x509Store = new X509Store(StoreName.Root, StoreLocation.LocalMachine);
			x509Store.Open(OpenFlags.ReadWrite);
			bool result;
			try
			{
				X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
				if (x509Certificate2Collection.Count == 0)
				{
					x509Store.Add(certificate);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			finally
			{
				x509Store.Close();
			}
			return result;
		}

		public static bool IsSelfSignedCertificate(X509Certificate2 cert)
		{
			bool result;
			using (ChainEnginePool chainEnginePool = new ChainEnginePool())
			{
				using (ChainEngine engine = chainEnginePool.GetEngine())
				{
					ChainBuildParameter parameter = new ChainBuildParameter(AndChainMatchIssuer.PkixKpServerAuth, TimeSpan.FromSeconds(10.0), false, TimeSpan.Zero);
					ChainContext chainContext2;
					ChainContext chainContext = chainContext2 = engine.Build(cert, ChainBuildOptions.CacheEndCert | ChainBuildOptions.RevocationCheckChainExcludeRoot | ChainBuildOptions.RevocationAccumulativeTimeout, parameter);
					try
					{
						if (chainContext != null)
						{
							result = chainContext.IsSelfSigned;
						}
						else
						{
							ExTraceGlobals.CertificateTracer.TraceError(0L, "IsSelfSignedCertificate: ChainContext was null.");
							result = false;
						}
					}
					finally
					{
						if (chainContext2 != null)
						{
							((IDisposable)chainContext2).Dispose();
						}
					}
				}
			}
			return result;
		}

		public static X509Certificate2 CreateSelfSignCertificate(X500DistinguishedName subject, IEnumerable<string> subjectAlternativeNames, TimeSpan validFor, CertificateCreationOption options, int keySizeRequested, string friendlyName)
		{
			Oid signatureAlgorithm = ((options & CertificateCreationOption.DSSProvider) == CertificateCreationOption.None) ? WellKnownOid.RsaSha1Rsa : WellKnownOid.X957Sha1Dsa;
			return TlsCertificateInfo.CreateSelfSignCertificate(subject, subjectAlternativeNames, validFor, options, keySizeRequested, friendlyName, false, null, signatureAlgorithm);
		}

		public static bool IsCNGProvider(X509Certificate2 certificate)
		{
			CapiNativeMethods.CryptKeyProvInfo cryptKeyProvInfo;
			if (!CapiNativeMethods.GetPrivateKeyInfo(certificate, out cryptKeyProvInfo))
			{
				throw new ArgumentException("certificate");
			}
			return cryptKeyProvInfo.ProviderType == CapiNativeMethods.ProviderType.CNG;
		}

		public static void GetProviderInfo(X509Certificate2 certificate, out string providerName, out string containerName)
		{
			CapiNativeMethods.CryptKeyProvInfo cryptKeyProvInfo;
			if (!CapiNativeMethods.GetPrivateKeyInfo(certificate, out cryptKeyProvInfo))
			{
				throw new ArgumentException("certificate");
			}
			containerName = cryptKeyProvInfo.ContainerName;
			providerName = cryptKeyProvInfo.ProviderName;
		}

		public static string GetKeyAlgorithmName(X509Certificate2 certificate)
		{
			return CapiNativeMethods.GetKeyAlgorithmName(certificate);
		}

		public static void AddAccessRule(X509Certificate2 certificate, AccessRule rule)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			CapiNativeMethods.CryptKeyProvInfo providerInfo;
			if (!CapiNativeMethods.GetPrivateKeyInfo(certificate, out providerInfo))
			{
				throw new ArgumentException("certificate");
			}
			bool flag;
			if (providerInfo.ProviderType != CapiNativeMethods.ProviderType.CNG)
			{
				flag = TlsCertificateInfo.CAPIAddAccessRule(certificate, rule);
			}
			else
			{
				flag = TlsCertificateInfo.CNGAddAccessRule(certificate, providerInfo, rule);
			}
			if (flag)
			{
				X509Store x509Store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
				x509Store.Open(OpenFlags.ReadWrite);
				try
				{
					X509Certificate2Collection x509Certificate2Collection = x509Store.Certificates.Find(X509FindType.FindByThumbprint, certificate.Thumbprint, false);
					if (x509Certificate2Collection.Count > 0)
					{
						X509Certificate2 x509Certificate = x509Certificate2Collection[0];
						try
						{
							x509Certificate.FriendlyName = x509Certificate.FriendlyName;
						}
						catch (CryptographicException)
						{
						}
					}
				}
				finally
				{
					x509Store.Close();
				}
			}
		}

		private static bool CAPIAddAccessRule(X509Certificate2 certificate, AccessRule rule)
		{
			AsymmetricAlgorithm privateKey;
			try
			{
				privateKey = certificate.PrivateKey;
			}
			catch (CryptographicException inner)
			{
				throw new UnauthorizedAccessException("Certificate", inner);
			}
			if (privateKey == null)
			{
				throw new ArgumentException("Private Key invalid", "Certificate");
			}
			ICspAsymmetricAlgorithm cspAsymmetricAlgorithm = privateKey as ICspAsymmetricAlgorithm;
			if (cspAsymmetricAlgorithm == null)
			{
				throw new ArgumentException("Wrong private key type", "Certificate");
			}
			CspKeyContainerInfo cspKeyContainerInfo = cspAsymmetricAlgorithm.CspKeyContainerInfo;
			CryptoKeySecurity cryptoKeySecurity = cspKeyContainerInfo.CryptoKeySecurity;
			bool flag = false;
			if (!cryptoKeySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out flag))
			{
				throw new UnauthorizedAccessException(SystemStrings.FailedToAddAccessRule);
			}
			if (flag)
			{
				SafeCryptProvHandle safeCryptProvHandle;
				if (!CapiNativeMethods.CryptAcquireContext(out safeCryptProvHandle, cspKeyContainerInfo.KeyContainerName, cspKeyContainerInfo.ProviderName, (CapiNativeMethods.ProviderType)cspKeyContainerInfo.ProviderType, cspKeyContainerInfo.MachineKeyStore ? CapiNativeMethods.AcquireContext.MachineKeyset : ((CapiNativeMethods.AcquireContext)0U)))
				{
					CryptographicException ex = new CryptographicException(Marshal.GetLastWin32Error());
					throw ex;
				}
				using (safeCryptProvHandle)
				{
					byte[] securityDescriptorBinaryForm = cryptoKeySecurity.GetSecurityDescriptorBinaryForm();
					if (!CapiNativeMethods.CryptSetProvParam(safeCryptProvHandle, CapiNativeMethods.SetProvParam.KeySetSecurityDescriptor, securityDescriptorBinaryForm, 4U))
					{
						CryptographicException ex2 = new CryptographicException(Marshal.GetLastWin32Error());
						throw ex2;
					}
				}
			}
			return flag;
		}

		private static bool CNGAddAccessRule(X509Certificate2 certificate, CapiNativeMethods.CryptKeyProvInfo providerInfo, AccessRule rule)
		{
			SafeNCryptHandle safeNCryptHandle;
			int num = CngNativeMethods.NCryptOpenStorageProvider(out safeNCryptHandle, providerInfo.ProviderName, 0U);
			if (num != 0)
			{
				throw new CryptographicException(num);
			}
			using (safeNCryptHandle)
			{
				CngNativeMethods.KeyOptions options = CngNativeMethods.KeyOptions.MachineKeyset;
				SafeNCryptHandle safeNCryptHandle3;
				num = CngNativeMethods.NCryptOpenKey(safeNCryptHandle, out safeNCryptHandle3, providerInfo.ContainerName, 0U, options);
				if (num != 0)
				{
					throw new CryptographicException(num);
				}
				using (safeNCryptHandle3)
				{
					byte[] array = null;
					uint valueSize = 0U;
					uint num2;
					num = CngNativeMethods.NCryptGetProperty(safeNCryptHandle3, "Security Descr", array, valueSize, out num2, CngNativeMethods.PropertyOptions.DACLSecurityInformation);
					if (num != 0)
					{
						throw new CryptographicException(num);
					}
					array = new byte[num2];
					valueSize = num2;
					num = CngNativeMethods.NCryptGetProperty(safeNCryptHandle3, "Security Descr", array, valueSize, out num2, CngNativeMethods.PropertyOptions.DACLSecurityInformation);
					if (num != 0)
					{
						throw new CryptographicException(num);
					}
					CryptoKeySecurity cryptoKeySecurity = new CryptoKeySecurity();
					cryptoKeySecurity.SetSecurityDescriptorBinaryForm(array);
					bool flag = false;
					if (!cryptoKeySecurity.ModifyAccessRule(AccessControlModification.Add, rule, out flag))
					{
						throw new UnauthorizedAccessException(SystemStrings.FailedToAddAccessRule);
					}
					if (!flag)
					{
						return false;
					}
					array = cryptoKeySecurity.GetSecurityDescriptorBinaryForm();
					num = CngNativeMethods.NCryptSetProperty(safeNCryptHandle3, "Security Descr", array, (uint)array.Length, CngNativeMethods.PropertyOptions.DACLSecurityInformation);
					if (num != 0)
					{
						throw new CryptographicException(num);
					}
				}
			}
			return true;
		}

		public static IList<AccessRule> GetAccessRules(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			CapiNativeMethods.CryptKeyProvInfo cryptKeyProvInfo;
			if (!CapiNativeMethods.GetPrivateKeyInfo(certificate, out cryptKeyProvInfo))
			{
				return null;
			}
			CryptoKeySecurity cryptoKeySecurity = null;
			if (cryptKeyProvInfo.ProviderType != CapiNativeMethods.ProviderType.CNG)
			{
				AsymmetricAlgorithm asymmetricAlgorithm = null;
				try
				{
					asymmetricAlgorithm = certificate.PrivateKey;
				}
				catch (CryptographicException)
				{
					return null;
				}
				if (asymmetricAlgorithm != null)
				{
					ICspAsymmetricAlgorithm cspAsymmetricAlgorithm = asymmetricAlgorithm as ICspAsymmetricAlgorithm;
					if (cspAsymmetricAlgorithm != null)
					{
						CspKeyContainerInfo cspKeyContainerInfo = cspAsymmetricAlgorithm.CspKeyContainerInfo;
						if (cspKeyContainerInfo != null)
						{
							cryptoKeySecurity = cspKeyContainerInfo.CryptoKeySecurity;
						}
					}
				}
			}
			else
			{
				SafeNCryptHandle safeNCryptHandle;
				int num = CngNativeMethods.NCryptOpenStorageProvider(out safeNCryptHandle, cryptKeyProvInfo.ProviderName, 0U);
				if (num != 0)
				{
					return null;
				}
				using (safeNCryptHandle)
				{
					CngNativeMethods.KeyOptions options = CngNativeMethods.KeyOptions.MachineKeyset;
					SafeNCryptHandle safeNCryptHandle3;
					num = CngNativeMethods.NCryptOpenKey(safeNCryptHandle, out safeNCryptHandle3, cryptKeyProvInfo.ContainerName, 0U, options);
					if (num != 0)
					{
						return null;
					}
					using (safeNCryptHandle3)
					{
						byte[] array = null;
						uint valueSize = 0U;
						uint num2;
						num = CngNativeMethods.NCryptGetProperty(safeNCryptHandle3, "Security Descr", array, valueSize, out num2, CngNativeMethods.PropertyOptions.DACLSecurityInformation);
						if (num != 0)
						{
							return null;
						}
						array = new byte[num2];
						valueSize = num2;
						num = CngNativeMethods.NCryptGetProperty(safeNCryptHandle3, "Security Descr", array, valueSize, out num2, CngNativeMethods.PropertyOptions.DACLSecurityInformation);
						if (num != 0)
						{
							return null;
						}
						cryptoKeySecurity = new CryptoKeySecurity();
						cryptoKeySecurity.SetSecurityDescriptorBinaryForm(array);
					}
				}
			}
			if (cryptoKeySecurity == null)
			{
				return null;
			}
			List<AccessRule> list = new List<AccessRule>();
			AuthorizationRuleCollection accessRules = cryptoKeySecurity.GetAccessRules(true, true, typeof(NTAccount));
			foreach (object obj in accessRules)
			{
				AuthorizationRule authorizationRule = (AuthorizationRule)obj;
				AccessRule item = authorizationRule as AccessRule;
				list.Add(item);
			}
			return list.AsReadOnly();
		}

		public static bool IsCertificateExportable(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			CapiNativeMethods.CryptKeyProvInfo cryptKeyProvInfo;
			if (!CapiNativeMethods.GetPrivateKeyInfo(certificate, out cryptKeyProvInfo))
			{
				return false;
			}
			if (cryptKeyProvInfo.ProviderType != CapiNativeMethods.ProviderType.CNG)
			{
				AsymmetricAlgorithm asymmetricAlgorithm = null;
				try
				{
					asymmetricAlgorithm = certificate.PrivateKey;
				}
				catch (CryptographicException)
				{
					return false;
				}
				if (asymmetricAlgorithm != null)
				{
					ICspAsymmetricAlgorithm cspAsymmetricAlgorithm = asymmetricAlgorithm as ICspAsymmetricAlgorithm;
					if (cspAsymmetricAlgorithm != null)
					{
						CspKeyContainerInfo cspKeyContainerInfo = cspAsymmetricAlgorithm.CspKeyContainerInfo;
						return cspKeyContainerInfo != null && cspKeyContainerInfo.Exportable;
					}
				}
			}
			else
			{
				SafeNCryptHandle safeNCryptHandle;
				int num = CngNativeMethods.NCryptOpenStorageProvider(out safeNCryptHandle, cryptKeyProvInfo.ProviderName, 0U);
				if (num != 0)
				{
					return false;
				}
				using (safeNCryptHandle)
				{
					CngNativeMethods.KeyOptions options = CngNativeMethods.KeyOptions.MachineKeyset;
					SafeNCryptHandle safeNCryptHandle3;
					num = CngNativeMethods.NCryptOpenKey(safeNCryptHandle, out safeNCryptHandle3, cryptKeyProvInfo.ContainerName, 0U, options);
					if (num != 0)
					{
						return false;
					}
					using (safeNCryptHandle3)
					{
						uint valueSize = (uint)Marshal.SizeOf(typeof(uint));
						uint num2;
						uint num3;
						if (CngNativeMethods.NCryptGetProperty(safeNCryptHandle3, "Export Policy", out num2, valueSize, out num3, (CngNativeMethods.PropertyOptions)0U) == 0)
						{
							CngNativeMethods.AllowExportPolicy allowExportPolicy = (CngNativeMethods.AllowExportPolicy)num2;
							return (allowExportPolicy & (CngNativeMethods.AllowExportPolicy.Exportable | CngNativeMethods.AllowExportPolicy.PlaintextExportable)) != (CngNativeMethods.AllowExportPolicy)0;
						}
					}
				}
			}
			return false;
		}

		public static IList<string> GetFQDNs(X509Certificate2 certificate)
		{
			int num;
			return TlsCertificateInfo.GetFQDNs(certificate, int.MaxValue, out num);
		}

		public static IList<string> GetFQDNs(X509Certificate2 certificate, int subjectAlternativeNameLimit, out int subjectAlternativeNameCount)
		{
			subjectAlternativeNameCount = 0;
			if (subjectAlternativeNameLimit < 0)
			{
				throw new ArgumentOutOfRangeException("subjectAlternativeNameLimit", subjectAlternativeNameLimit, "subjectAlternativeNameLimit value cannot be negative");
			}
			List<string> list = new List<string>();
			string certNameInfo = CapiNativeMethods.GetCertNameInfo(certificate, 0U, CapiNativeMethods.CertNameType.Attr);
			if (!string.IsNullOrEmpty(certNameInfo) && TlsCertificateInfo.IsValidDnsName(certNameInfo))
			{
				list.Add(certNameInfo);
			}
			X509ExtensionCollection extensions = certificate.Extensions;
			foreach (X509Extension x509Extension in extensions)
			{
				if (string.Compare(x509Extension.Oid.Value, WellKnownOid.SubjectAltName.Value, StringComparison.OrdinalIgnoreCase) == 0)
				{
					X509SubjectAltNameExtension x509SubjectAltNameExtension = X509SubjectAltNameExtension.Create(x509Extension);
					if (x509SubjectAltNameExtension.DnsNames.Count > subjectAlternativeNameLimit)
					{
						subjectAlternativeNameCount = x509SubjectAltNameExtension.DnsNames.Count;
						ExTraceGlobals.CertificateTracer.TraceError<string, int, int>(0L, "Certificate with thumbprint <{0}> contains {1} subject alternative names and exceeds the limit of {2}; ignoring subject alternative names for this certificate.", certificate.Thumbprint, subjectAlternativeNameCount, subjectAlternativeNameLimit);
						break;
					}
					using (IEnumerator<string> enumerator2 = x509SubjectAltNameExtension.DnsNames.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							string text = enumerator2.Current;
							if (TlsCertificateInfo.IsValidDnsName(text))
							{
								bool flag = false;
								foreach (string a in list)
								{
									if (string.Equals(a, text, StringComparison.OrdinalIgnoreCase))
									{
										flag = true;
										break;
									}
								}
								if (!flag)
								{
									list.Add(text);
									subjectAlternativeNameCount++;
								}
							}
						}
						break;
					}
				}
			}
			return list;
		}

		internal static X500DistinguishedName GetDefaultSubjectName(IEnumerable<string> subjectAlternativeNames)
		{
			if (subjectAlternativeNames == null)
			{
				return null;
			}
			foreach (string text in subjectAlternativeNames)
			{
				if (text.Length < 64)
				{
					return new X500DistinguishedName("cn=" + text, X500DistinguishedNameFlags.UseUTF8Encoding);
				}
			}
			return null;
		}

		private static bool IsValidDnsName(string name)
		{
			return Dns.IsValidName(name) || (Dns.IsValidWildcardName(name) && name.Length > 2);
		}

		private static bool SubjectMatichingExceptCN(string subject1, string subject2)
		{
			if (string.Equals(subject1, subject2, StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
			if (string.IsNullOrEmpty(subject1) || string.IsNullOrEmpty(subject2))
			{
				return false;
			}
			Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
			string[] array = subject1.Split(new string[]
			{
				","
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				if (!text.Trim().StartsWith("CN", StringComparison.OrdinalIgnoreCase))
				{
					string key = text.Trim().ToLower();
					if (dictionary.ContainsKey(key))
					{
						return false;
					}
					dictionary.Add(key, false);
				}
			}
			string[] array3 = subject2.Split(new string[]
			{
				","
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text2 in array3)
			{
				if (!text2.Trim().StartsWith("CN", StringComparison.OrdinalIgnoreCase))
				{
					string key2 = text2.Trim().ToLower();
					if (!dictionary.ContainsKey(key2))
					{
						return false;
					}
					bool flag = dictionary[key2];
					if (flag)
					{
						return false;
					}
					dictionary[key2] = true;
				}
			}
			return !dictionary.ContainsValue(false);
		}

		public const string DefaultFriendlyName = "Microsoft Exchange";

		public delegate List<X509Certificate2> FilterCert(string subject, string issuer, List<X509Certificate2> certList);

		private class TlsCertSearcher : CertSearcher
		{
			public TlsCertSearcher(X509Store certStore, ChainEngine chainEngine) : base(certStore, chainEngine, TlsCertificateInfo.TlsCertSearcher.usage, TlsCertificateInfo.TlsCertSearcher.defaultSSLChainPolicy)
			{
			}

			public List<X509Certificate2> FindAll(IEnumerable<string> names, out X509Certificate2 best)
			{
				List<X509Certificate2> list = base.FindAll(names);
				best = base.FindBest(names, list);
				return list;
			}

			public X509Certificate2 Search(IEnumerable<string> names, TlsCertificateInfo.FilterCert filterCert, string subject, string issuer)
			{
				List<X509Certificate2> certList = base.FindAll(names);
				List<X509Certificate2> certs = filterCert(subject, issuer, certList);
				return base.FindBest(names, certs);
			}

			public static bool IsValidServerKey(X509Certificate2 certificate)
			{
				return CertSearcher.IsValidServerKeyUsages(certificate);
			}

			protected override bool IsCertificateValid(X509Certificate2 certificate, IEnumerable<string> names)
			{
				return base.IsFQDNMatch(certificate, names) && certificate.HasPrivateKey && TlsCertificateInfo.TlsCertSearcher.IsValidTLSUsage(certificate);
			}

			private static bool IsValidTLSUsage(X509Certificate2 certificate)
			{
				bool flag = false;
				bool result;
				try
				{
					CapiNativeMethods.AlgorithmId algorithmId = CapiNativeMethods.GetAlgorithmId(certificate);
					CapiNativeMethods.AlgorithmId algorithmId2 = algorithmId;
					if (algorithmId2 != CapiNativeMethods.AlgorithmId.DsaSignature && algorithmId2 != CapiNativeMethods.AlgorithmId.RsaKeyExchange)
					{
						CertSearcher.Logger.TraceEvent(TraceEventType.Verbose, 0, "Considering certificate {0}", new object[]
						{
							certificate.Thumbprint
						});
						CertSearcher.Logger.TraceEvent(TraceEventType.Verbose, 0, "{0}: Is neither RSA or DSA, dropping from consideration.", new object[]
						{
							certificate.Thumbprint
						});
						result = false;
					}
					else
					{
						X509KeyUsageFlags intendedKeyUsage;
						try
						{
							intendedKeyUsage = CapiNativeMethods.GetIntendedKeyUsage(certificate);
						}
						catch (CryptographicException ex)
						{
							CertSearcher.Logger.TraceEvent(TraceEventType.Verbose, 0, "Considering certificate {0}", new object[]
							{
								certificate.Thumbprint
							});
							CertSearcher.Logger.TraceEvent(TraceEventType.Error, 0, "{0}: Caused a cryptographic exception [{1}] when attempting to determine its usage.", new object[]
							{
								certificate.Thumbprint,
								ex.Message
							});
							return false;
						}
						if (intendedKeyUsage == X509KeyUsageFlags.None)
						{
							flag = true;
						}
						if ((intendedKeyUsage & X509KeyUsageFlags.KeyEncipherment) == X509KeyUsageFlags.KeyEncipherment)
						{
						}
						if ((intendedKeyUsage & (X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature)) != X509KeyUsageFlags.None)
						{
							flag = true;
						}
						if (!flag)
						{
							CertSearcher.Logger.TraceEvent(TraceEventType.Verbose, 0, "Considering certificate {0}", new object[]
							{
								certificate.Thumbprint
							});
							CertSearcher.Logger.TraceEvent(TraceEventType.Verbose, 0, "{0}: Is not valid for signing, dropping from consideration.", new object[]
							{
								certificate.Thumbprint
							});
							result = false;
						}
						else
						{
							string[] enhancedKeyUsage;
							try
							{
								enhancedKeyUsage = CapiNativeMethods.GetEnhancedKeyUsage(certificate, CapiNativeMethods.EnhancedKeyUsageSearch.ExtensionAndProperty);
							}
							catch (CryptographicException ex2)
							{
								CertSearcher.Logger.TraceEvent(TraceEventType.Verbose, 0, "Considering certificate {0}", new object[]
								{
									certificate.Thumbprint
								});
								CertSearcher.Logger.TraceEvent(TraceEventType.Error, 0, "{0}: Caused a cryptographic exception [{1}] when attempting to determine its enhanced usage.", new object[]
								{
									certificate.Thumbprint,
									ex2.Message
								});
								return false;
							}
							bool flag2 = false;
							if (enhancedKeyUsage != null)
							{
								if (enhancedKeyUsage.Length == 0)
								{
									flag2 = true;
								}
								else
								{
									foreach (string strA in enhancedKeyUsage)
									{
										if (string.CompareOrdinal(strA, WellKnownOid.PkixKpServerAuth.Value) == 0)
										{
											flag2 = true;
											break;
										}
									}
								}
							}
							if (!flag2)
							{
								CertSearcher.Logger.TraceEvent(TraceEventType.Verbose, 0, "Considering certificate {0}", new object[]
								{
									certificate.Thumbprint
								});
								CertSearcher.Logger.TraceEvent(TraceEventType.Verbose, 0, "{0}: Has an EKU restriction, dropping from consideration.", new object[]
								{
									certificate.Thumbprint
								});
								result = false;
							}
							else
							{
								result = true;
							}
						}
					}
				}
				catch (CryptographicException ex3)
				{
					CertSearcher.Logger.TraceEvent(TraceEventType.Verbose, 0, "Considering certificate {0}", new object[]
					{
						certificate.Thumbprint
					});
					CertSearcher.Logger.TraceEvent(TraceEventType.Error, 0, "{0}: Cryptographic exception [{1}] while analyzing.", new object[]
					{
						certificate.Thumbprint,
						ex3.Message
					});
					result = false;
				}
				return result;
			}

			private static readonly ChainMatchIssuer match = AndChainMatchIssuer.PkixKpServerAuth;

			private static readonly ChainBuildParameter usage = new ChainBuildParameter(TlsCertificateInfo.TlsCertSearcher.match, TimeSpan.FromSeconds(10.0), false, TimeSpan.Zero);

			private static readonly SSLChainPolicyParameters defaultSSLChainPolicy = new SSLChainPolicyParameters(string.Empty, ChainPolicyOptions.None, SSLPolicyAuthorizationOptions.IgnoreCertCNInvalid, SSLPolicyAuthorizationType.Server);
		}
	}
}
