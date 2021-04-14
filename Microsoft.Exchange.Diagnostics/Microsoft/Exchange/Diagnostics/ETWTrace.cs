using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Diagnostics
{
	internal class ETWTrace
	{
		private ETWTrace()
		{
			ETWTrace.StartTraceSession();
			AppDomain.CurrentDomain.DomainUnload += delegate(object source, EventArgs args)
			{
				ETWTrace.EndTraceSession();
			};
		}

		public static event Action OnTraceStateChange;

		public static bool IsEnabled
		{
			get
			{
				return ETWTrace.ComponentSession.TracingEnabled;
			}
		}

		public static bool IsCasEnabled
		{
			get
			{
				return ETWTrace.CasPerfSession.TracingEnabled;
			}
		}

		public static bool IsExmonRpcEnabled
		{
			get
			{
				return ETWTrace.ExmonMapiRpcSession.TracingEnabled || ETWTrace.ExmonAdminRpcSession.TracingEnabled || ETWTrace.ExmonTaskSession.TracingEnabled;
			}
		}

		public static bool IsInternalTraceEnabled
		{
			get
			{
				return ETWTrace.InternalTraceSession.TracingEnabled;
			}
		}

		public static ETWTrace TraceSession
		{
			get
			{
				return ETWTrace.etwTraceSingletonInstance;
			}
		}

		public static bool ShouldTraceCasStop(Guid serviceProviderRequestId)
		{
			return ETWTrace.CasPerfSession.TracingEnabled && serviceProviderRequestId != Guid.Empty;
		}

		public static bool WriteBinary(TraceType traceType, Guid component, int traceTag, byte[] message, out int maxBytes)
		{
			maxBytes = 0;
			int num = message.Length;
			if (traceTag > 255)
			{
				throw new ArgumentException("Maximum allowed tag-value is 255 for binary traces", "traceTag");
			}
			if (num > 8064)
			{
				maxBytes = 8064;
				return false;
			}
			GCHandle? gchandle = null;
			uint num2 = 0U;
			try
			{
				DiagnosticsNativeMethods.BinaryEventTrace binaryEventTrace = new DiagnosticsNativeMethods.BinaryEventTrace(component, (byte)traceTag, message, ref gchandle);
				num2 = DiagnosticsNativeMethods.TraceEvent(ETWTrace.ComponentSession.Session.DangerousGetHandle(), ref binaryEventTrace);
			}
			finally
			{
				if (gchandle != null && gchandle.Value.IsAllocated)
				{
					gchandle.Value.Free();
				}
			}
			return num2 == 0U;
		}

		public static void Write(int lid, TraceType traceType, Guid component, int traceTag, long id, string message)
		{
			ETWTrace.Write(ETWTrace.ComponentSession, lid, traceType, component, traceTag, id, message);
		}

		public static void InternalWrite(int lid, TraceType traceType, Guid component, int traceTag, long id, string message)
		{
			ETWTrace.Write(ETWTrace.InternalTraceSession, lid, traceType, component, traceTag, id, message);
		}

		public static void Write(EtwSessionInfo sessionInfo, int lid, TraceType traceType, Guid component, int traceTag, long id, string message)
		{
			int num = (message.Length + 1) * 2;
			int num2 = 16;
			int num3 = 8064;
			int val = (num3 - num2) / 2 - 1;
			if (num2 + num < num3)
			{
				uint num4 = DiagnosticsNativeMethods.TraceMessage(sessionInfo.Session.DangerousGetHandle(), 43U, ref component, (ushort)traceType | 32, ref traceTag, 4, message, num, ref id, 8, ref lid, 4, IntPtr.Zero, 0);
				if (num4 != 0U)
				{
					return;
				}
			}
			else
			{
				int num5 = 0;
				int i = message.Length;
				while (i > 0)
				{
					int num6 = Math.Min(i, val);
					uint num7 = DiagnosticsNativeMethods.TraceMessage(sessionInfo.Session.DangerousGetHandle(), 43U, ref component, (ushort)traceType | 32, ref traceTag, 4, message.Substring(num5, num6), (num6 + 1) * 2, ref id, 8, ref lid, 4, IntPtr.Zero, 0);
					i -= num6;
					num5 += num6;
				}
			}
		}

		internal static void WriteCas(CasTraceEventType eventType, CasTraceStartStop traceType, Guid serviceProviderRequestID, int bytesIn, int bytesOut, string serverAddress, string userContext, string spOperation, string spOperationData, string clientOperation)
		{
			int num = (serverAddress.Length + 1) * 2;
			int num2 = (userContext.Length + 1) * 2;
			int num3 = (spOperation.Length + 1) * 2;
			int num4 = (spOperationData.Length + 1) * 2;
			int num5 = (clientOperation.Length + 1) * 2;
			int num6 = 12 + 2 * ETWTrace.GuidByteLength + num + num2 + num3 + num4 + num5;
			int num7 = 8064;
			if (num6 < num7)
			{
				int num8 = (int)traceType;
				Guid activityId = ETWTrace.GetActivityId();
				Guid events = ETWTrace.casPerfGuids.Events;
				uint num9 = DiagnosticsNativeMethods.TraceMessage(ETWTrace.CasPerfSession.Session.DangerousGetHandle(), 43U, ref events, (ushort)eventType, ref num8, 4, activityId.ToByteArray(), ETWTrace.GuidByteLength, serviceProviderRequestID.ToByteArray(), ETWTrace.GuidByteLength, ref bytesIn, 4, ref bytesOut, 4, serverAddress, num, userContext, num2, spOperation, num3, spOperationData, num4, clientOperation, num5, IntPtr.Zero, 0);
			}
		}

		private static void InvokeOnTraceStateChange()
		{
			Action onTraceStateChange = ETWTrace.OnTraceStateChange;
			if (onTraceStateChange != null)
			{
				onTraceStateChange();
			}
		}

		private static void StartTraceSession()
		{
			IntPtr intPtr;
			ETWTrace.RegisterGuid(ref ETWTrace.componentGuids, out ETWTrace.componentHandle, ETWTrace.ComponentSession.ControlCallback, out intPtr);
			ETWTrace.RegisterGuid(ref ETWTrace.casPerfGuids, out ETWTrace.casPerfHandle, ETWTrace.CasPerfSession.ControlCallback, out intPtr);
			ETWTrace.RegisterGuid(ref ETWTrace.exmonMapiRpcGuids, out ETWTrace.exmonMapiRpcHandle, ETWTrace.ExmonMapiRpcSession.ControlCallback, out ETWTrace.exmonMapiEventHandle);
			ETWTrace.RegisterGuid(ref ETWTrace.exmonAdminRpcGuids, out ETWTrace.exmonAdminRpcHandle, ETWTrace.ExmonAdminRpcSession.ControlCallback, out ETWTrace.exmonAdminEventHandle);
			ETWTrace.RegisterGuid(ref ETWTrace.exmonTaskGuids, out ETWTrace.exmonTaskHandle, ETWTrace.ExmonTaskSession.ControlCallback, out ETWTrace.exmonTaskEventHandle);
			ETWTrace.RegisterGuid(ref ETWTrace.internalTraceGuid, out ETWTrace.internalTraceHandle, ETWTrace.InternalTraceSession.ControlCallback, out intPtr);
			ETWTrace.ComponentSession.OnTraceStateChange += ETWTrace.InvokeOnTraceStateChange;
			ETWTrace.CasPerfSession.OnTraceStateChange += ETWTrace.InvokeOnTraceStateChange;
			ETWTrace.ExmonMapiRpcSession.OnTraceStateChange += ETWTrace.InvokeOnTraceStateChange;
			ETWTrace.ExmonAdminRpcSession.OnTraceStateChange += ETWTrace.InvokeOnTraceStateChange;
			ETWTrace.ExmonTaskSession.OnTraceStateChange += ETWTrace.InvokeOnTraceStateChange;
			ETWTrace.InternalTraceSession.OnTraceStateChange += ETWTrace.InvokeOnTraceStateChange;
		}

		private unsafe static void RegisterGuid(ref ETWTrace.EtwTraceGuids traceGuids, out DiagnosticsNativeMethods.CriticalTraceRegistrationHandle regHandle, DiagnosticsNativeMethods.ControlCallback callback, out IntPtr eventHandle)
		{
			Guid events = traceGuids.Events;
			DiagnosticsNativeMethods.TraceGuidRegistration traceGuidRegistration;
			traceGuidRegistration.guid = &events;
			traceGuidRegistration.handle = IntPtr.Zero;
			regHandle = DiagnosticsNativeMethods.CriticalTraceRegistrationHandle.RegisterTrace(traceGuids.Provider, ref traceGuidRegistration, callback);
			eventHandle = traceGuidRegistration.handle;
		}

		private static void EndTraceSession()
		{
			ETWTrace.componentHandle.Dispose();
			ETWTrace.casPerfHandle.Dispose();
			ETWTrace.exmonMapiRpcHandle.Dispose();
			ETWTrace.exmonAdminRpcHandle.Dispose();
			ETWTrace.exmonTaskHandle.Dispose();
			ETWTrace.internalTraceHandle.Dispose();
		}

		private static DiagnosticsNativeMethods.EventInstanceInfo CreateInstanceId(IntPtr eventHandle)
		{
			DiagnosticsNativeMethods.EventInstanceInfo result = default(DiagnosticsNativeMethods.EventInstanceInfo);
			DiagnosticsNativeMethods.CreateTraceInstanceId(eventHandle, ref result);
			return result;
		}

		internal static DiagnosticsNativeMethods.EventInstanceInfo CreateExmonMapiRpcInstanceId()
		{
			return ETWTrace.CreateInstanceId(ETWTrace.exmonMapiEventHandle);
		}

		internal static DiagnosticsNativeMethods.EventInstanceInfo CreateExmonAdminRpcInstanceId()
		{
			return ETWTrace.CreateInstanceId(ETWTrace.exmonAdminEventHandle);
		}

		internal static DiagnosticsNativeMethods.EventInstanceInfo CreateExmonTaskInstanceId()
		{
			return ETWTrace.CreateInstanceId(ETWTrace.exmonTaskEventHandle);
		}

		public static uint ExmonMapiRpcTraceEventInstance(byte[] buffer, ref DiagnosticsNativeMethods.EventInstanceInfo instanceInfo, ref DiagnosticsNativeMethods.EventInstanceInfo parentInstanceInfo)
		{
			DiagnosticsNativeMethods.CriticalTraceHandle session = ETWTrace.ExmonMapiRpcSession.Session;
			if (session != null)
			{
				return DiagnosticsNativeMethods.TraceEventInstance(session.DangerousGetHandle(), buffer, ref instanceInfo, ref parentInstanceInfo);
			}
			return 87U;
		}

		public static uint ExmonAdminRpcTraceEventInstance(byte[] buffer, ref DiagnosticsNativeMethods.EventInstanceInfo instanceInfo, ref DiagnosticsNativeMethods.EventInstanceInfo parentInstanceInfo)
		{
			DiagnosticsNativeMethods.CriticalTraceHandle session = ETWTrace.ExmonAdminRpcSession.Session;
			if (session != null)
			{
				return DiagnosticsNativeMethods.TraceEventInstance(session.DangerousGetHandle(), buffer, ref instanceInfo, ref parentInstanceInfo);
			}
			return 87U;
		}

		public static uint ExmonTaskTraceEventInstance(byte[] buffer, ref DiagnosticsNativeMethods.EventInstanceInfo instanceInfo, ref DiagnosticsNativeMethods.EventInstanceInfo parentInstanceInfo)
		{
			DiagnosticsNativeMethods.CriticalTraceHandle session = ETWTrace.ExmonTaskSession.Session;
			if (session != null)
			{
				return DiagnosticsNativeMethods.TraceEventInstance(session.DangerousGetHandle(), buffer, ref instanceInfo, ref parentInstanceInfo);
			}
			return 87U;
		}

		private static Guid GetActivityId()
		{
			return Trace.CorrelationManager.ActivityId;
		}

		internal const ushort EtlRecordVersion = 32;

		private const int MaxTraceMessageSize = 8064;

		private const uint TraceFlags = 43U;

		internal static readonly Guid ExchangeProviderGuid = new Guid("{79BB49E6-2A2C-46e4-9167-FA122525D540}");

		internal static readonly Guid InternalProviderGuid = new Guid("{096D3B9E-AA4F-4204-B1DD-7DC258498AB6}");

		private static readonly object LockObject = new object();

		private static readonly int GuidByteLength = Guid.Empty.ToByteArray().Length;

		private static readonly EtwSessionInfo ComponentSession = new EtwSessionInfo();

		private static readonly EtwSessionInfo CasPerfSession = new EtwSessionInfo();

		private static readonly EtwSessionInfo ExmonMapiRpcSession = new EtwSessionInfo();

		private static readonly EtwSessionInfo ExmonAdminRpcSession = new EtwSessionInfo();

		private static readonly EtwSessionInfo ExmonTaskSession = new EtwSessionInfo();

		private static readonly EtwSessionInfo InternalTraceSession = new EtwSessionInfo();

		private static ETWTrace.EtwTraceGuids componentGuids = new ETWTrace.EtwTraceGuids(ETWTrace.ExchangeProviderGuid, new Guid("{FBA968C6-3276-4135-B16B-9D90D7151E61}"));

		private static ETWTrace.EtwTraceGuids casPerfGuids = new ETWTrace.EtwTraceGuids(new Guid("{67C6E8E5-B62B-4f47-A04D-ECB487D00046}"), new Guid("{9BB29706-EC64-4345-8208-67B0C9E22283}"));

		private static ETWTrace.EtwTraceGuids exmonMapiRpcGuids = new ETWTrace.EtwTraceGuids(new Guid("{2EACCEDF-8648-453e-9250-27F0069F71D2}"), new Guid("{31F5A811-6EA0-4321-93D9-CDB9A70D50A1}"));

		private static ETWTrace.EtwTraceGuids exmonAdminRpcGuids = new ETWTrace.EtwTraceGuids(new Guid("{2EACCEDF-8648-453e-9250-27F0069F71D2}"), new Guid("{42EC5AC0-3D00-4DBF-A45C-EC569A40C512}"));

		private static ETWTrace.EtwTraceGuids exmonTaskGuids = new ETWTrace.EtwTraceGuids(new Guid("{2EACCEDF-8648-453e-9250-27F0069F71D2}"), new Guid("{D77063BD-E0BE-488B-AD3A-B27B7C110FEC}"));

		private static ETWTrace.EtwTraceGuids internalTraceGuid = new ETWTrace.EtwTraceGuids(ETWTrace.InternalProviderGuid, new Guid("{9E0B578C-4634-4212-82DF-063DF01E3728}"));

		private static DiagnosticsNativeMethods.CriticalTraceRegistrationHandle componentHandle;

		private static DiagnosticsNativeMethods.CriticalTraceRegistrationHandle casPerfHandle;

		private static DiagnosticsNativeMethods.CriticalTraceRegistrationHandle exmonMapiRpcHandle;

		private static DiagnosticsNativeMethods.CriticalTraceRegistrationHandle exmonAdminRpcHandle;

		private static DiagnosticsNativeMethods.CriticalTraceRegistrationHandle exmonTaskHandle;

		private static DiagnosticsNativeMethods.CriticalTraceRegistrationHandle internalTraceHandle;

		private static IntPtr exmonMapiEventHandle;

		private static IntPtr exmonAdminEventHandle;

		private static IntPtr exmonTaskEventHandle;

		private static ETWTrace etwTraceSingletonInstance = new ETWTrace();

		private struct EtwTraceGuids
		{
			public EtwTraceGuids(Guid provider, Guid eventClass)
			{
				this.Provider = provider;
				this.Events = eventClass;
			}

			public readonly Guid Provider;

			public readonly Guid Events;
		}
	}
}
