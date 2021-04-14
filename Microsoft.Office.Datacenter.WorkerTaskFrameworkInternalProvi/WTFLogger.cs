using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.WorkerTaskFramework;

namespace Microsoft.Office.Datacenter.WorkerTaskFramework
{
	public sealed class WTFLogger : DisposeTrackableBase
	{
		private WTFLogger()
		{
			this.logConfiguration = new WTFLogConfiguration();
			if (this.logConfiguration.IsLoggingEnabled)
			{
				this.logSchema = new WTFLogSchema(this.logConfiguration);
				this.log = new Log(this.logConfiguration.LogPrefix, new LogHeaderFormatter(this.logSchema), this.logConfiguration.LogComponent);
				this.log.Configure(this.logConfiguration.LogPath, this.logConfiguration.MaxLogAge, this.logConfiguration.MaxLogDirectorySizeInBytes, this.logConfiguration.MaxLogFileSizeInBytes, this.logConfiguration.LogBufferSizeInBytes, this.logConfiguration.FlushIntervalInMinutes);
			}
		}

		public static WTFLogger Instance
		{
			get
			{
				if (WTFLogger.theInstance == null)
				{
					lock (WTFLogger.syncRoot)
					{
						if (WTFLogger.theInstance == null)
						{
							WTFLogger.theInstance = new WTFLogger();
						}
					}
				}
				return WTFLogger.theInstance;
			}
		}

		public void Flush()
		{
			if (this.log != null)
			{
				this.log.Flush();
			}
		}

		public void LogDebug(WTFLogComponent component, TracingContext context, string message, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			this.LogMessage(WTFLogger.LogLevel.Debug, component, context, message, methodName, sourceFilePath, sourceLineNumber);
		}

		public void LogInformation(WTFLogComponent component, TracingContext context, string message, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			this.LogMessage(WTFLogger.LogLevel.Information, component, context, message, methodName, sourceFilePath, sourceLineNumber);
		}

		public void LogException(WTFLogComponent component, TracingContext context, Exception exception, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(exception.Message);
			stringBuilder.Append("/ Stack: ");
			stringBuilder.Append(exception.StackTrace);
			this.LogMessage(WTFLogger.LogLevel.Exception, component, context, stringBuilder.ToString(), methodName, sourceFilePath, sourceLineNumber);
		}

		public void LogWarning(WTFLogComponent component, TracingContext context, string message, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			this.LogMessage(WTFLogger.LogLevel.Warning, component, context, message, methodName, sourceFilePath, sourceLineNumber);
		}

		public void LogError(WTFLogComponent component, TracingContext context, string message, [CallerMemberName] string methodName = "", [CallerFilePath] string sourceFilePath = "", [CallerLineNumber] int sourceLineNumber = 0)
		{
			this.LogMessage(WTFLogger.LogLevel.Error, component, context, message, methodName, sourceFilePath, sourceLineNumber);
		}

		internal static WTFLogContext GetWorkItemContext(WTFLogger.LogLevel logLevel, WTFLogComponent component, TracingContext context, string message, string methodName, string sourceFilePath, int sourceLineNumber)
		{
			WTFLogContext result;
			result.WorkItemInstance = string.Empty;
			result.WorkItemType = string.Empty;
			result.WorkItemDefinition = string.Empty;
			result.WorkItemCreatedBy = string.Empty;
			result.WorkItemResult = string.Empty;
			result.ComponentName = string.Empty;
			result.ProcessAndThread = string.Empty;
			result.LogLevel = string.Empty;
			result.CallerMethod = string.Empty;
			result.CallerSourceLine = string.Empty;
			result.Message = message;
			if (context != null)
			{
				WorkItem workItem = context.WorkItem;
				if (workItem != null)
				{
					result.WorkItemInstance = workItem.InstanceId.ToString();
					result.WorkItemType = workItem.GetType().Name;
					WorkDefinition definition = workItem.Definition;
					WorkItemResult result2 = workItem.Result;
					if (definition != null)
					{
						result.WorkItemDefinition = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
						{
							definition.Name,
							definition.Id
						});
						result.WorkItemCreatedBy = definition.CreatedById.ToString();
					}
					if (result2 != null)
					{
						result.WorkItemResult = result2.ResultId.ToString();
					}
				}
			}
			if (component != null)
			{
				if (string.IsNullOrEmpty(component.Name))
				{
					result.ComponentName = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
					{
						component.Category,
						component.LogTag
					});
				}
				else
				{
					result.ComponentName = component.Name;
				}
			}
			result.ProcessAndThread = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				WTFLogger.CurrentProcess.Id,
				Thread.CurrentThread.ManagedThreadId
			});
			result.LogLevel = WTFLogger.LogLevelStrings[(int)logLevel];
			result.CallerMethod = methodName;
			result.CallerSourceLine = string.Format(CultureInfo.InvariantCulture, "{0}:{1}", new object[]
			{
				Path.GetFileName(sourceFilePath),
				sourceLineNumber
			});
			return result;
		}

		internal void LogWithContext(WTFLogComponent component, WTFLogContext logContext)
		{
			if (this.logConfiguration.IsLoggingEnabled && component.IsTraceLoggingEnabled)
			{
				LogRowFormatter row = this.CreateRow(logContext);
				this.log.Append(row, 0);
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<WTFLogger>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.log != null)
			{
				this.log.Flush();
				this.log.Close();
			}
		}

		private void LogMessage(WTFLogger.LogLevel logLevel, WTFLogComponent component, TracingContext context, string message, string methodName, string sourceFilePath, int sourceLineNumber)
		{
			WTFLogContext workItemContext = WTFLogger.GetWorkItemContext(logLevel, component, context, message, methodName, sourceFilePath, sourceLineNumber);
			ExTraceGlobals.CoreTracer.TraceInformation(context.LId, (long)context.Id, workItemContext.ToString());
			if (component.IsTraceLoggingEnabled && this.logConfiguration.IsLoggingEnabled)
			{
				LogRowFormatter row = this.CreateRow(workItemContext);
				this.log.Append(row, 0);
			}
		}

		private LogRowFormatter CreateRow(WTFLogContext logContext)
		{
			LogRowFormatter logRowFormatter = new LogRowFormatter(this.logSchema, true);
			logRowFormatter[1] = logContext.WorkItemInstance;
			logRowFormatter[2] = logContext.WorkItemType;
			logRowFormatter[3] = logContext.WorkItemDefinition;
			logRowFormatter[4] = logContext.WorkItemCreatedBy;
			logRowFormatter[5] = logContext.WorkItemResult;
			logRowFormatter[6] = logContext.ComponentName;
			logRowFormatter[7] = logContext.ProcessAndThread;
			logRowFormatter[8] = logContext.LogLevel;
			logRowFormatter[9] = logContext.CallerMethod;
			logRowFormatter[10] = logContext.CallerSourceLine;
			logRowFormatter[11] = logContext.Message;
			return logRowFormatter;
		}

		private static readonly string[] LogLevelStrings = new string[]
		{
			"Debug",
			"Information",
			"Exception",
			"Warning",
			"Error"
		};

		private static readonly Process CurrentProcess = Process.GetCurrentProcess();

		private static volatile WTFLogger theInstance;

		private static object syncRoot = new object();

		private readonly WTFLogConfiguration logConfiguration;

		private readonly WTFLogSchema logSchema;

		private readonly Log log;

		public enum LogLevel
		{
			Debug,
			Information,
			Exception,
			Warning,
			Error
		}
	}
}
