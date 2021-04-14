using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Diagnostics.Audit
{
	[SuppressUnmanagedCodeSecurity]
	internal sealed class NativeMethods
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseHandle(IntPtr handle);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AdjustTokenPrivileges([In] SafeTokenHandle TokenHandle, [In] bool DisableAllPrivileges, [In] ref NativeMethods.TOKEN_PRIVILEGE NewState, [In] uint BufferLength, [In] [Out] ref NativeMethods.TOKEN_PRIVILEGE PreviousState, [In] [Out] ref uint ReturnLength);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool RevertToSelf();

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "LookupPrivilegeValueW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool LookupPrivilegeValue([In] string lpSystemName, [In] string lpName, [In] [Out] ref NativeMethods.LUID Luid);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern IntPtr GetCurrentProcess();

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern SafeTokenHandle GetCurrentThread();

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool OpenProcessToken([In] IntPtr ProcessToken, [In] TokenAccessLevels DesiredAccess, [In] [Out] ref SafeTokenHandle TokenHandle);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool OpenThreadToken([In] SafeTokenHandle ThreadToken, [In] TokenAccessLevels DesiredAccess, [In] bool OpenAsSelf, out SafeTokenHandle TokenHandle);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DuplicateTokenEx([In] SafeTokenHandle ExistingTokenHandle, [In] TokenAccessLevels DesiredAccess, [In] IntPtr TokenAttributes, [In] SecurityImpersonationLevel ImpersonationLevel, [In] TokenType TokenType, [In] [Out] ref SafeTokenHandle DuplicateTokenHandle);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetThreadToken([In] IntPtr Thread, [In] SafeTokenHandle Token);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool ImpersonateSelf(int impersonationLevel);

		[DllImport("advapi32.dll")]
		internal static extern void MapGenericMask(ref uint accessMask, [In] ref NativeMethods.GENERIC_MAPPING genericMapping);

		[DllImport("advapi32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AccessCheckByType([MarshalAs(UnmanagedType.LPArray)] byte[] pSecurityDescriptor, IntPtr principalSelfSid, SafeTokenHandle clientToken, uint DesiredAccess, IntPtr objectTypeList, int ObjectTypeListLength, ref NativeMethods.GENERIC_MAPPING GenericMapping, SafeHandle PrivilegeSet, ref int PrivilegeSetLength, out uint GrantedAccess, out bool AccessStatus);

		[DllImport("authz.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzUnregisterSecurityEventSource(uint dwFlags, [In] [Out] ref IntPtr providerHandle);

		[DllImport("authz.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzRegisterSecurityEventSource(uint dwFlags, string szEventSourceName, out SafeAuditHandle ProviderHandle);

		[DllImport("authz.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzReportSecurityEventFromParams(uint dwFlags, SafeAuditHandle providerHandle, uint auditId, byte[] securityIdentifier, [In] ref NativeMethods.AUDIT_PARAMS auditParams);

		[DllImport("authz.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzInstallSecurityEventSource(uint dwFlags, [In] ref NativeMethods.AUTHZ_SOURCE_SCHEMA_REGISTRATION pRegistration);

		[DllImport("authz.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzUninstallSecurityEventSource(uint dwFlags, string eventSourceName);

		internal const string KERNEL32 = "kernel32.dll";

		internal const string AUTHZ = "authz.dll";

		internal const string ADVAPI32 = "advapi32.dll";

		internal const string SECUR32 = "secur32.dll";

		internal const uint SE_PRIVILEGE_DISABLED = 0U;

		internal const uint SE_PRIVILEGE_ENABLED_BY_DEFAULT = 1U;

		internal const uint SE_PRIVILEGE_ENABLED = 2U;

		internal const uint SE_PRIVILEGE_USED_FOR_ACCESS = 2147483648U;

		internal const uint APF_AuditFailure = 0U;

		internal const uint APF_AuditSuccess = 1U;

		internal const int ERROR_SUCCESS = 0;

		internal const int ERROR_INVALID_FUNCTION = 1;

		internal const int ERROR_FILE_NOT_FOUND = 2;

		internal const int ERROR_PATH_NOT_FOUND = 3;

		internal const int ERROR_ACCESS_DENIED = 5;

		internal const int ERROR_INVALID_HANDLE = 6;

		internal const int ERROR_NOT_ENOUGH_MEMORY = 8;

		internal const int ERROR_INVALID_DRIVE = 15;

		internal const int ERROR_NO_MORE_FILES = 18;

		internal const int ERROR_NOT_READY = 21;

		internal const int ERROR_BAD_LENGTH = 24;

		internal const int ERROR_SHARING_VIOLATION = 32;

		internal const int ERROR_NOT_SUPPORTED = 50;

		internal const int ERROR_FILE_EXISTS = 80;

		internal const int ERROR_INVALID_PARAMETER = 87;

		internal const int ERROR_CALL_NOT_IMPLEMENTED = 120;

		internal const int ERROR_INSUFFICIENT_BUFFER = 122;

		internal const int ERROR_INVALID_NAME = 123;

		internal const int ERROR_BAD_PATHNAME = 161;

		internal const int ERROR_ALREADY_EXISTS = 183;

		internal const int ERROR_ENVVAR_NOT_FOUND = 203;

		internal const int ERROR_FILENAME_EXCED_RANGE = 206;

		internal const int ERROR_MORE_DATA = 234;

		internal const int ERROR_OPERATION_ABORTED = 995;

		internal const int ERROR_NO_TOKEN = 1008;

		internal const int ERROR_DLL_INIT_FAILED = 1114;

		internal const int ERROR_NON_ACCOUNT_SID = 1257;

		internal const int ERROR_NOT_ALL_ASSIGNED = 1300;

		internal const int ERROR_UNKNOWN_REVISION = 1305;

		internal const int ERROR_INVALID_OWNER = 1307;

		internal const int ERROR_INVALID_PRIMARY_GROUP = 1308;

		internal const int ERROR_NO_SUCH_PRIVILEGE = 1313;

		internal const int ERROR_PRIVILEGE_NOT_HELD = 1314;

		internal const int ERROR_NONE_MAPPED = 1332;

		internal const int ERROR_INVALID_ACL = 1336;

		internal const int ERROR_INVALID_SID = 1337;

		internal const int ERROR_INVALID_SECURITY_DESCR = 1338;

		internal const int ERROR_BAD_IMPERSONATION_LEVEL = 1346;

		internal const int ERROR_CANT_OPEN_ANONYMOUS = 1347;

		internal const int ERROR_NO_SECURITY_ON_OBJECT = 1350;

		internal const int ERROR_TRUSTED_RELATIONSHIP_FAILURE = 1789;

		internal const int ERROR_OBJECT_ALREADY_EXISTS = 5010;

		internal const uint STATUS_SOME_NOT_MAPPED = 263U;

		internal const uint STATUS_NO_MEMORY = 3221225495U;

		internal const uint STATUS_NONE_MAPPED = 3221225587U;

		internal const uint STATUS_INSUFFICIENT_RESOURCES = 3221225626U;

		internal const uint STATUS_ACCESS_DENIED = 3221225506U;

		internal const int STATUS_ACCOUNT_RESTRICTION = -1073741714;

		internal struct GENERIC_MAPPING
		{
			public uint GenericRead;

			public uint GenericWrite;

			public uint GenericExecute;

			public uint GenericAll;
		}

		internal struct LUID
		{
			internal uint LowPart;

			internal uint HighPart;
		}

		internal struct LUID_AND_ATTRIBUTES
		{
			internal NativeMethods.LUID Luid;

			internal uint Attributes;
		}

		internal struct TOKEN_PRIVILEGE
		{
			internal uint PrivilegeCount;

			internal NativeMethods.LUID_AND_ATTRIBUTES Privilege;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct AUTHZ_REGISTRATION_OBJECT_TYPE_NAME_OFFSET
		{
			internal string szObjectTypeName;

			internal uint dwOffset;
		}

		internal struct AUDIT_PARAM
		{
			internal uint Type;

			internal uint Length;

			internal uint Flags;

			internal IntPtr Data0;

			internal IntPtr Data1;
		}

		internal struct AUDIT_PARAMS
		{
			internal uint Length;

			internal uint Flags;

			internal ushort Count;

			internal IntPtr Parameters;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct AUTHZ_SOURCE_SCHEMA_REGISTRATION
		{
			internal uint dwFlags;

			internal string eventSourceName;

			internal string eventMessageFile;

			internal string eventSourceXmlSchemaFile;

			internal string eventAccessStringsFile;

			internal string executableImagePath;

			internal IntPtr pReserved;

			internal uint dwObjectTypeNameCount;

			internal NativeMethods.AUTHZ_REGISTRATION_OBJECT_TYPE_NAME_OFFSET objectTypeNames;
		}
	}
}
