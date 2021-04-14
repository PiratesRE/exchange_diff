using System;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security;
using Microsoft.Exchange.Security.Cryptography.X509Certificates;

namespace Microsoft.Exchange.Net.ExSmtpClient
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NativeMethods
	{
		[DllImport("secur32.dll", CharSet = CharSet.Unicode)]
		internal static extern int QuerySecurityPackageInfo(string pszPackageName, ref IntPtr ppPackageInfo);

		[DllImport("secur32.dll", BestFitMapping = false, CharSet = CharSet.Ansi, EntryPoint = "AcquireCredentialsHandleA", ExactSpelling = true, SetLastError = true, ThrowOnUnmappableChar = true)]
		internal static extern int AcquireCredentialsHandle([In] string pszPrincipal, [In] string pszPackage, [In] int fCredentialUse, [In] IntPtr pvLogonID, [In] ref NativeMethods.AuthData pAuthData, [In] IntPtr pGetKeyFn, [In] IntPtr pvGetKeyArgument, ref NativeMethods.CredHandle phCredential, out long ptsExpiry);

		[DllImport("secur32.dll", BestFitMapping = false, CharSet = CharSet.Ansi, EntryPoint = "AcquireCredentialsHandleA", ExactSpelling = true, SetLastError = true, ThrowOnUnmappableChar = true)]
		internal static extern int AcquireCredentialsHandle([In] string pszPrincipal, [In] string pszPackage, [In] int fCredentialUse, [In] IntPtr pvLogonID, [In] IntPtr pAuthData, [In] IntPtr pGetKeyFn, [In] IntPtr pvGetKeyArgument, ref NativeMethods.CredHandle phCredential, out long ptsExpiry);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode, EntryPoint = "AcquireCredentialsHandleW")]
		internal static extern int AcquireCredentialsHandleForSchannel([In] string pszPrincipal, [In] string pszPackage, [In] int fCredentialUse, [In] IntPtr pvLogonID, [In] ref NativeMethods.AuthDataForSchannel pAuthData, [In] IntPtr pGetKeyFn, [In] IntPtr pvGetKeyArgument, ref NativeMethods.CredHandle phCredential, out long ptsExpiry);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode)]
		internal static extern int FreeCredentialsHandle(ref NativeMethods.CredHandle phCredential);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode)]
		internal static extern int FreeContextBuffer(IntPtr pvContextBuffer);

		[DllImport("secur32.dll", CharSet = CharSet.Ansi)]
		internal static extern int InitializeSecurityContext(ref NativeMethods.CredHandle phCredential, [In] IntPtr phContext, [In] string pszTargetName, [In] int fContextReq, [In] int Reserved1, [In] NativeMethods.Endianness TargetDataRep, [In] IntPtr pInput, [In] int Reserved2, ref NativeMethods.CredHandle phNewContext, [In] [Out] NativeMethods.SecBufferDesc pOutput, [In] [Out] ref int pfContextAttr, out long ptsExpiry);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode, EntryPoint = "QueryContextAttributesW", SetLastError = true)]
		internal static extern int QueryContextAttributes(ref NativeMethods.CredHandle phContext, NativeMethods.ContextAttribute ulAttribute, [Out] byte[] buffer);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode, EntryPoint = "QueryContextAttributesW", SetLastError = true)]
		internal static extern int QueryContextAttributes(ref NativeMethods.CredHandle phContext, NativeMethods.ContextAttribute ulAttribute, out SafeCertContextHandle certContext);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode, EntryPoint = "QueryContextAttributesW", SetLastError = true)]
		internal static extern int QueryContextAttributes(ref NativeMethods.CredHandle phContext, NativeMethods.ContextAttribute ulAttribute, [In] [Out] ref NativeMethods.SessionKey sessionkey);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode, EntryPoint = "QueryContextAttributesW", SetLastError = true)]
		internal static extern int QueryContextAttributes(ref NativeMethods.CredHandle phContext, NativeMethods.ContextAttribute ulAttribute, [In] [Out] ref NativeMethods.EapKeyBlock keyBlock);

		[DllImport("secur32.dll", CharSet = CharSet.Ansi, EntryPoint = "InitializeSecurityContext")]
		internal static extern int InitializeSecurityContext_SecondSspiBlob(ref NativeMethods.CredHandle phCredential, [In] ref NativeMethods.CredHandle phContext, [In] string pszTargetName, [In] int fContextReq, [In] int Reserved1, [In] NativeMethods.Endianness TargetDataRep, [In] NativeMethods.SecBufferDesc pInput, [In] int Reserved2, ref NativeMethods.CredHandle phNewContext, [In] [Out] NativeMethods.SecBufferDesc pOutput, [In] [Out] ref int pfContextAttr, out long ptsExpiry);

		[DllImport("secur32.dll", CharSet = CharSet.Ansi, EntryPoint = "InitializeSecurityContext")]
		internal static extern int InitializeSecurityContext_NextSslBlob(ref NativeMethods.CredHandle phCredential, [In] ref NativeMethods.CredHandle phContext, [In] string pszTargetName, [In] int fContextReq, [In] int Reserved1, [In] NativeMethods.Endianness TargetDataRep, [In] NativeMethods.SecBufferDesc pInput, [In] int Reserved2, ref NativeMethods.CredHandle phNewContext, [In] [Out] NativeMethods.SecBufferDesc pOutput, [In] [Out] ref int pfContextAttr, out long ptsExpiry);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode)]
		internal static extern int CompleteAuthToken(NativeMethods.CredHandle phContext, NativeMethods.SecBufferDesc pToken);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode)]
		internal static extern int DeleteSecurityContext(ref NativeMethods.CredHandle phContext);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode)]
		internal static extern int EncryptMessage([In] ref NativeMethods.CredHandle phContext, [In] uint fQOP, [In] [Out] NativeMethods.SecBufferDesc pMessage, [In] uint MessageSeqNo);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode)]
		internal static extern int DecryptMessage([In] ref NativeMethods.CredHandle phContext, [In] [Out] NativeMethods.SecBufferDesc pMessage, [In] uint MessageSeqNo, out uint pfQOP);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode)]
		internal static extern int AcceptSecurityContext(ref NativeMethods.CredHandle phCredential, [In] IntPtr phContext, [In] NativeMethods.SecBufferDesc pInput, [In] NativeMethods.ContextFlags fContextReq, [In] NativeMethods.Endianness TargetDataRep, ref NativeMethods.CredHandle phNewContext, [In] [Out] NativeMethods.SecBufferDesc pOutput, [In] [Out] ref NativeMethods.ContextFlags pfContextAttr, out long ptsTimeStamp);

		[DllImport("secur32.dll", CharSet = CharSet.Unicode)]
		internal static extern int AcceptSecurityContext(ref NativeMethods.CredHandle phCredential, [In] ref NativeMethods.CredHandle phContext, [In] NativeMethods.SecBufferDesc pInput, [In] NativeMethods.ContextFlags fContextReq, [In] NativeMethods.Endianness TargetDataRep, ref NativeMethods.CredHandle phNewContext, [In] [Out] NativeMethods.SecBufferDesc pOutput, [In] [Out] ref NativeMethods.ContextFlags pfContextAttr, out long ptsTimeStamp);

		[DllImport("Crypt32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int CertFreeCertificateContext(IntPtr pcertContext);

		internal const int SECPKG_CRED_OUTBOUND = 2;

		internal const int SECPKG_CRED_INBOUND = 1;

		internal const int SEC_WINNT_AUTH_IDENTITY_UNICODE = 2;

		internal const int SEC_I_CONTINUE_NEEDED = 590610;

		internal const int SEC_E_WRONG_PRINCIPAL = 590626;

		internal const string SEC_STATUS_LOGON_DENIED = "0x8009030c";

		internal const string SEC_E_INCOMPLETE_MESSAGE = "0x80090318";

		internal const int SECBUFFER_VERSION = 0;

		internal const int SECBUFFER_EMPTY = 0;

		internal const int SECBUFFER_DATA = 1;

		internal const int SECBUFFER_TOKEN = 2;

		internal const int SECBUFFER_PKG_PARAMS = 3;

		internal const int SECBUFFER_MISSING = 4;

		internal const int SECBUFFER_EXTRA = 5;

		internal const int SECBUFFER_STREAM_TRAILER = 6;

		internal const int SECBUFFER_STREAM_HEADER = 7;

		internal const int SEC_E_OK = 0;

		internal const string UNISP_NAME = "Microsoft Unified Security Protocol Provider";

		[Flags]
		internal enum ContextFlags
		{
			Zero = 0,
			Delegate = 1,
			MutualAuth = 2,
			ReplayDetect = 4,
			SequenceDetect = 8,
			Confidentiality = 16,
			UseSessionKey = 32,
			AllocateMemory = 256,
			Connection = 2048,
			InitExtendedError = 16384,
			AcceptExtendedError = 32768,
			InitStream = 32768,
			AcceptStream = 65536,
			InitIntegrity = 65536,
			AcceptIntegrity = 131072,
			InitManualCredValidation = 524288,
			InitUseSuppliedCreds = 128,
			InitIdentify = 131072,
			AcceptIdentify = 524288,
			InitHttp = 268435456,
			AcceptHttp = 268435456
		}

		internal enum ContextAttribute
		{
			Sizes,
			Names,
			Lifespan,
			DceInfo,
			StreamSizes,
			KeyInfo,
			Authority,
			SECPKG_ATTR_PROTO_INFO,
			SECPKG_ATTR_PASSWORD_EXPIRY,
			SECPKG_ATTR_SESSION_KEY,
			PackageInfo,
			SECPKG_ATTR_USER_FLAGS,
			NegotiationInfo,
			SECPKG_ATTR_NATIVE_NAMES,
			SECPKG_ATTR_FLAGS,
			SECPKG_ATTR_USE_VALIDATED,
			SECPKG_ATTR_CREDENTIAL_NAME,
			SECPKG_ATTR_TARGET_INFORMATION,
			SECPKG_ATTR_ACCESS_TOKEN,
			SECPKG_ATTR_TARGET,
			SECPKG_ATTR_AUTHENTICATION_ID,
			RemoteCertificate = 83,
			LocalCertificate,
			RootStore,
			IssuerListInfoEx = 89,
			ConnectionInfo,
			EapKeyBlock
		}

		internal enum Endianness
		{
			Network,
			Native = 16
		}

		public struct SEC_STATUS
		{
			public SEC_STATUS(int intValue)
			{
				this.intValue = intValue;
			}

			public static implicit operator NativeMethods.SEC_STATUS(int intValue)
			{
				return new NativeMethods.SEC_STATUS(intValue);
			}

			public static implicit operator int(NativeMethods.SEC_STATUS secStatus)
			{
				return secStatus.intValue;
			}

			public static implicit operator string(NativeMethods.SEC_STATUS secStatus)
			{
				return "0x" + secStatus.intValue.ToString("x", CultureInfo.InvariantCulture);
			}

			public override string ToString()
			{
				return "0x" + this.intValue.ToString("x", CultureInfo.InvariantCulture);
			}

			private int intValue;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct SecurityPackageInfo
		{
			public SecurityPackageInfo(IntPtr buffer)
			{
				NativeMethods.SecurityPackageInfo securityPackageInfo = (NativeMethods.SecurityPackageInfo)Marshal.PtrToStructure(buffer, typeof(NativeMethods.SecurityPackageInfo));
				this.fCapabilities = securityPackageInfo.fCapabilities;
				this.wVersion = securityPackageInfo.wVersion;
				this.wRPCID = securityPackageInfo.wRPCID;
				this.cbMaxToken = securityPackageInfo.cbMaxToken;
				this.Name = securityPackageInfo.Name;
				this.Comment = securityPackageInfo.Comment;
			}

			internal NativeMethods.SecurityPackageInfo.cap fCapabilities;

			internal short wVersion;

			internal short wRPCID;

			internal int cbMaxToken;

			internal string Name;

			internal string Comment;

			[Flags]
			internal enum cap
			{
				Integrity = 1,
				Privacy = 2,
				TokenOnly = 4,
				Datagram = 8,
				Connection = 16,
				MultiLegRequired = 32,
				ClientOnly = 64,
				ExtendedError = 128,
				Impersonation = 256,
				AcceptsWin32Names = 512,
				Stream = 1024,
				Negotiable = 2048,
				GssCompatible = 4096,
				Logon = 8192,
				AsciiBuffers = 16384,
				Fragment = 32768,
				MutualAuth = 65536,
				Delegation = 131072,
				ReadonlyWithChecksum = 262144
			}
		}

		internal struct SecurityPackageContextStreamSizes
		{
			internal unsafe SecurityPackageContextStreamSizes(byte[] memory)
			{
				fixed (IntPtr* ptr = memory)
				{
					IntPtr ptr2 = new IntPtr((void*)ptr);
					checked
					{
						this.cbHeader = (int)((uint)Marshal.ReadInt32(ptr2));
						this.cbTrailer = (int)((uint)Marshal.ReadInt32(ptr2, 4));
						this.cbMaximumMessage = (int)((uint)Marshal.ReadInt32(ptr2, 8));
						this.cBuffers = (int)((uint)Marshal.ReadInt32(ptr2, 12));
						this.cbBlockSize = (int)((uint)Marshal.ReadInt32(ptr2, 16));
					}
				}
			}

			public static readonly int SizeOf = Marshal.SizeOf(typeof(NativeMethods.SecurityPackageContextStreamSizes));

			public int cbHeader;

			public int cbTrailer;

			public int cbMaximumMessage;

			public int cBuffers;

			public int cbBlockSize;
		}

		internal struct EapKeyBlock
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
			public byte[] rgbKeys;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
			public byte[] rgbIVs;
		}

		internal struct SessionKey
		{
			public int SessionKeyLength;

			public IntPtr SessionKeyValuePtr;
		}

		internal struct CRYPT_INTEGER_BLOB
		{
			public uint cbData;

			public IntPtr pbData;
		}

		internal struct CRYPT_OBJID_BLOB
		{
			public uint cbData;

			public IntPtr pbData;
		}

		internal struct CERT_NAME_BLOB
		{
			public uint cbData;

			public IntPtr pbData;
		}

		internal struct CRYPT_ALGORITHM_IDENTIFIER
		{
			public string pszObjId;

			public NativeMethods.CRYPT_OBJID_BLOB Parameters;
		}

		internal struct CRYPT_BIT_BLOB
		{
			public uint cbData;

			public IntPtr pbData;

			public uint cUnusedBits;
		}

		internal struct FILETIME
		{
			public uint dwLowDateTime;

			public uint dwHighDateTime;
		}

		internal struct CERT_PUBLIC_KEY_INFO
		{
			public NativeMethods.CRYPT_ALGORITHM_IDENTIFIER Algorithm;

			public NativeMethods.CRYPT_BIT_BLOB PublicKey;
		}

		internal struct CertInfo
		{
			public uint dwVersion;

			public NativeMethods.CRYPT_INTEGER_BLOB SerialNumber;

			public NativeMethods.CRYPT_ALGORITHM_IDENTIFIER SignatureAlgorithm;

			public NativeMethods.CERT_NAME_BLOB Issuer;

			public NativeMethods.FILETIME NotBefore;

			public NativeMethods.FILETIME NotAfter;

			public NativeMethods.CERT_NAME_BLOB Subject;

			public NativeMethods.CERT_PUBLIC_KEY_INFO SubjectPublicKeyInfo;

			public NativeMethods.CRYPT_BIT_BLOB IssuerUniqueId;

			public NativeMethods.CRYPT_BIT_BLOB SubjectUniqueId;

			public uint cExtension;

			public IntPtr rgExtension;
		}

		internal struct CertContext
		{
			public uint dwCertEncodingType;

			public IntPtr pbCertEncoded;

			public uint cbCertEncoded;

			public IntPtr pCertInfo;

			public IntPtr hCertStore;
		}

		[StructLayout(LayoutKind.Sequential, Pack = 1)]
		internal struct CredHandle
		{
			internal bool IsZero
			{
				get
				{
					return this.HandleHi == IntPtr.Zero && this.HandleLo == IntPtr.Zero;
				}
			}

			public override string ToString()
			{
				return this.HandleHi.ToString("x") + ":" + this.HandleLo.ToString("x");
			}

			internal void SetToInvalid()
			{
				this.HandleHi = IntPtr.Zero;
				this.HandleLo = IntPtr.Zero;
			}

			internal IntPtr HandleHi;

			internal IntPtr HandleLo;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct SecBuffer
		{
			public int cbBuffer;

			public int BufferType;

			public IntPtr buffer;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct AuthData
		{
			public string User;

			public int UserLength;

			public string Domain;

			public int DomainLength;

			public string Password;

			public int PasswordLength;

			public int Flags;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct AuthDataForSchannel
		{
			public static NativeMethods.AuthDataForSchannel GetAuthDataForSchannel()
			{
				return new NativeMethods.AuthDataForSchannel
				{
					dwVersion = 4,
					cCreds = 0,
					paCred = IntPtr.Zero,
					hRootStore = IntPtr.Zero,
					cMappers = 0,
					aphMappers = IntPtr.Zero,
					cSupportedAlgs = 0,
					palgSupportedAlgs = IntPtr.Zero,
					grbitEnabledProtocols = SchannelProtocols.Zero,
					dwMinimumCipherStrength = 0,
					dwMaximumCipherStrength = 0,
					dwSessionLifespan = 0,
					dwFlags = 24,
					reserved = 0
				};
			}

			public const int CurrentVersion = 4;

			public int dwVersion;

			public int cCreds;

			public IntPtr paCred;

			public IntPtr hRootStore;

			public int cMappers;

			public IntPtr aphMappers;

			public int cSupportedAlgs;

			public IntPtr palgSupportedAlgs;

			public SchannelProtocols grbitEnabledProtocols;

			public int dwMinimumCipherStrength;

			public int dwMaximumCipherStrength;

			public int dwSessionLifespan;

			public int dwFlags;

			public int reserved;

			[Flags]
			public enum Flags
			{
				Zero = 0,
				NoSystemMapper = 2,
				NoNameCheck = 4,
				ValidateManual = 8,
				NoDefaultCred = 16,
				ValidateAuto = 32
			}
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal class SecBufferDesc
		{
			public int ulVersion;

			public int cBuffers;

			public IntPtr pBuffer;
		}
	}
}
