using System;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Net;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal class ExMonLogger : BaseObject
	{
		internal ExMonLogger(bool enableTestMode, string clientAddress, string serviceName, ExMonLogger.CreateExmonRpcInstanceId delegateCreateInstanceId, ExMonLogger.ExmonRpcTraceEventInstance delegateTraceEventInstance)
		{
			this.header = new ExMonLogger.EventInstanceHeader(this);
			this.clientAddress = clientAddress;
			this.ServiceName = serviceName;
			this.enableTestMode = enableTestMode;
			if (!this.enableTestMode)
			{
				this.hookableCreateInstance = Hookable<ExMonLogger.CreateExmonRpcInstanceId>.Create(true, delegateCreateInstanceId);
				this.hookableTraceInstance = Hookable<ExMonLogger.ExmonRpcTraceEventInstance>.Create(true, delegateTraceEventInstance);
				return;
			}
			this.hookableCreateInstance = Hookable<ExMonLogger.CreateExmonRpcInstanceId>.Create(true, () => default(DiagnosticsNativeMethods.EventInstanceInfo));
			this.hookableTraceInstance = Hookable<ExMonLogger.ExmonRpcTraceEventInstance>.Create(true, delegate(byte[] buffer, ref DiagnosticsNativeMethods.EventInstanceInfo instanceInfo, ref DiagnosticsNativeMethods.EventInstanceInfo parentInstanceInfo)
			{
				return 0U;
			});
		}

		public string ServiceName
		{
			get
			{
				return this.serviceName;
			}
			set
			{
				this.serviceName = value;
			}
		}

		public string UserName
		{
			get
			{
				return this.userName;
			}
			set
			{
				this.userName = value;
			}
		}

		public string ClientAddress
		{
			get
			{
				return this.clientAddress;
			}
		}

		internal ExMonLogger.EventInstanceHeader Header
		{
			get
			{
				return this.header;
			}
		}

		internal byte[] Buffer
		{
			get
			{
				if (this.buffer == null)
				{
					this.buffer = ExMonLogger.bufferPool.Acquire();
				}
				return this.buffer;
			}
		}

		internal bool IsTracingEnabled
		{
			get
			{
				return this.enableTestMode || ETWTrace.IsExmonRpcEnabled;
			}
		}

		internal IDisposable SetCreateInstanceHook(ExMonLogger.CreateExmonRpcInstanceId hookFunction)
		{
			return this.hookableCreateInstance.SetTestHook(hookFunction);
		}

		internal IDisposable SetTraceInstanceHook(ExMonLogger.ExmonRpcTraceEventInstance hookFunction)
		{
			return this.hookableTraceInstance.SetTestHook(hookFunction);
		}

		internal void SetClassType(byte classType)
		{
			this.header.ClassType = classType;
		}

		internal void SetTraceSize(int bufferSize)
		{
			this.bytesWrittenToBuffer = bufferSize;
			this.Clear();
		}

		internal void WriteByte(int offset, byte value)
		{
			this.Buffer[offset] = value;
			this.UpdateBytesWritten(offset, 1);
		}

		internal void WriteUShort(int offset, ushort value)
		{
			int bytesWritten = ExBitConverter.Write(value, this.Buffer, offset);
			this.UpdateBytesWritten(offset, bytesWritten);
		}

		internal void WriteUInt32(int offset, uint value)
		{
			int bytesWritten = ExBitConverter.Write(value, this.Buffer, offset);
			this.UpdateBytesWritten(offset, bytesWritten);
		}

		internal void WriteByteArray(int offset, byte[] value)
		{
			Array.Copy(value, 0, this.Buffer, offset, value.Length);
			int bytesWritten = value.Length;
			this.UpdateBytesWritten(offset, bytesWritten);
		}

		internal void WriteUInt64(int offset, ulong value)
		{
			int bytesWritten = ExBitConverter.Write(value, this.Buffer, offset);
			this.UpdateBytesWritten(offset, bytesWritten);
		}

		internal void WriteGuid(int offset, Guid value)
		{
			int bytesWritten = ExBitConverter.Write(value, this.Buffer, offset);
			this.UpdateBytesWritten(offset, bytesWritten);
		}

		internal void WriteUserAddressApplication(int offset, string user, string address, string application)
		{
			this.WriteUserAddressApplication(offset, user, address, application, null, null, null, null);
		}

		internal void WriteUserAddressApplication(int offset, string user, string address, string application, string activityId, string component, string protocol, string action)
		{
			int num = 0;
			num += ExBitConverter.Write(string.IsNullOrEmpty(user) ? "<none>" : user, 32, true, this.Buffer, offset + num);
			num += ExBitConverter.Write(string.IsNullOrEmpty(address) ? "<none>" : address, ExMonLogger.MaximumIpAddressCharacterLength, true, this.Buffer, offset + num);
			num += ExBitConverter.Write(string.IsNullOrEmpty(application) ? "<none>" : application, 32, false, this.Buffer, offset + num);
			num += ExBitConverter.Write(string.IsNullOrEmpty(activityId) ? "<none>" : activityId, 32, false, this.Buffer, offset + num);
			num += ExBitConverter.Write(string.IsNullOrEmpty(component) ? "<none>" : component, 32, false, this.Buffer, offset + num);
			num += ExBitConverter.Write(string.IsNullOrEmpty(protocol) ? "<none>" : protocol, 32, false, this.Buffer, offset + num);
			num += ExBitConverter.Write(string.IsNullOrEmpty(action) ? "<none>" : action, 32, false, this.Buffer, offset + num);
			this.UpdateBytesWritten(offset, num);
		}

		protected override DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<ExMonLogger>(this);
		}

		protected override void InternalDispose()
		{
			this.ReleaseBuffer();
			base.InternalDispose();
		}

		protected void ReleaseBuffer()
		{
			if (this.buffer != null)
			{
				ExMonLogger.bufferPool.Release(this.buffer);
				this.buffer = null;
			}
		}

		protected void UpdateBytesWritten(int offset, int bytesWritten)
		{
			if (this.bytesWrittenToBuffer < offset + bytesWritten)
			{
				this.bytesWrittenToBuffer = offset + bytesWritten;
			}
		}

		protected void GetNewInstanceId()
		{
			this.instanceInfo = this.hookableCreateInstance.Value();
		}

		protected uint Submit()
		{
			this.header.Config(this.bytesWrittenToBuffer);
			return this.hookableTraceInstance.Value(this.Buffer, ref this.instanceInfo, ref this.instanceInfo);
		}

		private void Clear()
		{
			Array.Clear(this.Buffer, 0, this.bytesWrittenToBuffer);
		}

		private const int MaximumUserNameCharacterLength = 32;

		private const int MaximumApplicationNameCharacterLength = 32;

		private const string MaximumIpAddressString = "ABCD:ABCD:ABCD:ABCD:ABCD:ABCD.XXX.XXX.XXX.XXX";

		private static readonly int MaximumIpAddressCharacterLength = "ABCD:ABCD:ABCD:ABCD:ABCD:ABCD.XXX.XXX.XXX.XXX".Length + 1;

		private static readonly BufferPool bufferPool = BufferPoolCollection.AutoCleanupCollection.Acquire(BufferPoolCollection.BufferSize.Size1K);

		private readonly bool enableTestMode;

		private readonly string clientAddress;

		private readonly Hookable<ExMonLogger.CreateExmonRpcInstanceId> hookableCreateInstance;

		private readonly Hookable<ExMonLogger.ExmonRpcTraceEventInstance> hookableTraceInstance;

		private string serviceName;

		private string userName;

		private byte[] buffer;

		private int bytesWrittenToBuffer;

		private DiagnosticsNativeMethods.EventInstanceInfo instanceInfo;

		private ExMonLogger.EventInstanceHeader header;

		internal delegate DiagnosticsNativeMethods.EventInstanceInfo CreateExmonRpcInstanceId();

		internal delegate uint ExmonRpcTraceEventInstance(byte[] buffer, ref DiagnosticsNativeMethods.EventInstanceInfo instanceInfo, ref DiagnosticsNativeMethods.EventInstanceInfo parentInstanceInfo);

		internal struct JetThreadStats
		{
			internal JetThreadStats(ExMonLogger exmonLogger, int baseOffset)
			{
				this.exmonLogger = exmonLogger;
				this.baseOffset = baseOffset;
			}

			public JET_THREADSTATS ThreadStats
			{
				set
				{
					this.exmonLogger.WriteUInt32(this.baseOffset, 32U);
					this.exmonLogger.WriteUInt32(4 + this.baseOffset, (uint)value.cPageReferenced);
					this.exmonLogger.WriteUInt32(8 + this.baseOffset, (uint)value.cPageRead);
					this.exmonLogger.WriteUInt32(12 + this.baseOffset, (uint)value.cPagePreread);
					this.exmonLogger.WriteUInt32(16 + this.baseOffset, (uint)value.cPageDirtied);
					this.exmonLogger.WriteUInt32(20 + this.baseOffset, (uint)value.cPageRedirtied);
					this.exmonLogger.WriteUInt32(24 + this.baseOffset, (uint)value.cLogRecord);
					this.exmonLogger.WriteUInt32(28 + this.baseOffset, (uint)value.cbLogRecord);
				}
			}

			internal const int StructSize = 32;

			private ExMonLogger exmonLogger;

			private int baseOffset;

			private enum Offsets
			{
				Size,
				PagesReferenced = 4,
				PagesRead = 8,
				PagesPreread = 12,
				PagesDirtied = 16,
				PagesRedirtied = 20,
				LogRecordsGenerated = 24,
				BytesLogged = 28,
				MaxOffset = 32
			}
		}

		internal struct EventInstanceHeader
		{
			internal EventInstanceHeader(ExMonLogger exmonLogger)
			{
				this.exmonLogger = exmonLogger;
			}

			internal ushort Size
			{
				set
				{
					this.exmonLogger.WriteUShort(0, value);
				}
			}

			internal ushort FieldTypeFlags
			{
				set
				{
					this.exmonLogger.WriteUShort(2, value);
				}
			}

			internal byte HeaderType
			{
				set
				{
					this.exmonLogger.WriteByte(2, value);
				}
			}

			internal byte MarkerFlags
			{
				set
				{
					this.exmonLogger.WriteByte(3, value);
				}
			}

			internal uint Version
			{
				set
				{
					this.exmonLogger.WriteUInt32(4, value);
				}
			}

			internal byte ClassType
			{
				set
				{
					this.exmonLogger.WriteByte(4, value);
				}
			}

			internal byte ClassLevel
			{
				set
				{
					this.exmonLogger.WriteByte(5, value);
				}
			}

			internal ushort ClassVersion
			{
				set
				{
					this.exmonLogger.WriteUShort(6, value);
				}
			}

			internal uint ThreadId
			{
				set
				{
					this.exmonLogger.WriteUInt32(8, value);
				}
			}

			internal uint ProcessId
			{
				set
				{
					this.exmonLogger.WriteUInt32(12, value);
				}
			}

			internal ulong TimeStamp
			{
				set
				{
					this.exmonLogger.WriteUInt64(16, value);
				}
			}

			internal ulong RegHandle
			{
				set
				{
					this.exmonLogger.WriteUInt64(24, value);
				}
			}

			internal ulong ParentRegHandle
			{
				set
				{
					this.exmonLogger.WriteUInt64(48, value);
				}
			}

			internal uint InstanceId
			{
				set
				{
					this.exmonLogger.WriteUInt32(32, value);
				}
			}

			internal uint ParentInstanceId
			{
				set
				{
					this.exmonLogger.WriteUInt32(36, value);
				}
			}

			internal ulong ProcessorTime
			{
				set
				{
					this.exmonLogger.WriteUInt64(40, value);
				}
			}

			internal uint KernelTime
			{
				set
				{
					this.exmonLogger.WriteUInt32(40, value);
				}
			}

			internal uint UserTime
			{
				set
				{
					this.exmonLogger.WriteUInt32(44, value);
				}
			}

			internal uint EventId
			{
				set
				{
					this.exmonLogger.WriteUInt32(40, value);
				}
			}

			internal uint Flags
			{
				set
				{
					this.exmonLogger.WriteUInt32(44, value);
				}
			}

			internal void Config(int bytesWrittenToBuffer)
			{
				this.Flags = 131072U;
				this.ClassVersion = 2;
				this.Size = Convert.ToUInt16(bytesWrittenToBuffer);
			}

			internal const int StructSize = 56;

			internal const int HeaderClassTypeOffset = 4;

			private ExMonLogger exmonLogger;

			private enum Offsets
			{
				HeaderSize,
				HeaderFieldTypeFlags = 2,
				HeaderHeaderType = 2,
				HeaderMarkerFlags,
				HeaderVersion,
				HeaderClassType = 4,
				HeaderClassLevel,
				HeaderClassVersion,
				HeaderThreadId = 8,
				HeaderProcessId = 12,
				HeaderTimeStamp = 16,
				HeaderRegHandle = 24,
				HeaderInstanceId = 32,
				HeaderParentInstanceId = 36,
				HeaderProcessorTime = 40,
				HeaderKernelTime = 40,
				HeaderUserTime = 44,
				HeaderEventId = 40,
				HeaderFlags = 44,
				HeaderParentRegHandle = 48,
				MaxOffset = 56
			}
		}
	}
}
