using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Diagnostics
{
	[ComVisible(false)]
	[SuppressUnmanagedCodeSecurity]
	internal static class DiagnosticsNativeMethods
	{
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("kernel32.dll")]
		internal static extern int GetCurrentThreadId();

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("kernel32.dll")]
		internal static extern int GetCurrentProcessId();

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "StartTraceW")]
		internal static extern uint StartTrace(out long sessionHandle, [In] string sessionName, [In] [Out] ref DiagnosticsNativeMethods.EventTraceProperties properties);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "FlushTraceW")]
		internal static extern uint FlushTrace([In] long sessionHandle, [In] string sessionName, [In] [Out] ref DiagnosticsNativeMethods.EventTraceProperties properties);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "ControlTraceW")]
		internal static extern uint ControlTrace([In] long sessionHandle, [In] string sessionName, [In] [Out] ref DiagnosticsNativeMethods.EventTraceProperties properties, [In] uint controlCode);

		[DllImport("advapi32.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal unsafe static extern uint TraceMessage([In] long sessionHandle, [In] uint messageFlags, [In] ref Guid messageGuid, [In] ushort messageNumber, [In] byte* buffer, [In] int bufferLength, [In] IntPtr terminationPtr, [In] int terminationSize);

		[DllImport("advapi32.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern uint TraceMessage([In] long sessionHandle, [In] uint messageFlags, [In] ref Guid messageGuid, [In] ushort messageNumber, [In] ref int traceTag, [In] int sizeOfTraceTag, [In] string message, [In] int messageLengthInBytes, [In] ref long userId, [In] int sizeOfuserId, [In] ref int locationId, [In] int sizeOflocationId, [In] IntPtr terminationPtr, [In] int terminationSize);

		[DllImport("advapi32.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
		internal static extern uint TraceMessage([In] long sessionHandle, [In] uint messageFlags, [In] ref Guid messageGuid, [In] ushort messageNumber, [In] ref int startStop, [In] int sizeOfStartStop, [In] byte[] clientRequestID, [In] int sizeOfClientRequestID, [In] byte[] serviceProviderRequestID, [In] int sizeOfServiceProviderRequestID, [In] ref int bytesIn, [In] int sizeOfBytesIn, [In] ref int bytesOut, [In] int sizeOfBytesOut, [In] string serverAddress, [In] int sizeOfServerAddress, [In] string userContext, [In] int sizeOfUserContext, [In] string SpOp, [In] int sizeOfSpOp, [In] string SpOpData, [In] int sizeOfSpOpData, [In] string ClientOp, [In] int sizeOfClientOp, [In] IntPtr terminationPtr, [In] int terminationSize);

		[DllImport("advapi32.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint EnableTrace([In] uint enable, [In] uint enableFlag, [In] uint enableLevel, [In] ref Guid controlGuid, [In] long sessionHandle);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("kernel32.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool CloseHandle(IntPtr hObject);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		public static extern DiagnosticsNativeMethods.ErrorCode RegOpenKeyEx(SafeRegistryHandle parent, string subKey, int options, int samDesired, out SafeRegistryHandle key);

		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		public static extern DiagnosticsNativeMethods.ErrorCode RegNotifyChangeKeyValue([In] SafeRegistryHandle key, [MarshalAs(UnmanagedType.Bool)] [In] bool watchSubtree, [In] DiagnosticsNativeMethods.RegistryNotifications notifyFilter, [In] SafeWaitHandle notifyEvent, [MarshalAs(UnmanagedType.Bool)] [In] bool asynchronous);

		[SuppressUnmanagedCodeSecurity]
		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("advapi32.dll")]
		public static extern DiagnosticsNativeMethods.ErrorCode RegCloseKey([In] IntPtr key);

		[DllImport("advapi32.dll")]
		public static extern uint CreateTraceInstanceId([In] IntPtr registrationHandle, [In] [Out] ref DiagnosticsNativeMethods.EventInstanceInfo eventInstanceInfo);

		[DllImport("advapi32.dll")]
		public static extern uint TraceEventInstance([In] long sessionHandle, [MarshalAs(UnmanagedType.LPArray)] [In] byte[] eventTrace, [In] ref DiagnosticsNativeMethods.EventInstanceInfo eventInstanceInfo, [In] ref DiagnosticsNativeMethods.EventInstanceInfo parentEventInstanceInfo);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("Kernel32.dll", SetLastError = true)]
		internal static extern int GetModuleFileName(IntPtr hModule, StringBuilder lpFileName, int nSize);

		[DllImport("kernel32", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool FlushFileBuffers(SafeFileHandle handle);

		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetShortPathNameW", SetLastError = true)]
		internal static extern int GetShortPathName([In] string longPath, [MarshalAs(UnmanagedType.LPArray)] [In] [Out] char[] shortPath, [In] int bufferSize);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern ProcSafeHandle OpenProcess(DiagnosticsNativeMethods.ProcessAccess dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern ThreadSafeHandle OpenThread(DiagnosticsNativeMethods.ThreadAccess desiredAccess, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, int threadId);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern int SuspendThread(ThreadSafeHandle thread);

		[DllImport("kernel32.dll", SetLastError = true)]
		internal static extern int ResumeThread(ThreadSafeHandle thread);

		[DllImport("wer.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
		internal static extern WerSafeHandle WerReportCreate([In] string eventType, [In] DiagnosticsNativeMethods.WER_REPORT_TYPE repType, [In] [Optional] DiagnosticsNativeMethods.WER_REPORT_INFORMATION reportInformation);

		[DllImport("wer.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
		internal static extern void WerReportSetParameter([In] WerSafeHandle reportHandle, [In] uint paramID, [MarshalAs(UnmanagedType.LPWStr)] [In] [Optional] string name, [MarshalAs(UnmanagedType.LPWStr)] [In] string value);

		[DllImport("wer.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
		internal static extern void WerReportAddFile([In] WerSafeHandle reportHandle, [MarshalAs(UnmanagedType.LPWStr)] [In] string path, [In] DiagnosticsNativeMethods.WER_FILE_TYPE fileType, [In] DiagnosticsNativeMethods.WER_FILE_FLAGS fileFlags);

		[DllImport("wer.dll", PreserveSig = false)]
		internal static extern void WerReportAddDump([In] WerSafeHandle reportHandle, [In] ProcSafeHandle processHandle, [In] [Optional] IntPtr threadHandle, [In] DiagnosticsNativeMethods.WER_DUMP_TYPE dumpType, [In] [Optional] DiagnosticsNativeMethods.WER_EXCEPTION_INFORMATION exceptionParam, [In] [Optional] DiagnosticsNativeMethods.WER_DUMP_CUSTOM_OPTIONS dumpCustomOptions, [In] uint flags);

		[DllImport("kernel32.dll", PreserveSig = false)]
		internal static extern void WerSetFlags(DiagnosticsNativeMethods.WER_FLAGS flags);

		[DllImport("wer.dll", PreserveSig = false)]
		internal static extern int WerReportSubmit([In] WerSafeHandle reportHandle, [In] DiagnosticsNativeMethods.WER_CONSENT consent, [In] DiagnosticsNativeMethods.WER_SUBMIT_FLAGS flags, IntPtr submitResult);

		[DllImport("ExWatson.dll", PreserveSig = false)]
		internal static extern int SubmitExWatsonReport([In] WerSafeHandle reportHandle, [In] DiagnosticsNativeMethods.WER_CONSENT consent, [In] DiagnosticsNativeMethods.WER_SUBMIT_FLAGS flags, IntPtr submitResult, [In] bool fDumpRequested, [In] bool fProcessTerminating);

		[DllImport("wer.dll", PreserveSig = false)]
		internal static extern void WerReportCloseHandle([In] IntPtr reportHandle);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("kernel32.dll")]
		internal static extern int TerminateProcess(IntPtr hProcess, int exitCode);

		[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[DllImport("kernel32.dll")]
		internal static extern int ExitProcess(int exitCode);

		[DllImport("advapi32.dll", CallingConvention = CallingConvention.Cdecl)]
		internal static extern uint TraceEvent([In] long sessionHandle, [In] ref DiagnosticsNativeMethods.BinaryEventTrace binaryEventTrace);

		private const string ADVAPI32 = "advapi32.dll";

		private const int KEY_QUERY_VALUE = 1;

		private const int KEY_ENUMERATE_SUB_KEYS = 8;

		private const int KEY_NOTIFY = 16;

		private const int STANDARD_RIGHTS_READ = 131072;

		private const int SYNCHRONIZE = 1048576;

		public const int KEY_READ = 131097;

		private const int MAX_PATH = 260;

		internal const uint WnodeFlagTracedGuid = 131072U;

		internal const uint WnodeFlagUseMofPtr = 1048576U;

		internal const uint EVENT_TRACE_CONTROL_QUERY = 0U;

		internal const uint EVENT_TRACE_CONTROL_STOP = 1U;

		internal const uint EVENT_TRACE_CONTROL_UPDATE = 2U;

		internal const uint EVENT_TRACE_CONTROL_FLUSH = 3U;

		internal const int WMI_ENABLE_EVENTS = 4;

		internal const int WMI_DISABLE_EVENTS = 5;

		internal const uint TRACE_MESSAGE_SEQUENCE = 1U;

		internal const uint TRACE_MESSAGE_GUID = 2U;

		internal const uint TRACE_MESSAGE_TIMESTAMP = 8U;

		internal const uint TRACE_MESSAGE_SYSTEMINFO = 32U;

		internal const int TRACE_MESSAGE_MAXIMUM_SIZE = 8192;

		internal const uint TRACE_DISABLE = 0U;

		internal const uint TRACE_ENABLE = 1U;

		internal const uint TRACE_LEVEL_CRITICAL = 1U;

		internal const uint TRACE_LEVEL_ERROR = 2U;

		internal const uint TRACE_LEVEL_WARNING = 3U;

		internal const uint TRACE_LEVEL_INFORMATION = 4U;

		internal const uint TRACE_LEVEL_VERBOSE = 5U;

		internal const int MAXIMUM_SUSPEND_COUNT = 127;

		private static readonly ushort BinaryEventTraceSize = (ushort)(Marshal.SizeOf(typeof(DiagnosticsNativeMethods.EventTraceHeader)) + Marshal.SizeOf(typeof(DiagnosticsNativeMethods.MofField)));

		public enum ErrorCode
		{
			Success,
			FileNotFound = 2,
			InvalidHandle = 6,
			NotEnoughMemory = 8,
			OutOfMemory = 14,
			DiskFull = 112,
			KeyDeleted = 1018,
			NoMoreItems = 259,
			MoreData = 234
		}

		internal enum ExceptionFlag
		{
			Continuable,
			NonContinuable = -1073741787
		}

		[Flags]
		public enum LogFileMode : uint
		{
			EVENT_TRACE_FILE_MODE_NONE = 0U,
			EVENT_TRACE_FILE_MODE_SEQUENTIAL = 1U,
			EVENT_TRACE_FILE_MODE_CIRCULAR = 2U,
			EVENT_TRACE_FILE_MODE_APPEND = 4U,
			EVENT_TRACE_FILE_MODE_NEWFILE = 8U,
			EVENT_TRACE_FILE_MODE_PREALLOCATE = 32U,
			EVENT_TRACE_SECURE_MODE = 128U,
			EVENT_TRACE_REAL_TIME_MODE = 256U,
			EVENT_TRACE_BUFFERING_MODE = 1024U,
			EVENT_TRACE_PRIVATE_LOGGER_MODE = 2048U,
			EVENT_TRACE_USE_KBYTES_FOR_SIZE = 8192U,
			EVENT_TRACE_USE_GLOBAL_SEQUENCE = 16384U,
			EVENT_TRACE_USE_LOCAL_SEQUENCE = 32768U,
			EVENT_TRACE_RELOG_MODE = 65536U,
			EVENT_TRACE_PRIVATE_IN_PROC = 131072U,
			EVENT_TRACE_USE_PAGED_MEMORY = 16777216U
		}

		[Flags]
		internal enum ProcessAccess
		{
			Terminate = 1,
			CreateThread = 2,
			SetSessionId = 4,
			VmOperation = 8,
			VmRead = 16,
			VmWrite = 32,
			DupHandle = 64,
			CreateProcess = 128,
			SetQuota = 256,
			SetInformation = 512,
			QueryInformation = 1024,
			SuspendResume = 2048,
			StandardRightsRead = 131072,
			StandardRightsRequired = 983040,
			Synchronize = 1048576,
			AllAccess = 2035711
		}

		[Flags]
		internal enum ThreadAccess
		{
			Synchronize = 1048576,
			Terminate = 1,
			SuspendResume = 2,
			GetContext = 8,
			SetContext = 16,
			SetInformation = 32,
			QueryInformation = 64,
			SetThreadToken = 128,
			Impersonate = 256,
			DirectImpersonation = 512,
			SetLimitedInformation = 1024,
			QueryLimitedInformation = 2048,
			StandardRightsRequired = 983040,
			AllAccess = 2097151
		}

		internal enum WER_FLAGS
		{
			WER_FAULT_REPORTING_FLAG_NOHEAP = 1,
			WER_FAULT_REPORTING_FLAG_QUEUE,
			WER_FAULT_REPORTING_FLAG_DISABLE_THREAD_SUSPENSION = 4,
			WER_FAULT_REPORTING_FLAG_QUEUE_UPLOAD = 8
		}

		internal enum WER_REPORT_TYPE
		{
			WerReportNonCritical,
			WerReportCritical,
			WerReportApplicationCrash,
			WerReportApplicationHang,
			WerReportKernel,
			WerReportInvalid
		}

		[Flags]
		internal enum WER_FILE_FLAGS : uint
		{
			WER_FILE_DELETE_WHEN_DONE = 1U,
			WER_FILE_ANONYMOUS_DATA = 2U
		}

		[Flags]
		internal enum WER_DUMP_FLAGS
		{
			WER_DUMP_MASK_DUMPTYPE = 1,
			WER_DUMP_MASK_ONLY_THISTHREAD = 2,
			WER_DUMP_MASK_THREADFLAGS = 4,
			WER_DUMP_MASK_THREADFLAGS_EX = 8,
			WER_DUMP_MASK_OTHERTHREADFLAGS = 16,
			WER_DUMP_MASK_OTHERTHREADFLAGS_EX = 32,
			WER_DUMP_MASK_PREFERRED_MODULESFLAGS = 64,
			WER_DUMP_MASK_OTHER_MODULESFLAGS = 128,
			WER_DUMP_MASK_PREFERRED_MODULE_LIST = 256
		}

		[Flags]
		internal enum MINIDUMP_TYPE
		{
			MiniDumpNormal = 0,
			MiniDumpWithDataSegs = 1,
			MiniDumpWithFullMemory = 2,
			MiniDumpWithHandleData = 4,
			MiniDumpFilterMemory = 8,
			MiniDumpScanMemory = 16,
			MiniDumpWithUnloadedModules = 32,
			MiniDumpWithIndirectlyReferencedMemory = 64,
			MiniDumpFilterModulePaths = 128,
			MiniDumpWithProcessThreadData = 256,
			MiniDumpWithPrivateReadWriteMemory = 512,
			MiniDumpWithoutOptionalData = 1024,
			MiniDumpWithFullMemoryInfo = 2048,
			MiniDumpWithThreadInfo = 4096
		}

		internal enum WER_CONSENT
		{
			WerConsentNotAsked = 1,
			WerConsentApproved,
			WerConsentDenied
		}

		[Flags]
		internal enum WER_SUBMIT_FLAGS
		{
			WER_SUBMIT_HONOR_RECOVERY = 1,
			WER_SUBMIT_HONOR_RESTART = 2,
			WER_SUBMIT_QUEUE = 4,
			WER_SUBMIT_SHOW_DEBUG = 8,
			WER_SUBMIT_ADD_REGISTERED_DATA = 16,
			WER_SUBMIT_OUTOFPROCESS = 32,
			WER_SUBMIT_NO_CLOSE_UI = 64,
			WER_SUBMIT_NO_QUEUE = 128,
			WER_SUBMIT_NO_ARCHIVE = 256,
			WER_SUBMIT_START_MINIMIZED = 512,
			WER_SUBMIT_OUTOFPROCESS_ASYNC = 1024,
			WER_SUBMIT_BYPASS_DATA_THROTTLING = 2048
		}

		internal enum WER_FILE_TYPE
		{
			WerFileTypeMicrodump = 1,
			WerFileTypeMinidump,
			WerFileTypeHeapdump,
			WerFileTypeUserDocument,
			WerFileTypeOther
		}

		public enum WER_SUBMIT_RESULT
		{
			WerReportQueued = 1,
			WerReportUploaded,
			WerReportDebug,
			WerReportFailed,
			WerDisabled,
			WerReportCancelled,
			WerDisabledQueue,
			WerReportAsync
		}

		public enum WER_DUMP_TYPE
		{
			WerDumpTypeMicroDump = 1,
			WerDumpTypeMiniDump,
			WerDumpTypeHeapDump
		}

		[Flags]
		public enum RegistryNotifications : uint
		{
			None = 0U,
			ChangeName = 1U,
			ChangeAttributes = 2U,
			LastSet = 4U,
			ChangeSecurity = 8U
		}

		[StructLayout(LayoutKind.Explicit)]
		internal struct WNodeHeader
		{
			[FieldOffset(0)]
			public uint bufferSize;

			[FieldOffset(4)]
			public uint providerId;

			[FieldOffset(8)]
			public uint version;

			[FieldOffset(12)]
			public uint linkage;

			[FieldOffset(16)]
			public IntPtr kernelHandle;

			[FieldOffset(24)]
			public Guid guid;

			[FieldOffset(40)]
			public uint clientContext;

			[FieldOffset(44)]
			public uint flags;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct EventTraceProperties_Inner
		{
			public DiagnosticsNativeMethods.WNodeHeader wnode;

			public uint bufferSize;

			public uint minimumBuffers;

			public uint maximumBuffers;

			public uint maximumFileSize;

			public uint logFileMode;

			public uint flushTimer;

			public uint enableFlags;

			public int ageLimit;

			public uint numberOfBuffers;

			public uint freeBuffers;

			public uint eventsLost;

			public uint buffersWritten;

			public uint logBuffersLost;

			public uint realTimeBuffersLost;

			public IntPtr loggerThreadId;

			public uint logFileNameOffset;

			public uint loggerNameOffset;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct EventTraceHeader
		{
			public EventTraceHeader(Guid eventClassGuid, byte eventTypeId)
			{
				this.eventClassGuid = eventClassGuid;
				this.eventTypeId = eventTypeId;
				this.flags = 1048576U;
				this.size = DiagnosticsNativeMethods.BinaryEventTraceSize;
				this.fieldTypeFlags = 0;
				this.level = 0;
				this.version = 0;
				this.threadId = 0U;
				this.processId = 0U;
				this.timeStamp = 0UL;
				this.clientContext = 0U;
			}

			private ushort size;

			private ushort fieldTypeFlags;

			private byte eventTypeId;

			private byte level;

			private ushort version;

			private uint threadId;

			private uint processId;

			private ulong timeStamp;

			private Guid eventClassGuid;

			private uint clientContext;

			private uint flags;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct MofField
		{
			public GCHandle SetData(byte[] dataBuffer)
			{
				GCHandle result = GCHandle.Alloc(dataBuffer, GCHandleType.Pinned);
				this.dataPtr = (ulong)result.AddrOfPinnedObject().ToInt64();
				this.length = (uint)dataBuffer.Length;
				return result;
			}

			[MarshalAs(UnmanagedType.U8)]
			private ulong dataPtr;

			[MarshalAs(UnmanagedType.U4)]
			private uint length;

			[MarshalAs(UnmanagedType.U4)]
			private uint dataType;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal struct BinaryEventTrace
		{
			public BinaryEventTrace(Guid componentGuid, byte traceTag, byte[] traceData, out GCHandle? pinnedMemory)
			{
				this.header = new DiagnosticsNativeMethods.EventTraceHeader(componentGuid, traceTag);
				this.mofField = default(DiagnosticsNativeMethods.MofField);
				pinnedMemory = new GCHandle?(this.mofField.SetData(traceData));
			}

			[MarshalAs(UnmanagedType.Struct)]
			private DiagnosticsNativeMethods.EventTraceHeader header;

			[MarshalAs(UnmanagedType.Struct)]
			private DiagnosticsNativeMethods.MofField mofField;
		}

		[StructLayout(LayoutKind.Explicit, CharSet = CharSet.Unicode)]
		public struct EventTraceProperties
		{
			[FieldOffset(0)]
			public DiagnosticsNativeMethods.EventTraceProperties_Inner etp;

			[FieldOffset(120)]
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
			public string loggerName;

			[FieldOffset(2168)]
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 1024)]
			public string logFileName;
		}

		internal struct TraceGuidRegistration
		{
			public unsafe Guid* guid;

			public IntPtr handle;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal class WER_REPORT_INFORMATION
		{
			public uint size;

			public IntPtr process;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
			public string consentKey;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string friendlyEventName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string applicationName;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public string applicationPath;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 512)]
			public string description;

			public IntPtr parentWindowHandle;
		}

		[StructLayout(LayoutKind.Sequential)]
		internal class WER_EXCEPTION_INFORMATION
		{
			public IntPtr exceptionPointers;

			public bool clientPointers;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal class WER_DUMP_CUSTOM_OPTIONS
		{
			public int size;

			public DiagnosticsNativeMethods.WER_DUMP_FLAGS mask;

			public DiagnosticsNativeMethods.MINIDUMP_TYPE dumpFlags;

			[MarshalAs(UnmanagedType.Bool)]
			public bool onlyThisThread;

			public uint exceptionThreadFlags;

			public uint otherThreadFlags;

			public uint exceptionThreadExFlags;

			public uint otherThreadExFlags;

			public uint preferredModuleFlags;

			public uint otherModuleFlags;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
			public string preferredModuleList;
		}

		internal struct ExceptionPointers
		{
			public IntPtr ExceptionRecord;

			public IntPtr ContextRecord;

			public static DiagnosticsNativeMethods.ExceptionPointers Empty = default(DiagnosticsNativeMethods.ExceptionPointers);
		}

		internal struct ExceptionRecord
		{
			public const int AccessViolation = -1073741819;

			public const int ArrayBoundsExceeded = -1073741684;

			public const int Breakpoint = -2147483645;

			public const int DatatypeMisalignment = -2147483646;

			public const int FloatDenormalOperand = -1073741683;

			public const int FloatDivideByZero = -1073741682;

			public const int FloatInexactResult = -1073741681;

			public const int FloatInvalidOperation = -1073741680;

			public const int FloatOverflow = -1073741679;

			public const int FloatStackCheck = -1073741678;

			public const int FloatUnderflow = -1073741677;

			public const int IllegalInstruction = -1073741795;

			public const int PageError = -1073741818;

			public const int IntegerDivideByZero = -1073741676;

			public const int IntegerOverflow = -1073741675;

			public const int InvalidDisposition = -1073741786;

			public const int NonContinuableException = -1073741787;

			public const int PrivilegedInstruction = -1073741674;

			public const int SingleStep = -2147483644;

			public const int StackOverflow = -1073741571;

			public int ExceptionCode;

			public DiagnosticsNativeMethods.ExceptionFlag ExceptionFlags;

			public IntPtr InnerExceptionRecord;

			public IntPtr ExceptionAddress;

			public int NumberParameters;

			public IntPtr ExceptionInformation;

			public static DiagnosticsNativeMethods.ExceptionRecord Empty = default(DiagnosticsNativeMethods.ExceptionRecord);
		}

		internal delegate uint ControlCallback(int requestCode, IntPtr context, IntPtr reserved, IntPtr buffer);

		internal sealed class CriticalTraceRegistrationHandle : CriticalHandle
		{
			private CriticalTraceRegistrationHandle() : base(IntPtr.Zero)
			{
				this.traceHandle = -1L;
			}

			private void Initialize(DiagnosticsNativeMethods.ControlCallback callback, long handle)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					this.callback = callback;
					this.traceHandle = handle;
				}
			}

			public override bool IsInvalid
			{
				get
				{
					return this.traceHandle == -1L;
				}
			}

			[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegisterTraceGuidsW")]
			private static extern int RegisterTraceGuids([In] DiagnosticsNativeMethods.ControlCallback callback, [In] IntPtr context, [In] ref Guid controlGuid, [In] int guidCount, [In] ref DiagnosticsNativeMethods.TraceGuidRegistration guidRegistration, [In] string mofImagePath, [In] string mofResourceName, out long registrationHandle);

			[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegisterTraceGuidsW")]
			private static extern int RegisterTraceGuids([In] DiagnosticsNativeMethods.ControlCallback callback, [In] IntPtr context, [In] ref Guid controlGuid, [In] int guidCount, IntPtr guidRegistrations, [In] string mofImagePath, [In] string mofResourceName, out long registrationHandle);

			public static DiagnosticsNativeMethods.CriticalTraceRegistrationHandle RegisterTrace(Guid provider, ref DiagnosticsNativeMethods.TraceGuidRegistration guidRegistration, DiagnosticsNativeMethods.ControlCallback callback)
			{
				DiagnosticsNativeMethods.CriticalTraceRegistrationHandle criticalTraceRegistrationHandle = new DiagnosticsNativeMethods.CriticalTraceRegistrationHandle();
				long handle;
				int num = DiagnosticsNativeMethods.CriticalTraceRegistrationHandle.RegisterTraceGuids(callback, IntPtr.Zero, ref provider, 1, ref guidRegistration, null, null, out handle);
				if (num != 0)
				{
					throw new Win32Exception(num);
				}
				criticalTraceRegistrationHandle.Initialize(callback, handle);
				return criticalTraceRegistrationHandle;
			}

			public static DiagnosticsNativeMethods.CriticalTraceRegistrationHandle RegisterTrace(Guid provider, DiagnosticsNativeMethods.ControlCallback callback)
			{
				DiagnosticsNativeMethods.CriticalTraceRegistrationHandle criticalTraceRegistrationHandle = new DiagnosticsNativeMethods.CriticalTraceRegistrationHandle();
				long handle;
				int num = DiagnosticsNativeMethods.CriticalTraceRegistrationHandle.RegisterTraceGuids(callback, IntPtr.Zero, ref provider, 0, IntPtr.Zero, null, null, out handle);
				if (num != 0)
				{
					throw new Win32Exception(num);
				}
				criticalTraceRegistrationHandle.Initialize(callback, handle);
				return criticalTraceRegistrationHandle;
			}

			[DllImport("advapi32.dll")]
			private static extern uint UnregisterTraceGuids([In] long registrationHandle);

			protected override bool ReleaseHandle()
			{
				if (!this.IsInvalid)
				{
					DiagnosticsNativeMethods.CriticalTraceRegistrationHandle.UnregisterTraceGuids(this.traceHandle);
					this.traceHandle = -1L;
				}
				return true;
			}

			protected override void Dispose(bool disposing)
			{
				if (!base.IsClosed && disposing)
				{
					base.Dispose(disposing);
					this.ReleaseHandle();
				}
			}

			private long traceHandle;

			private DiagnosticsNativeMethods.ControlCallback callback;
		}

		internal sealed class CriticalTraceHandle : CriticalHandle
		{
			private CriticalTraceHandle() : base(IntPtr.Zero)
			{
				this.traceHandle = -1L;
			}

			[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
			public long DangerousGetHandle()
			{
				return this.traceHandle;
			}

			private void Initialize(long handle)
			{
				RuntimeHelpers.PrepareConstrainedRegions();
				try
				{
				}
				finally
				{
					this.traceHandle = handle;
				}
			}

			public override bool IsInvalid
			{
				get
				{
					return this.traceHandle == -1L;
				}
			}

			[DllImport("advapi32.dll", SetLastError = true)]
			private static extern long GetTraceLoggerHandle([In] IntPtr buffer);

			public static DiagnosticsNativeMethods.CriticalTraceHandle Attach(IntPtr buffer)
			{
				DiagnosticsNativeMethods.CriticalTraceHandle criticalTraceHandle = new DiagnosticsNativeMethods.CriticalTraceHandle();
				criticalTraceHandle.Initialize(DiagnosticsNativeMethods.CriticalTraceHandle.GetTraceLoggerHandle(buffer));
				if (criticalTraceHandle.IsInvalid)
				{
					throw new Win32Exception(Marshal.GetLastWin32Error());
				}
				return criticalTraceHandle;
			}

			protected override bool ReleaseHandle()
			{
				return true;
			}

			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			private long traceHandle;
		}

		public struct EventInstanceInfo
		{
			public IntPtr RegistrationHandle;

			public uint InstanceId;
		}
	}
}
