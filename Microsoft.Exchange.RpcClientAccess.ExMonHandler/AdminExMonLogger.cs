using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal class AdminExMonLogger : ExMonLogger
	{
		internal AdminExMonLogger(bool enableTestMode, string clientAddress) : base(enableTestMode, clientAddress, string.Empty, new ExMonLogger.CreateExmonRpcInstanceId(ETWTrace.CreateExmonAdminRpcInstanceId), new ExMonLogger.ExmonRpcTraceEventInstance(ETWTrace.ExmonAdminRpcTraceEventInstance))
		{
			this.end = new AdminExMonLogger.AdminRpcEnd(this);
			this.start = new AdminExMonLogger.AdminRpcStart(this);
		}

		internal AdminExMonLogger.AdminRpcStart RpcStart
		{
			get
			{
				return this.start;
			}
		}

		internal AdminExMonLogger.AdminRpcEnd RpcEnd
		{
			get
			{
				return this.end;
			}
		}

		public void BeginRpcProcessing(JET_THREADSTATS threadStats)
		{
			if (base.IsTracingEnabled)
			{
				base.GetNewInstanceId();
				this.start.Clear();
				this.start.ThreadStats = threadStats;
				base.Submit();
				this.end.Clear();
			}
		}

		public void SetMdbGuid(Guid mdbGuid)
		{
			if (base.IsTracingEnabled)
			{
				this.end.MdbGuid = mdbGuid;
			}
		}

		public void SetMailboxGuid(Guid mailboxGuid)
		{
			if (base.IsTracingEnabled)
			{
				this.end.MailboxGuid = mailboxGuid;
			}
		}

		public void EndRpcProcessing(uint methodId, JET_THREADSTATS threadStats, uint rpcResult)
		{
			if (base.IsTracingEnabled)
			{
				this.end.SetStrings(base.UserName, base.ClientAddress, base.ServiceName);
				this.end.MethodId = methodId;
				this.end.ThreadStats = threadStats;
				this.end.RpcResult = rpcResult;
				base.Submit();
			}
			base.ReleaseBuffer();
		}

		private const byte ExMonStart = 201;

		private const byte ExMonEnd = 202;

		private AdminExMonLogger.AdminRpcEnd end;

		private AdminExMonLogger.AdminRpcStart start;

		internal struct AdminRpcEnd
		{
			internal AdminRpcEnd(AdminExMonLogger exmonLogger)
			{
				this.exmonLogger = exmonLogger;
				this.statsLogger = new ExMonLogger.JetThreadStats(exmonLogger, 96);
			}

			public JET_THREADSTATS ThreadStats
			{
				set
				{
					this.statsLogger.ThreadStats = value;
				}
			}

			public Guid MdbGuid
			{
				set
				{
					this.exmonLogger.WriteGuid(60, value);
				}
			}

			public Guid MailboxGuid
			{
				set
				{
					this.exmonLogger.WriteGuid(76, value);
				}
			}

			public uint RpcResult
			{
				set
				{
					this.exmonLogger.WriteUInt32(92, value);
				}
			}

			public uint MethodId
			{
				set
				{
					this.exmonLogger.WriteUInt32(56, value);
				}
			}

			public void Clear()
			{
				this.exmonLogger.SetTraceSize(133);
				this.exmonLogger.SetClassType(202);
			}

			public void SetStrings(string user, string address, string application)
			{
				this.exmonLogger.WriteUserAddressApplication(128, user, address, application);
			}

			internal const int StructSize = 133;

			private AdminExMonLogger exmonLogger;

			private ExMonLogger.JetThreadStats statsLogger;

			private enum Offsets
			{
				MethodId = 56,
				MdbGuid = 60,
				MailboxGuid = 76,
				Ec = 92,
				JetStats = 96,
				StringsBuffer = 128,
				MaxOffset = 133
			}
		}

		internal struct AdminRpcStart
		{
			internal AdminRpcStart(AdminExMonLogger exmonLogger)
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
				this.exmonLogger.SetClassType(201);
			}

			internal const int StructSize = 88;

			private AdminExMonLogger exmonLogger;

			private ExMonLogger.JetThreadStats statsLogger;

			private enum Offsets
			{
				JetStats = 56,
				MaxOffset = 88
			}
		}
	}
}
