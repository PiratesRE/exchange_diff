using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Isam.Esent.Interop.Vista;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	internal class TaskExMonLogger : ExMonLogger
	{
		internal TaskExMonLogger(bool enableTestMode) : base(enableTestMode, string.Empty, string.Empty, new ExMonLogger.CreateExmonRpcInstanceId(ETWTrace.CreateExmonTaskInstanceId), new ExMonLogger.ExmonRpcTraceEventInstance(ETWTrace.ExmonTaskTraceEventInstance))
		{
			this.end = new TaskExMonLogger.TaskEnd(this);
			this.start = new TaskExMonLogger.TaskStart(this);
		}

		internal TaskExMonLogger.TaskStart Start
		{
			get
			{
				return this.start;
			}
		}

		internal TaskExMonLogger.TaskEnd End
		{
			get
			{
				return this.end;
			}
		}

		public void BeginTaskProcessing(JET_THREADSTATS threadStats)
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

		public void EndTaskProcessing(uint taskId, JET_THREADSTATS threadStats)
		{
			if (base.IsTracingEnabled)
			{
				this.end.SetStrings(base.UserName, base.ClientAddress, base.ServiceName);
				this.end.TaskId = taskId;
				this.end.ThreadStats = threadStats;
				base.Submit();
			}
			base.ReleaseBuffer();
		}

		private const byte ExMonStart = 203;

		private const byte ExMonEnd = 204;

		private TaskExMonLogger.TaskEnd end;

		private TaskExMonLogger.TaskStart start;

		internal struct TaskEnd
		{
			internal TaskEnd(TaskExMonLogger exmonLogger)
			{
				this.exmonLogger = exmonLogger;
				this.statsLogger = new ExMonLogger.JetThreadStats(exmonLogger, 92);
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

			public uint TaskId
			{
				set
				{
					this.exmonLogger.WriteUInt32(56, value);
				}
			}

			public void Clear()
			{
				this.exmonLogger.SetTraceSize(129);
				this.exmonLogger.SetClassType(204);
			}

			public void SetStrings(string user, string address, string application)
			{
				this.exmonLogger.WriteUserAddressApplication(124, user, address, application);
			}

			internal const int StructSize = 129;

			private TaskExMonLogger exmonLogger;

			private ExMonLogger.JetThreadStats statsLogger;

			private enum Offsets
			{
				TaskId = 56,
				MdbGuid = 60,
				MailboxGuid = 76,
				JetStats = 92,
				StringsBuffer = 124,
				MaxOffset = 129
			}
		}

		internal struct TaskStart
		{
			internal TaskStart(TaskExMonLogger exmonLogger)
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
				this.exmonLogger.SetClassType(203);
			}

			internal const int StructSize = 88;

			private TaskExMonLogger exmonLogger;

			private ExMonLogger.JetThreadStats statsLogger;

			private enum Offsets
			{
				JetStats = 56,
				MaxOffset = 88
			}
		}
	}
}
