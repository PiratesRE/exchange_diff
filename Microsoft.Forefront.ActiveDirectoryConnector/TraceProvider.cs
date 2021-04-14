using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;

namespace Microsoft
{
	[GeneratedCode("ManagedWPP", "1.0.0.0")]
	[Guid("748004CA-4959-409a-887C-6546438CF48E")]
	internal class TraceProvider
	{
		internal TraceProvider(string applicationName, Guid controlGuid, bool useSequenceNumbers)
		{
			this.applicationName = applicationName;
			this.controlGuid = controlGuid;
			this.useSequenceNumbers = useSequenceNumbers;
			IntPtr moduleHandle = UnsafeNativeMethods.GetModuleHandle("advapi32.dll");
			IntPtr procAddress = UnsafeNativeMethods.GetProcAddress(moduleHandle, "GetTraceLoggerHandle");
			this.getTraceLoggerHandle = (TraceProvider.GetTraceLoggerHandle)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(TraceProvider.GetTraceLoggerHandle));
			procAddress = UnsafeNativeMethods.GetProcAddress(moduleHandle, "GetTraceEnableFlags");
			this.getTraceEnableFlags = (TraceProvider.GetTraceEnableFlags)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(TraceProvider.GetTraceEnableFlags));
			procAddress = UnsafeNativeMethods.GetProcAddress(moduleHandle, "GetTraceEnableLevel");
			this.getTraceEnableLevel = (TraceProvider.GetTraceEnableLevel)Marshal.GetDelegateForFunctionPointer(procAddress, typeof(TraceProvider.GetTraceEnableLevel));
			this.Register();
		}

		private unsafe uint Register()
		{
			this.etwCallback = new UnsafeNativeMethods.WMIDPREQUEST(this.MyCallback);
			UnsafeNativeMethods.TRACE_GUID_REGISTRATION trace_GUID_REGISTRATION = default(UnsafeNativeMethods.TRACE_GUID_REGISTRATION);
			Guid guid = new Guid("{b4955bf0-3af1-4740-b475-99055d3fe9aa}");
			trace_GUID_REGISTRATION.Guid = &guid;
			trace_GUID_REGISTRATION.RegHandle = IntPtr.Zero;
			return UnsafeNativeMethods.RegisterTraceGuids(this.etwCallback, null, ref this.controlGuid, 1U, ref trace_GUID_REGISTRATION, null, null, out this.registrationHandle);
		}

		private unsafe uint MyCallback(UnsafeNativeMethods.WMIDPREQUESTCODE requestCode, IntPtr context, uint* bufferSize, byte* byteBuffer)
		{
			switch (requestCode)
			{
			case UnsafeNativeMethods.WMIDPREQUESTCODE.WMI_ENABLE_EVENTS:
				this.traceHandle = this.getTraceLoggerHandle((UnsafeNativeMethods.WNODE_HEADER*)byteBuffer);
				this.flags = this.getTraceEnableFlags(this.traceHandle);
				this.level = this.getTraceEnableLevel(this.traceHandle);
				this.enabled = true;
				break;
			case UnsafeNativeMethods.WMIDPREQUESTCODE.WMI_DISABLE_EVENTS:
				this.enabled = false;
				this.traceHandle = 0UL;
				this.level = 0;
				this.flags = 0U;
				break;
			default:
				this.enabled = false;
				this.traceHandle = 0UL;
				break;
			}
			return 0U;
		}

		~TraceProvider()
		{
			UnsafeNativeMethods.UnregisterTraceGuids(this.registrationHandle);
			GC.KeepAlive(this.etwCallback);
		}

		internal int Flags
		{
			get
			{
				return (int)this.flags;
			}
		}

		internal byte Level
		{
			get
			{
				return this.level;
			}
		}

		internal bool IsEnabled
		{
			get
			{
				return this.enabled;
			}
		}

		internal string ApplicationName
		{
			get
			{
				return this.applicationName;
			}
		}

		internal static string MakeStringArg(object arg)
		{
			if (arg != null)
			{
				return arg.ToString();
			}
			return "NULL";
		}

		internal static long MakeTimeStampArg(object arg)
		{
			if (arg != null)
			{
				if (arg is DateTime)
				{
					return ((DateTime)arg).ToFileTime();
				}
				try
				{
					return Convert.ToInt64(arg);
				}
				catch (InvalidCastException)
				{
					return 0L;
				}
			}
			return 0L;
		}

		internal static int GetBufferSize(int numberOfFields, int totalParameterSize)
		{
			return sizeof(UnsafeNativeMethods.VALIST_HEADER) + (numberOfFields + 1) * sizeof(UnsafeNativeMethods.VALIST_FIELD) + totalParameterSize;
		}

		internal unsafe static void InitializeTraceBuffer(void* buffer, Guid messageGuid, int messageNumber)
		{
			((UnsafeNativeMethods.VALIST_HEADER*)buffer)->MessageGuid = messageGuid;
			((UnsafeNativeMethods.VALIST_HEADER*)buffer)->MessageNumber = (ushort)messageNumber;
			((UnsafeNativeMethods.VALIST_HEADER*)buffer)->NumberOfFields = 0;
			UnsafeNativeMethods.VALIST_FIELD* ptr = (UnsafeNativeMethods.VALIST_FIELD*)((byte*)buffer + sizeof(UnsafeNativeMethods.VALIST_HEADER));
			ptr->DataPointer = null;
			ptr->DataLength = 0U;
		}

		internal unsafe static void* InitializeTraceBuffer(void* buffer, Guid messageGuid, int messageNumber, int numberOfFields, ref int dataBufferOffset)
		{
			((UnsafeNativeMethods.VALIST_HEADER*)buffer)->MessageGuid = messageGuid;
			((UnsafeNativeMethods.VALIST_HEADER*)buffer)->MessageNumber = (ushort)messageNumber;
			((UnsafeNativeMethods.VALIST_HEADER*)buffer)->NumberOfFields = (ushort)numberOfFields;
			UnsafeNativeMethods.VALIST_FIELD* ptr = (UnsafeNativeMethods.VALIST_FIELD*)((byte*)buffer + sizeof(UnsafeNativeMethods.VALIST_HEADER));
			UnsafeNativeMethods.VALIST_FIELD* ptr2 = ptr + numberOfFields;
			ptr2->DataPointer = null;
			ptr2->DataLength = 0U;
			byte* ptr3 = (byte*)(ptr2 + 1);
			dataBufferOffset = (int)((long)((byte*)ptr3 - (byte*)buffer));
			return (void*)ptr;
		}

		internal unsafe static void* InitializeTraceField(void* field, string value, void* buffer, ref int dataBufferOffset, char* charBuffer)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			int num = (value == null) ? 0 : (value.Length * 2);
			*(short*)ptr = (short)((ushort)num);
			dataBufferOffset += 2;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 2U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			UnsafeNativeMethods.VALIST_FIELD* ptr2 = (UnsafeNativeMethods.VALIST_FIELD*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
			ptr2->DataLength = (uint)num;
			ptr2->DataPointer = (void*)charBuffer;
			return (void*)(ptr2 + 1);
		}

		internal unsafe static void* InitializeTraceField(void* field, byte[] value, void* buffer, ref int dataBufferOffset, byte* byteBuffer)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			int num = (value == null) ? 0 : value.Length;
			*(short*)ptr = (short)((ushort)num);
			dataBufferOffset += 2;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 2U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			UnsafeNativeMethods.VALIST_FIELD* ptr2 = (UnsafeNativeMethods.VALIST_FIELD*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
			ptr2->DataLength = (uint)num;
			ptr2->DataPointer = (void*)byteBuffer;
			return (void*)(ptr2 + 1);
		}

		internal unsafe static void* InitializeTraceField(void* field, char value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*(short*)ptr = (short)value;
			dataBufferOffset += 2;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 2U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe static void* InitializeTraceField(void* field, byte value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*ptr = value;
			dataBufferOffset++;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 1U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe static void* InitializeTraceField(void* field, short value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*(short*)ptr = value;
			dataBufferOffset += 2;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 2U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe static void* InitializeTraceField(void* field, ushort value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*(short*)ptr = (short)value;
			dataBufferOffset += 2;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 2U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe static void* InitializeTraceField(void* field, int value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*(int*)ptr = value;
			dataBufferOffset += 4;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 4U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe static void* InitializeTraceField(void* field, uint value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*(int*)ptr = (int)value;
			dataBufferOffset += 4;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 4U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe static void* InitializeTraceField(void* field, IntPtr value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*(IntPtr*)ptr = value;
			dataBufferOffset += sizeof(IntPtr);
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = (uint)sizeof(IntPtr);
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe static void* InitializeTraceField(void* field, long value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*(long*)ptr = value;
			dataBufferOffset += 8;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 8U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe static void* InitializeTraceField(void* field, ulong value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*(long*)ptr = (long)value;
			dataBufferOffset += 8;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 8U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe static void* InitializeTraceField(void* field, double value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*(double*)ptr = value;
			dataBufferOffset += 8;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 8U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe static void* InitializeTraceField(void* field, Guid value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*(Guid*)ptr = value;
			dataBufferOffset += sizeof(Guid);
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = (uint)sizeof(Guid);
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe static void* InitializeTraceField(void* field, DateTime value, void* buffer, ref int dataBufferOffset)
		{
			byte* ptr = (byte*)buffer + dataBufferOffset;
			*(long*)ptr = value.Ticks;
			dataBufferOffset += 8;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataLength = 8U;
			((UnsafeNativeMethods.VALIST_FIELD*)field)->DataPointer = (void*)ptr;
			return (void*)((byte*)field + sizeof(UnsafeNativeMethods.VALIST_FIELD));
		}

		internal unsafe uint TraceEvent(void* buffer)
		{
			UnsafeNativeMethods.TRACE_MESSAGE_FLAGS trace_MESSAGE_FLAGS = UnsafeNativeMethods.TRACE_MESSAGE_FLAGS.TRACE_MESSAGE_GUID | UnsafeNativeMethods.TRACE_MESSAGE_FLAGS.TRACE_MESSAGE_TIMESTAMP | UnsafeNativeMethods.TRACE_MESSAGE_FLAGS.TRACE_MESSAGE_SYSTEMINFO;
			if (this.useSequenceNumbers)
			{
				trace_MESSAGE_FLAGS |= UnsafeNativeMethods.TRACE_MESSAGE_FLAGS.TRACE_MESSAGE_SEQUENCE;
			}
			return UnsafeNativeMethods.TraceMessageVa(this.traceHandle, (uint)trace_MESSAGE_FLAGS, &((UnsafeNativeMethods.VALIST_HEADER*)buffer)->MessageGuid, ((UnsafeNativeMethods.VALIST_HEADER*)buffer)->MessageNumber, (void*)((byte*)buffer + sizeof(UnsafeNativeMethods.VALIST_HEADER)));
		}

		internal UnsafeNativeMethods.WMIDPREQUEST etwCallback;

		private readonly bool useSequenceNumbers;

		private ulong registrationHandle;

		private ulong traceHandle;

		private byte level;

		private uint flags;

		private bool enabled;

		private Guid controlGuid;

		private readonly string applicationName;

		private TraceProvider.GetTraceLoggerHandle getTraceLoggerHandle;

		private TraceProvider.GetTraceEnableFlags getTraceEnableFlags;

		private TraceProvider.GetTraceEnableLevel getTraceEnableLevel;

		private unsafe delegate ulong GetTraceLoggerHandle(UnsafeNativeMethods.WNODE_HEADER* Buffer);

		private delegate uint GetTraceEnableFlags(ulong TraceHandle);

		private delegate byte GetTraceEnableLevel(ulong TraceHandle);

		internal enum TraceLevel : byte
		{
			TRACE_LEVEL_NOISE = 6,
			TRACE_LEVEL_INFORMATION = 4,
			TRACE_LEVEL_FATAL = 1,
			TRACE_LEVEL_ERROR,
			TRACE_LEVEL_WARNING,
			TRACE_LEVEL_VERBOSE = 5
		}
	}
}
