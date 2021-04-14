using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.Exchange.Extensions;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Security.Cryptography
{
	[SuppressUnmanagedCodeSecurity]
	[ComVisible(false)]
	internal static class CapiNativeMethods
	{
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "CryptAcquireContextW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptAcquireContext(out SafeCryptProvHandle hCryptProv, string pszContainer, string pszProvider, CapiNativeMethods.ProviderType provType, CapiNativeMethods.AcquireContext dwFlags);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("crypt32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CertFreeCertificateContext(IntPtr pCertContext);

		[DllImport("crypt32.dll", SetLastError = true)]
		public static extern SafeCertContextHandle CertDuplicateCertificateContext(IntPtr handle);

		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("crypt32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CertCloseStore(IntPtr hCertStore, uint dwFlags);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CertGetCertificateContextProperty(SafeCertContextHandle certContext, CapiNativeMethods.CertificatePropertyId property, SafeHGlobalHandle data, [In] [Out] ref uint size);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CertSetCertificateContextProperty(SafeCertContextHandle certContext, CapiNativeMethods.CertificatePropertyId property, uint flags, SafeHGlobalHandle data);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CertSetCertificateContextProperty(SafeCertContextHandle certContext, CapiNativeMethods.CertificatePropertyId propertyId, uint flags, [In] ref CapiNativeMethods.CryptoApiBlob data);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptSetProvParam(SafeCryptProvHandle hCryptProv, CapiNativeMethods.SetProvParam dwParam, [In] byte[] pbData, uint dwFlags);

		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptReleaseContext(IntPtr hCryptProv, uint dwFlags);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptEncodeObject(CapiNativeMethods.EncodeType encoding, [MarshalAs(UnmanagedType.LPStr)] string oidString, ref CapiNativeMethods.CryptoApiBlob bytes, [In] [Out] byte[] encoded, [In] [Out] ref uint size);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptEncodeObject(CapiNativeMethods.EncodeType encoding, IntPtr oidString, ref CapiNativeMethods.CertPublicKeyInfo bytes, [In] [Out] byte[] encoded, [In] [Out] ref uint size);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptEncodeObject(CapiNativeMethods.EncodeType dwCertEncodingType, IntPtr oid, ref CapiNativeMethods.CryptoApiBlob bytes, [In] [Out] byte[] encoded, [In] [Out] ref uint size);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptDecodeObject(CapiNativeMethods.EncodeType encodingType, IntPtr oidString, [In] byte[] data, uint count, uint flags, [In] [Out] SafeHGlobalHandle pvStructInfo, [In] [Out] ref uint size);

		[DllImport("crypt32.dll", SetLastError = true)]
		public static extern SafeCertContextHandle CertCreateCertificateContext(CapiNativeMethods.EncodeType dwCertEncodingType, byte[] rawData, int length);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptGetUserKey(SafeCryptProvHandle CryptProv, CapiNativeMethods.KeySpec KeySpec, out SafeCryptKeyHandle Key);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptGetKeyParam(SafeCryptKeyHandle key, CapiNativeMethods.KeyParameter parameter, [In] [Out] ref uint value, [In] [Out] ref uint size, uint reserved);

		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptDestroyKey(IntPtr key);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptGenKey(SafeCryptProvHandle hCryptProv, CapiNativeMethods.KeySpec Algid, uint dwFlags, [In] [Out] ref SafeCryptProvHandle hKey);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptGetProvParam(SafeCryptProvHandle handle, CapiNativeMethods.ProviderParameter parameter, out uint value, [In] [Out] ref uint length, uint extra);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern void CertFreeCertificateChain(IntPtr chainContext);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CertCreateCertificateChainEngine(ref ChainEnginePool.ChainEngineConfig configuration, out SafeChainEngineHandle engine);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern void CertFreeCertificateChainEngine(IntPtr handle);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CertControlStore([In] SafeCertStoreHandle hCertStore, [In] uint dwFlags, [In] CapiNativeMethods.StoreControl dwCtrlType, [In] ref IntPtr pvCtrlPara);

		public unsafe static X509Certificate2 CreateSelfSignCertificate(SafeCryptProvHandle providerHandle, X500DistinguishedName subjectName, uint flags, CapiNativeMethods.CryptKeyProvInfo keyProvInfo, string signatureAlgorithm, DateTime fromTime, DateTime toTime, X509ExtensionCollection extensions, string friendlyName)
		{
			if (string.IsNullOrEmpty(signatureAlgorithm))
			{
				throw new ArgumentNullException("signatureAlgorithm");
			}
			long num = fromTime.ToFileTimeUtc();
			NativeMethods.SystemTime systemTime;
			NativeMethods.FileTimeToSystemTime(ref num, out systemTime);
			num = toTime.ToFileTimeUtc();
			NativeMethods.SystemTime systemTime2;
			NativeMethods.FileTimeToSystemTime(ref num, out systemTime2);
			CapiNativeMethods.CryptAlgorithmIdentifier cryptAlgorithmIdentifier = default(CapiNativeMethods.CryptAlgorithmIdentifier);
			cryptAlgorithmIdentifier.ObjectId = signatureAlgorithm;
			SafeCertContextHandle safeCertContextHandle = null;
			X509Certificate2 result;
			try
			{
				SafeHGlobalHandle pExtensions = CapiNativeMethods.AllocateExtensions(extensions);
				int num2 = 0;
				try
				{
					fixed (byte* ptr = subjectName.RawData)
					{
						CapiNativeMethods.CryptoApiBlob cryptoApiBlob = default(CapiNativeMethods.CryptoApiBlob);
						cryptoApiBlob.Count = (uint)subjectName.RawData.Length;
						cryptoApiBlob.DataPointer = new IntPtr((void*)ptr);
						safeCertContextHandle = CapiNativeMethods.CertCreateSelfSignCertificate(providerHandle, new IntPtr((void*)(&cryptoApiBlob)), 0U, ref keyProvInfo, ref cryptAlgorithmIdentifier, ref systemTime, ref systemTime2, pExtensions);
						if (safeCertContextHandle.IsInvalid)
						{
							num2 = Marshal.GetLastWin32Error();
						}
					}
				}
				finally
				{
					byte* ptr = null;
				}
				if (num2 != 0)
				{
					throw new CryptographicException(num2);
				}
				if (!string.IsNullOrEmpty(friendlyName))
				{
					using (SafeHGlobalHandle safeHGlobalHandle = new SafeHGlobalHandle(Marshal.StringToHGlobalUni(friendlyName)))
					{
						CapiNativeMethods.CryptoApiBlob cryptoApiBlob2 = new CapiNativeMethods.CryptoApiBlob((uint)(2 * (friendlyName.Length + 1)), safeHGlobalHandle);
						if (!CapiNativeMethods.CertSetCertificateContextProperty(safeCertContextHandle, CapiNativeMethods.CertificatePropertyId.FriendlyName, 0U, ref cryptoApiBlob2))
						{
							throw new CryptographicException(Marshal.GetLastWin32Error());
						}
					}
				}
				X509Certificate2 x509Certificate = new X509Certificate2(safeCertContextHandle.DangerousGetHandle());
				result = x509Certificate;
			}
			finally
			{
				if (safeCertContextHandle != null)
				{
					safeCertContextHandle.Dispose();
					safeCertContextHandle = null;
				}
			}
			return result;
		}

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern SafeCertStoreHandle CertOpenStore(IntPtr lpszStoreProvider, CapiNativeMethods.EncodeType dwMsgAndCertEncodingType, IntPtr hCryptProv, CapiNativeMethods.CertificateStoreOptions dwFlags, string pvPara);

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern SafeCertStoreHandle CertOpenStore(IntPtr lpszStoreProvider, CapiNativeMethods.EncodeType dwMsgAndCertEncodingType, IntPtr hCryptProv, CapiNativeMethods.CertificateStoreOptions dwFlags, CapiNativeMethods.CryptoApiBlob pvPara);

		[DllImport("crypt32.dll", SetLastError = true)]
		internal static extern SafeCertStoreHandle CertDuplicateStore(IntPtr handle);

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CertCloseStore(IntPtr certStore, CapiNativeMethods.CertCloseStoreFlag flags);

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern int CertRDNValueToStr([In] int dwValueType, [In] ref CapiNativeMethods.CryptoApiBlob pValue, [In] [Out] SafeHGlobalHandle pszNameString, [In] int cchNameString);

		internal static CapiNativeMethods.AlgorithmId GetAlgorithmId(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			string keyAlgorithm = certificate.GetKeyAlgorithm();
			return (CapiNativeMethods.AlgorithmId)CapiNativeMethods.FindOIDInfo(CapiNativeMethods.OidSearchKey.Oid, keyAlgorithm, CapiNativeMethods.OidSearchScope.PublicKeyAlgorithm).Algid;
		}

		internal static string GetKeyAlgorithmName(X509Certificate2 certificate)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			string keyAlgorithm = certificate.GetKeyAlgorithm();
			return CapiNativeMethods.FindOIDInfo(CapiNativeMethods.OidSearchKey.Oid, keyAlgorithm, CapiNativeMethods.OidSearchScope.PublicKeyAlgorithm).Name;
		}

		internal static string[] GetEnhancedKeyUsage(X509Certificate2 certificate, CapiNativeMethods.EnhancedKeyUsageSearch flags)
		{
			int num = 0;
			uint size = 512U;
			SafeHGlobalHandle safeHGlobalHandle = SafeHGlobalHandle.InvalidHandle;
			int i = 0;
			while (i < 2)
			{
				num = 0;
				SafeHGlobalHandle safeHGlobalHandle2;
				safeHGlobalHandle = (safeHGlobalHandle2 = NativeMethods.AllocHGlobal((int)size));
				try
				{
					bool flag = CapiNativeMethods.CertGetEnhancedKeyUsage(certificate.Handle, flags, safeHGlobalHandle, ref size);
					num = Marshal.GetLastWin32Error();
					if (flag)
					{
						CapiNativeMethods.CryptoApiBlob cryptoApiBlob = (CapiNativeMethods.CryptoApiBlob)Marshal.PtrToStructure(safeHGlobalHandle.DangerousGetHandle(), typeof(CapiNativeMethods.CryptoApiBlob));
						if (cryptoApiBlob.Count == 0U && -2146885628 != num)
						{
							return null;
						}
						string[] array = new string[cryptoApiBlob.Count];
						IntPtr dataPointer = cryptoApiBlob.DataPointer;
						for (int j = 0; j < (int)cryptoApiBlob.Count; j++)
						{
							IntPtr ptr = Marshal.ReadIntPtr(dataPointer);
							array[j] = Marshal.PtrToStringAnsi(ptr);
							dataPointer = new IntPtr((long)dataPointer + (long)IntPtr.Size);
						}
						return array;
					}
				}
				finally
				{
					if (safeHGlobalHandle2 != null)
					{
						((IDisposable)safeHGlobalHandle2).Dispose();
					}
				}
				if (234 == num)
				{
					i++;
					continue;
				}
				break;
			}
			throw new CryptographicException(num);
		}

		internal static bool DecodeObject(string sourceType, byte[] source, out SafeHGlobalHandle destination, out uint destinationSize)
		{
			IntPtr intPtr = IntPtr.Zero;
			bool result;
			try
			{
				intPtr = Marshal.StringToHGlobalAnsi(sourceType);
				result = CapiNativeMethods.DecodeObject(intPtr, source, out destination, out destinationSize);
			}
			finally
			{
				if (intPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(intPtr);
				}
			}
			return result;
		}

		internal static bool DecodeObject(CapiNativeMethods.EncodeDecodeObjectType sourceType, byte[] source, out SafeHGlobalHandle destination, out uint destinationSize)
		{
			return CapiNativeMethods.DecodeObject((IntPtr)((long)((ulong)sourceType)), source, out destination, out destinationSize);
		}

		private static bool DecodeObject(IntPtr sourceType, byte[] source, out SafeHGlobalHandle destination, out uint destinationSize)
		{
			destination = SafeHGlobalHandle.InvalidHandle;
			destinationSize = 0U;
			uint num = 0U;
			SafeHGlobalHandle safeHGlobalHandle = SafeHGlobalHandle.InvalidHandle;
			if (!CapiNativeMethods.CryptDecodeObject(CapiNativeMethods.EncodeType.X509Asn | CapiNativeMethods.EncodeType.Pkcs7Asn, sourceType, source, (uint)source.Length, 0U, safeHGlobalHandle, ref num))
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			safeHGlobalHandle = NativeMethods.AllocHGlobal((int)num);
			if (!CapiNativeMethods.CryptDecodeObject(CapiNativeMethods.EncodeType.X509Asn | CapiNativeMethods.EncodeType.Pkcs7Asn, sourceType, source, (uint)source.Length, 0U, safeHGlobalHandle, ref num))
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			destination = safeHGlobalHandle;
			destinationSize = num;
			return true;
		}

		internal static X509KeyUsageFlags GetIntendedKeyUsage(X509Certificate2 certificate)
		{
			byte[] array = new byte[2];
			if (CapiNativeMethods.CertGetIntendedKeyUsage(CapiNativeMethods.EncodeType.X509Asn | CapiNativeMethods.EncodeType.Pkcs7Asn, ((CapiNativeMethods.CertContext)Marshal.PtrToStructure(certificate.Handle, typeof(CapiNativeMethods.CertContext))).CertInfo, array, 2U))
			{
				return (X509KeyUsageFlags)((int)array[1] << 8 | (int)array[0]);
			}
			int lastWin32Error = Marshal.GetLastWin32Error();
			if (lastWin32Error != 0)
			{
				throw new CryptographicException(lastWin32Error);
			}
			return X509KeyUsageFlags.None;
		}

		internal static SafeCertStoreHandle GetStoreHandleFromCertificate(X509Certificate2 certificate)
		{
			return SafeCertStoreHandle.Clone(((CapiNativeMethods.CertContext)Marshal.PtrToStructure(certificate.Handle, typeof(CapiNativeMethods.CertContext))).CertStore);
		}

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CertGetIntendedKeyUsage([In] CapiNativeMethods.EncodeType encodingType, [In] IntPtr pCertInfo, [In] [Out] byte[] usage, [In] uint size);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CertGetEnhancedKeyUsage(IntPtr pCertContext, CapiNativeMethods.EnhancedKeyUsageSearch flags, [In] SafeHGlobalHandle usage, [In] [Out] ref uint size);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern SafeCertContextHandle CertCreateSelfSignCertificate([In] SafeCryptProvHandle hProv, [In] IntPtr pSubjectIssuerBlob, [In] uint dwFlags, [In] ref CapiNativeMethods.CryptKeyProvInfo keyProvInfo, [In] ref CapiNativeMethods.CryptAlgorithmIdentifier signatureAlgorithm, [In] ref NativeMethods.SystemTime fromTime, [In] ref NativeMethods.SystemTime toTime, [In] SafeHGlobalHandle pExtensions);

		[DllImport("crypt32.dll", CharSet = CharSet.Ansi)]
		private static extern IntPtr CryptFindOIDInfo(CapiNativeMethods.OidSearchKey keyType, [MarshalAs(UnmanagedType.LPStr)] string key, CapiNativeMethods.OidSearchScope groupId);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptDestroyHash(IntPtr handle);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptCreateHash(IntPtr hProv, CapiNativeMethods.AlgorithmId Algid, IntPtr hKey, int flags, [In] [Out] ref SafeHashHandle hashHandle);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public unsafe static extern bool CryptHashData(SafeHashHandle hHash, byte* pbData, uint length, int flags);

		[DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CryptGetHashParam(SafeHashHandle hHash, CapiNativeMethods.HashParameter parameter, [In] [Out] byte[] bytes, [In] [Out] ref uint size, uint flags);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CertVerifyCertificateChainPolicy([MarshalAs(UnmanagedType.LPStr)] string pszPolicyOID, SafeChainContextHandle chainContext, [In] ref CapiNativeMethods.CertChainPolicyParameters pPolicyPara, [In] [Out] ref CapiNativeMethods.CertChainPolicyStatus pPolicyStatus);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CertVerifyCertificateChainPolicy(IntPtr pszPolicyOID, SafeChainContextHandle chainContext, [In] ref CapiNativeMethods.CertChainPolicyParameters pPolicyPara, [In] [Out] ref CapiNativeMethods.CertChainPolicyStatus pPolicyStatus);

		[DllImport("crypt32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CertGetCertificateChain(SafeChainEngineHandle engine, IntPtr pCertContext, IntPtr time, SafeCertStoreHandle hAdditionalStore, [In] ref CapiNativeMethods.CertChainParameter parameters, ChainBuildOptions flags, IntPtr reserved, out SafeChainContextHandle chainContext);

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern uint CertGetNameStringW(IntPtr pCertContext, CapiNativeMethods.CertNameType displayType, uint dwFlags, [In] ref uint pvTypePara, [In] [Out] SafeHGlobalHandle pszNameString, uint cchNameString);

		[DllImport("crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		private static extern uint CertGetNameStringW(IntPtr pCertContext, CapiNativeMethods.CertNameType displayType, uint dwFlags, [MarshalAs(UnmanagedType.LPStr)] string oidString, [In] [Out] SafeHGlobalHandle pszNameString, [In] uint cchNameString);

		public static string GetCertNameInfo([In] X509Certificate2 certificate, [In] uint dwFlags, [In] CapiNativeMethods.CertNameType displayType)
		{
			if (certificate == null)
			{
				throw new ArgumentNullException("certificate");
			}
			SafeHGlobalHandle safeHGlobalHandle = SafeHGlobalHandle.InvalidHandle;
			string result;
			try
			{
				if (displayType == CapiNativeMethods.CertNameType.Attr)
				{
					uint num = CapiNativeMethods.CertGetNameStringW(certificate.Handle, displayType, dwFlags, WellKnownOid.CommonName.Value, safeHGlobalHandle, 0U);
					if (num == 0U)
					{
						throw new CryptographicException(Marshal.GetLastWin32Error());
					}
					safeHGlobalHandle = NativeMethods.AllocHGlobal((int)(2U * num));
					if (CapiNativeMethods.CertGetNameStringW(certificate.Handle, displayType, dwFlags, WellKnownOid.CommonName.Value, safeHGlobalHandle, num) == 0U)
					{
						throw new CryptographicException(Marshal.GetLastWin32Error());
					}
					result = Marshal.PtrToStringUni(safeHGlobalHandle.DangerousGetHandle());
				}
				else
				{
					uint num2 = 33554435U;
					uint num3 = CapiNativeMethods.CertGetNameStringW(certificate.Handle, displayType, dwFlags, ref num2, safeHGlobalHandle, 0U);
					if (num3 == 0U)
					{
						throw new CryptographicException(Marshal.GetLastWin32Error());
					}
					safeHGlobalHandle = NativeMethods.AllocHGlobal((int)(2U * num3));
					if (CapiNativeMethods.CertGetNameStringW(certificate.Handle, displayType, dwFlags, ref num2, safeHGlobalHandle, num3) == 0U)
					{
						throw new CryptographicException(Marshal.GetLastWin32Error());
					}
					result = Marshal.PtrToStringUni(safeHGlobalHandle.DangerousGetHandle());
				}
			}
			finally
			{
				safeHGlobalHandle.Dispose();
			}
			return result;
		}

		private static CapiNativeMethods.CryptOidInfo FindOIDInfo(CapiNativeMethods.OidSearchKey keyType, string key, CapiNativeMethods.OidSearchScope groupId)
		{
			CapiNativeMethods.CryptOidInfo result = new CapiNativeMethods.CryptOidInfo(CapiNativeMethods.CryptOidInfo.MarshalSize);
			IntPtr intPtr = CapiNativeMethods.CryptFindOIDInfo(keyType, key, groupId);
			if (intPtr != IntPtr.Zero)
			{
				result = (CapiNativeMethods.CryptOidInfo)Marshal.PtrToStructure(intPtr, typeof(CapiNativeMethods.CryptOidInfo));
			}
			return result;
		}

		private static SafeHGlobalHandle AllocateExtensions(X509ExtensionCollection extensions)
		{
			SafeHGlobalHandle safeHGlobalHandle = SafeHGlobalHandle.InvalidHandle;
			if (extensions == null || extensions.Count == 0)
			{
				return safeHGlobalHandle;
			}
			int num = CapiNativeMethods.CryptoApiBlob.MarshalSize;
			foreach (X509Extension x509Extension in extensions)
			{
				num += CapiNativeMethods.CertExtension.MarshalSize;
				num += x509Extension.Oid.Value.Length + 1;
				num += x509Extension.RawData.Length;
			}
			safeHGlobalHandle = NativeMethods.AllocHGlobal(num);
			CapiNativeMethods.CryptoApiBlob cryptoApiBlob;
			cryptoApiBlob.Count = (uint)extensions.Count;
			cryptoApiBlob.DataPointer = (IntPtr)((long)safeHGlobalHandle.DangerousGetHandle() + (long)CapiNativeMethods.CryptoApiBlob.MarshalSize);
			Marshal.StructureToPtr(cryptoApiBlob, safeHGlobalHandle.DangerousGetHandle(), false);
			IntPtr intPtr = cryptoApiBlob.DataPointer;
			IntPtr intPtr2 = (IntPtr)((long)intPtr + (long)extensions.Count * (long)CapiNativeMethods.CertExtension.MarshalSize);
			foreach (X509Extension x509Extension2 in extensions)
			{
				byte[] rawData = x509Extension2.RawData;
				CapiNativeMethods.CertExtension certExtension;
				certExtension.ObjectId = intPtr2;
				byte[] array = new byte[x509Extension2.Oid.Value.Length + 1];
				Encoding.ASCII.GetBytes(x509Extension2.Oid.Value, 0, x509Extension2.Oid.Value.Length, array, 0);
				Marshal.Copy(array, 0, intPtr2, array.Length);
				intPtr2 = (IntPtr)((long)intPtr2 + (long)array.Length);
				certExtension.IsCritical = x509Extension2.Critical;
				certExtension.Value.Count = (uint)rawData.Length;
				certExtension.Value.DataPointer = ((rawData.Length != 0) ? intPtr2 : IntPtr.Zero);
				Marshal.StructureToPtr(certExtension, intPtr, false);
				intPtr = (IntPtr)((long)intPtr + (long)CapiNativeMethods.CertExtension.MarshalSize);
				if (rawData.Length != 0)
				{
					Marshal.Copy(rawData, 0, intPtr2, rawData.Length);
					intPtr2 = (IntPtr)((long)intPtr2 + (long)rawData.Length);
				}
			}
			return safeHGlobalHandle;
		}

		internal static string RDNValueToString(CapiNativeMethods.CertRdnAttribute attribute)
		{
			SafeHGlobalHandle safeHGlobalHandle = SafeHGlobalHandle.InvalidHandle;
			int num = CapiNativeMethods.CertRDNValueToStr(attribute.dwValueType, ref attribute.value, safeHGlobalHandle, 0);
			if (num == 0)
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			safeHGlobalHandle = NativeMethods.AllocHGlobal(2 * num);
			if (CapiNativeMethods.CertRDNValueToStr(attribute.dwValueType, ref attribute.value, safeHGlobalHandle, num) != num)
			{
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			return Marshal.PtrToStringUni(safeHGlobalHandle.DangerousGetHandle());
		}

		public static bool GetPrivateKeyInfo(X509Certificate2 certificate, out CapiNativeMethods.CryptKeyProvInfo info)
		{
			info = default(CapiNativeMethods.CryptKeyProvInfo);
			using (SafeCertContextHandle safeCertContextHandle = SafeCertContextHandle.Clone(certificate.Handle))
			{
				uint size = 0U;
				if (!CapiNativeMethods.CertGetCertificateContextProperty(safeCertContextHandle, CapiNativeMethods.CertificatePropertyId.KeyProviderInfo, SafeHGlobalHandle.InvalidHandle, ref size))
				{
					int lastWin32Error = Marshal.GetLastWin32Error();
					if (lastWin32Error == -2146885628)
					{
						return false;
					}
					throw new CryptographicException(lastWin32Error);
				}
				else
				{
					SafeHGlobalHandle safeHGlobalHandle = NativeMethods.AllocHGlobal((int)size);
					if (!CapiNativeMethods.CertGetCertificateContextProperty(safeCertContextHandle, CapiNativeMethods.CertificatePropertyId.KeyProviderInfo, safeHGlobalHandle, ref size))
					{
						throw new CryptographicException(Marshal.GetLastWin32Error());
					}
					info = (CapiNativeMethods.CryptKeyProvInfo)Marshal.PtrToStructure(safeHGlobalHandle.DangerousGetHandle(), typeof(CapiNativeMethods.CryptKeyProvInfo));
				}
			}
			return true;
		}

		[DllImport("Crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CryptProtectData(ref CapiNativeMethods.CryptoApiBlob pDataIn, IntPtr szDataDescr, IntPtr pOptionalEntropy, IntPtr pvReserved, IntPtr szPrompt, CapiNativeMethods.DPAPIFlags dwFlags, out CapiNativeMethods.CryptoApiBlob pDataOut);

		[DllImport("Crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CryptUnprotectData(ref CapiNativeMethods.CryptoApiBlob pDataIn, IntPtr ppszDataDescr, IntPtr pOptionalEntropy, IntPtr pvReserved, IntPtr szPrompt, CapiNativeMethods.DPAPIFlags dwFlags, out CapiNativeMethods.CryptoApiBlob pDataOut);

		public static bool DPAPIDecryptData(byte[] pIn, out byte[] pOut)
		{
			if (pIn.Length > 0)
			{
				pOut = CapiNativeMethods.DPAPIDecryptData<byte[]>(pIn, CapiNativeMethods.DPAPIFlags.CRYPTPROTECT_LOCAL_MACHINE, delegate(SafeSecureHGlobalHandle decryptedData)
				{
					byte[] array = new byte[decryptedData.Length];
					Marshal.Copy(decryptedData.DangerousGetHandle(), array, 0, decryptedData.Length);
					return array;
				});
				return true;
			}
			pOut = null;
			return false;
		}

		public unsafe static SecureString DPAPIDecryptDataToSecureString(byte[] pIn, CapiNativeMethods.DPAPIFlags flags)
		{
			return CapiNativeMethods.DPAPIDecryptData<SecureString>(pIn, flags, (SafeSecureHGlobalHandle decryptedData) => new SecureString((char*)decryptedData.DangerousGetHandle().ToPointer(), decryptedData.Length / 2));
		}

		[SecurityCritical]
		private static T DPAPIDecryptData<T>(byte[] encryptedData, CapiNativeMethods.DPAPIFlags flags, Func<SafeSecureHGlobalHandle, T> resultMarshaller) where T : class
		{
			using (SafeHGlobalHandle safeHGlobalHandle = SafeHGlobalHandle.CopyToHGlobal(encryptedData))
			{
				CapiNativeMethods.CryptoApiBlob cryptoApiBlob = new CapiNativeMethods.CryptoApiBlob((uint)encryptedData.Length, safeHGlobalHandle);
				CapiNativeMethods.CryptoApiBlob cryptoApiBlob2 = default(CapiNativeMethods.CryptoApiBlob);
				if (CapiNativeMethods.CryptUnprotectData(ref cryptoApiBlob, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, flags | CapiNativeMethods.DPAPIFlags.CRYPTPROTECT_UI_FORBIDDEN, out cryptoApiBlob2))
				{
					using (SafeSecureHGlobalHandle safeSecureHGlobalHandle = SafeSecureHGlobalHandle.Assign(cryptoApiBlob2.DataPointer, (int)cryptoApiBlob2.Count))
					{
						return resultMarshaller(safeSecureHGlobalHandle);
					}
				}
				throw new CryptographicException(Marshal.GetLastWin32Error());
			}
			T result;
			return result;
		}

		public static bool DPAPIEncryptData(byte[] sensitiveData, out byte[] encryptedData)
		{
			if (sensitiveData.Length > 0)
			{
				using (SafeSecureHGlobalHandle safeSecureHGlobalHandle = SafeSecureHGlobalHandle.CopyToHGlobal(sensitiveData))
				{
					encryptedData = CapiNativeMethods.DPAPIEncryptData(safeSecureHGlobalHandle, CapiNativeMethods.DPAPIFlags.CRYPTPROTECT_LOCAL_MACHINE);
					return true;
				}
			}
			encryptedData = null;
			return false;
		}

		public static byte[] DPAPIEncryptData(SecureString sensitiveData, CapiNativeMethods.DPAPIFlags flags)
		{
			byte[] result;
			using (SafeSecureHGlobalHandle safeSecureHGlobalHandle = sensitiveData.ConvertToUnsecureHGlobal())
			{
				result = CapiNativeMethods.DPAPIEncryptData(safeSecureHGlobalHandle, flags);
			}
			return result;
		}

		private static byte[] DPAPIEncryptData(SafeSecureHGlobalHandle sensitiveData, CapiNativeMethods.DPAPIFlags flags)
		{
			CapiNativeMethods.CryptoApiBlob cryptoApiBlob = new CapiNativeMethods.CryptoApiBlob((uint)sensitiveData.Length, sensitiveData);
			CapiNativeMethods.CryptoApiBlob cryptoApiBlob2 = default(CapiNativeMethods.CryptoApiBlob);
			if (CapiNativeMethods.CryptProtectData(ref cryptoApiBlob, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, flags | CapiNativeMethods.DPAPIFlags.CRYPTPROTECT_UI_FORBIDDEN, out cryptoApiBlob2))
			{
				using (SafeHGlobalHandle safeHGlobalHandle = new SafeHGlobalHandle(cryptoApiBlob2.DataPointer))
				{
					byte[] array = new byte[cryptoApiBlob2.Count];
					Marshal.Copy(safeHGlobalHandle.DangerousGetHandle(), array, 0, (int)cryptoApiBlob2.Count);
					return array;
				}
			}
			throw new CryptographicException(Marshal.GetLastWin32Error());
		}

		private const string ADVAPI32 = "advapi32.dll";

		private const string CRYPT32 = "crypt32.dll";

		private const uint CERT_X500_NAME_STR = 3U;

		private const uint CERT_NAME_STR_REVERSE_FLAG = 33554432U;

		internal const int CertSystemStoreServicesID = 5;

		internal const int CertSystemStoreLocationShift = 16;

		private static IntPtr x509PublicKeyInfo = new IntPtr(8);

		[Flags]
		public enum CertificateStoreOptions : uint
		{
			Delete = 16U,
			Create = 8192U,
			OpenExisting = 16384U,
			ReadOnly = 32768U,
			LocalMachine = 131072U,
			Services = 327680U
		}

		public enum CertificateStoreProvider : uint
		{
			Memory = 2U,
			Serialized = 6U,
			System = 10U,
			Physical = 14U
		}

		public enum WinCapiStatus : uint
		{
			MoreData = 234U,
			NotFound = 2148081668U,
			SilentContext = 2148073506U
		}

		internal enum CertificatePropertyId : uint
		{
			KeyProviderHandle = 1U,
			KeyProviderInfo,
			FriendlyName = 11U,
			XEnrollmentRequest = 52312U
		}

		internal enum ProviderType : uint
		{
			CNG,
			RsaFull,
			Dss = 3U,
			RsaSChannel = 12U,
			DssDiffieHellman,
			AES = 24U
		}

		internal enum OidSearchKey : uint
		{
			Oid = 1U,
			Name,
			AlgId,
			Sign
		}

		internal enum OidSearchScope : uint
		{
			All,
			HashAlgorithm,
			EncryptAlgorithm,
			PublicKeyAlgorithm,
			SignatureAlgorithm,
			RdnAttribute,
			ExtensionOrAttribute,
			EnhancedKeyUsage,
			Policy,
			Template
		}

		[Flags]
		internal enum EncodeType : uint
		{
			None = 0U,
			X509Asn = 1U,
			Pkcs7Asn = 65536U
		}

		[Flags]
		internal enum CertCloseStoreFlag : uint
		{
			CertCloseStoreForceFlag = 1U,
			CertCloseStoreCheckFlag = 2U
		}

		[Flags]
		internal enum AcquireContext : uint
		{
			NewKeyset = 8U,
			DeleteKeyset = 16U,
			MachineKeyset = 32U,
			Silent = 64U,
			Verify = 4026531840U
		}

		internal enum SetProvParam : uint
		{
			KeySetSecurityDescriptor = 8U
		}

		internal enum ProviderParameter : uint
		{
			EnumerateAlgorithms = 1U,
			EnumerateContainers,
			ImplementationType,
			Name,
			Version,
			Container
		}

		[Flags]
		internal enum ProviderImplementationType : uint
		{
			Hardware = 1U,
			Software = 2U,
			Mixed = 3U,
			Unknown = 4U,
			Removable = 8U
		}

		[Flags]
		internal enum SecurityInformation : uint
		{
			Owner = 1U,
			Group = 2U,
			Dacl = 4U,
			Sacl = 8U
		}

		[Flags]
		internal enum CertKeyOptions : uint
		{
			None = 0U,
			Exportable = 1U,
			UserProtected = 2U,
			CreateSalt = 4U,
			UpdateKey = 8U,
			Archivable = 16384U
		}

		[Flags]
		internal enum CertKeyUsage : uint
		{
			EncipherOnly = 1U,
			CrlSign = 2U,
			KeyCertSign = 4U,
			KeyAgreement = 8U,
			DataEncipherment = 16U,
			KeyEncipherment = 32U,
			NonRepudiation = 64U,
			DigitalSignature = 128U,
			DecipherOnly = 32768U
		}

		internal enum CertNameType : uint
		{
			Email = 1U,
			Rdn,
			Attr,
			SimpleDisplay,
			FriendlyDisplay,
			Dns,
			Url,
			Upn
		}

		internal enum CertAltNameType : uint
		{
			OtherName = 1U,
			Rfc822Name,
			DnsName,
			X400Address,
			DirectoryName,
			EdiPartyName,
			Url,
			IpAddress,
			RegisteredId
		}

		internal enum KeySpec
		{
			KeyExchange = 1,
			Signature
		}

		internal enum AlgorithmId : uint
		{
			RsaKeyExchange = 41984U,
			DsaSignature = 8704U,
			DiffieHellmanStoreAndForward = 43521U,
			DiffieHellmanEphemeral,
			Sha256 = 32780U,
			Sha512 = 32782U,
			CAlgOIDInfoParameters = 4294967294U
		}

		internal enum HashParameter : uint
		{
			AlgorithmId = 1U,
			HashValue,
			HashSize = 4U
		}

		[Flags]
		internal enum EnhancedKeyUsageSearch : uint
		{
			ExtensionAndProperty = 0U,
			Extension = 2U,
			Property = 4U
		}

		internal enum EncodeDecodeObjectType : uint
		{
			X509Cert = 1U,
			X509CertToBeSigned,
			X509CertCrlToBeSigned,
			X509CertRequestToBeSigned,
			X509Extensions,
			X509AnyString,
			X509NameValue = 6U,
			X509Name,
			X509PublicKeyInfo,
			X509AuthorityKeyId,
			X509KeyAttributes,
			X509KeyUsageRestriction,
			X509AlternateName,
			X509BasicConstraints,
			X509KeyUsage,
			X509BasicConstraints2,
			X509CertPolicies,
			pkcsUtcTime,
			PkcsTimeRequest,
			RsaCspPublickeyblob,
			X509UnicodeName,
			X509KeygenRequestToBeSigned,
			PkcsAttribute,
			PkcsContentInfoSequenceOfAny,
			X509UnicodeAnyString,
			X509UnicodeNameValue = 24U,
			X509OctetString,
			X509Bits,
			X509Integer,
			X509MultiByteInteger,
			X509Enumerated,
			X509CrlReasonCode = 29U,
			X509ChoiceOfTime,
			X509AuthorityKeyId2,
			X509AuthorityInfoAccess,
			PkcsContentInfo,
			X509SequenceOfAny,
			X509CrlDistPoints,
			X509EnhancedKeyUsage,
			PkcsCtl,
			X509DssPublickey,
			X509MultiByteUint = 38U,
			X509DssParameters,
			X509DssSignature,
			PkcsRc2CbcParameters,
			pkcsSmimeCapabilities,
			X509CertPair = 53U,
			X509IssuingDistPoint,
			X509NameConstraints,
			X509PolicyMappings,
			X509PolicyConstraints,
			X509CrossCertDistPoints,
			cmcData,
			cmcResponse,
			cmcStatus,
			cmcAddExtensions,
			cmcAddAttributes,
			X509CertificateTemplate,
			Pkcs7SignerInfo = 500U,
			cmsSignerInfo
		}

		internal enum KeyParameter : uint
		{
			KeyLength = 9U
		}

		private enum AlgorithmClass : uint
		{
			KeyExchange = 40960U,
			Signature = 8192U,
			Hash = 32768U
		}

		private enum Algorithm : uint
		{
			Any,
			Dss = 512U,
			Rsa = 1024U,
			Block = 1536U,
			Stream = 2048U,
			DiffieHellman = 2560U,
			SecureChannel = 3072U
		}

		private enum DssSubId : uint
		{
			Any
		}

		private enum RsaSubId : uint
		{
			Any
		}

		private enum DiffieHellmanSubId : uint
		{
			StoreAndForward = 1U,
			Ephemeral
		}

		private enum HashSubId : uint
		{
			Sha1 = 4U,
			Mac,
			HMac = 9U,
			Sha256 = 12U,
			Sha384,
			Sha512
		}

		internal enum StoreControl : uint
		{
			Resync = 1U,
			NotifiyChange,
			Commit,
			AutoResync,
			CancelNotifyChange
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct CryptKeyProvInfo
		{
			public string ContainerName;

			public string ProviderName;

			public CapiNativeMethods.ProviderType ProviderType;

			public CapiNativeMethods.AcquireContext Flags;

			public uint CountParameters;

			public IntPtr Parameters;

			public CapiNativeMethods.KeySpec KeySpec;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct CertAltNameDnsEntry
		{
			public static readonly int MarshalSize = Marshal.SizeOf(typeof(CapiNativeMethods.CertAltNameDnsEntry));

			public CapiNativeMethods.CertAltNameType Type;

			public string Name;

			private uint padding;
		}

		internal struct CertUsageMatch
		{
			public CertUsageMatch(CapiNativeMethods.CertUsageMatch.Operator flags, CapiNativeMethods.CryptoApiBlob usage)
			{
				this.type = flags;
				this.usage = usage;
			}

			public static CapiNativeMethods.CertUsageMatch Empty = new CapiNativeMethods.CertUsageMatch(CapiNativeMethods.CertUsageMatch.Operator.And, CapiNativeMethods.CryptoApiBlob.Empty);

			public CapiNativeMethods.CertUsageMatch.Operator type;

			public CapiNativeMethods.CryptoApiBlob usage;

			public enum Operator : uint
			{
				And,
				Or
			}
		}

		internal struct CertTrustStatus
		{
			public Microsoft.Exchange.Security.Cryptography.X509Certificates.TrustStatus error;

			public TrustInformation information;
		}

		internal struct CTLEntry
		{
			public CapiNativeMethods.CryptoApiBlob subjectIdentifier;

			public int attributeCount;

			public IntPtr attributes;
		}

		internal struct CertTrustListInfo
		{
			private uint size;

			public IntPtr pCtlEntry;

			public IntPtr pCtlContext;
		}

		internal struct CryptoApiBlob
		{
			public CryptoApiBlob(uint count, SafeHGlobalHandleBase handle)
			{
				this.Count = count;
				this.DataPointer = handle.DangerousGetHandle();
			}

			public static readonly int MarshalSize = Marshal.SizeOf(typeof(CapiNativeMethods.CryptoApiBlob));

			public static CapiNativeMethods.CryptoApiBlob Empty = new CapiNativeMethods.CryptoApiBlob(0U, SafeHGlobalHandle.InvalidHandle);

			public uint Count;

			public IntPtr DataPointer;
		}

		internal struct CertChainParameter
		{
			public CertChainParameter(CapiNativeMethods.CertUsageMatch match, TimeSpan timeout, bool overrideCRLTime, TimeSpan freshnessTime)
			{
				this.size = CapiNativeMethods.CertChainParameter.MarshalSize;
				this.requestedUsage = match;
				this.requestedIssuancePolicy = CapiNativeMethods.CertUsageMatch.Empty;
				this.urlRetrievalTimeout = (int)timeout.TotalMilliseconds;
				this.checkRevocationFreshnessTime = overrideCRLTime;
				this.revocationFreshnessTime = (int)freshnessTime.TotalSeconds;
			}

			private static readonly int MarshalSize = Marshal.SizeOf(typeof(CapiNativeMethods.CertChainParameter));

			private int size;

			private CapiNativeMethods.CertUsageMatch requestedUsage;

			private CapiNativeMethods.CertUsageMatch requestedIssuancePolicy;

			private int urlRetrievalTimeout;

			[MarshalAs(UnmanagedType.Bool)]
			private bool checkRevocationFreshnessTime;

			private int revocationFreshnessTime;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct CryptAlgorithmIdentifier
		{
			[MarshalAs(UnmanagedType.LPStr)]
			internal string ObjectId;

			internal CapiNativeMethods.CryptoApiBlob Parameters;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct CertExtension
		{
			public static readonly int MarshalSize = Marshal.SizeOf(typeof(CapiNativeMethods.CertExtension));

			public IntPtr ObjectId;

			[MarshalAs(UnmanagedType.Bool)]
			public bool IsCritical;

			public CapiNativeMethods.CryptoApiBlob Value;
		}

		private struct CertContext
		{
			public uint CertEncodingType;

			public IntPtr CertEncoded;

			public uint CertEncodedSize;

			public IntPtr CertInfo;

			public IntPtr CertStore;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		private struct CryptOidInfo
		{
			public CryptOidInfo(int size)
			{
				this.Size = (uint)size;
				this.OID = null;
				this.Name = null;
				this.GroupId = 0U;
				this.Algid = 0U;
				this.ExtraInfo = default(CapiNativeMethods.CryptoApiBlob);
			}

			public static readonly int MarshalSize = Marshal.SizeOf(typeof(CapiNativeMethods.CryptOidInfo));

			public uint Size;

			[MarshalAs(UnmanagedType.LPStr)]
			public string OID;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string Name;

			public uint GroupId;

			public uint Algid;

			public CapiNativeMethods.CryptoApiBlob ExtraInfo;
		}

		public struct CertChainPolicyParameters
		{
			public CertChainPolicyParameters(ChainPolicyOptions flags)
			{
				this.size = CapiNativeMethods.CertChainPolicyParameters.marshalSize;
				this.flags = flags;
				this.extraPolicyPara = IntPtr.Zero;
			}

			public IntPtr ExtraPolicy
			{
				get
				{
					return this.extraPolicyPara;
				}
				set
				{
					this.extraPolicyPara = value;
				}
			}

			private static uint marshalSize = (uint)Marshal.SizeOf(typeof(CapiNativeMethods.CertChainPolicyParameters));

			private uint size;

			private ChainPolicyOptions flags;

			private IntPtr extraPolicyPara;
		}

		public struct CertChainPolicyStatus
		{
			public ChainValidityStatus Status
			{
				get
				{
					return this.status;
				}
			}

			public static CapiNativeMethods.CertChainPolicyStatus Create()
			{
				return new CapiNativeMethods.CertChainPolicyStatus
				{
					size = CapiNativeMethods.CertChainPolicyStatus.MarshalSize,
					status = ChainValidityStatus.Valid,
					chainIndex = 0,
					elementIndex = 0,
					extraPolicyStatus = IntPtr.Zero
				};
			}

			private static uint MarshalSize = (uint)Marshal.SizeOf(typeof(CapiNativeMethods.CertChainPolicyStatus));

			private uint size;

			private ChainValidityStatus status;

			private int chainIndex;

			private int elementIndex;

			private IntPtr extraPolicyStatus;
		}

		public struct CryptBitBlob
		{
			public CryptBitBlob(uint size, IntPtr data)
			{
				this.size = size;
				this.data = data;
				this.unusedBitsSize = 0U;
			}

			private uint size;

			private IntPtr data;

			private uint unusedBitsSize;
		}

		public struct CertPublicKeyInfo
		{
			public CertPublicKeyInfo(string objectId, CapiNativeMethods.CryptoApiBlob algorithmParams, CapiNativeMethods.CryptBitBlob publicKey)
			{
				this.algorithm.ObjectId = objectId;
				this.algorithm.Parameters = algorithmParams;
				this.publicKey = publicKey;
			}

			public static byte[] Encode(X509Certificate2 certificate)
			{
				byte[] array;
				using (SafeHGlobalHandle safeHGlobalHandle = NativeMethods.AllocHGlobal(certificate.PublicKey.EncodedParameters.RawData.Length))
				{
					Marshal.Copy(certificate.PublicKey.EncodedParameters.RawData, 0, safeHGlobalHandle.DangerousGetHandle(), certificate.PublicKey.EncodedParameters.RawData.Length);
					CapiNativeMethods.CryptoApiBlob algorithmParams = new CapiNativeMethods.CryptoApiBlob((uint)certificate.PublicKey.EncodedParameters.RawData.Length, safeHGlobalHandle);
					using (SafeHGlobalHandle safeHGlobalHandle2 = NativeMethods.AllocHGlobal(certificate.PublicKey.EncodedKeyValue.RawData.Length))
					{
						Marshal.Copy(certificate.PublicKey.EncodedKeyValue.RawData, 0, safeHGlobalHandle2.DangerousGetHandle(), certificate.PublicKey.EncodedKeyValue.RawData.Length);
						CapiNativeMethods.CryptBitBlob cryptBitBlob = new CapiNativeMethods.CryptBitBlob((uint)certificate.PublicKey.EncodedKeyValue.RawData.Length, safeHGlobalHandle2.DangerousGetHandle());
						CapiNativeMethods.CertPublicKeyInfo certPublicKeyInfo = new CapiNativeMethods.CertPublicKeyInfo(certificate.PublicKey.Oid.Value, algorithmParams, cryptBitBlob);
						uint num = 0U;
						if (!CapiNativeMethods.CryptEncodeObject(CapiNativeMethods.EncodeType.X509Asn | CapiNativeMethods.EncodeType.Pkcs7Asn, CapiNativeMethods.x509PublicKeyInfo, ref certPublicKeyInfo, null, ref num))
						{
							throw new CryptographicException(Marshal.GetLastWin32Error());
						}
						array = new byte[num];
						if (!CapiNativeMethods.CryptEncodeObject(CapiNativeMethods.EncodeType.X509Asn | CapiNativeMethods.EncodeType.Pkcs7Asn, CapiNativeMethods.x509PublicKeyInfo, ref certPublicKeyInfo, array, ref num))
						{
							throw new CryptographicException(Marshal.GetLastWin32Error());
						}
					}
				}
				return array;
			}

			private CapiNativeMethods.CryptAlgorithmIdentifier algorithm;

			private CapiNativeMethods.CryptBitBlob publicKey;
		}

		public struct CertInfo
		{
			public static CapiNativeMethods.CertInfo Create(X509Certificate2 certificate)
			{
				return (CapiNativeMethods.CertInfo)Marshal.PtrToStructure(((CapiNativeMethods.CertContext)Marshal.PtrToStructure(certificate.Handle, typeof(CapiNativeMethods.CertContext))).CertInfo, typeof(CapiNativeMethods.CertInfo));
			}

			public byte[] GetSerialNumberRawData()
			{
				byte[] array = new byte[this.serialNumber.Count];
				Marshal.Copy(this.serialNumber.DataPointer, array, 0, (int)this.serialNumber.Count);
				return array;
			}

			private uint version;

			private CapiNativeMethods.CryptoApiBlob serialNumber;

			private CapiNativeMethods.CryptAlgorithmIdentifier signatureAlgorithm;

			private CapiNativeMethods.CryptoApiBlob issuer;

			private System.Runtime.InteropServices.ComTypes.FILETIME notBefore;

			private System.Runtime.InteropServices.ComTypes.FILETIME notAfter;

			private CapiNativeMethods.CryptoApiBlob subject;

			private CapiNativeMethods.CertPublicKeyInfo subjectPublicKeyInfo;

			private CapiNativeMethods.CryptBitBlob issuerUniqueId;

			private CapiNativeMethods.CryptBitBlob subjectUniqueId;

			private uint cExtension;

			private CapiNativeMethods.CertExtension rgExtension;
		}

		internal struct CertRdnAttribute
		{
			[MarshalAs(UnmanagedType.LPStr)]
			public string OID;

			public int dwValueType;

			public CapiNativeMethods.CryptoApiBlob value;
		}

		[Flags]
		public enum DPAPIFlags : uint
		{
			CRYPTPROTECT_UI_FORBIDDEN = 1U,
			CRYPTPROTECT_LOCAL_MACHINE = 4U
		}
	}
}
