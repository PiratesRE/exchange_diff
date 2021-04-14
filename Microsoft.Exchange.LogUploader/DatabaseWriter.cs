using System;
using System.Diagnostics;
using System.ServiceModel;
using System.Text;
using System.Threading;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal abstract class DatabaseWriter<T> where T : LogDataBatch
	{
		public DatabaseWriter(ThreadSafeQueue<T> queue, int id, ConfigInstance config, string instanceName)
		{
			this.InitializeDatabaseWriter(queue, id, config, instanceName);
			this.perfCounterInstance = PerfCountersInstanceCache.GetInstance(this.instance);
		}

		public bool Stopped
		{
			get
			{
				return this.stopped;
			}
		}

		public ThreadSafeQueue<T> Queue
		{
			get
			{
				return this.queue;
			}
		}

		public string Instance
		{
			get
			{
				return this.instance;
			}
		}

		public TimeSpan SleepTimeForTransientDBError
		{
			get
			{
				return this.config.SleepTimeForTransientDBError;
			}
		}

		public int RetryCount
		{
			get
			{
				return this.config.RetryCount;
			}
		}

		public int RetriesBeforeAlert
		{
			get
			{
				return this.config.RetriesBeforeAlert;
			}
		}

		public int Id
		{
			get
			{
				return this.id;
			}
		}

		public CancellationContext CancellationContext
		{
			get
			{
				return this.cancellationContext;
			}
			internal set
			{
				this.cancellationContext = value;
			}
		}

		protected bool IsStopRequested
		{
			set
			{
				this.isStopRequested = value;
			}
		}

		public void SetLogMonitorInterface(ILogManager logManager)
		{
			this.logManager = logManager;
		}

		public void DoWork(object obj)
		{
			ArgumentValidator.ThrowIfNull("obj", obj);
			if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
			{
				Thread.CurrentThread.Name = string.Format("Writer {0} for log type {1}", this.id, this.instance);
			}
			ServiceLogger.LogDebug(ServiceLogger.Component.DatabaseWriter, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("DoWork: called : thread={0}, log={1}  ?---LogWriter", Thread.CurrentThread.ManagedThreadId, this.instance), "", "");
			this.cancellationContext = (CancellationContext)obj;
			while (!this.CheckServiceStopRequest("DoWork()"))
			{
				this.DequeueAndWriteOneItem();
			}
			string message = string.Format("Writer {0} for log type {1} stopped.", this.id, this.instance);
			ExTraceGlobals.WriterTracer.TraceDebug((long)this.GetHashCode(), message);
			ServiceLogger.LogInfo(ServiceLogger.Component.DatabaseWriter, LogUploaderEventLogConstants.Message.LogMonitorRequestedStop, Thread.CurrentThread.Name, this.instance, "");
			this.stopped = true;
		}

		internal void DequeueAndWriteOneItem()
		{
			string text = string.Empty;
			IWatermarkFile watermarkFile = null;
			try
			{
				T t = this.queue.Dequeue(this.cancellationContext);
				if (t != null)
				{
					this.perfCounterInstance.BatchQueueLength.Decrement();
					text = t.FullLogName;
					watermarkFile = this.logManager.FindWatermarkFileObject(text);
					if (ExTraceGlobals.WriterTracer.IsTraceEnabled(TraceType.DebugTrace))
					{
						StringBuilder stringBuilder = new StringBuilder();
						foreach (LogFileRange logFileRange in t.LogRanges)
						{
							string value = string.Format("({0}, {1}) ", logFileRange.StartOffset, logFileRange.EndOffset);
							stringBuilder.Append(value);
						}
						string message = string.Format("For log {0}, writer {1} get batch that has {2} ranges: {3}", new object[]
						{
							text,
							this.id,
							t.LogRanges.Count,
							stringBuilder.ToString()
						});
						ExTraceGlobals.WriterTracer.TraceDebug((long)this.GetHashCode(), message);
					}
					this.WriteBatch(t);
				}
				else if (!this.CheckServiceStopRequest("DequeueAndWriteOneItem()"))
				{
					Tools.DebugAssert(this.cancellationContext.StopToken.IsCancellationRequested, "If Enqueue is not cancelled, it should always get a batch back");
				}
			}
			catch (FaultException ex)
			{
				ExTraceGlobals.WriterTracer.TraceError<string, string>((long)this.GetHashCode(), "Writer failed to write data through web service DAL when processing log {0}. Check the event log on the FFO web service role. The exception is: {1}.", text, ex.Message);
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_WebServiceWriteException, ex.Message, new object[]
				{
					text,
					ex.ToString()
				});
				ServiceLogger.LogError(ServiceLogger.Component.DatabaseWriter, (LogUploaderEventLogConstants.Message)3221489634U, ex.Message, this.instance, text);
			}
			catch (CommunicationException ex2)
			{
				ExTraceGlobals.WriterTracer.TraceError<string, string>((long)this.GetHashCode(), "Writer failed to write data through web service DAL when processing log {0}. The exception is: {1}.", text, ex2.Message);
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_WebServiceWriteException, ex2.Message, new object[]
				{
					text,
					ex2.ToString()
				});
				ServiceLogger.LogError(ServiceLogger.Component.DatabaseWriter, (LogUploaderEventLogConstants.Message)3221489633U, ex2.Message, this.instance, text);
			}
			catch (ADTopologyEndpointNotFoundException ex3)
			{
				ExTraceGlobals.WriterTracer.TraceError<string, string>((long)this.GetHashCode(), "Writer caught an ADTopologyEndpointNotFoundException when processing log {0}. Details is: {1}", text, ex3.Message);
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_ADTopologyEndpointNotFound, ex3.Message, new object[]
				{
					text,
					ex3.Message
				});
				ServiceLogger.LogError(ServiceLogger.Component.DatabaseWriter, (LogUploaderEventLogConstants.Message)3221489629U, ex3.Message, this.instance, text);
			}
			catch (Exception ex4)
			{
				if (!(ex4 is ThreadAbortException) || !this.cancellationContext.StopToken.IsCancellationRequested)
				{
					string text2 = string.Format("Writer caught an exception when processing log {0}: {1}", text, ex4);
					ExTraceGlobals.WriterTracer.TraceError((long)this.GetHashCode(), text2);
					EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_DatabaseWriterUnknownException, ex4.Message, new object[]
					{
						text,
						ex4
					});
					EventNotificationItem.Publish(ExchangeComponent.Name, "UnexpectedWriterError", null, text2, ResultSeverityLevel.Error, false);
					this.perfCounterInstance.TotalUnexpectedWriterErrors.Increment();
					ServiceLogger.LogError(ServiceLogger.Component.DatabaseWriter, (LogUploaderEventLogConstants.Message)3221489617U, text2, this.instance, text);
					throw;
				}
				Thread.ResetAbort();
				ServiceLogger.LogInfo(ServiceLogger.Component.DatabaseWriter, LogUploaderEventLogConstants.Message.LogMonitorRequestedStop, Thread.CurrentThread.Name + " received thread abort event", this.instance, "");
			}
			finally
			{
				if (watermarkFile != null)
				{
					watermarkFile.InMemoryCountDecrease();
				}
			}
		}

		internal void WriteBatch(T batch)
		{
			bool flag = false;
			Stopwatch stopwatch = Stopwatch.StartNew();
			try
			{
				ExTraceGlobals.WriterTracer.TraceDebug<string>((long)this.GetHashCode(), "Start writing batch to db for log {0}", this.instance);
				flag = (batch.NumberOfLinesInBatch == 0 || this.WriteBatchDataToDataStore(batch));
				if (flag && !this.isStopRequested)
				{
					this.UpdateWatermark(batch);
				}
			}
			finally
			{
				stopwatch.Stop();
				this.perfCounterInstance.TotalDBWrite.Increment();
				if (batch.NumberOfLinesInBatch != 0)
				{
					this.perfCounterInstance.AverageDbWriteLatencyBase.Increment();
					this.perfCounterInstance.AverageDbWriteLatency.IncrementBy(stopwatch.ElapsedTicks);
				}
				if (flag)
				{
					if (Tools.IsRawProcessingType<T>())
					{
						this.perfCounterInstance.RawWrittenBytes.IncrementBy(batch.Size);
					}
					else
					{
						this.perfCounterInstance.TotalLogBytesProcessed.IncrementBy(batch.Size);
					}
				}
			}
		}

		protected abstract bool WriteBatchDataToDataStore(T batch);

		protected void InitializeDatabaseWriter(ThreadSafeQueue<T> queue, int id, ConfigInstance config, string instanceName)
		{
			ArgumentValidator.ThrowIfNull("queue", queue);
			ArgumentValidator.ThrowIfNull("config", config);
			ArgumentValidator.ThrowIfNullOrEmpty("instanceName", instanceName);
			if (id < 0)
			{
				throw new ArgumentOutOfRangeException("id should be equal or greater than 0.");
			}
			this.queue = queue;
			this.config = config;
			this.id = id;
			this.stopped = false;
			this.instance = instanceName;
			this.isStopRequested = false;
		}

		protected bool CheckServiceStopRequest(string caller)
		{
			if (this.CancellationContext.StopToken.IsCancellationRequested)
			{
				ExTraceGlobals.WriterTracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "At {0}, received stop request in {1}", DateTime.UtcNow, caller);
				this.CancellationContext.StopWaitHandle.Set();
				this.IsStopRequested = true;
				return true;
			}
			return false;
		}

		private void UpdateWatermark(T batch)
		{
			string fullLogName = batch.FullLogName;
			IWatermarkFile watermarkFile = this.logManager.FindWatermarkFileObject(fullLogName);
			if (watermarkFile != null)
			{
				watermarkFile.WriteWatermark(batch.LogRanges);
				return;
			}
			string message = string.Format("DatabaseWriter failed to get a watermark instance for {0}", fullLogName);
			ExTraceGlobals.WriterTracer.TraceError((long)this.GetHashCode(), message);
			EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_MissingWatermark, fullLogName, new object[]
			{
				fullLogName
			});
			Tools.DebugAssert(false, "WatermarkFile.GetInstanceBasedOnLogName " + fullLogName);
		}

		private int id;

		private ThreadSafeQueue<T> queue;

		private bool stopped;

		private string instance;

		private CancellationContext cancellationContext;

		private bool isStopRequested;

		private ConfigInstance config;

		private ILogUploaderPerformanceCounters perfCounterInstance;

		private ILogManager logManager;
	}
}
