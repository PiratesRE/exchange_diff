using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Diagnostics.Service.ExchangeJobs.Etw
{
	public static class EtwTraceNativeComponents
	{
		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "OpenTraceW", SetLastError = true)]
		internal static extern ulong OpenTrace([In] [Out] ref EtwTraceNativeComponents.EVENT_TRACE_LOGFILEW logfile);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		internal static extern int ProcessTrace([In] ulong[] handleArray, [In] uint handleCount, [In] IntPtr startTime, [In] IntPtr endTime);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode)]
		internal static extern int CloseTrace([In] ulong traceHandle);

		internal const ushort EventHeaderFlag32BitHeader = 32;

		internal const ushort EventHeaderFlag64BitHeader = 64;

		internal const ushort EventHeaderFlagClassicHeader = 256;

		internal const uint ProcessTraceModeEventRecord = 268435456U;

		internal const uint ProcessTraceModeRawTimestamp = 4096U;

		internal const ulong InvalidHeaderValue = 18446744073709551615UL;

		internal delegate bool EventTraceBufferCallback([In] IntPtr logfile);

		internal unsafe delegate void EventTraceEventCallback([In] EtwTraceNativeComponents.EVENT_RECORD* rawData);

		internal enum TraceMessageFlags
		{
			Sequence = 1,
			Guid,
			ComponentId = 4,
			Timestamp = 8,
			PerformanceTimestamp = 16,
			SystemInfo = 32,
			FlagMask = 65535
		}

		internal enum EventIndex : uint
		{
			Invalid = 4294967295U
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Size = 172)]
		internal struct TIME_ZONE_INFORMATION
		{
			public uint Bias;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string StandardName;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U2)]
			public ushort[] StandardDate;

			public uint StandardBias;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string DaylightName;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.U2)]
			public ushort[] DaylightDate;

			public uint DaylightBias;
		}

		internal struct EVENT_TRACE_HEADER
		{
			public ushort Size;

			public ushort FieldTypeFlags;

			public byte Type;

			public byte Level;

			public ushort Version;

			public int ThreadId;

			public int ProcessId;

			public long TimeStamp;

			public Guid Guid;

			public int KernelTime;

			public int UserTime;
		}

		internal struct EVENT_TRACE
		{
			public EtwTraceNativeComponents.EVENT_TRACE_HEADER Header;

			public uint InstanceId;

			public uint ParentInstanceId;

			public Guid ParentGuid;

			public IntPtr MofData;

			public int MofLength;

			public EtwTraceNativeComponents.ETW_BUFFER_CONTEXT BufferContext;
		}

		internal struct TRACE_LOGFILE_HEADER
		{
			public uint BufferSize;

			public uint Version;

			public uint ProviderVersion;

			public uint NumberOfProcessors;

			public long EndTime;

			public uint TimerResolution;

			public uint MaximumFileSize;

			public uint LogFileMode;

			public uint BuffersWritten;

			public uint StartBuffers;

			public uint PointerSize;

			public uint EventsLost;

			public uint CpuSpeedInMHz;

			public IntPtr LoggerName;

			public IntPtr LogFileName;

			public EtwTraceNativeComponents.TIME_ZONE_INFORMATION TimeZone;

			public long BootTime;

			public long PerfFreq;

			public long StartTime;

			public uint ReservedFlags;

			public uint BuffersLost;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
		internal struct EVENT_TRACE_LOGFILEW
		{
			[MarshalAs(UnmanagedType.LPWStr)]
			public string LogFileName;

			[MarshalAs(UnmanagedType.LPWStr)]
			public string LoggerName;

			public long CurrentTime;

			public uint BuffersRead;

			public uint LogFileMode;

			public EtwTraceNativeComponents.EVENT_TRACE CurrentEvent;

			public EtwTraceNativeComponents.TRACE_LOGFILE_HEADER LogfileHeader;

			public EtwTraceNativeComponents.EventTraceBufferCallback BufferCallback;

			public int BufferSize;

			public int Filled;

			public int EventsLost;

			public EtwTraceNativeComponents.EventTraceEventCallback EventCallback;

			public int IsKernelTrace;

			public IntPtr Context;
		}

		internal struct EVENT_HEADER
		{
			public ushort Size;

			public ushort HeaderType;

			public ushort Flags;

			public ushort EventProperty;

			public int ThreadId;

			public int ProcessId;

			public long TimeStamp;

			public Guid ProviderId;

			public ushort Id;

			public byte Version;

			public byte Channel;

			public byte Level;

			public byte Opcode;

			public ushort Task;

			public ulong Keyword;

			public int KernelTime;

			public int UserTime;

			public Guid ActivityId;
		}

		internal struct ETW_BUFFER_CONTEXT
		{
			public byte ProcessorNumber;

			public byte Alignment;

			public ushort LoggerId;
		}

		internal struct EVENT_RECORD
		{
			public EtwTraceNativeComponents.EVENT_HEADER EventHeader;

			public EtwTraceNativeComponents.ETW_BUFFER_CONTEXT BufferContext;

			public ushort ExtendedDataCount;

			public ushort UserDataLength;

			public unsafe EtwTraceNativeComponents.EVENT_HEADER_EXTENDED_DATA_ITEM* ExtendedData;

			public IntPtr UserData;

			public IntPtr UserContext;
		}

		internal struct EVENT_HEADER_EXTENDED_DATA_ITEM
		{
			public ushort Reserved1;

			public ushort ExtType;

			public ushort Reserved2;

			public ushort DataSize;

			public ulong DataPtr;
		}
	}
}
