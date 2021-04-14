using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal class InputBuffer<T> : IDisposable where T : LogDataBatch
	{
		public InputBuffer(int batchSizeInBytes, long beginOffset, ILogFileInfo logFileInfoObj, ThreadSafeQueue<T> logDataBatchQueue, string prefix, ILogMonitorHelper<T> logMonitorHelper, int messageBatchFlushInterval, CancellationContext cancelContext, int maxBatchCount, string instanceName = null)
		{
			if (batchSizeInBytes <= 0)
			{
				throw new ArgumentOutOfRangeException("batchSizeInByte", "The batch size should be greater than 0.");
			}
			if (beginOffset < 0L)
			{
				throw new ArgumentOutOfRangeException("beginOffset", "The beginOffset should be equal or greater than 0.");
			}
			if (logDataBatchQueue == null)
			{
				throw new ArgumentNullException("logDataBatchQueue cannot be null.");
			}
			if (messageBatchFlushInterval < 0)
			{
				throw new ArgumentOutOfRangeException("messageBatchFlushInterval", "The messageBatchFlushInterval must not be a negative number.");
			}
			ArgumentValidator.ThrowIfNull("logFileInfoObj", logFileInfoObj);
			ArgumentValidator.ThrowIfNull("watermarkFileRef", logFileInfoObj.WatermarkFileObj);
			if (string.IsNullOrEmpty(logFileInfoObj.FullFileName))
			{
				throw new ArgumentException("fullLogName cannot be null or emtpy.");
			}
			ArgumentValidator.ThrowIfNull("cancelContext", cancelContext);
			this.cancellationContext = cancelContext;
			this.messageBatchFlushInterval = messageBatchFlushInterval;
			this.batchSizeInBytes = batchSizeInBytes;
			this.logDataBatchQueue = logDataBatchQueue;
			this.watermarkFileRef = logFileInfoObj.WatermarkFileObj;
			this.maximumBatchCount = ((maxBatchCount <= 0) ? int.MaxValue : maxBatchCount);
			this.lastFluchCheckTime = DateTime.UtcNow;
			this.logMonitorHelper = logMonitorHelper;
			this.fullLogName = logFileInfoObj.FullFileName;
			this.instance = (string.IsNullOrEmpty(instanceName) ? prefix : instanceName);
			this.logPrefix = prefix;
			this.perfCounterInstance = PerfCountersInstanceCache.GetInstance(this.instance);
			this.shouldBufferBatches = this.ShouldBufferBatches();
			this.CreateNewBatch(beginOffset);
			if (this.shouldBufferBatches)
			{
				MessageBatchBase messageBatchBase = this.activeBatch as MessageBatchBase;
				if (messageBatchBase != null)
				{
					messageBatchBase.MessageBatchFlushInterval = this.messageBatchFlushInterval;
				}
			}
		}

		public IWatermarkFile UnitTestGetWatermarkFileObjectReference
		{
			get
			{
				return this.watermarkFileRef;
			}
		}

		public string FullLogName
		{
			get
			{
				return this.fullLogName;
			}
		}

		public void EnqueueBatch(T batch)
		{
			if (ExTraceGlobals.ReaderTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				for (int i = 0; i < batch.LogRanges.Count; i++)
				{
					string message = string.Format("InputBuffer: enqueueBatch range {0}: ({1}, {2}) for log {3}", new object[]
					{
						i,
						batch.LogRanges[i].StartOffset,
						batch.LogRanges[i].EndOffset,
						this.fullLogName
					});
					ExTraceGlobals.ReaderTracer.TraceDebug((long)this.GetHashCode(), message);
				}
			}
			long size = batch.Size;
			long num = (long)batch.NumberOfLinesInBatch;
			ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("before enqueue; batch size (in original log) {0}, lines {1} +++++++++ ", size, num), this.instance, this.fullLogName);
			this.watermarkFileRef.InMemoryCountIncrease();
			try
			{
				this.logDataBatchQueue.Enqueue(batch, this.cancellationContext);
			}
			catch
			{
				this.watermarkFileRef.InMemoryCountDecrease();
				throw;
			}
			if (this.cancellationContext.StopToken.IsCancellationRequested)
			{
				return;
			}
			ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("after enqueue; batch size (in original log) {0}, lines {1} +++++++++ ", size, num), this.instance, this.fullLogName);
			this.perfCounterInstance.InputBufferBatchCounts.Decrement();
			this.perfCounterInstance.BatchQueueLength.Increment();
			this.perfCounterInstance.InputBufferBackfilledLines.IncrementBy(this.newBackfilledLogLines);
			this.newBackfilledLogLines = 0L;
		}

		public void AddInvalidRowToSkip(long startOffset, long endOffset)
		{
			this.activeBatch.UpdateCurrentRangeOffsets(startOffset, endOffset);
		}

		public void LineReceived(ReadOnlyRow row)
		{
			if (this.activeBatch.IsBatchFull(row))
			{
				this.FinishAndCreateNewBatch(row);
			}
			if (this.cancellationContext.StopToken.IsCancellationRequested)
			{
				return;
			}
			if (this.shouldBufferBatches)
			{
				this.MsgtrkLineReceivedHelper(row);
			}
			else
			{
				bool flag = this.activeBatch.LineReceived(row);
				if (flag)
				{
					this.activeBatch.UpdateCurrentRangeOffsets(row.Position, row.EndPosition);
				}
			}
			this.FlushMessageBatchBufferToWriter(false);
			Tools.DebugAssert(this.messageBatchBuffer.Count <= this.maximumBatchCount, "Verify the buffer size will never go above the maximumBatchCount");
		}

		public void FinishAndCreateNewBatch(ReadOnlyRow row)
		{
			this.FinishBatch();
			this.CreateNewBatch(row.Position);
			MessageBatchBase messageBatchBase = this.activeBatch as MessageBatchBase;
			if (messageBatchBase != null)
			{
				messageBatchBase.MessageBatchFlushInterval = this.messageBatchFlushInterval;
			}
		}

		public void Dispose()
		{
			if (!this.disposed)
			{
				this.disposed = true;
				GC.SuppressFinalize(this);
			}
		}

		public void BeforeDataBlockIsProcessed(LogFileRange block, bool isFirstBlock)
		{
			if (!isFirstBlock && this.activeBatch != null)
			{
				this.activeBatch.CreateNewRange(block.StartOffset);
			}
		}

		public void AfterDataBlockIsProcessed()
		{
			this.activeBatch.RemoveLastOpenRange();
		}

		internal T GetBatch()
		{
			if (this.logDataBatchQueue.Count == 0)
			{
				return default(T);
			}
			T t = this.logDataBatchQueue.Dequeue(null);
			if (t != null)
			{
				this.perfCounterInstance.BatchQueueLength.Decrement();
				return t;
			}
			return default(T);
		}

		internal void FinishBatch()
		{
			if (this.activeBatch != null)
			{
				if (this.activeBatch.RangeCount > 0)
				{
					if (this.activeBatch.EndOffset == 2147483647L)
					{
						this.activeBatch.RemoveLastRange();
					}
					else
					{
						this.activeBatch.CurrentRange.ProcessingStatus = ProcessingStatus.ReadyToWriteToDatabase;
					}
					this.activeBatch.ProcessingStatus = ProcessingStatus.ReadyToWriteToDatabase;
					this.BufferOrEnqueueToWriter();
				}
				else
				{
					this.perfCounterInstance.InputBufferBatchCounts.Decrement();
				}
				this.activeBatch = default(T);
			}
		}

		internal void FlushMessageBatchBufferToWriter(bool forceFlush)
		{
			if (!this.shouldBufferBatches)
			{
				return;
			}
			if (!LogDataBatchReflectionCache<T>.IsMessageBatch)
			{
				return;
			}
			List<T> list = this.messageBatchBuffer;
			Tools.DebugAssert(list.Count <= this.maximumBatchCount, "Verify the buffer size will never goes above the maximumBatchCount");
			if (!forceFlush && (DateTime.UtcNow - this.lastFluchCheckTime).TotalMilliseconds < 1000.0 && list.Count < this.maximumBatchCount)
			{
				return;
			}
			this.lastFluchCheckTime = DateTime.UtcNow;
			foreach (T t in list)
			{
				MessageBatchBase messageBatchBase = t as MessageBatchBase;
				Tools.DebugAssert(messageBatchBase != null, "Failed cast to MessageBatchBase.");
				if (forceFlush || messageBatchBase.ReadyToFlush(this.newestLogLineTS))
				{
					ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("FlushMessageBatchBufferToWriter: called : thread={0}, forceFlush={1}, this.shouldBufferBatches={2}, bufferCount={3}, this.maximumBatchCount={4} ?---BufEnqueBatch ", new object[]
					{
						Thread.CurrentThread.ManagedThreadId,
						forceFlush,
						this.shouldBufferBatches,
						list.Count,
						this.maximumBatchCount
					}), "", "");
					this.EnqueueBatch(t);
					messageBatchBase.Flushed = true;
				}
			}
			list.RemoveAll(delegate(T batch)
			{
				MessageBatchBase messageBatchBase3 = batch as MessageBatchBase;
				Tools.DebugAssert(messageBatchBase3 != null, "Failed cast to MessageBatchBase.");
				return messageBatchBase3.Flushed;
			});
			while (list.Count >= this.maximumBatchCount)
			{
				ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("FlushMessageBatchBufferToWriter: called : thread={0}, forceFlush={1}, this.shouldBufferBatches={2}, bufferCount={3}, this.maximumBatchCount={4} ?---BufEnqueBatch2 ", new object[]
				{
					Thread.CurrentThread.ManagedThreadId,
					forceFlush,
					this.shouldBufferBatches,
					list.Count,
					this.maximumBatchCount
				}), "", "");
				this.EnqueueBatch(list[0]);
				MessageBatchBase messageBatchBase2 = list[0] as MessageBatchBase;
				Tools.DebugAssert(messageBatchBase2 != null, "Failed cast to MessageBatchBase.");
				messageBatchBase2.Flushed = true;
				list.RemoveAt(0);
			}
		}

		private void BufferOrEnqueueToWriter()
		{
			if (!this.shouldBufferBatches)
			{
				ServiceLogger.LogDebug(ServiceLogger.Component.LogReader, LogUploaderEventLogConstants.Message.LogBatchEnqueue, string.Format("BufferOrEnqueueToWriter: called : thread={0} ?---BufEnque2 ", Thread.CurrentThread.ManagedThreadId), "", "");
				this.EnqueueBatch(this.activeBatch);
				return;
			}
			this.messageBatchBuffer.Add(this.activeBatch);
			Tools.DebugAssert(this.messageBatchBuffer.Count <= this.maximumBatchCount, "Verify the buffer size will never go above the maximumBatchCount");
		}

		private bool ShouldBufferBatches()
		{
			return DatacenterRegistry.IsForefrontForOffice() && (this.logPrefix.StartsWith("MSGTRKSPLIT", StringComparison.OrdinalIgnoreCase) || this.logPrefix.StartsWith("MSGTRACECOMBOSPLIT", StringComparison.OrdinalIgnoreCase)) && this.messageBatchFlushInterval > 0;
		}

		private bool TryBackfillBufferedMessage(ParsedReadOnlyRow parsedRow)
		{
			if (!this.shouldBufferBatches)
			{
				return false;
			}
			ReadOnlyRow unParsedRow = parsedRow.UnParsedRow;
			foreach (T t in this.messageBatchBuffer)
			{
				MessageBatchBase messageBatchBase = t as MessageBatchBase;
				Tools.DebugAssert(messageBatchBase != null, "Failed cast to MessageBatchBase.");
				if (!messageBatchBase.ReachedDalOptimizationLimit() && messageBatchBase.ContainsMessage(parsedRow))
				{
					if (messageBatchBase.LineReceived(unParsedRow))
					{
						messageBatchBase.AddRangeToBufferedBatch(unParsedRow.Position, unParsedRow.EndPosition);
					}
					this.newBackfilledLogLines += 1L;
					return true;
				}
			}
			return false;
		}

		private void MsgtrkLineReceivedHelper(ReadOnlyRow row)
		{
			bool flag = false;
			try
			{
				ParsedReadOnlyRow parsedReadOnlyRow = new ParsedReadOnlyRow(row);
				this.newestLogLineTS = parsedReadOnlyRow.GetField<DateTime>("date-time");
				if (this.TryBackfillBufferedMessage(parsedReadOnlyRow))
				{
					this.activeBatch.StartNewRangeForActiveBatch(row.EndPosition);
				}
				else
				{
					flag = this.activeBatch.LineReceived(row);
				}
			}
			catch (InvalidLogLineException exception)
			{
				this.activeBatch.LogErrorAndUpdatePerfCounter(row.Position, row.EndPosition, exception, LogUploaderEventLogConstants.Tuple_LogLineParseError, (LogUploaderEventLogConstants.Message)3221230473U, "InvalidLogLineInParse");
				flag = true;
			}
			catch (InvalidPropertyValueException exception2)
			{
				this.activeBatch.LogErrorAndUpdatePerfCounter(row.Position, row.EndPosition, exception2, LogUploaderEventLogConstants.Tuple_InvalidPropertyValueInParse, (LogUploaderEventLogConstants.Message)3221230481U, "InvalidPropertyValueInParse");
				flag = true;
			}
			catch (MissingPropertyException exception3)
			{
				this.activeBatch.LogErrorAndUpdatePerfCounter(row.Position, row.EndPosition, exception3, LogUploaderEventLogConstants.Tuple_MissingPropertyInParse, (LogUploaderEventLogConstants.Message)3221230480U, "MissingPropertyInParse");
				flag = true;
			}
			catch (InvalidCastException exception4)
			{
				this.activeBatch.LogErrorAndUpdatePerfCounter(row.Position, row.EndPosition, exception4, LogUploaderEventLogConstants.Tuple_InvalidCastInParse, (LogUploaderEventLogConstants.Message)3221230482U, "InvalidCastInParse");
				flag = true;
			}
			if (flag)
			{
				this.activeBatch.UpdateCurrentRangeOffsets(row.Position, row.EndPosition);
			}
		}

		private void CreateNewBatch(long beginOffset)
		{
			this.activeBatch = this.logMonitorHelper.CreateBatch(this.batchSizeInBytes, beginOffset, this.fullLogName, this.logPrefix);
			this.activeBatch.Instance = this.instance;
			this.perfCounterInstance.InputBufferBatchCounts.Increment();
		}

		private readonly string fullLogName;

		private readonly int batchSizeInBytes;

		private readonly int messageBatchFlushInterval;

		private readonly string instance;

		private readonly string logPrefix;

		private readonly int maximumBatchCount;

		private readonly bool shouldBufferBatches;

		private T activeBatch;

		private List<T> messageBatchBuffer = new List<T>();

		private ThreadSafeQueue<T> logDataBatchQueue;

		private ILogMonitorHelper<T> logMonitorHelper;

		private DateTime newestLogLineTS = DateTime.MinValue;

		private bool disposed;

		private CancellationContext cancellationContext;

		private DateTime lastFluchCheckTime;

		private ILogUploaderPerformanceCounters perfCounterInstance;

		private long newBackfilledLogLines;

		private IWatermarkFile watermarkFileRef;
	}
}
