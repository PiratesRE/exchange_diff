using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Win32
{
	[ComVisible(false)]
	[SuppressUnmanagedCodeSecurity]
	internal static class NativeMethods
	{
		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FileTimeToSystemTime([In] ref long FileTime, out NativeMethods.SystemTime systemTime);

		[DllImport("KERNEL32.DLL")]
		internal static extern NativeMethods.TimeZoneId GetTimeZoneInformation(out NativeMethods.TIME_ZONE_INFORMATION timeZoneInformation);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "DeleteFileW", SetLastError = true)]
		internal static extern bool DeleteFile([In] string fileName);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetTempFileNameW", SetLastError = true)]
		internal static extern int GetTempFileName([In] string pathName, [In] string prefixString, [In] uint unique, [Out] StringBuilder tempFileName);

		public static SafeHGlobalHandle AllocHGlobal(int size)
		{
			return SafeHGlobalHandle.AllocHGlobal(size);
		}

		internal static NativeMethods.SecurityIdentifierAndAttributes AuthzGetInformationFromContextTokenUser(AuthzContextHandle hAuthzClientContext)
		{
			uint num = 0U;
			NativeMethods.AuthzGetInformationFromContext(hAuthzClientContext, AuthzContextInformation.UserSid, 0U, ref num, SafeHGlobalHandle.InvalidHandle);
			if (num == 0U)
			{
				throw new Win32Exception();
			}
			NativeMethods.SecurityIdentifierAndAttributes result;
			using (SafeHGlobalHandle safeHGlobalHandle = new SafeHGlobalHandle(Marshal.AllocHGlobal((int)num)))
			{
				if (!NativeMethods.AuthzGetInformationFromContext(hAuthzClientContext, AuthzContextInformation.UserSid, num, ref num, safeHGlobalHandle))
				{
					throw new Win32Exception();
				}
				result = new NativeMethods.SecurityIdentifierAndAttributes(safeHGlobalHandle.DangerousGetHandle());
			}
			return result;
		}

		internal static NativeMethods.SecurityIdentifierAndAttributes[] AuthzGetInformationFromContextTokenGroup(AuthzContextHandle hAuthzClientContext)
		{
			uint num = 0U;
			NativeMethods.SecurityIdentifierAndAttributes[] array = null;
			NativeMethods.AuthzGetInformationFromContext(hAuthzClientContext, AuthzContextInformation.GroupSids, 0U, ref num, SafeHGlobalHandle.InvalidHandle);
			if (num == 0U)
			{
				throw new Win32Exception();
			}
			using (SafeHGlobalHandle safeHGlobalHandle = new SafeHGlobalHandle(Marshal.AllocHGlobal((int)num)))
			{
				if (!NativeMethods.AuthzGetInformationFromContext(hAuthzClientContext, AuthzContextInformation.GroupSids, num, ref num, safeHGlobalHandle))
				{
					throw new Win32Exception();
				}
				int num2 = Marshal.ReadInt32(safeHGlobalHandle.DangerousGetHandle());
				if (num2 > 0)
				{
					array = new NativeMethods.SecurityIdentifierAndAttributes[num2];
					IntPtr value = new IntPtr((long)safeHGlobalHandle.DangerousGetHandle() + (long)NativeMethods.TokenGroups.SidAndAttributesOffset);
					for (int i = 0; i < num2; i++)
					{
						array[i] = new NativeMethods.SecurityIdentifierAndAttributes(new IntPtr((long)value + (long)(i * NativeMethods.SecurityIdentifierAndAttributes.SidAndAttributesSize)));
					}
				}
			}
			return array;
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("AUTHZ.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool AuthzInitializeResourceManager([In] ResourceManagerFlags flags, [In] IntPtr pfnAccessCheck, [In] IntPtr pfnComputeDynamicGroups, [In] IntPtr pfnFreeDynamicGroups, [In] string resourceManagerName, out ResourceManagerHandle resourceManagerHandle);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("AUTHZ.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzAccessCheck(uint flags, AuthzContextHandle hClientContext, SafeHGlobalHandle pRequest, IntPtr hAuditInfo, byte[] securityDescriptor, IntPtr optionalSecurityDescriptorArray, uint optionalSecurityDescriptorCount, SafeHGlobalHandle pReply, [In] [Out] IntPtr pAuthzHandle);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("AUTHZ.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzGetInformationFromContext(AuthzContextHandle hAuthzClientContext, AuthzContextInformation InfoClass, uint BufferSize, ref uint SizeRequired, SafeHGlobalHandle ContextInfoBuffer);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("AUTHZ.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzInitializeContextFromSid(AuthzFlags Flags, byte[] UserSid, ResourceManagerHandle AuthzResourceManager, IntPtr pExpirationTime, NativeMethods.AuthzLuid Identifier, IntPtr DynamicGroupArgs, out AuthzContextHandle pAuthzClientContext);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("AUTHZ.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzInitializeContextFromToken(AuthzFlags Flags, IntPtr TokenHandle, ResourceManagerHandle AuthzResourceManager, IntPtr pExpirationTime, NativeMethods.AuthzLuid Identifier, IntPtr DynamicGroupArgs, out AuthzContextHandle pAuthzClientContext);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("AUTHZ.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzInitializeContextFromAuthzContext(AuthzFlags Flags, IntPtr authzClientContext, IntPtr pExpirationTime, NativeMethods.AuthzLuid Identifier, IntPtr DynamicGroupArgs, out AuthzContextHandle pAuthzNewClientContext);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("AUTHZ.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzAddSidsToContext(AuthzContextHandle OrigClientContext, NativeMethods.SidAndAttributes[] groupSids, uint groupSidCount, NativeMethods.SidAndAttributes[] restrictedSids, uint restrictedSidCount, out AuthzContextHandle pNewClientContext);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("ADVAPI32.DLL", SetLastError = true)]
		public static extern int MakeAbsoluteSD([MarshalAs(UnmanagedType.LPArray)] byte[] pSecurityDescriptor, IntPtr pAbsoluteSD, [In] [Out] ref int lpdwAbsoluteSDSize, IntPtr pDacl, [In] [Out] ref int lpdwDaclSize, IntPtr pSacl, [In] [Out] ref int lpdwSaclSize, IntPtr pOwner, [In] [Out] ref int lpdwOwnerSize, IntPtr pPrimaryGroup, [In] [Out] ref int lpdwPrimaryGroupSize);

		[DllImport("SECUR32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetComputerObjectNameW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetComputerObjectName(NativeMethods.ExtendedNameFormat nameFormat, StringBuilder nameBuffer, ref int size);

		[DllImport("SECUR32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetUserNameExW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetUserNameEx(NativeMethods.ExtendedNameFormat nameFormat, StringBuilder nameBuffer, ref int nSize);

		[DllImport("NTDSAPI.DLL", CharSet = CharSet.Unicode, EntryPoint = "DsServerRegisterSpnW")]
		internal static extern int DsServerRegisterSpn(NativeMethods.SpnWriteOperation op, string serviceClass, string userObjectDn);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GlobalMemoryStatusEx(ref NativeMethods.MEMORYSTATUSEX lpBuffer);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		internal static extern uint WaitForMultipleObjects(uint count, IntPtr[] handles, [MarshalAs(UnmanagedType.Bool)] bool isWaitAll, uint milliseconds);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		internal static extern uint WaitForSingleObject(SafeProcessHandle processHandle, uint milliseconds);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetQueuedCompletionStatus(IoCompletionPort completionPort, out uint numberOfBytes, out UIntPtr completionKey, out IntPtr overlapped, uint milliseconds);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool PostQueuedCompletionStatus(IoCompletionPort completionPort, uint numberOfBytes, UIntPtr completionKey, IntPtr overlapped);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.I4)]
		internal static extern uint GetProcessId(IntPtr hProcess);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

		[DllImport("ADVAPI32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CreateProcessAsUser(SafeUserTokenHandle hToken, string lpApplicationName, string lpCommandLine, IntPtr processAttributes, IntPtr threadAttributes, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref NativeMethods.STARTUPINFO lpStartupInfo, ref NativeMethods.PROCESS_INFORMATION lpProcessInformation);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandles, uint dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref NativeMethods.STARTUPINFO lpStartupInfo, ref NativeMethods.PROCESS_INFORMATION lpProcessInformation);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		public static extern SafeProcessHandle OpenProcess(NativeMethods.ProcessAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, uint dwProcessId);

		[DllImport("KERNEL32.DLL")]
		internal static extern uint GetCurrentProcessId();

		[DllImport("ADVAPI32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool LogonUser(string userName, string domainName, string password, int logonType, int logonProvider, out SafeUserTokenHandle token);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("ADVAPI32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool OpenProcessToken([In] IntPtr ProcessHandle, [In] uint DesiredAccess, [In] [Out] ref SafeUserTokenHandle TokenHandle);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("ADVAPI32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetTokenInformation([In] SafeUserTokenHandle TokenHandle, [In] NativeMethods.TOKEN_INFORMATION_CLASS TokenInformationClass, [Out] SafeHGlobalHandle TokenInformation, [In] int TokenInformationLength, [In] [Out] ref int ReturnLength);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("ADVAPI32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool LookupPrivilegeValue([In] string lpSystemName, [In] string lpName, [In] [Out] ref NativeMethods.LUID Luid);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("ADVAPI32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AdjustTokenPrivileges([In] SafeUserTokenHandle TokenHandle, [MarshalAs(UnmanagedType.Bool)] [In] bool DisableAllPrivileges, [In] SafeHGlobalHandle NewState, [In] uint BufferLength, [In] [Out] IntPtr PreviousState, [In] [Out] IntPtr ReturnLength);

		internal static bool GlobalMemoryStatusEx(out NativeMethods.MemoryStatusEx memoryStatusEx)
		{
			memoryStatusEx = default(NativeMethods.MemoryStatusEx);
			memoryStatusEx.Length = NativeMethods.MemoryStatusEx.Size;
			return NativeMethods.GlobalMemoryStatusExInternal(ref memoryStatusEx);
		}

		[DllImport("KERNEL32.DLL", EntryPoint = "GlobalMemoryStatusEx", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GlobalMemoryStatusExInternal([In] [Out] ref NativeMethods.MemoryStatusEx memoryStatusEx);

		[DllImport("PSAPI.DLL")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetProcessMemoryInfo([In] SafeProcessHandle processHandle, out NativeMethods.ProcessMemoryCounterEx counters, [In] uint size);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetDiskFreeSpaceExW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetDiskFreeSpaceEx([In] string directoryName, out ulong freeBytesAvailable, out ulong totalNumberOfBytes, out ulong totalNumberOfFreeBytes);

		[DllImport("RPCRT4.DLL")]
		internal static extern int RpcTestCancel();

		[DllImport("KERNEL32.DLL", BestFitMapping = false, EntryPoint = "DuplicateHandle", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool DuplicateThreadHandle([In] SafeProcessHandle sourceProcessHandle, [In] SafeThreadHandle sourceHandle, [In] SafeProcessHandle targetProcessHandle, out SafeThreadHandle targetHandle, [In] uint dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] [In] bool bInheritHandle, [In] uint dwOptions);

		[DllImport("KERNEL32.DLL")]
		internal static extern SafeThreadHandle GetCurrentThread();

		[DllImport("KERNEL32.DLL")]
		internal static extern SafeProcessHandle GetCurrentProcess();

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool FlushViewOfFile([In] SafeViewOfFileHandle lpBaseAddress, [In] UIntPtr dwNumBytesToFlush);

		[DllImport("ADVAPI32.DLL", CharSet = CharSet.Unicode, EntryPoint = "ConvertStringSecurityDescriptorToSecurityDescriptorW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool ConvertStringSecurityDescriptorToSecurityDescriptor([In] string StringSecurityDescriptor, [In] uint SDRevision, out SafeHGlobalHandle SecurityDescriptor, out ulong SecurityDescriptorSize);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "GetComputerNameExW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool GetComputerNameEx([In] NativeMethods.ComputerNameFormat nameType, [Out] StringBuilder buffer, [In] [Out] ref uint bufferSize);

		public static bool CreatePrivateObjectSecurityEx([In] RawSecurityDescriptor parentDescriptor, [In] RawSecurityDescriptor creatorDescriptor, out RawSecurityDescriptor newDescriptor, [In] Guid objectType, [In] bool isContainerObject, [In] uint autoInheritFlags, [In] WindowsIdentity identity, [In] NativeMethods.GENERIC_MAPPING mapping)
		{
			byte[] array = null;
			byte[] array2 = null;
			byte[] binaryForm = null;
			newDescriptor = null;
			if (parentDescriptor != null)
			{
				array = new byte[parentDescriptor.BinaryLength];
				parentDescriptor.GetBinaryForm(array, 0);
			}
			if (creatorDescriptor != null)
			{
				array2 = new byte[creatorDescriptor.BinaryLength];
				creatorDescriptor.GetBinaryForm(array2, 0);
			}
			bool result = NativeMethods.CreatePrivateObjectSecurityEx(array, array2, out binaryForm, objectType, isContainerObject, autoInheritFlags, identity, mapping);
			newDescriptor = new RawSecurityDescriptor(binaryForm, 0);
			return result;
		}

		[DllImport("ADVAPI32.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.U4)]
		public static extern uint GetSecurityDescriptorLength(SafeHandle pSecurityDescriptor);

		[DllImport("ADVAPI32.DLL", SetLastError = true)]
		public static extern void MapGenericMask(ref uint AccessMask, ref NativeMethods.GENERIC_MAPPING GenericMapping);

		public static bool LookupAccountSid(SecurityIdentifier sid, out string domainName, out string accountName)
		{
			byte[] array = new byte[sid.BinaryLength];
			sid.GetBinaryForm(array, 0);
			uint capacity = 64U;
			uint capacity2 = 64U;
			StringBuilder stringBuilder = new StringBuilder((int)capacity);
			StringBuilder stringBuilder2 = new StringBuilder((int)capacity2);
			int num;
			bool flag = NativeMethods.LookupAccountSid(null, array, stringBuilder, ref capacity, stringBuilder2, ref capacity2, out num);
			if (!flag && Marshal.GetLastWin32Error() == 122)
			{
				stringBuilder = new StringBuilder((int)capacity);
				stringBuilder2 = new StringBuilder((int)capacity2);
				flag = NativeMethods.LookupAccountSid(null, array, stringBuilder, ref capacity, stringBuilder2, ref capacity2, out num);
			}
			if (flag)
			{
				domainName = stringBuilder2.ToString();
				accountName = stringBuilder.ToString();
			}
			else
			{
				accountName = null;
				domainName = null;
			}
			return flag;
		}

		public static int HRESULT_FROM_WIN32(uint win32)
		{
			if (win32 <= 0U)
			{
				return (int)win32;
			}
			return (int)((win32 & 65535U) | 2147942400U);
		}

		[DllImport("ADVAPI32.DLL", CharSet = CharSet.Unicode, EntryPoint = "LookupAccountSidW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool LookupAccountSid(string systemName, byte[] sid, StringBuilder accountName, ref uint accountNameLength, StringBuilder domainName, ref uint domainNameLength, out int usage);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("KERNEL32.DLL", SetLastError = true)]
		internal static extern void ZeroMemory(IntPtr handle, uint length);

		[DllImport("KERNEL32.DLL", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindFirstFileW", SetLastError = true)]
		internal static extern SafeFindHandle FindFirstFile([In] string fileName, out NativeMethods.WIN32_FIND_DATA data);

		[DllImport("KERNEL32.DLL", BestFitMapping = false, CharSet = CharSet.Unicode, EntryPoint = "FindNextFileW", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FindNextFile([In] SafeFindHandle hndFindFile, out NativeMethods.WIN32_FIND_DATA lpFindFileData);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "LoadLibraryW", SetLastError = true)]
		public static extern SafeLibraryHandle LoadLibrary([In] string fileName);

		internal static bool AuthzInitializeResourceManager(ResourceManagerFlags flags, string resourceManagerName, out ResourceManagerHandle resourceManagerHandle)
		{
			return NativeMethods.AuthzInitializeResourceManager(flags, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, resourceManagerName, out resourceManagerHandle);
		}

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("AUTHZ.DLL")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzFreeContext(IntPtr clientContextHandle);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("AUTHZ.DLL")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool AuthzFreeResourceManager(IntPtr resourceManagerHandle);

		[DllImport("NTDSAPI.DLL", CharSet = CharSet.Unicode, EntryPoint = "DsBindW")]
		internal static extern uint DsBind([In] string domainControllerName, [In] string dnsDomainName, out SafeDsHandle handle);

		[DllImport("NTDSAPI.DLL", CharSet = CharSet.Unicode, EntryPoint = "DsWriteAccountSpnW")]
		internal static extern uint DsWriteAccountSpn([In] SafeDsHandle handle, [In] NativeMethods.SpnWriteOperation operation, [In] string account, [In] uint spnCount, [In] string[] spns);

		[DllImport("NTDSAPI.DLL", CharSet = CharSet.Unicode, EntryPoint = "DsGetSpnW")]
		internal static extern int DsGetSpn([In] NativeMethods.SpnNameType serviceType, [In] string serviceClass, [In] string serviceName, [In] ushort instancePort, [In] ushort instanceNameCount, [In] string[] instanceNames, [In] ushort[] instancePorts, out uint spnCount, out SafeSpnArrayHandle spnArray);

		internal static uint DsGetDcName(string server, string domainName, string siteName, NativeMethods.DsGetDCNameFlags flags, out SafeDomainControllerInfoHandle domainControllerInfo)
		{
			return NativeMethods.DsGetDcName(server, domainName, IntPtr.Zero, siteName, flags, out domainControllerInfo);
		}

		[DllImport("NETAPI32.DLL", CharSet = CharSet.Unicode, EntryPoint = "DsGetDcNameW")]
		private static extern uint DsGetDcName([In] string server, [In] string domainName, [In] IntPtr guidPtr, [In] string siteName, [In] NativeMethods.DsGetDCNameFlags flags, out SafeDomainControllerInfoHandle domainControllerInfo);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		public static extern IoCompletionPort CreateIoCompletionPort(SafeFileHandle fileHandle, IoCompletionPort existingCompletionPort, UIntPtr completionKey, uint numberOfConcurrentThreads);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "CreateJobObjectW", SetLastError = true)]
		public static extern SafeJobHandle CreateJobObject(IntPtr jobAttributes, string name);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		public static extern SafeJobHandle OpenJobObject(uint dwDesiredAccess, bool bInheritHandles, string jobName);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool IsProcessInJob(SafeProcessHandle processHandle, SafeJobHandle job, [MarshalAs(UnmanagedType.Bool)] out bool isProcessInJob);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public unsafe static extern bool SetInformationJobObject(SafeJobHandle job, NativeMethods.JOBOBJECTINFOCLASS jobObjectInfoClass, void* jobObjectInfo, int jobObjectInfoLength);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public unsafe static extern bool QueryInformationJobObject(SafeJobHandle job, NativeMethods.JOBOBJECTINFOCLASS iobObjectInfoClass, void* jobObjectInfo, uint jobObjectInfoLength, out uint returnLength);

		[DllImport("KERNEL32.DLL", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AssignProcessToJobObject(SafeJobHandle job, IntPtr processHandle);

		[DllImport("KERNEL32.DLL", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool AssignProcessToJobObject(SafeJobHandle job, SafeProcessHandle processHandle);

		[DllImport("KERNEL32.DLL", ExactSpelling = true, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool TerminateJobObject(SafeJobHandle job, uint uExitCode);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("ADVAPI32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetKernelObjectSecurity(SafeHandle handle, [MarshalAs(UnmanagedType.U4)] SecurityInfos requestedInformation, IntPtr securityDescriptor, uint length, out uint lengthNeeded);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[DllImport("ADVAPI32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetKernelObjectSecurity(SafeHandle handle, [MarshalAs(UnmanagedType.U4)] SecurityInfos securityInformation, IntPtr securityDescriptor);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "CreateNamedPipeW", SetLastError = true)]
		internal static extern SafeFileHandle CreateNamedPipe(string name, NativeMethods.PipeOpen openMode, NativeMethods.PipeModes pipeMode, uint maxInstances, uint outBufferSize, uint inBufferSize, uint defaultTimeOut, IntPtr securityAttributes);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool SetNamedPipeHandleState(SafeFileHandle pipeHandle, ref NativeMethods.PipeModes mode, IntPtr maxCollectionCount, IntPtr collectDataTimeout);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "CreateFileW", SetLastError = true)]
		internal static extern SafeFileHandle CreateFile([In] string lpFileName, [In] NativeMethods.CreateFileAccess dwDesiredAccess, [In] NativeMethods.CreateFileShare dwShareMode, [In] ref NativeMethods.SECURITY_ATTRIBUTES lpSecurityAttributes, [In] FileMode dwCreationDisposition, [In] NativeMethods.CreateFileFileAttributes dwFlagsAndAttributes, [In] IntPtr hTemplateFile);

		[DllImport("RPCRT4.DLL")]
		internal static extern int RpcCancelThreadEx([In] SafeThreadHandle threadHandle, [In] int timeoutSeconds);

		[DllImport("KERNEL32.DLL", CharSet = CharSet.Unicode, EntryPoint = "CreateFileMappingW", SetLastError = true)]
		public static extern SafeFileHandle CreateFileMapping([In] SafeFileHandle hFile, [In] ref NativeMethods.SECURITY_ATTRIBUTES lpAttributes, [In] NativeMethods.MemoryAccessControl accessControl, [In] uint dwMaximumSizeHigh, [In] uint dwMaximumSizeLow, [In] string lpName);

		[DllImport("KERNEL32.DLL", SetLastError = true)]
		public static extern SafeViewOfFileHandle MapViewOfFile([In] SafeFileHandle fileMappingObject, [In] NativeMethods.FileMapAccessControl desiredAccess, [In] uint dwFileOffsetHigh, [In] uint dwFileOffsetLow, [In] UIntPtr dwNumBytesToMap);

		[DllImport("NTDSAPI.DLL", CharSet = CharSet.Unicode, EntryPoint = "DsCrackNamesW", ExactSpelling = true)]
		internal static extern uint DsCrackNames(SafeDsHandle dsHandle, NativeMethods.DSNameFlags flags, NativeMethods.DSNameFormat formatOffered, NativeMethods.DSNameFormat formatDesired, uint nameCount, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPWStr, SizeParamIndex = 4)] string[] toBeResolvedNames, out SafeDsNameResultHandle dsNameResultHandle);

		public static bool CreatePrivateObjectSecurityEx([In] byte[] parentDescriptorBlob, [In] byte[] creatorDescriptorBlob, out byte[] newDescriptorBlob, [In] Guid objectType, [In] bool isContainerObject, [In] uint autoInheritFlags, [In] WindowsIdentity identity, [In] NativeMethods.GENERIC_MAPPING mapping)
		{
			bool flag = false;
			SafeHGlobalHandle safeHGlobalHandle = null;
			SafePrivateObjectSecurityDescriptorHandle safePrivateObjectSecurityDescriptorHandle = null;
			newDescriptorBlob = null;
			try
			{
				if (objectType != Guid.Empty)
				{
					byte[] array = objectType.ToByteArray();
					safeHGlobalHandle = NativeMethods.AllocHGlobal(array.Length);
					Marshal.Copy(array, 0, safeHGlobalHandle.DangerousGetHandle(), array.Length);
				}
				else
				{
					safeHGlobalHandle = SafeHGlobalHandle.InvalidHandle;
				}
				flag = NativeMethods.CreatePrivateObjectSecurityEx(parentDescriptorBlob, creatorDescriptorBlob, out safePrivateObjectSecurityDescriptorHandle, safeHGlobalHandle, isContainerObject, autoInheritFlags, (identity != null) ? identity.Token : ((IntPtr)0), ref mapping);
				if (flag)
				{
					int securityDescriptorLength = (int)NativeMethods.GetSecurityDescriptorLength(safePrivateObjectSecurityDescriptorHandle);
					newDescriptorBlob = new byte[securityDescriptorLength];
					Marshal.Copy(safePrivateObjectSecurityDescriptorHandle.DangerousGetHandle(), newDescriptorBlob, 0, securityDescriptorLength);
				}
			}
			finally
			{
				safeHGlobalHandle.Dispose();
				if (safePrivateObjectSecurityDescriptorHandle != null)
				{
					safePrivateObjectSecurityDescriptorHandle.Dispose();
				}
			}
			return flag;
		}

		[DllImport("ADVAPI32.DLL", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool CreatePrivateObjectSecurityEx([MarshalAs(UnmanagedType.LPArray)] [In] byte[] parentDescriptor, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] creatorDescriptor, out SafePrivateObjectSecurityDescriptorHandle newDescriptor, [In] SafeHGlobalHandle objectType, [MarshalAs(UnmanagedType.Bool)] [In] bool isContainerObject, [MarshalAs(UnmanagedType.U4)] [In] uint autoInheritFlags, [In] IntPtr token, [In] ref NativeMethods.GENERIC_MAPPING mapping);

		private const string Kernel32 = "KERNEL32.DLL";

		private const string AdvApi32 = "ADVAPI32.DLL";

		private const string AuthZ32 = "AUTHZ.DLL";

		private const string NtdsApi = "NTDSAPI.DLL";

		private const string Secur32 = "SECUR32.DLL";

		private const string PSAPI = "PSAPI.DLL";

		private const string RpcRt4 = "RPCRT4.DLL";

		private const string NetApi32 = "NETAPI32.DLL";

		internal const uint JOB_OBJECT_LIMIT_KILL_ON_JOB_CLOSE = 8192U;

		internal const uint NMPWAIT_WAIT_FOREVER = 4294967295U;

		internal const uint SDDL_REVISION_1 = 1U;

		internal const int ERROR_ALREADY_EXISTS = 183;

		internal const int ErrorSuccess = 0;

		internal enum ErrorCode
		{
			Success,
			FileNotFound = 2,
			PathNotFound,
			AccessDenied = 5,
			InvalidParameter = 87,
			InsufficientBuffer = 122,
			MoreData = 234,
			WaitTimeout = 258
		}

		[Flags]
		internal enum AccessRights : uint
		{
			Delete = 65536U,
			ReadControl = 131072U,
			WriteDac = 262144U,
			WriteOwner = 524288U,
			Synchronize = 1048576U,
			StandardRightsRequired = 983040U,
			StandardRightsRead = 131072U,
			StandardRightsWrite = 131072U,
			StandardRightsExecute = 131072U,
			StandardRightsAll = 2031616U,
			AccessSystemSecurity = 16777216U,
			MaximumAllowed = 33554432U,
			GenericRead = 2147483648U,
			GenericWrite = 1073741824U,
			GenericExecute = 536870912U,
			GenericAll = 268435456U
		}

		[Flags]
		internal enum MailboxPermissionRights : uint
		{
			SDPERM_USER_MAILBOX_OWNER = 1U,
			SDPERM_USER_SEND_AS = 2U,
			SDPERM_USER_PRIMARY_USER = 4U
		}

		[Flags]
		internal enum AutoInheritFlags : uint
		{
			AutoInheritDACL = 1U,
			AutoInheritSACL = 2U,
			DefaultDescriptorForObject = 4U,
			AvoidPrivilegeCheck = 8U,
			AvoidOwnerCheck = 16U,
			DefaultOwnerFromParent = 32U,
			DefaultGroupFromParent = 64U
		}

		internal struct AuthzLuid
		{
			internal uint LowPart;

			internal int HighPart;
		}

		internal struct SidAndAttributes
		{
			internal IntPtr Sid;

			internal uint Attributes;
		}

		internal struct UserToken
		{
			internal byte[] Sid;

			internal uint Attributes;
		}

		internal struct GroupsToken
		{
			internal uint GroupCount;

			internal NativeMethods.SidAndAttributes[] Groups;
		}

		public struct SystemTime : IEquatable<NativeMethods.SystemTime>
		{
			public static bool operator ==(NativeMethods.SystemTime v1, NativeMethods.SystemTime v2)
			{
				return v1.Equals(v2);
			}

			public static bool operator !=(NativeMethods.SystemTime v1, NativeMethods.SystemTime v2)
			{
				return !v1.Equals(v2);
			}

			public static NativeMethods.SystemTime Parse(ArraySegment<byte> buffer)
			{
				if (buffer.Count < NativeMethods.SystemTime.Size)
				{
					throw new ArgumentOutOfRangeException();
				}
				NativeMethods.SystemTime result;
				result.Year = BitConverter.ToUInt16(buffer.Array, buffer.Offset + NativeMethods.SystemTime.YearOffset);
				result.Month = BitConverter.ToUInt16(buffer.Array, buffer.Offset + NativeMethods.SystemTime.MonthOffset);
				result.DayOfWeek = BitConverter.ToUInt16(buffer.Array, buffer.Offset + NativeMethods.SystemTime.DayOfWeekOffset);
				result.Day = BitConverter.ToUInt16(buffer.Array, buffer.Offset + NativeMethods.SystemTime.DayOffset);
				result.Hour = BitConverter.ToUInt16(buffer.Array, buffer.Offset + NativeMethods.SystemTime.HourOffset);
				result.Minute = BitConverter.ToUInt16(buffer.Array, buffer.Offset + NativeMethods.SystemTime.MinuteOffset);
				result.Second = BitConverter.ToUInt16(buffer.Array, buffer.Offset + NativeMethods.SystemTime.SecondOffset);
				result.Milliseconds = BitConverter.ToUInt16(buffer.Array, buffer.Offset + NativeMethods.SystemTime.MillisecondsOffset);
				return result;
			}

			public override bool Equals(object o)
			{
				return o is NativeMethods.SystemTime && this.Equals((NativeMethods.SystemTime)o);
			}

			public bool Equals(NativeMethods.SystemTime v)
			{
				return this.Milliseconds == v.Milliseconds && this.Second == v.Second && this.Minute == v.Minute && this.Hour == v.Hour && this.Day == v.Day && this.DayOfWeek == v.DayOfWeek && this.Month == v.Month && this.Year == v.Year;
			}

			public override int GetHashCode()
			{
				return (int)this.Hour << 28 | (int)this.Minute << 20 | (int)this.Second << 12 | (int)this.Milliseconds;
			}

			public int Write(ArraySegment<byte> buffer)
			{
				if (buffer.Count < NativeMethods.SystemTime.Size)
				{
					throw new ArgumentOutOfRangeException();
				}
				ExBitConverter.Write(this.Year, buffer.Array, buffer.Offset + NativeMethods.SystemTime.YearOffset);
				ExBitConverter.Write(this.Month, buffer.Array, buffer.Offset + NativeMethods.SystemTime.MonthOffset);
				ExBitConverter.Write(this.DayOfWeek, buffer.Array, buffer.Offset + NativeMethods.SystemTime.DayOfWeekOffset);
				ExBitConverter.Write(this.Day, buffer.Array, buffer.Offset + NativeMethods.SystemTime.DayOffset);
				ExBitConverter.Write(this.Hour, buffer.Array, buffer.Offset + NativeMethods.SystemTime.HourOffset);
				ExBitConverter.Write(this.Minute, buffer.Array, buffer.Offset + NativeMethods.SystemTime.MinuteOffset);
				ExBitConverter.Write(this.Second, buffer.Array, buffer.Offset + NativeMethods.SystemTime.SecondOffset);
				ExBitConverter.Write(this.Milliseconds, buffer.Array, buffer.Offset + NativeMethods.SystemTime.MillisecondsOffset);
				return NativeMethods.SystemTime.Size;
			}

			internal ushort Year;

			internal ushort Month;

			internal ushort DayOfWeek;

			internal ushort Day;

			internal ushort Hour;

			internal ushort Minute;

			internal ushort Second;

			internal ushort Milliseconds;

			public static readonly int Size = Marshal.SizeOf(typeof(NativeMethods.SystemTime));

			private static readonly int YearOffset = (int)Marshal.OffsetOf(typeof(NativeMethods.SystemTime), "Year");

			private static readonly int MonthOffset = (int)Marshal.OffsetOf(typeof(NativeMethods.SystemTime), "Month");

			private static readonly int DayOfWeekOffset = (int)Marshal.OffsetOf(typeof(NativeMethods.SystemTime), "DayOfWeek");

			private static readonly int DayOffset = (int)Marshal.OffsetOf(typeof(NativeMethods.SystemTime), "Day");

			private static readonly int HourOffset = (int)Marshal.OffsetOf(typeof(NativeMethods.SystemTime), "Hour");

			private static readonly int MinuteOffset = (int)Marshal.OffsetOf(typeof(NativeMethods.SystemTime), "Minute");

			private static readonly int SecondOffset = (int)Marshal.OffsetOf(typeof(NativeMethods.SystemTime), "Second");

			private static readonly int MillisecondsOffset = (int)Marshal.OffsetOf(typeof(NativeMethods.SystemTime), "Milliseconds");
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct TIME_ZONE_INFORMATION
		{
			public int Bias;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string StandardName;

			public NativeMethods.SystemTime StandardDate;

			public int StandardBias;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string DaylightName;

			public NativeMethods.SystemTime DaylightDate;

			public int DaylightBias;
		}

		internal enum TimeZoneId : uint
		{
			Unknown,
			Standard,
			DayLight,
			Invalid = 4294967295U
		}

		[Flags]
		public enum FileAttributes
		{
			None = 0,
			ReadOnly = 1,
			Hidden = 2,
			System = 4,
			Directory = 16,
			Archive = 32,
			Device = 64,
			Normal = 128,
			Temporary = 256,
			SparseFile = 512,
			ReparsePoint = 1024,
			Compressed = 2048,
			Offline = 4096,
			NonContentIndexed = 8192,
			Encrypted = 16384,
			Virtual = 65536
		}

		[BestFitMapping(false)]
		[Serializable]
		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		public struct WIN32_FIND_DATA
		{
			internal NativeMethods.FileAttributes FileAttributes;

			internal System.Runtime.InteropServices.ComTypes.FILETIME CreationTime;

			internal System.Runtime.InteropServices.ComTypes.FILETIME LastAccessTime;

			internal System.Runtime.InteropServices.ComTypes.FILETIME LastWriteTime;

			internal uint FileSizeHigh;

			internal uint FileSizeLow;

			internal uint Reserved0;

			internal uint Reserved1;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			internal string FileName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
			internal string AlternateFileName;
		}

		internal struct TokenGroups
		{
			public int count;

			public NativeMethods.SidAndAttributes sidAndAttributes;

			internal static readonly int SidAndAttributesOffset = (int)Marshal.OffsetOf(typeof(NativeMethods.TokenGroups), "sidAndAttributes");

			internal static readonly int Size = Marshal.SizeOf(typeof(NativeMethods.TokenGroups));
		}

		public struct SecurityIdentifierAndAttributes
		{
			public SecurityIdentifierAndAttributes(SecurityIdentifier sid, int attributes)
			{
				this.sid = sid;
				this.attributes = attributes;
			}

			public SecurityIdentifierAndAttributes(IntPtr pointer)
			{
				this.sid = new SecurityIdentifier(Marshal.ReadIntPtr(pointer, NativeMethods.SecurityIdentifierAndAttributes.SidOffset));
				this.attributes = Marshal.ReadInt32(pointer, NativeMethods.SecurityIdentifierAndAttributes.AttributesOffset);
			}

			public SecurityIdentifier sid;

			public int attributes;

			private static readonly int SidOffset = (int)Marshal.OffsetOf(typeof(NativeMethods.SidAndAttributes), "Sid");

			private static readonly int AttributesOffset = (int)Marshal.OffsetOf(typeof(NativeMethods.SidAndAttributes), "Attributes");

			internal static readonly int SidAndAttributesSize = Marshal.SizeOf(typeof(NativeMethods.SidAndAttributes));
		}

		internal enum ExtendedNameFormat
		{
			Unknown,
			FullyQualifiedDN,
			SamCompatible,
			Display,
			UniqueId = 6,
			Canonical,
			UserPrincipal,
			CanonicalEx,
			ServicePrincipal,
			DnsDomain = 12
		}

		internal enum JobObjectRateControlToleranceInterval
		{
			ToleranceIntervalShort = 1,
			ToleranceIntervalMedium,
			ToleranceIntervalLong
		}

		internal enum JobObjectRateControlTolerance
		{
			ToleranceLow = 1,
			ToleranceMedium,
			ToleranceHigh
		}

		[Flags]
		internal enum JobCpuRateControlLimit : uint
		{
			CpuRateControlEnable = 1U,
			CpuRateControlWeightBased = 2U,
			CpuRateControlHardCap = 4U,
			CpuRateControlNotify = 8U
		}

		[Flags]
		internal enum JobNotificationLimit : uint
		{
			JobObjectLimitRateControl = 262144U,
			JobObjectLimitJobMemory = 512U
		}

		internal enum JobObjectAccessRights : uint
		{
			JobObjectAllAccess = 2031647U
		}

		internal enum SpnWriteOperation
		{
			Add,
			Replace,
			Delete
		}

		internal enum SpnNameType
		{
			DnsHost,
			DnHost,
			NetbiosHost,
			Domain,
			NetbiosDomain,
			Service
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct DomainControllerInformation
		{
			public string DomainControllerName;

			public string DomainControllerAddress;

			public uint DomainControllerAddressType;

			public Guid DomainGuid;

			public string DomainName;

			public string DnsForestName;

			public uint Flags;

			public string DcSiteName;

			public string ClientSiteName;
		}

		[Flags]
		internal enum DsGetDCNameFlags : uint
		{
			ForceRediscovery = 1U,
			DirectoryServiceRequired = 16U,
			DirectoryServicePreferred = 32U,
			GCServerRequired = 64U,
			PDCRequired = 128U,
			BackgroundOnly = 256U,
			IPRequired = 512U,
			KDCRequired = 1024U,
			TimeServRequired = 2048U,
			WritableRequired = 4096U,
			GoodTimeServPreferred = 8192U,
			AvoidSelf = 16384U,
			OnlyLDAPNeeded = 32768U,
			IsFlatName = 65536U,
			IsDNSName = 131072U,
			ReturnDNSName = 1073741824U,
			ReturnFlatName = 2147483648U
		}

		internal struct MEMORYSTATUSEX
		{
			public uint dwLength;

			public uint dwMemoryLoad;

			public ulong ullTotalPhys;

			public ulong ullAvailPhys;

			public ulong ullTotalPageFile;

			public ulong ullAvailPageFile;

			public ulong ullTotalVirtual;

			public ulong ullAvailVirtual;

			public ulong ullAvailExtendedVirtual;
		}

		internal struct IO_COUNTERS
		{
			public ulong ReadOperationCount;

			public ulong WriteOperationCount;

			public ulong OtherOperationCount;

			public ulong ReadTransferCount;

			public ulong WriteTransferCount;

			public ulong OtherTransferCount;
		}

		internal struct JOBOBJECT_BASIC_LIMIT_INFORMATION
		{
			public long PerProcessUserTimeLimit;

			public long PerJobUserTimeLimit;

			public uint LimitFlags;

			public UIntPtr MinimumWorkingSetSize;

			public UIntPtr MaximumWorkingSetSize;

			public uint ActiveProcessLimit;

			public IntPtr Affinity;

			public uint PriorityClass;

			public uint SchedulingClass;
		}

		internal struct JOBOBJECT_EXTENDED_LIMIT_INFORMATION
		{
			public NativeMethods.JOBOBJECT_BASIC_LIMIT_INFORMATION BasicLimitInformation;

			public NativeMethods.IO_COUNTERS IoInfo;

			public UIntPtr ProcessMemoryLimit;

			public UIntPtr JobMemoryLimit;

			public UIntPtr PeakProcessMemoryUsed;

			public UIntPtr PeakJobMemoryUsed;
		}

		internal struct JOBOBJECT_CPU_RATE_CONTROL_INFORMATION
		{
			public uint ControlFlags;

			public uint CpuRate;
		}

		internal struct JOBOBJECT_NOTIFICATION_LIMIT_INFORMATION
		{
			public ulong IoReadBytesLimit;

			public ulong IoWriteBytesLimit;

			public long PerJobUserTimeLimit;

			public ulong JobMemoryLimit;

			public NativeMethods.JobObjectRateControlTolerance RateControlTolerance;

			public NativeMethods.JobObjectRateControlToleranceInterval RateControlToleranceInterval;

			public uint LimitFlags;
		}

		internal struct JOBOBJECT_LIMIT_VIOLATION_INFORMATION
		{
			public uint LimitFlags;

			public uint ViolationLimitFlags;

			public ulong IoReadBytes;

			public ulong IoReadBytesLimit;

			public ulong IoWriteBytes;

			public ulong IoWriteBytesLimit;

			public long PerJobUserTime;

			public long PerJobUserTimeLimit;

			public ulong JobMemory;

			public ulong JobMemoryLimit;

			public NativeMethods.JobObjectRateControlTolerance RateControlTolerance;

			public NativeMethods.JobObjectRateControlTolerance RateControlToleranceLimit;
		}

		internal struct JobObjectAssociateCompletionPort
		{
			public JobObjectAssociateCompletionPort(IntPtr completionKey, IntPtr completionPort)
			{
				this.completionKey = completionKey;
				this.completionPort = completionPort;
			}

			private IntPtr completionKey;

			private IntPtr completionPort;
		}

		internal struct JobObjectBasicProcessIdList
		{
			public uint NumberOfAssignedProcess;

			public uint NumberOfProcessIdsInList;

			public UIntPtr ProcessIdList;
		}

		public enum JOBOBJECTINFOCLASS
		{
			JobObjectBasicAccountingInformation = 1,
			JobObjectBasicLimitInformation,
			JobObjectBasicProcessIdList,
			JobObjectBasicUIRestrictions,
			JobObjectSecurityLimitInformation,
			JobObjectEndOfJobTimeInformation,
			JobObjectAssociateCompletionPortInformation,
			JobObjectBasicAndIoAccountingInformation,
			JobObjectExtendedLimitInformation,
			JobObjectJobSetInformation,
			JobObjectGroupInformation,
			JobObjectNotificationLimitInformation,
			JobObjectLimitViolationInformation,
			JobObjectGroupInformationEx,
			JobObjectCpuRateControlInformation,
			JobObjectCompletionFilter,
			JobObjectCompletionCounter,
			JobObjectReserved1Information,
			JobObjectReserved2Information,
			JobObjectReserved3Information,
			JobObjectReserved4Information,
			JobObjectReserved5Information,
			JobObjectReserved6Information,
			JobObjectReserved7Information,
			JobObjectReserved8Information,
			MaxJobObjectInfoClass
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct STARTUPINFO
		{
			private int cb;

			private string lpReserved;

			private string lpDesktop;

			private string lpTitle;

			private int dwX;

			private int dwY;

			private int dwXSize;

			private int dwYSize;

			private int dwXCountChars;

			private int dwYCountChars;

			private int dwFillAttribute;

			private int dwFlags;

			private short wShowWindow;

			private short cbReserved2;

			private IntPtr lpReserved2;

			private IntPtr hStdInput;

			private IntPtr hStdOutput;

			private IntPtr hStdError;
		}

		internal struct PROCESS_INFORMATION
		{
			public IntPtr hProcess;

			public IntPtr hThread;

			public int dwProcessId;

			public int dwThreadId;
		}

		[Flags]
		internal enum ProcessAccess
		{
			DupHandle = 64,
			QueryInformation = 1024,
			Synchronize = 1048576,
			SetQuota = 256,
			Terminate = 1
		}

		internal enum TOKEN_INFORMATION_CLASS
		{
			TokenUser = 1,
			TokenGroups,
			TokenPrivileges,
			TokenOwner,
			TokenPrimaryGroup,
			TokenDefaultDacl,
			TokenSource,
			TokenType,
			TokenImpersonationLevel,
			TokenStatistics,
			TokenRestrictedSids,
			TokenSessionId,
			TokenGroupsAndPrivileges,
			TokenSessionReference,
			TokenSandBoxInert,
			TokenAuditPolicy,
			TokenOrigin
		}

		[Flags]
		internal enum TOKEN_PRIVILEGES_ATTRIBUTES : uint
		{
			SE_PRIVILEGE_DISABLED = 0U,
			SE_PRIVILEGE_ENABLED_BY_DEFAULT = 1U,
			SE_PRIVILEGE_ENABLED = 2U,
			SE_PRIVILEGE_REMOVED = 4U,
			SE_PRIVILEGE_USED_FOR_ACCESS = 2147483648U
		}

		internal struct TOKEN_PRIVILEGES
		{
			internal uint PrivilegeCount;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
			internal NativeMethods.LUID_AND_ATTRIBUTES[] Privileges;
		}

		internal struct LUID_AND_ATTRIBUTES
		{
			internal NativeMethods.LUID Luid;

			internal uint Attributes;
		}

		internal struct LUID
		{
			internal uint LowPart;

			internal uint HighPart;
		}

		[Flags]
		internal enum PipeOpen : uint
		{
			AccessInbound = 1U,
			AccessOutbound = 2U,
			AccessDuplex = 3U,
			FileFlagWriteThrough = 2147483648U
		}

		[Flags]
		internal enum PipeModes : uint
		{
			TypeByte = 0U,
			TypeMessage = 4U,
			ReadModeByte = 0U,
			ReadModeMessage = 2U
		}

		[Flags]
		internal enum CreateFileAccess : uint
		{
			GenericRead = 2147483648U,
			GenericWrite = 1073741824U,
			GenericExecute = 536870912U,
			GenericAll = 268435456U,
			FileWriteAttributes = 256U
		}

		[Flags]
		internal enum CreateFileShare : uint
		{
			None = 0U,
			Read = 1U,
			Write = 2U,
			Delete = 4U
		}

		[Flags]
		internal enum CreateFileFileAttributes : uint
		{
			None = 0U,
			Readonly = 1U,
			Hidden = 2U,
			System = 4U,
			Directory = 16U,
			Archive = 32U,
			Device = 64U,
			Normal = 128U,
			Temporary = 256U,
			SparseFile = 512U,
			ReparsePoint = 1024U,
			Compressed = 2048U,
			Offline = 4096U,
			NotContentIndexed = 8192U,
			Encrypted = 16384U,
			Write_Through = 2147483648U,
			Overlapped = 1073741824U,
			NoBuffering = 536870912U,
			RandomAccess = 268435456U,
			SequentialScan = 134217728U,
			DeleteOnClose = 67108864U,
			BackupSemantics = 33554432U,
			PosixSemantics = 16777216U,
			OpenReparsePoint = 2097152U,
			OpenNoRecall = 1048576U,
			FirstPipeInstance = 524288U
		}

		public struct MemoryStatusEx
		{
			public static readonly uint Size = (uint)Marshal.SizeOf(typeof(NativeMethods.MemoryStatusEx));

			public uint Length;

			public uint MemoryLoad;

			public ulong TotalPhys;

			public ulong AvailPhys;

			public ulong TotalPageFile;

			public ulong AvailPageFile;

			public ulong TotalVirtual;

			public ulong AvailVirtual;

			public ulong AvailExtendedVirtual;
		}

		public struct ProcessMemoryCounterEx
		{
			public static readonly uint Size = (uint)Marshal.SizeOf(typeof(NativeMethods.ProcessMemoryCounterEx));

			public uint cb;

			public uint pageFaultCount;

			public UIntPtr peakWorkingSetSize;

			public UIntPtr workingSetSize;

			public UIntPtr quotaPeakPagedPoolUsage;

			public UIntPtr quotaPagedPoolUsage;

			public UIntPtr quotaPeakNonPagedPoolUsage;

			public UIntPtr quotaNonPagedPoolUsage;

			public UIntPtr pagefileUsage;

			public UIntPtr peakPagefileUsage;

			public UIntPtr privateUsage;
		}

		[Flags]
		internal enum MemoryAccessControl : uint
		{
			Readonly = 2U,
			ReadWrite = 4U,
			WriteCopy = 8U,
			ExecuteRead = 32U,
			ExecuteReadWrite = 64U,
			SecFile = 8388608U,
			SecImage = 16777216U,
			SecProtectedImage = 33554432U,
			SecReserver = 67108864U,
			SecCommit = 134217728U,
			SecNoCache = 268435456U,
			SecWriteCombine = 1073741824U,
			SecLargePages = 2147483648U
		}

		internal enum FileMapAccessControl : uint
		{
			Copy = 1U,
			Write,
			Read = 4U,
			Execute = 32U
		}

		public struct SECURITY_ATTRIBUTES
		{
			public SECURITY_ATTRIBUTES(SafeHGlobalHandle securityDescriptor)
			{
				this.nLength = NativeMethods.SECURITY_ATTRIBUTES.Size;
				this.lpSecurityDescriptor = securityDescriptor;
				this.bInheritHandle = false;
			}

			public static readonly uint Size = (uint)Marshal.SizeOf(typeof(NativeMethods.SECURITY_ATTRIBUTES));

			internal uint nLength;

			internal SafeHGlobalHandle lpSecurityDescriptor;

			[MarshalAs(UnmanagedType.Bool)]
			internal bool bInheritHandle;
		}

		public struct GENERIC_MAPPING
		{
			internal static int StructSize = Marshal.SizeOf(typeof(NativeMethods.GENERIC_MAPPING));

			internal uint GenericRead;

			internal uint GenericWrite;

			internal uint GenericExecute;

			internal uint GenericAll;
		}

		internal enum ComputerNameFormat
		{
			NetBios,
			DnsHostname,
			DnsDomain,
			DnsFullyQualified,
			PhysicalNetBios,
			PhysicalDnsHostname,
			PhysicalDnsDomain,
			PhysicalDnsFullyQualified
		}

		internal enum DSNameError
		{
			NoError,
			ErrorResolving,
			ErrorNotFound,
			ErrorNotUnique,
			ErrorNoMapping,
			ErrorDomainOnly,
			ErrorNoSyntacticalMapping,
			ErrorTrustReferral
		}

		[Flags]
		internal enum DSNameFlags
		{
			NoFlags = 0,
			SyntacticalOnly = 1,
			EvalAtDC = 2,
			FlagGCVerify = 4,
			TrustReferral = 8
		}

		internal enum DSNameFormat
		{
			UnknownName,
			FQDN1779Name,
			NT4AccountName,
			DisplayName,
			UniqueIdName = 6,
			CanonicalName,
			UserPrincipalName,
			CanonicalNameEx,
			ServicePrincipalName,
			SidOrSidHistoryName,
			DnsDomainName
		}

		internal struct DSNameResultItem
		{
			public NativeMethods.DSNameError Status;

			public string Domain;

			public string Name;
		}

		internal struct DSNameResult
		{
			public uint Count;

			public IntPtr Items;
		}
	}
}
