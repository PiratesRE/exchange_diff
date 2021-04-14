using System;
using System.Collections;
using System.Threading;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Diagnostics.CmdletInfra;
using Microsoft.Exchange.Diagnostics.Components.Configuration.Core;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Configuration.Core
{
	internal class RequestMonitor
	{
		private RequestMonitor(string logFolderPath, int cacheLimitSize)
		{
			if (!RequestMonitor.Enabled.Value)
			{
				return;
			}
			this.cacheLimitSize = cacheLimitSize;
			this.logFolderPath = logFolderPath;
		}

		internal static RequestMonitor Instance
		{
			get
			{
				return RequestMonitor.instance;
			}
		}

		private bool IsThreadStopped
		{
			get
			{
				return this.workerThread == null || !this.workerThread.IsAlive;
			}
		}

		private RequestMonitorLogger Logger
		{
			get
			{
				return this.logger;
			}
		}

		internal static void InitRequestMonitor(string logFolderPath, int cacheLimitSize = 300000)
		{
			RequestMonitor.instance = new RequestMonitor(logFolderPath, cacheLimitSize);
			if (!RequestMonitor.Enabled.Value)
			{
				return;
			}
			RequestMonitor.Instance.Initialize();
		}

		internal bool TryGetCurrentRequestMonitorContext(Guid requestId, out RequestMonitorContext context)
		{
			context = RequestMonitorContext.Current;
			return context != null || this.requestContextCache.TryGetValue(requestId, out context);
		}

		internal void Log(Guid requestId, RequestMonitorMetadata key, object value)
		{
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::Log] Enter.");
			if (!RequestMonitor.Enabled.Value)
			{
				ExTraceGlobals.InstrumentationTracer.TraceDebug((long)this.GetHashCode(), "[RequestMonitor::Log] Exit. RequestMonitorEnabled=false");
				return;
			}
			RequestMonitorContext requestMonitorContext;
			if (this.TryGetCurrentRequestMonitorContext(requestId, out requestMonitorContext))
			{
				ExTraceGlobals.InstrumentationTracer.TraceDebug<RequestMonitorMetadata, object>((long)this.GetHashCode(), "[RequestMonitor::Log] Key={0}, Value={1}.", key, value);
				requestMonitorContext[key] = value;
			}
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::Log] Exit.");
		}

		internal void RegisterRequest(Guid requestId)
		{
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::RegisterRequest] Enter.");
			if (!RequestMonitor.Enabled.Value || this.IsThreadStopped)
			{
				ExTraceGlobals.InstrumentationTracer.TraceDebug<bool, bool>((long)this.GetHashCode(), "[RequestMonitor::RegisterRequest] Exit. RequestMonitorEnabled={0}, ThreadStopped={1}", RequestMonitor.Enabled.Value, this.IsThreadStopped);
				return;
			}
			RequestMonitorContext value = new RequestMonitorContext(requestId);
			this.requestContextCache.TryAddAbsolute(requestId, value, RequestMonitor.MaxRequestLifeTime.Value);
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::RegisterRequest] Exit.");
		}

		internal void UnRegisterRequest(Guid requestId)
		{
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::UnRegisterRequest] Enter.");
			if (!RequestMonitor.Enabled.Value)
			{
				return;
			}
			if (this.requestContextCache.Contains(requestId))
			{
				this.requestContextCache.Remove(requestId);
			}
			else
			{
				RequestMonitorContext requestMonitorContext = RequestMonitorContext.Current;
				if (requestMonitorContext != null)
				{
					requestMonitorContext[RequestMonitorMetadata.LoggingReason] = LoggingReason.End;
					this.Commit(RequestMonitorContext.Current);
				}
			}
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::UnRegisterRequest] Exit.");
		}

		private void Initialize()
		{
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::Initialize] Enter.");
			this.requestContextCache = new ExactTimeoutCache<Guid, RequestMonitorContext>(null, new ShouldRemoveDelegate<Guid, RequestMonitorContext>(this.OnBeforeExpire), null, this.cacheLimitSize, false);
			this.logItemQueue = Queue.Synchronized(new Queue());
			this.workerSignal = new AutoResetEvent(false);
			this.CreateWorkerThread();
			this.logger = new RequestMonitorLogger(this.logFolderPath);
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::Initialize] Exit.");
		}

		private void Commit(RequestMonitorContext context)
		{
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::Commit] Enter.");
			if (this.IsThreadStopped)
			{
				ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::Commit] Exit. Worker thread stopped.");
				return;
			}
			context[RequestMonitorMetadata.EndTime] = DateTime.UtcNow;
			this.logItemQueue.Enqueue(context);
			this.workerSignal.Set();
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::Commit] Exit.");
		}

		private bool OnBeforeExpire(Guid requestId, RequestMonitorContext context)
		{
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::OnBeforeExpire] Enter.");
			context[RequestMonitorMetadata.LoggingReason] = LoggingReason.Expired;
			ExTraceGlobals.InstrumentationTracer.TraceDebug<Guid>((long)this.GetHashCode(), "[RequestMonitor::OnBeforeExpire] Request {0} was committed.", requestId);
			this.Commit(context);
			ExTraceGlobals.InstrumentationTracer.TraceFunction((long)this.GetHashCode(), "[RequestMonitor::OnBeforeExpire] Exit.");
			return true;
		}

		private void CreateWorkerThread()
		{
			this.workerThread = new Thread(new ParameterizedThreadStart(this.WorkerProc));
			this.workerThread.IsBackground = true;
			this.workerThread.Start();
		}

		private void WorkerProc(object state)
		{
			for (;;)
			{
				try
				{
					while (this.logItemQueue.Count > 0)
					{
						RequestMonitorContext context = this.logItemQueue.Dequeue() as RequestMonitorContext;
						this.Logger.Commit(context);
					}
				}
				catch (Exception exception)
				{
					Diagnostics.ReportException(exception, LoggerSettings.EventLog, LoggerSettings.EventTuple, null, null, ExTraceGlobals.InstrumentationTracer, "Exception from RequestMonitor : {0}");
				}
				this.workerSignal.WaitOne();
			}
		}

		private static readonly BoolAppSettingsEntry Enabled = new BoolAppSettingsEntry("RequestMonitor.Enabled", false, ExTraceGlobals.InstrumentationTracer);

		private static readonly TimeSpanAppSettingsEntry MaxRequestLifeTime = new TimeSpanAppSettingsEntry("RequestMonitor.DelayTimeToLogRequestInMinutes", TimeSpanUnit.Minutes, TimeSpan.FromMinutes(1.0), ExTraceGlobals.InstrumentationTracer);

		private static RequestMonitor instance;

		private readonly int cacheLimitSize;

		private readonly string logFolderPath;

		private Queue logItemQueue;

		private AutoResetEvent workerSignal;

		private Thread workerThread;

		private RequestMonitorLogger logger;

		private ExactTimeoutCache<Guid, RequestMonitorContext> requestContextCache;
	}
}
