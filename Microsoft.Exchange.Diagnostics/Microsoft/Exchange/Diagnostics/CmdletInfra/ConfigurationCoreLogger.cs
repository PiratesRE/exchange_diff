using System;
using System.Collections;
using System.IO;
using System.Threading;
using System.Web;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Diagnostics.WorkloadManagement;
using Microsoft.Exchange.Diagnostics.WorkloadManagement.Implementation;

namespace Microsoft.Exchange.Diagnostics.CmdletInfra
{
	internal abstract class ConfigurationCoreLogger<T> : RequestDetailsLoggerBase<T> where T : ConfigurationCoreLogger<T>, new()
	{
		protected static string DefaultLogFolderPath
		{
			get
			{
				string result;
				try
				{
					string text;
					if ((text = ConfigurationCoreLogger<T>.defaultLogFolderPath) == null)
					{
						text = (ConfigurationCoreLogger<T>.defaultLogFolderPath = Path.Combine(ExchangeSetupContext.InstallPath, "Logging", "CmdletInfra"));
					}
					result = text;
				}
				catch (SetupVersionInformationCorruptException)
				{
					result = "C:\\Program Files\\Microsoft\\Exchange Server\\V15\\CmdletInfra";
				}
				return result;
			}
		}

		internal static void FlushQueuedFileWrites()
		{
			ConfigurationCoreLogger<T>.workerSignal.Set();
		}

		internal void AsyncCommit(bool forceSync)
		{
			if (!base.IsDisposed)
			{
				ExTraceGlobals.InstrumentationTracer.TraceDebug<ConfigurationCoreLogger<T>, bool>((long)this.GetHashCode(), "Dispose {0} logger. forceSync = {1}.", this, forceSync);
				ServiceCommonMetadataPublisher.PublishMetadata();
				if (!forceSync)
				{
					ConfigurationCoreLogger<T>.fileIoQueue.Enqueue(this);
					ConfigurationCoreLogger<T>.workerSignal.Set();
				}
				else
				{
					this.Dispose();
				}
				RequestDetailsLoggerBase<T>.SetCurrent(HttpContext.Current, default(T));
			}
		}

		protected override void SetPerLogLineSizeLimit()
		{
			ActivityScopeImpl.MaxAppendableColumnLength = LoggerSettings.MaxAppendableColumnLength;
			RequestDetailsLoggerBase<T>.ErrorMessageLengthThreshold = LoggerSettings.ErrorMessageLengthThreshold;
			RequestDetailsLoggerBase<T>.ProcessExceptionMessage = LoggerSettings.ProcessExceptionMessage;
		}

		protected override bool LogFullException(Exception ex)
		{
			return true;
		}

		protected override void InitializeLogger()
		{
			ActivityContext.RegisterMetadata(typeof(ConfigurationCoreMetadata));
			ActivityContext.RegisterMetadata(typeof(ServiceCommonMetadata));
			ActivityContext.RegisterMetadata(typeof(ServiceLatencyMetadata));
			ThreadPool.QueueUserWorkItem(new WaitCallback(ConfigurationCoreLogger<T>.CommitLogLines));
			base.InitializeLogger();
		}

		private static void CommitLogLines(object state)
		{
			for (;;)
			{
				try
				{
					while (ConfigurationCoreLogger<T>.fileIoQueue.Count > 0)
					{
						ConfigurationCoreLogger<T> configurationCoreLogger = ConfigurationCoreLogger<T>.fileIoQueue.Dequeue() as ConfigurationCoreLogger<T>;
						if (configurationCoreLogger != null && !configurationCoreLogger.IsDisposed)
						{
							configurationCoreLogger.Dispose();
						}
					}
				}
				catch (Exception exception)
				{
					Diagnostics.ReportException(exception, LoggerSettings.EventLog, LoggerSettings.EventTuple, null, null, ExTraceGlobals.InstrumentationTracer, "Exception from ConfigurationCoreLogger<T>.CommitLogLines : {0}");
				}
				ConfigurationCoreLogger<T>.workerSignal.WaitOne();
			}
		}

		private static Queue fileIoQueue = Queue.Synchronized(new Queue());

		private static AutoResetEvent workerSignal = new AutoResetEvent(false);

		private static string defaultLogFolderPath;
	}
}
