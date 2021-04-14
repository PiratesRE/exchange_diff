using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Win32;

namespace Microsoft.Exchange.Server.Storage.HA
{
	internal class LastLogWriter
	{
		public LastLogWriter(Guid dbGuid, string dbName, JET_INSTANCE jetInstance, string dbFileName)
		{
			this.dbGuid = dbGuid;
			this.dbName = dbName;
			this.jetInstance = jetInstance;
			this.dbFileName = dbFileName;
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.LastLogWriterTracer;
			}
		}

		public static int ReadUpdateInterval()
		{
			IRegistryReader instance = RegistryReader.Instance;
			return instance.GetValue<int>(Registry.LocalMachine, "SYSTEM\\CurrentControlSet\\Services\\MSExchangeIS\\ParametersSystem", "Last Log Committed Update Interval in Seconds", 300);
		}

		public void Start(int updateIntervalInSec)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds((double)updateIntervalInSec);
			this.timer = new Timer(new TimerCallback(this.Callback), null, timeSpan, timeSpan);
		}

		public void Stop()
		{
			using (LockManager.Lock(this.workerLock))
			{
				if (this.timer != null)
				{
					this.timer.Dispose();
					this.timer = null;
				}
			}
		}

		internal long GetLastLog()
		{
			long result;
			using (Session session = new Session(this.jetInstance))
			{
				JET_DBID nil = JET_DBID.Nil;
				try
				{
					Api.JetOpenDatabase(session, this.dbFileName, null, out nil, OpenDatabaseGrbit.None);
					JET_DBINFOMISC jet_DBINFOMISC;
					Api.JetGetDatabaseInfo(session, nil, out jet_DBINFOMISC, JET_DbInfo.Misc);
					long num = (long)(jet_DBINFOMISC.genCommitted - 1);
					LastLogWriter.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "Last Log for db '{0}' is {1}", this.dbName, num);
					result = num;
				}
				finally
				{
					if (nil != JET_DBID.Nil)
					{
						Api.JetCloseDatabase(session, nil, CloseDatabaseGrbit.None);
					}
				}
			}
			return result;
		}

		private void Callback(object ignored)
		{
			bool flag = false;
			try
			{
				IClusterWriter instance = ClusterWriter.Instance;
				long lastLogGen = 0L;
				using (LockManager.Lock(this.workerLock))
				{
					if (this.timer == null)
					{
						LastLogWriter.Tracer.TraceError((long)this.GetHashCode(), "Callback scheduled after stop.");
						return;
					}
					if (this.workerIsBusy)
					{
						LastLogWriter.Tracer.TraceError<string>((long)this.GetHashCode(), "Callback detected hung writer for db '{0}'", this.dbName);
						Globals.LogPeriodicEvent(this.dbName, MSExchangeISEventLogConstants.Tuple_LastLogWriterHung, new object[]
						{
							this.dbName
						});
						return;
					}
					if (!instance.IsClusterRunning())
					{
						return;
					}
					lastLogGen = this.GetLastLog();
					flag = true;
					this.workerIsBusy = true;
				}
				this.UpdateLastLog(instance, lastLogGen);
			}
			finally
			{
				if (flag)
				{
					using (LockManager.Lock(this.workerLock))
					{
						this.workerIsBusy = false;
					}
				}
			}
		}

		private void UpdateLastLog(IClusterWriter writer, long lastLogGen)
		{
			Exception ex = writer.TryWriteLastLog(this.dbGuid, lastLogGen);
			if (ex != null)
			{
				LastLogWriter.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "TryWriteLastLog on db '{0}' failed: {1}", this.dbName, ex);
				Globals.LogPeriodicEvent(this.dbName, MSExchangeISEventLogConstants.Tuple_LastLogUpdateFailed, new object[]
				{
					this.dbName,
					ex.ToString()
				});
			}
		}

		private readonly Guid dbGuid;

		private readonly string dbName;

		private readonly string dbFileName;

		private JET_INSTANCE jetInstance;

		private Timer timer;

		private object workerLock = new object();

		private bool workerIsBusy;
	}
}
