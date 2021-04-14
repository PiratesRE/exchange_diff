using System;
using System.Collections.Generic;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Parser;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal class MapiExMonLogger : ExMonLogger
	{
		internal MapiExMonLogger(bool enableTestMode, int sessionId, string accessingPrincipalLegacyDN, string clientAddress, MapiVersion clientVersion, string serviceName) : base(enableTestMode, clientAddress, serviceName, new ExMonLogger.CreateExmonRpcInstanceId(ETWTrace.CreateExmonMapiRpcInstanceId), new ExMonLogger.ExmonRpcTraceEventInstance(ETWTrace.ExmonMapiRpcTraceEventInstance))
		{
			this.sessionId = sessionId;
			this.clientVersion = clientVersion;
			this.AccessedMailboxLegacyDn = accessingPrincipalLegacyDN;
			this.end = new MapiExMonLogger.MapiRpcEnd(this);
			this.start = new MapiExMonLogger.MapiRpcStart(this);
			this.operation = new MapiExMonLogger.MapiRpcOperation(this);
		}

		public string AccessedMailboxLegacyDn
		{
			set
			{
				if (!object.ReferenceEquals(this.mailboxDn, value) && this.mailboxDn != value)
				{
					this.mailboxDn = value;
					LegacyDN legacyDN;
					if (LegacyDN.TryParse(value, out legacyDN))
					{
						string text;
						string userName;
						legacyDN.GetParentLegacyDN(out text, out userName);
						base.UserName = userName;
					}
				}
			}
		}

		internal MapiExMonLogger.MapiRpcStart RpcStart
		{
			get
			{
				return this.start;
			}
		}

		internal MapiExMonLogger.MapiRpcOperation RpcOperation
		{
			get
			{
				return this.operation;
			}
		}

		internal MapiExMonLogger.MapiRpcEnd RpcEnd
		{
			get
			{
				return this.end;
			}
		}

		public void BeginRopProcessing(JET_THREADSTATS threadStats)
		{
			if (base.IsTracingEnabled)
			{
				base.GetNewInstanceId();
				this.start.Clear();
				this.start.ThreadStats = threadStats;
				this.operationNumber = 0U;
				base.Submit();
			}
		}

		public void LogInputRops(IEnumerable<RopId> rops)
		{
		}

		public void LogPrepareForRop(RopId ropId)
		{
			if (base.IsTracingEnabled)
			{
				this.operation.Clear();
			}
		}

		public void OnFid(StoreId folderId)
		{
			if (base.IsTracingEnabled)
			{
				this.operation.Fid = folderId;
			}
		}

		public void OnMid(StoreId messageId)
		{
			if (base.IsTracingEnabled)
			{
				this.operation.Mid = messageId;
			}
		}

		public void SetClientActivityInfo(string activityId, string component, string protocol, string action)
		{
			if (base.IsTracingEnabled)
			{
				this.activityid = activityId;
				this.component = component;
				this.protocol = protocol;
				this.action = action;
			}
		}

		public void LogCompletedRop(RopId ropId, ErrorCode errorCode, JET_THREADSTATS threadStats)
		{
			if (base.IsTracingEnabled)
			{
				this.operation.Rop = ropId;
				this.operation.RopNumber = this.operationNumber++;
				this.operation.ErrorCode = (uint)errorCode;
				this.operation.ThreadStats = threadStats;
				base.Submit();
			}
		}

		public void EndRopProcessing()
		{
			if (base.IsTracingEnabled)
			{
				this.end.Clear();
				this.end.ClientVersion = this.clientVersion;
				this.end.ConnectionHandle = (ulong)this.sessionId;
				this.end.SetStrings(base.UserName, base.ClientAddress, base.ServiceName, this.activityid, this.component, this.protocol, this.action);
				base.Submit();
			}
			base.ReleaseBuffer();
		}

		protected void LogCompletedRop(RopId ropId, ErrorCode errorCode)
		{
			JET_THREADSTATS threadStats = JET_THREADSTATS.Create(0, 0, 0, 0, 0, 0, 0);
			this.LogCompletedRop(ropId, errorCode, threadStats);
		}

		private void WriteMapiVersion(int offset, MapiVersion version)
		{
			ushort[] array = version.ToTriplet();
			int num = 0;
			num += ExBitConverter.Write(array[0], base.Buffer, offset);
			num += ExBitConverter.Write(array[1], base.Buffer, offset + 2);
			num += ExBitConverter.Write(array[2], base.Buffer, offset + 4);
			base.UpdateBytesWritten(offset, num);
		}

		private const byte ExMonStart = 199;

		private const byte ExMonEnd = 200;

		private readonly int sessionId;

		private readonly MapiVersion clientVersion;

		private uint operationNumber;

		private MapiExMonLogger.MapiRpcEnd end;

		private MapiExMonLogger.MapiRpcOperation operation;

		private MapiExMonLogger.MapiRpcStart start;

		private string mailboxDn;

		private string activityid;

		private string component;

		private string protocol;

		private string action;

		internal struct MapiRpcEnd
		{
			internal MapiRpcEnd(MapiExMonLogger exmonLogger)
			{
				this.exmonLogger = exmonLogger;
			}

			public ulong ConnectionHandle
			{
				set
				{
					this.exmonLogger.WriteUInt64(56, value);
				}
			}

			public uint ErrorCode
			{
				set
				{
					this.exmonLogger.WriteUInt32(64, value);
				}
			}

			public uint BytesIn
			{
				set
				{
					this.exmonLogger.WriteUInt32(68, value);
				}
			}

			public uint BytesOut
			{
				set
				{
					this.exmonLogger.WriteUInt32(72, value);
				}
			}

			public ulong PLogon
			{
				set
				{
					this.exmonLogger.WriteUInt64(80, value);
				}
			}

			public MapiVersion ClientVersion
			{
				set
				{
					this.exmonLogger.WriteMapiVersion(88, value);
				}
			}

			public void Clear()
			{
				this.exmonLogger.SetTraceSize(99);
				this.exmonLogger.SetClassType(200);
			}

			public void SetStrings(string user, string address, string application, string activityId, string component, string protocol, string action)
			{
				this.exmonLogger.WriteUserAddressApplication(94, user, address, application, activityId, component, protocol, action);
			}

			internal const int StructSize = 99;

			private MapiExMonLogger exmonLogger;

			private enum Offsets
			{
				ConnectionHandle = 56,
				Ec = 64,
				BytesIn = 68,
				BytesOut = 72,
				LogonPtr = 80,
				ClientVersion = 88,
				StringsBuffer = 94,
				MaxOffset = 99
			}
		}

		internal struct MapiRpcOperation
		{
			internal MapiRpcOperation(MapiExMonLogger exmonLogger)
			{
				this.exmonLogger = exmonLogger;
				this.statsLogger = new ExMonLogger.JetThreadStats(exmonLogger, 88);
			}

			public uint RopNumber
			{
				set
				{
					this.exmonLogger.WriteUInt32(56, value);
				}
			}

			public uint ErrorCode
			{
				set
				{
					this.exmonLogger.WriteUInt32(60, value);
				}
			}

			public uint Hsot
			{
				set
				{
					this.exmonLogger.WriteUInt32(64, value);
				}
			}

			public uint Flags
			{
				set
				{
					this.exmonLogger.WriteUInt32(68, value);
				}
			}

			public StoreId Fid
			{
				set
				{
					this.exmonLogger.WriteUInt64(72, value);
				}
			}

			public StoreId Mid
			{
				set
				{
					this.exmonLogger.WriteUInt64(80, value);
				}
			}

			public RopId Rop
			{
				set
				{
					this.exmonLogger.WriteByte(4, (byte)value);
				}
			}

			public JET_THREADSTATS ThreadStats
			{
				set
				{
					this.statsLogger.ThreadStats = value;
				}
			}

			public void Clear()
			{
				this.exmonLogger.SetTraceSize(120);
			}

			internal const int StructSize = 120;

			private MapiExMonLogger exmonLogger;

			private ExMonLogger.JetThreadStats statsLogger;

			private enum Offsets
			{
				RopNumber = 56,
				Ec = 60,
				Hsot = 64,
				Flags = 68,
				Fid = 72,
				Mid = 80,
				JetStats = 88,
				MaxOffset = 120
			}
		}

		internal struct MapiRpcStart
		{
			internal MapiRpcStart(MapiExMonLogger exmonLogger)
			{
				this.exmonLogger = exmonLogger;
				this.statsLogger = new ExMonLogger.JetThreadStats(exmonLogger, 56);
			}

			public JET_THREADSTATS ThreadStats
			{
				set
				{
					this.statsLogger.ThreadStats = value;
				}
			}

			public void Clear()
			{
				this.exmonLogger.SetTraceSize(88);
				this.exmonLogger.SetClassType(199);
			}

			internal const int StructSize = 88;

			private MapiExMonLogger exmonLogger;

			private ExMonLogger.JetThreadStats statsLogger;

			private enum Offsets
			{
				JetStats = 56,
				MaxOffset = 88
			}
		}
	}
}
