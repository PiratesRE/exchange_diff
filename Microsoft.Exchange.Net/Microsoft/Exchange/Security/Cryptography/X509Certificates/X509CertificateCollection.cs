using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Security.Cryptography.X509Certificates
{
	internal class X509CertificateCollection : X509Certificate2Collection
	{
		public X509CertificateCollection()
		{
		}

		public X509CertificateCollection(X509Certificate2Collection certificates) : base(certificates)
		{
		}

		public static ChainValidityStatus ValidateCertificate(X509Certificate2 certificate, IEnumerable<string> emails, X509KeyUsageFlags expectedUsage, bool checkRevocation, X509Store trustedStore, X509Store chainBuildStore, ref ChainContext context, bool exclusiveTrustMode = false, string uid = null)
		{
			return X509CertificateCollection.ValidateCertificate(certificate, emails, expectedUsage, checkRevocation, trustedStore, chainBuildStore, X509CertificateCollection.DefaultCRLConnectionTimeout, X509CertificateCollection.DefaultCRLRetrievalTimeout, ref context, exclusiveTrustMode, uid);
		}

		public static ChainValidityStatus ValidateCertificate(X509Certificate2 certificate, IEnumerable<string> emails, X509KeyUsageFlags expectedUsage, bool checkRevocation, X509Store trustedStore, X509Store chainBuildStore, TimeSpan connectionTimeout, TimeSpan retrievalTimeout, ref ChainContext context, bool exclusiveTrustMode = false, string poolUid = null)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			if (emails != null && !X509CertificateCollection.IsUserMatch(certificate, emails))
			{
				return ChainValidityStatus.SubjectMismatch;
			}
			if (!X509CertificateCollection.IsValidUsage(certificate, expectedUsage))
			{
				return (ChainValidityStatus)2148204816U;
			}
			ChainValidityStatus result;
			using (ChainEngine chainEngine = X509CertificateCollection.BuildChainEngine(retrievalTimeout, trustedStore, exclusiveTrustMode, poolUid))
			{
				ChainBuildOptions chainBuildOptions = X509CertificateCollection.GetChainBuildOptions(checkRevocation);
				ChainPolicyParameters chainPolicyParameters = X509CertificateCollection.GetChainPolicyParameters(checkRevocation);
				ChainBuildParameter chainBuildParameter = X509CertificateCollection.GetChainBuildParameter(connectionTimeout);
				context = ((chainBuildStore == null) ? chainEngine.Build(certificate, chainBuildOptions, chainBuildParameter) : chainEngine.Build(certificate, chainBuildOptions, chainBuildParameter, chainBuildStore));
				if (context == null)
				{
					result = (ChainValidityStatus)2148204810U;
				}
				else
				{
					ChainSummary chainSummary = context.Validate(chainPolicyParameters);
					result = chainSummary.Status;
				}
			}
			return result;
		}

		public void ImportFromContact(byte[] rawData)
		{
			if (rawData == null)
			{
				throw new ArgumentNullException("rawData");
			}
			int i = 0;
			while (i < rawData.Length)
			{
				if (i > rawData.Length - X509CertificateCollection.fieldSize * 2)
				{
					throw new FormatException("Data stream truncated.");
				}
				X509CertificateCollection.OutlookCertificateTag outlookCertificateTag = (X509CertificateCollection.OutlookCertificateTag)BitConverter.ToUInt16(rawData, i);
				ushort num = BitConverter.ToUInt16(rawData, i + X509CertificateCollection.fieldSize);
				X509CertificateCollection.OutlookCertificateTag outlookCertificateTag2 = outlookCertificateTag;
				switch (outlookCertificateTag2)
				{
				case X509CertificateCollection.OutlookCertificateTag.Version:
				case X509CertificateCollection.OutlookCertificateTag.AsymmetricCapabilities:
				case (X509CertificateCollection.OutlookCertificateTag)5:
				case X509CertificateCollection.OutlookCertificateTag.MessageEncoding:
				case (X509CertificateCollection.OutlookCertificateTag)7:
				case X509CertificateCollection.OutlookCertificateTag.SignatureCertificate:
				case X509CertificateCollection.OutlookCertificateTag.SignSha1Hash:
				case (X509CertificateCollection.OutlookCertificateTag)10:
				case X509CertificateCollection.OutlookCertificateTag.DisplayName:
				case X509CertificateCollection.OutlookCertificateTag.NortelBulkAlgorithm:
				case X509CertificateCollection.OutlookCertificateTag.CertificateTime:
					break;
				case X509CertificateCollection.OutlookCertificateTag.KeyExchangeCertificate:
				{
					int num2 = 2 * X509CertificateCollection.fieldSize;
					int num3 = (int)num - num2;
					if (num3 > 0)
					{
						if (i + num2 > rawData.Length - num3)
						{
							throw new FormatException("Data stream truncated.");
						}
						byte[] array = new byte[num3];
						Array.Copy(rawData, i + num2, array, 0, num3);
						using (SafeCertContextHandle safeCertContextHandle = CapiNativeMethods.CertCreateCertificateContext(CapiNativeMethods.EncodeType.X509Asn, array, array.Length))
						{
							if (safeCertContextHandle == null || safeCertContextHandle.IsInvalid)
							{
								throw new CryptographicException(Marshal.GetLastWin32Error());
							}
							base.Add(new X509Certificate2(safeCertContextHandle.DangerousGetHandle()));
							break;
						}
						goto IL_128;
					}
					break;
				}
				case X509CertificateCollection.OutlookCertificateTag.CertificateChain:
					goto IL_128;
				default:
					switch (outlookCertificateTag2)
					{
					}
					break;
				}
				IL_166:
				i += (int)num;
				continue;
				IL_128:
				int num4 = 2 * Marshal.SizeOf(typeof(ushort));
				int num5 = (int)num - num4;
				if (num5 > 0)
				{
					byte[] array2 = new byte[num5];
					Array.Copy(rawData, i + num4, array2, 0, num5);
					base.Import(array2);
					goto IL_166;
				}
				goto IL_166;
			}
		}

		public X509Certificate2 FindSMimeCertificate(IEnumerable<string> emails, X509KeyUsageFlags expectedUsage, bool checkRevocation, X509Store chainCertStore = null, string poolUid = null)
		{
			return this.FindSMimeCertificate(emails, expectedUsage, checkRevocation, X509CertificateCollection.DefaultCRLConnectionTimeout, X509CertificateCollection.DefaultCRLRetrievalTimeout, chainCertStore, poolUid);
		}

		public X509Certificate2 FindSMimeCertificate(IEnumerable<string> emails, X509KeyUsageFlags expectedUsage, bool checkRevocation, TimeSpan connectionTimeout, TimeSpan retrievalTimeout, X509Store chainCertStore = null, string poolUid = null)
		{
			X509KeyUsageFlags x509KeyUsageFlags = expectedUsage & (X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature);
			if (x509KeyUsageFlags != X509KeyUsageFlags.None && x509KeyUsageFlags != (X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature))
			{
				throw new ArgumentException("SMIME signature usage flags must be DigitalSignature | NonRepudiation", "expectedUsage");
			}
			List<X509Certificate2> list = new List<X509Certificate2>();
			foreach (X509Certificate2 x509Certificate in this)
			{
				if ((emails == null || X509CertificateCollection.IsUserMatch(x509Certificate, emails)) && X509CertificateCollection.IsValidUsage(x509Certificate, expectedUsage))
				{
					list.Add(x509Certificate);
				}
			}
			if (list.Count == 0)
			{
				return null;
			}
			list.Sort(new Comparison<X509Certificate2>(CertSearcher.CompareByNotBefore));
			ChainBuildOptions chainBuildOptions = X509CertificateCollection.GetChainBuildOptions(checkRevocation);
			ChainPolicyParameters chainPolicyParameters = X509CertificateCollection.GetChainPolicyParameters(checkRevocation);
			ChainBuildParameter chainBuildParameter = X509CertificateCollection.GetChainBuildParameter(connectionTimeout);
			using (ChainEngine chainEngine = X509CertificateCollection.BuildChainEngine(retrievalTimeout, chainCertStore, chainCertStore != null, poolUid))
			{
				foreach (X509Certificate2 x509Certificate2 in list)
				{
					using (ChainContext chainContext = chainEngine.Build(x509Certificate2, chainBuildOptions, chainBuildParameter))
					{
						if (chainContext != null)
						{
							ChainSummary chainSummary = chainContext.Validate(chainPolicyParameters);
							if (chainSummary.Status == ChainValidityStatus.Valid)
							{
								return x509Certificate2;
							}
						}
					}
				}
			}
			return null;
		}

		private static bool IsUserMatch(X509Certificate2 certificate, IEnumerable<string> proxies)
		{
			string certNameInfo;
			try
			{
				certNameInfo = CapiNativeMethods.GetCertNameInfo(certificate, 0U, CapiNativeMethods.CertNameType.Email);
			}
			catch (CryptographicException arg)
			{
				ExTraceGlobals.CertificateTracer.TraceDebug<CryptographicException>(0L, "Failed to get the SubjectAltName of the certificate. Exception: {0}", arg);
				return false;
			}
			if (string.IsNullOrEmpty(certNameInfo))
			{
				return false;
			}
			foreach (string a in proxies)
			{
				if (string.Equals(a, certNameInfo, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsValidUsage(X509Certificate2 certificate, X509KeyUsageFlags expectedUsage)
		{
			CapiNativeMethods.AlgorithmId algorithmId = CapiNativeMethods.GetAlgorithmId(certificate);
			bool flag = algorithmId == CapiNativeMethods.AlgorithmId.DiffieHellmanStoreAndForward || algorithmId == CapiNativeMethods.AlgorithmId.DiffieHellmanEphemeral;
			bool flag2 = X509CertificateCollection.IsECCertificate(certificate);
			X509KeyUsageFlags intendedKeyUsage;
			try
			{
				intendedKeyUsage = CapiNativeMethods.GetIntendedKeyUsage(certificate);
			}
			catch (CryptographicException arg)
			{
				ExTraceGlobals.CertificateTracer.TraceDebug<CryptographicException>(0L, "Failed to get the key usage of the certificate. Exception: {0}", arg);
				return false;
			}
			X509KeyUsageFlags x509KeyUsageFlags = expectedUsage & (X509KeyUsageFlags.NonRepudiation | X509KeyUsageFlags.DigitalSignature);
			X509KeyUsageFlags x509KeyUsageFlags2 = expectedUsage & X509KeyUsageFlags.KeyEncipherment;
			if (intendedKeyUsage == X509KeyUsageFlags.None)
			{
				return x509KeyUsageFlags == X509KeyUsageFlags.None || !flag;
			}
			return (x509KeyUsageFlags == X509KeyUsageFlags.None || (!flag && (intendedKeyUsage & x509KeyUsageFlags) != X509KeyUsageFlags.None)) && (x509KeyUsageFlags2 == X509KeyUsageFlags.None || ((flag || flag2 || (intendedKeyUsage & X509KeyUsageFlags.KeyEncipherment) != X509KeyUsageFlags.None) && (!flag2 || (intendedKeyUsage & X509KeyUsageFlags.KeyEncipherment) != X509KeyUsageFlags.None || (intendedKeyUsage & X509KeyUsageFlags.KeyAgreement) != X509KeyUsageFlags.None) && (!flag || (intendedKeyUsage & X509KeyUsageFlags.KeyAgreement) != X509KeyUsageFlags.KeyAgreement || (intendedKeyUsage & X509KeyUsageFlags.DecipherOnly) != X509KeyUsageFlags.DecipherOnly)));
		}

		private static ChainBuildOptions GetChainBuildOptions(bool checkRevocation)
		{
			if (!checkRevocation)
			{
				return (ChainBuildOptions.CacheEndCert | ChainBuildOptions.RevocationCheckChainExcludeRoot | ChainBuildOptions.RevocationAccumulativeTimeout) & ~X509CertificateCollection.CheckCRLInCertificateChainOptions;
			}
			return ChainBuildOptions.CacheEndCert | ChainBuildOptions.RevocationCheckChainExcludeRoot | ChainBuildOptions.RevocationAccumulativeTimeout | X509CertificateCollection.CheckCRLInCertificateChainOptions;
		}

		private static ChainPolicyParameters GetChainPolicyParameters(bool checkRevocation)
		{
			if (!checkRevocation)
			{
				return X509CertificateCollection.PolicyIgnoreRevocation;
			}
			return X509CertificateCollection.DefaultPolicy;
		}

		private static ChainEngine BuildChainEngine(TimeSpan retrievalTimeout, X509Store store, bool exclusiveTrustMode, string poolUid = null)
		{
			if (string.IsNullOrEmpty(poolUid))
			{
				poolUid = "UID_C6FC2B0E-0DA3-4D1F-ACFD-B8CB400EB4A5";
			}
			ChainEnginePool chainEnginePool = null;
			if (!X509CertificateCollection.chainEnginePoolCache.TryGetValue(poolUid, out chainEnginePool))
			{
				chainEnginePool = new ChainEnginePool(10, ChainEngineOptions.CacheEndCert | ChainEngineOptions.UseLocalMachineStore | ChainEngineOptions.EnableCacheAutoUpdate | ChainEngineOptions.EnableShareStore, retrievalTimeout, 0, store, exclusiveTrustMode);
				X509CertificateCollection.chainEnginePoolCache.Add(poolUid, chainEnginePool);
			}
			return chainEnginePool.GetEngine();
		}

		private static ChainBuildParameter GetChainBuildParameter(TimeSpan connectionTimeout)
		{
			return new ChainBuildParameter(AndChainMatchIssuer.EmailProtection, connectionTimeout, false, TimeSpan.Zero);
		}

		private static bool IsECCertificate(X509Certificate2 certificate)
		{
			return string.Equals(certificate.PublicKey.Oid.Value, WellKnownOid.ECPublicKey.Value, StringComparison.OrdinalIgnoreCase);
		}

		private const string defaultChainEnginePoolUid = "UID_C6FC2B0E-0DA3-4D1F-ACFD-B8CB400EB4A5";

		private const int DefaultCacheLimit = 0;

		private const ChainEngineOptions DefaultOptions = ChainEngineOptions.CacheEndCert | ChainEngineOptions.UseLocalMachineStore | ChainEngineOptions.EnableCacheAutoUpdate | ChainEngineOptions.EnableShareStore;

		private static readonly TimeSpan DefaultCRLConnectionTimeout = TimeSpan.FromSeconds(10.0);

		private static readonly TimeSpan DefaultCRLRetrievalTimeout = TimeSpan.FromSeconds(10.0);

		private static int fieldSize = Marshal.SizeOf(typeof(ushort));

		private static ChainPolicyParameters DefaultPolicy = new BaseChainPolicyParameters(ChainPolicyOptions.None);

		private static ChainPolicyParameters PolicyIgnoreRevocation = new BaseChainPolicyParameters(ChainPolicyOptions.IgnoreEndRevUnknown | ChainPolicyOptions.IgnoreCTLSignerRevUnknown | ChainPolicyOptions.IgnoreCARevUnknown | ChainPolicyOptions.IgnoreRootRevUnknown);

		private static ChainBuildOptions CheckCRLInCertificateChainOptions = ChainBuildOptions.RevocationCheckEndCert | ChainBuildOptions.RevocationCheckChain | ChainBuildOptions.RevocationCheckChainExcludeRoot;

		private static MruDictionaryCache<string, ChainEnginePool> chainEnginePoolCache = new MruDictionaryCache<string, ChainEnginePool>(5000, 5);

		private enum OutlookCertificateTag : ushort
		{
			Version = 1,
			AsymmetricCapabilities,
			KeyExchangeCertificate,
			CertificateChain,
			MessageEncoding = 6,
			SignatureCertificate = 8,
			SignSha1Hash,
			DisplayName = 11,
			NortelBulkAlgorithm,
			CertificateTime,
			Defaults = 32,
			KeyExchangeSha1Hash = 34
		}
	}
}
