using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning
{
	internal class NativeMethods
	{
		internal NativeMethods()
		{
			NativeMethods.Initialize();
		}

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Ansi, SetLastError = true, ThrowOnUnmappableChar = true)]
		internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] [In] string dllname);

		[DllImport("kernel32.dll", BestFitMapping = false, CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true, ThrowOnUnmappableChar = true)]
		internal static extern IntPtr GetProcAddress([In] IntPtr hModule, [MarshalAs(UnmanagedType.LPStr)] [In] string procname);

		internal virtual int CloseIdentityHandle([In] IntPtr hIdentity)
		{
			return NativeMethods.closeIdentityHandleFuncPtr(hIdentity);
		}

		internal virtual int LogonIdentityExSSO([In] IntPtr hIdentity, [MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string authPolicy, [In] uint dwAuthFlags, [In] uint dwSSOFlags, [In] [Out] [Optional] NativeMethods.PCUIParam2 pcUIParam2, [MarshalAs(UnmanagedType.LPArray)] [In] NativeMethods.RstParams[] pcRSTParams, [In] uint dwpcRSTParamsCount)
		{
			return NativeMethods.logonIdentityExSSOFuncPtr(hIdentity, authPolicy, dwAuthFlags, dwSSOFlags, pcUIParam2, pcRSTParams, dwpcRSTParamsCount);
		}

		internal virtual int LogonIdentityEx([In] IntPtr hIdentity, [MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string authPolicy, [In] uint dwAuthFlags, [MarshalAs(UnmanagedType.LPArray)] [In] NativeMethods.RstParams[] pcRSTParams, [In] uint dwpcRSTParamsCount)
		{
			return NativeMethods.logonIdentityExFuncPtr(hIdentity, authPolicy, dwAuthFlags, pcRSTParams, dwpcRSTParamsCount);
		}

		internal virtual int GetAuthenticationStatus([In] IntPtr hIdentity, [MarshalAs(UnmanagedType.LPWStr)] [In] string wzServiceTarget, [In] uint dwVersion, out IntPtr ppStatus)
		{
			return NativeMethods.getAuthenticationStatusFuncPtr(hIdentity, wzServiceTarget, dwVersion, out ppStatus);
		}

		internal virtual int PassportFreeMemory([In] [Out] IntPtr pMemoryToFree)
		{
			return NativeMethods.passportFreeMemoryFuncPtr(pMemoryToFree);
		}

		internal virtual int AuthIdentityToService([In] IntPtr hIdentity, [MarshalAs(UnmanagedType.LPWStr)] [In] string szServiceTarget, [MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string szServicePolicy, [In] uint dwTokenRequestFlags, out IntPtr szToken, out uint pdwResultFlags, out IntPtr ppbSessionKey, out uint pcbSessionKeyLength)
		{
			return NativeMethods.authIdentityToServiceFuncPtr(hIdentity, szServiceTarget, szServicePolicy, dwTokenRequestFlags, out szToken, out pdwResultFlags, out ppbSessionKey, out pcbSessionKeyLength);
		}

		internal virtual int SetCredential([In] IntPtr hIdentity, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszCredType, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszCredValue)
		{
			return NativeMethods.setCredentialFuncPtr(hIdentity, wszCredType, wszCredValue);
		}

		internal virtual int CreateIdentityHandle2([MarshalAs(UnmanagedType.LPWStr)] [In] string wszFederationProvider, [MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string wszMemberName, [In] uint dwFlags, out IntPtr pihIdentity)
		{
			return NativeMethods.createIdentityHandle2FuncPtr(wszFederationProvider, wszMemberName, dwFlags, out pihIdentity);
		}

		internal virtual int InitializeEx([In] ref Guid guid, [In] int lPPCRLVersion, [In] uint dwFlags, [MarshalAs(UnmanagedType.LPArray)] [In] NativeMethods.IdcrlOption[] pOptions, [In] uint dwOptions)
		{
			return NativeMethods.initializeExFuncPtr(ref guid, lPPCRLVersion, dwFlags, pOptions, dwOptions);
		}

		internal virtual int EnumIdentitiesWithCachedCredentials([MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string wszCachedCredType, out IntPtr peihEnumHandle)
		{
			return NativeMethods.enumIdentitiesWithCachedCredentialsFuncPtr(wszCachedCredType, out peihEnumHandle);
		}

		internal virtual int NextIdentity(IntPtr hEnumHandle, ref string wszMemberName)
		{
			IntPtr ptr = 0;
			int result = NativeMethods.nextIdentityFuncPtr(hEnumHandle, ref ptr);
			wszMemberName = null;
			wszMemberName = Marshal.PtrToStringUni(ptr);
			return result;
		}

		internal virtual int CloseEnumIdentitiesHandle(IntPtr hEnumHandle)
		{
			return NativeMethods.closeEnumIdentitiesHandleFuncPtr(hEnumHandle);
		}

		internal virtual int Uninitialize()
		{
			return NativeMethods.uninitializeFuncPtr();
		}

		private static string GetIdentityCrlDllPath()
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\Microsoft\\MSOIdentityCRL");
			string path = (string)registryKey.GetValue("TargetDir");
			return Path.Combine(path, "msoidcli.dll");
		}

		private static Delegate GetFunctionPointer(IntPtr msoidcli, string methodName, Type delegateType)
		{
			IntPtr procAddress = NativeMethods.GetProcAddress(msoidcli, methodName);
			if (procAddress == IntPtr.Zero)
			{
				int lastWin32Error = Marshal.GetLastWin32Error();
				throw new Exception(string.Format(CultureInfo.InvariantCulture, "Failed to get address for method: {0} from library: {1}. GetLastError code: {2}", new object[]
				{
					methodName,
					NativeMethods.identityCrlDllPath,
					lastWin32Error
				}));
			}
			return Marshal.GetDelegateForFunctionPointer(procAddress, delegateType);
		}

		private static void Initialize()
		{
			if (NativeMethods.initialized)
			{
				return;
			}
			lock (NativeMethods.syncObject)
			{
				if (!NativeMethods.initialized)
				{
					NativeMethods.identityCrlDllPath = NativeMethods.GetIdentityCrlDllPath();
					IntPtr intPtr = NativeMethods.LoadLibrary(NativeMethods.identityCrlDllPath);
					if (intPtr == IntPtr.Zero)
					{
						int lastWin32Error = Marshal.GetLastWin32Error();
						throw new Exception(string.Format(CultureInfo.InvariantCulture, "Failed to load library: {0}. GetLastError code: {1}", new object[]
						{
							NativeMethods.identityCrlDllPath,
							lastWin32Error
						}));
					}
					NativeMethods.closeIdentityHandleFuncPtr = (NativeMethods._CloseIdentityHandle)NativeMethods.GetFunctionPointer(intPtr, "CloseIdentityHandle", typeof(NativeMethods._CloseIdentityHandle));
					NativeMethods.logonIdentityExSSOFuncPtr = (NativeMethods._LogonIdentityExSSO)NativeMethods.GetFunctionPointer(intPtr, "LogonIdentityExSSO", typeof(NativeMethods._LogonIdentityExSSO));
					NativeMethods.logonIdentityExFuncPtr = (NativeMethods._LogonIdentityEx)NativeMethods.GetFunctionPointer(intPtr, "LogonIdentityEx", typeof(NativeMethods._LogonIdentityEx));
					NativeMethods.getAuthenticationStatusFuncPtr = (NativeMethods._GetAuthenticationStatus)NativeMethods.GetFunctionPointer(intPtr, "GetAuthenticationStatus", typeof(NativeMethods._GetAuthenticationStatus));
					NativeMethods.passportFreeMemoryFuncPtr = (NativeMethods._PassportFreeMemory)NativeMethods.GetFunctionPointer(intPtr, "PassportFreeMemory", typeof(NativeMethods._PassportFreeMemory));
					NativeMethods.authIdentityToServiceFuncPtr = (NativeMethods._AuthIdentityToService)NativeMethods.GetFunctionPointer(intPtr, "AuthIdentityToService", typeof(NativeMethods._AuthIdentityToService));
					NativeMethods.setCredentialFuncPtr = (NativeMethods._SetCredential)NativeMethods.GetFunctionPointer(intPtr, "SetCredential", typeof(NativeMethods._SetCredential));
					NativeMethods.createIdentityHandle2FuncPtr = (NativeMethods._CreateIdentityHandle2)NativeMethods.GetFunctionPointer(intPtr, "CreateIdentityHandle2", typeof(NativeMethods._CreateIdentityHandle2));
					NativeMethods.initializeExFuncPtr = (NativeMethods._InitializeEx)NativeMethods.GetFunctionPointer(intPtr, "InitializeEx", typeof(NativeMethods._InitializeEx));
					NativeMethods.uninitializeFuncPtr = (NativeMethods._Uninitialize)NativeMethods.GetFunctionPointer(intPtr, "Uninitialize", typeof(NativeMethods._Uninitialize));
					NativeMethods.enumIdentitiesWithCachedCredentialsFuncPtr = (NativeMethods._EnumIdentitiesWithCachedCredentials)NativeMethods.GetFunctionPointer(intPtr, "EnumIdentitiesWithCachedCredentials", typeof(NativeMethods._EnumIdentitiesWithCachedCredentials));
					NativeMethods.nextIdentityFuncPtr = (NativeMethods._NextIdentity)NativeMethods.GetFunctionPointer(intPtr, "NextIdentity", typeof(NativeMethods._NextIdentity));
					NativeMethods.closeEnumIdentitiesHandleFuncPtr = (NativeMethods._CloseEnumIdentitiesHandle)NativeMethods.GetFunctionPointer(intPtr, "CloseEnumIdentitiesHandle", typeof(NativeMethods._CloseEnumIdentitiesHandle));
					NativeMethods.initialized = true;
				}
			}
		}

		private const string IdentityCrlRegistrySubKey = "Software\\Microsoft\\MSOIdentityCRL";

		private const string IdentityCrlInstallPathRegKeyName = "TargetDir";

		private const string IdentityCrlDllToLoadName = "msoidcli.dll";

		private static bool initialized;

		private static object syncObject = new object();

		private static string identityCrlDllPath = string.Empty;

		private static NativeMethods._CloseIdentityHandle closeIdentityHandleFuncPtr;

		private static NativeMethods._LogonIdentityExSSO logonIdentityExSSOFuncPtr;

		private static NativeMethods._LogonIdentityEx logonIdentityExFuncPtr;

		private static NativeMethods._GetAuthenticationStatus getAuthenticationStatusFuncPtr;

		private static NativeMethods._PassportFreeMemory passportFreeMemoryFuncPtr;

		private static NativeMethods._AuthIdentityToService authIdentityToServiceFuncPtr;

		private static NativeMethods._SetCredential setCredentialFuncPtr;

		private static NativeMethods._CreateIdentityHandle2 createIdentityHandle2FuncPtr;

		private static NativeMethods._InitializeEx initializeExFuncPtr;

		private static NativeMethods._Uninitialize uninitializeFuncPtr;

		private static NativeMethods._EnumIdentitiesWithCachedCredentials enumIdentitiesWithCachedCredentialsFuncPtr;

		private static NativeMethods._NextIdentity nextIdentityFuncPtr;

		private static NativeMethods._CloseEnumIdentitiesHandle closeEnumIdentitiesHandleFuncPtr;

		private delegate int _CloseIdentityHandle([In] IntPtr hIdentity);

		private delegate int _LogonIdentityExSSO([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string authPolicy, [In] uint dwAuthFlags, [In] uint dwSsoFlags, [In] [Out] [Optional] NativeMethods.PCUIParam2 pcUIParam2, [MarshalAs(UnmanagedType.LPArray)] [In] NativeMethods.RstParams[] pcRSTParams, [In] uint dwpcRSTParamsCount);

		private delegate int _LogonIdentityEx([In] IntPtr Identity, [MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string authPolicy, [In] uint dwAuthFlags, [MarshalAs(UnmanagedType.LPArray)] [In] NativeMethods.RstParams[] pcRSTParams, [In] uint dwpcRSTParamsCount);

		private delegate int _GetAuthenticationStatus([In] IntPtr hIdentity, [MarshalAs(UnmanagedType.LPWStr)] [In] string wzServiceTarget, [In] uint dwVersion, out IntPtr ppStatus);

		private delegate int _PassportFreeMemory([In] [Out] IntPtr pMemoryToFree);

		private delegate int _AuthIdentityToService([In] IntPtr hIdentity, [MarshalAs(UnmanagedType.LPWStr)] [In] string szServiceTarget, [MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string szServicePolicy, [In] uint dwTokenRequestFlags, out IntPtr szToken, out uint pdwResultFlags, out IntPtr ppbSessionKey, out uint pcbSessionKeyLength);

		private delegate int _CreateIdentityHandle2([MarshalAs(UnmanagedType.LPWStr)] [In] string wszFederationProvider, [MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string wszMemberName, [In] uint dwFlags, out IntPtr pihIdentity);

		private delegate int _SetCredential([In] IntPtr hIdentity, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszCredType, [MarshalAs(UnmanagedType.LPWStr)] [In] string wszCredValue);

		private delegate int _InitializeEx([In] ref Guid guid, [In] int lPPCRLVersion, [In] uint dwFlags, [MarshalAs(UnmanagedType.LPArray)] [In] NativeMethods.IdcrlOption[] pOptions, [In] uint dwOptions);

		private delegate int _Uninitialize();

		private delegate int _EnumIdentitiesWithCachedCredentials([MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string wszCachedCredType, out IntPtr peihEnumHandle);

		private delegate int _NextIdentity(IntPtr hEnumHandle, ref IntPtr wszMemberName);

		private delegate int _CloseEnumIdentitiesHandle(IntPtr hEnumHandle);

		internal enum LogOnFlag
		{
			LogOnIdentityAllBit = 511,
			LogOnIdentityDefault = 0,
			LogOnIdentityAllowOffline,
			LogOnIdentityForceOffline,
			LogOnIdentityCreateOfflineHash = 4,
			LogOnIdentityAllowPersistentCookies = 8,
			LogOnIdentityUseEasyIdAuth = 16,
			LogOnIdentityUseLinkedAccounts = 32,
			LogOnIdentityFederated = 64,
			LogOnIdentityWindowsLiveId = 128,
			LogOnIdentityAutoPartnerRedirect = 256
		}

		internal enum SsoFlag
		{
			SsoAllBit = 15,
			SsoDefault = 0,
			SsoNoUi,
			SsoNoAutoSignIn,
			SsoNoHandleError = 4
		}

		internal enum SsoGroup
		{
			SsoGroupNone,
			SsoGroupLive = 16,
			SsoGroupEnterprise = 32
		}

		internal enum UpdateFlag : uint
		{
			UpdateFlagAllBit = 15U,
			DefaultUpdatePolicy = 0U,
			OfflineModeAllowed,
			NoUI,
			SkipConnectionCheck = 4U,
			SetExtendedError = 8U,
			SendVersion = 16U,
			UpdateDefault = 0U
		}

		internal enum ServiceTokenFlags : uint
		{
			ServiceTokenLegacyPassport = 1U,
			ServiceTokenWebSso,
			ServiceTokenCompactWebSso = 4U,
			ServiceTokenAny = 255U,
			ServiceTokenFromCache = 65536U,
			ServiceTokenIgnoreCache = 131072U,
			ServiceTokenX509v3 = 8U,
			ServiceTokenCertInMemoryPrivateKey = 16U,
			ServiceTokenRequestTypeNone = 0U,
			ServiceTokenTypeProprietary,
			ServiceTokenTypeSaml
		}

		internal enum IdentityFlag : uint
		{
			IdentityAllBit = 1023U,
			IdentityShareAll = 255U,
			IdentityLoadFromPersistedStore,
			IdentityAuthStateEncrypted = 512U
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct IdcrlOption
		{
			public int EnvironmentId;

			public IntPtr EnvironmentValue;

			public uint EnvironmentLength;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct RstParams
		{
			internal int CbSize;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string ServiceName;

			[MarshalAs(UnmanagedType.LPWStr)]
			internal string ServicePolicy;

			internal int TokenFlags;

			internal int TokenParams;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class IdcrlStatusCurrent
		{
			internal int AuthState { get; set; }

			internal int AuthRequired { get; set; }

			internal int RequestStatus { get; set; }

			internal int UserInterfaceError { get; set; }

			internal string WebFlowUrl { get; set; }
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal class PCUIParam2
		{
		}
	}
}
