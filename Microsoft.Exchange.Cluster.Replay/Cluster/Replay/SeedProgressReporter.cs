using System;
using System.IO;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SeedProgressReporter : ChangePoller
	{
		public SeedProgressReporter(Guid dbGuid, string databaseName, SeederClient client, Task.TaskErrorLoggingDelegate writeError, Task.TaskWarningLoggingDelegate writeWarning, SeedProgressReporter.ProgressReportDelegate progressDelegate, SeedProgressReporter.ProgressCompletedDelegate progressCompleted, SeedProgressReporter.ProgressFailedDelegate progressFailed) : base(true)
		{
			this.m_guid = dbGuid;
			this.m_databaseName = databaseName;
			this.m_client = client;
			this.m_writeError = writeError;
			this.m_writeWarning = writeWarning;
			this.m_progressDelegate = progressDelegate;
			this.m_progressCompleted = progressCompleted;
			this.m_progressFailed = progressFailed;
		}

		private string GetFileName(string fullFilePath)
		{
			string result;
			try
			{
				result = Path.GetFileName(fullFilePath);
			}
			catch (ArgumentException arg)
			{
				ExTraceGlobals.SeederClientTracer.TraceError<ArgumentException>((long)this.GetHashCode(), "SeedProgressReporter: MonitorProgress caught exception in GetFileName : {0}", arg);
				result = fullFilePath;
			}
			return result;
		}

		public bool ErrorOccurred
		{
			get
			{
				bool result;
				lock (this)
				{
					result = (this.m_lastException != null || (this.m_status != null && this.m_status.ErrorInfo.IsFailed()));
				}
				return result;
			}
		}

		public void MonitorProgress()
		{
			ManualResetEvent firstRpcCompletedEvent;
			lock (this)
			{
				firstRpcCompletedEvent = this.m_firstRpcCompletedEvent;
			}
			if (firstRpcCompletedEvent != null)
			{
				firstRpcCompletedEvent.WaitOne();
			}
			lock (this)
			{
				if (this.ErrorOccurred)
				{
					this.HandleError();
					return;
				}
				goto IL_F8;
			}
			IL_5D:
			RpcSeederStatus rpcSeederStatus = null;
			lock (this)
			{
				if (this.m_status.State == SeederState.SeedSuccessful)
				{
					this.m_progressCompleted();
					return;
				}
				if (this.m_status.IsSeederStatusDataAvailable())
				{
					rpcSeederStatus = new RpcSeederStatus(this.m_status);
				}
			}
			if (rpcSeederStatus != null)
			{
				this.m_progressDelegate(this.GetFileName(rpcSeederStatus.FileFullPath), rpcSeederStatus.AddressForData, rpcSeederStatus.PercentComplete, rpcSeederStatus.BytesRead, rpcSeederStatus.BytesWritten, rpcSeederStatus.BytesRemaining, this.m_databaseName);
			}
			Thread.Sleep(1000);
			IL_F8:
			if (!this.ErrorOccurred)
			{
				goto IL_5D;
			}
			lock (this)
			{
				this.HandleError();
			}
		}

		protected override void PollerThread()
		{
			while (!this.m_fShutdown)
			{
				try
				{
					ExTraceGlobals.SeederClientTracer.TraceDebug((long)this.GetHashCode(), "SeedProgressReporter: PollerThread now making GetDbSeedStatus RPC.");
					RpcSeederStatus databaseSeedStatus = this.m_client.GetDatabaseSeedStatus(this.m_guid);
					lock (this)
					{
						this.m_status = databaseSeedStatus;
					}
				}
				catch (SeederServerTransientException ex)
				{
					this.m_lastException = ex;
					ExTraceGlobals.SeederClientTracer.TraceError<SeederServerTransientException>((long)this.GetHashCode(), "SeedProgressReporter: PollerThread caught exception in GetDbSeedStatus RPC: {0}", ex);
				}
				catch (SeederServerException ex2)
				{
					this.m_lastException = ex2;
					ExTraceGlobals.SeederClientTracer.TraceError<SeederServerException>((long)this.GetHashCode(), "SeedProgressReporter: PollerThread caught exception in GetDbSeedStatus RPC: {0}", ex2);
				}
				finally
				{
					lock (this)
					{
						ManualResetEvent firstRpcCompletedEvent = this.m_firstRpcCompletedEvent;
						if (this.m_firstRpcCompletedEvent != null)
						{
							this.m_firstRpcCompletedEvent = null;
							firstRpcCompletedEvent.Set();
							ExTraceGlobals.SeederClientTracer.TraceDebug((long)this.GetHashCode(), "SeedProgressReporter: Setting m_firstRpcCompletedEvent.");
						}
					}
				}
				if (this.ErrorOccurred)
				{
					ExTraceGlobals.SeederClientTracer.TraceDebug((long)this.GetHashCode(), "SeedProgressReporter: PollerThread exiting due to an error.");
					break;
				}
				if (this.m_status.State == SeederState.SeedSuccessful)
				{
					ExTraceGlobals.SeederClientTracer.TraceDebug((long)this.GetHashCode(), "SeedProgressReporter: PollerThread exiting because seeding was successful.");
					break;
				}
				if (this.m_shutdownEvent.WaitOne(1000, false))
				{
					break;
				}
			}
			if (this.m_fShutdown)
			{
				ExTraceGlobals.SeederClientTracer.TraceDebug((long)this.GetHashCode(), "SeedProgressReporter: PollerThread exiting due to shutdown.");
			}
		}

		private void HandleError()
		{
			this.m_progressFailed();
			Exception exception = this.GetException();
			string error;
			if (SeedHelper.IsPerformingFastOperationException(exception as SeederServerException, out error))
			{
				this.m_writeWarning(ReplayStrings.WarningPerformingFastOperationException(this.m_databaseName, error));
				return;
			}
			this.m_writeError(exception, ErrorCategory.InvalidOperation, null);
		}

		private Exception GetException()
		{
			if (this.m_lastException != null)
			{
				return this.m_lastException;
			}
			try
			{
				SeederRpcExceptionWrapper.Instance.ClientRethrowIfFailed(this.m_databaseName, this.m_client.ServerName, this.m_status.ErrorInfo);
			}
			catch (SeederServerException result)
			{
				return result;
			}
			catch (SeederServerTransientException result2)
			{
				return result2;
			}
			DiagCore.RetailAssert(false, "No exception was thrown in GetException()!", new object[0]);
			return null;
		}

		private const int StatusQueryIntervalSecs = 1;

		private Guid m_guid;

		private string m_databaseName;

		private SeederClient m_client;

		private Task.TaskErrorLoggingDelegate m_writeError;

		private Task.TaskWarningLoggingDelegate m_writeWarning;

		private RpcSeederStatus m_status;

		private SeedProgressReporter.ProgressReportDelegate m_progressDelegate;

		private SeedProgressReporter.ProgressCompletedDelegate m_progressCompleted;

		private SeedProgressReporter.ProgressFailedDelegate m_progressFailed;

		private Exception m_lastException;

		private ManualResetEvent m_firstRpcCompletedEvent = new ManualResetEvent(false);

		public delegate void ProgressReportDelegate(string edbFileName, string addressForData, int percentComplete, long bytesRead, long bytesWritten, long bytesRemaining, string databaseName);

		public delegate void ProgressCompletedDelegate();

		public delegate void ProgressFailedDelegate();
	}
}
