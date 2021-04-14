using System;
using System.CodeDom.Compiler;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft
{
	[GeneratedCode("ManagedWPP", "1.0.0.0")]
	internal sealed class UnsafeNativeMethods
	{
		internal UnsafeNativeMethods()
		{
		}

		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", CharSet = CharSet.Unicode, EntryPoint = "RegisterTraceGuidsW")]
		internal unsafe static extern uint RegisterTraceGuids([In] UnsafeNativeMethods.WMIDPREQUEST RequestAddress, [In] void* RequestContext, [In] ref Guid ControlGuid, [In] uint GuidCount, ref UnsafeNativeMethods.TRACE_GUID_REGISTRATION TraceGuidReg, [In] string MofImagePath, [In] string MofResourceName, out ulong RegistrationHandle);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", ExactSpelling = true)]
		internal static extern int UnregisterTraceGuids(ulong RegistrationHandle);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", ExactSpelling = true)]
		internal unsafe static extern uint TraceEvent(ulong TraceHandle, UnsafeNativeMethods.EVENT_TRACE_BUFFER* EventTrace);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("advapi32.dll", ExactSpelling = true)]
		internal unsafe static extern uint TraceMessageVa(ulong TraceHandle, uint MessageFlags, Guid* MessageGuid, ushort MessageNumber, void* MessageArgList);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, EntryPoint = "GetModuleHandleW")]
		internal static extern IntPtr GetModuleHandle([In] string lpModuleName);

		[SuppressUnmanagedCodeSecurity]
		[DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
		internal static extern IntPtr GetProcAddress([In] IntPtr hModule, [In] string lpProcName);

		internal const int TRACE_MESSAGE_MAXIMUM_SIZE = 8192;

		internal const uint ERROR_INVALID_PARAMETER = 87U;

		internal struct VALIST_FIELD
		{
			internal unsafe void* DataPointer;

			internal uint DataLength;
		}

		[StructLayout(LayoutKind.Explicit, Size = 20)]
		internal struct VALIST_HEADER
		{
			[FieldOffset(0)]
			internal Guid MessageGuid;

			[FieldOffset(16)]
			internal ushort MessageNumber;

			[FieldOffset(18)]
			internal ushort NumberOfFields;
		}

		[StructLayout(LayoutKind.Explicit, Size = 16)]
		internal struct MOF_FIELD
		{
			[FieldOffset(0)]
			internal ulong ZeroInit;

			[FieldOffset(0)]
			internal unsafe void* DataPointer;

			[FieldOffset(8)]
			internal uint DataLength;

			[FieldOffset(12)]
			internal uint DataType;
		}

		[StructLayout(LayoutKind.Explicit, Size = 48)]
		internal struct WNODE_HEADER
		{
			[FieldOffset(0)]
			internal uint BufferSize;

			[FieldOffset(4)]
			internal uint ProviderId;

			[FieldOffset(8)]
			internal IntPtr HistoricalContext;

			[FieldOffset(16)]
			internal long TimeStamp;

			[FieldOffset(24)]
			internal Guid Guid;

			[FieldOffset(40)]
			internal uint ClientContext;

			[FieldOffset(44)]
			internal uint Flags;
		}

		internal struct TRACE_GUID_REGISTRATION
		{
			internal unsafe Guid* Guid;

			internal IntPtr RegHandle;
		}

		[StructLayout(LayoutKind.Explicit, Size = 336)]
		internal struct EVENT_TRACE_BUFFER
		{
			[FieldOffset(0)]
			internal uint BufferSize;

			[FieldOffset(4)]
			internal uint ProviderId;

			[FieldOffset(8)]
			internal uint ThreadId;

			[FieldOffset(12)]
			internal uint ProcessId;

			[FieldOffset(16)]
			internal long TimeStamp;

			[FieldOffset(24)]
			internal Guid Guid;

			[FieldOffset(40)]
			internal uint ClientContext;

			[FieldOffset(44)]
			internal uint Flags;

			[FieldOffset(48)]
			internal UnsafeNativeMethods.MOF_FIELD UserData;
		}

		internal enum WNODE_FLAGS : uint
		{
			WNODE_FLAG_TRACED_GUID = 131072U,
			WNODE_FLAG_USE_GUID_PTR = 524288U,
			WNODE_FLAG_USE_MOF_PTR = 1048576U
		}

		[Flags]
		internal enum TRACE_MESSAGE_FLAGS : uint
		{
			TRACE_MESSAGE_SEQUENCE = 1U,
			TRACE_MESSAGE_GUID = 2U,
			TRACE_MESSAGE_COMPONENTID = 4U,
			TRACE_MESSAGE_TIMESTAMP = 8U,
			TRACE_MESSAGE_PERFORMANCE_TIMESTAMP = 16U,
			TRACE_MESSAGE_SYSTEMINFO = 32U
		}

		internal enum WMIDPREQUESTCODE : uint
		{
			WMI_ENABLE_EVENTS = 4U,
			WMI_DISABLE_EVENTS
		}

		internal unsafe delegate uint WMIDPREQUEST(UnsafeNativeMethods.WMIDPREQUESTCODE RequestCode, IntPtr RequestContext, uint* BufferSize, byte* Buffer);
	}
}
