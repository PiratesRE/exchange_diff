using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Microsoft.Exchange.Data.Directory
{
	[SuppressUnmanagedCodeSecurity]
	internal static class NativeMethods
	{
		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
		internal static extern int EnumSystemGeoID(int GeoClass, int ParentGeoId, NativeMethods.GeoEnumProc lpGeoEnumProc);

		[DllImport("Kernel32.dll", CharSet = CharSet.Unicode)]
		internal static extern int GetGeoInfo(int GeoId, NativeMethods.SysGeoType GeoType, StringBuilder lpGeoData, int cchData, int language);

		[DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetComputerNameEx(int nameType, StringBuilder buffer, ref uint bufferSize);

		[DllImport("kernel32.dll")]
		internal static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetProcessTimes(IntPtr process, out long creationTime, out long exitTime, out long kernelTime, out long userTime);

		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetSystemTimes(out long idleTime, out long kernelTime, out long userTime);

		[DllImport("kernel32.dll")]
		internal static extern uint GetLastError();

		[DllImport("NETAPI32.DLL", CharSet = CharSet.Unicode)]
		internal static extern int DsRoleGetPrimaryDomainInformation(string server, int infoLevel, out SafeDsRolePrimaryDomainInfoLevelHandle buffer);

		[DllImport("NETAPI32.DLL")]
		internal static extern void DsRoleFreeMemory(IntPtr buffer);

		[DllImport("NETAPI32.DLL", CharSet = CharSet.Unicode)]
		internal static extern int DsGetSiteName(string server, out SafeDsSiteNameHandle siteName);

		[DllImport("NETAPI32.DLL", CharSet = CharSet.Unicode)]
		internal static extern int NetApiBufferFree(IntPtr buffer);

		[DllImport("NETAPI32.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int DsGetDcSiteCoverage([MarshalAs(UnmanagedType.LPTStr)] string ServerName, out long EntryCount, out SafeDsSiteNameHandle SiteNames);

		[DllImport("NtDsapi.dll", CharSet = CharSet.Unicode, EntryPoint = "DsBindW")]
		internal static extern uint DsBind([MarshalAs(UnmanagedType.LPWStr)] string DomainControllerName, [MarshalAs(UnmanagedType.LPWStr)] string DnsDomainName, out SafeDsBindHandle phDS);

		[DllImport("Ntdsapi.dll", EntryPoint = "DsUnBindW", ExactSpelling = true)]
		internal static extern uint DsUnBind(ref IntPtr phDS);

		[DllImport("ntdsapi.dll", CharSet = CharSet.Unicode, EntryPoint = "DsCrackNamesW", ExactSpelling = true, SetLastError = true)]
		internal static extern uint DsCrackNames(SafeDsBindHandle phDS, NativeMethods.DsNameFlags flags, ExtendedNameFormat formatOffered, ExtendedNameFormat formatDesired, uint cNames, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 4)] string[] rpNames, out SafeDsNameResultHandle ppResult);

		[DllImport("ntdsapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern void DsFreeNameResult(IntPtr pResult);

		[DllImport("ntdsapi.dll", CharSet = CharSet.Unicode, EntryPoint = "DsRemoveDsServerW", ExactSpelling = true, SetLastError = true)]
		internal static extern uint DsRemoveDsServer(SafeDsBindHandle phDS, [MarshalAs(UnmanagedType.LPWStr)] string ServerDN, [MarshalAs(UnmanagedType.LPWStr)] string DomainDN, out bool pfLastDCInDomain, bool fCommit);

		[DllImport("ntdsapi.dll", CharSet = CharSet.Unicode, EntryPoint = "DsAddSidHistoryW", ExactSpelling = true, SetLastError = true)]
		internal static extern uint DsAddSidHistory(SafeDsBindHandle phDS, uint flags, [MarshalAs(UnmanagedType.LPWStr)] string srcDomain, [MarshalAs(UnmanagedType.LPWStr)] string srcPrincipal, [MarshalAs(UnmanagedType.LPWStr)] string srcDomainController, IntPtr srcDomainCreds, [MarshalAs(UnmanagedType.LPWStr)] string dstDomain, [MarshalAs(UnmanagedType.LPWStr)] string dstPrincipal);

		[DllImport("ntdsapi.dll", CharSet = CharSet.Unicode, EntryPoint = "DsBindWithSpnExW", ExactSpelling = true, SetLastError = true)]
		internal static extern uint DsBindWithSpnEx([MarshalAs(UnmanagedType.LPWStr)] string DomainControllerName, [MarshalAs(UnmanagedType.LPWStr)] string DnsDomainName, IntPtr AuthIdentity, [MarshalAs(UnmanagedType.LPWStr)] string ServicePrincipalName, uint BindFlags, out SafeDsBindHandle phDS);

		[DllImport("NETAPI32.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "DsGetDcOpenW")]
		internal static extern int DsGetDcOpen([In] string dnsName, [In] int optionFlags, [In] string siteName, [In] IntPtr domainGuid, [In] string dnsForestName, [In] int dcFlags, out SafeDsGetDcContextHandle retGetDcContext);

		[DllImport("NETAPI32.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "DsGetDcNextW")]
		internal static extern int DsGetDcNext([In] SafeDsGetDcContextHandle getDcContextHandle, [In] [Out] ref IntPtr sockAddressCount, out IntPtr sockAdresses, out SafeDnsHostNameHandle dnsHostName);

		[DllImport("NETAPI32.DLL", CallingConvention = CallingConvention.StdCall, CharSet = CharSet.Unicode, EntryPoint = "DsGetDcCloseW")]
		internal static extern void DsGetDcClose([In] IntPtr getDcContextHandle);

		[DllImport("dsaccessperf.dll", CharSet = CharSet.Ansi)]
		internal static extern void DsaccessPerfCounterUpdate(uint objectType, uint counterId, uint modType, uint value, string instanceName);

		[DllImport("dsaccessperf.dll")]
		internal static extern void DsaccessPerfDCPrepareForRefresh();

		[DllImport("dsaccessperf.dll")]
		internal static extern void DsaccessPerfDCFinalizeRefresh();

		[DllImport("dsaccessperf.dll", CharSet = CharSet.Ansi)]
		internal static extern void DsaccessPerfDCAddToList(string serverName);

		[DllImport("dsaccessperf.dll", CharSet = CharSet.Ansi)]
		internal static extern void DsaccessPerfSetProcessName(string processName, string applicationName, bool hasMultiInstance);

		[DllImport("NETAPI32.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
		internal static extern int I_NetLogonControl2([In] string lpServerName, uint lpFunctionCode, uint lpQueryLevel, ref IntPtr lpInputData, out IntPtr queryInformation);

		private const string NetApi32 = "NETAPI32.DLL";

		internal const string DsAccessPerf = "dsaccessperf.dll";

		internal const int ERROR_NO_MORE_ITEMS = 259;

		internal const int ERROR_FILE_MARK_DETECTED = 1101;

		internal const int DNS_ERROR_RCODE_NAME_ERROR = 9003;

		internal static readonly int DsRolePrimaryDomainInfoBasic = 1;

		internal static readonly int ERROR_NO_SITE = 1919;

		internal static readonly int ERROR_NOT_ENOUGH_MEMORY = 8;

		internal enum ComputerNameFormat
		{
			ComputerNameNetBIOS,
			ComputerNameDnsHostname,
			ComputerNameDnsDomain,
			ComputerNameDnsFullyQualified,
			ComputerNamePhysicalNetBIOS,
			ComputerNamePhysicalDnsHostname,
			ComputerNamePhysicalDnsDomain,
			ComputerNamePhysicalDnsFullyQualified,
			ComputerNameMax
		}

		internal enum SysGeoType
		{
			Iso2 = 4,
			FriendlyName = 8
		}

		internal delegate bool GeoEnumProc(int geiId);

		internal enum MachineRole
		{
			StandaloneWorkstation,
			MemberWorkstation,
			StandaloneServer,
			MemberServer,
			BackupDomainController,
			PrimaryDomainController
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class InteropDsRolePrimaryDomainInfoBasic
		{
			public NativeMethods.MachineRole machineRole;

			public int flags;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string domainNameFlat;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string domainNameDns;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string domainForestName;

			public Guid guid;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class DsNameResult
		{
			public uint cItems;

			public IntPtr rItems;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class DsNameResultItem
		{
			public int status;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string domain;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string name;
		}

		[Flags]
		internal enum DsNameFlags
		{
			NoFlags = 0,
			SyntacticalOnly = 1,
			EvalAtDC = 2,
			GCVerify = 4,
			TrustReferral = 8
		}

		internal enum BindFlags : uint
		{
			BIND_ALLOW_DELEGATION = 1U,
			BIND_FIND_BINDING,
			BIND_FORCE_KERBEROS = 4U
		}

		[Flags]
		public enum DsGetDcOpenFlags
		{
			ForceRediscovery = 1,
			GCRequired = 64,
			PdcRequired = 128,
			DSWriteableRequired = 4096
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		internal struct NetLogonInfo2
		{
			internal uint Flags;

			internal uint PdcConnectionStatus;

			internal string TrustedDcName;

			internal uint TdcConnectionStatus;
		}

		internal enum NetLogonControlOperation : uint
		{
			NetLogonControlRediscover = 5U,
			NetLogonControlTrustedChannelStatus
		}
	}
}
