using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal class WatermarkFile : IWatermarkFile, IDisposable
	{
		public WatermarkFile(string logFileName, string watermarkFileName, string instanceName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("logFileName", logFileName);
			ArgumentValidator.ThrowIfNullOrEmpty("instance", instanceName);
			this.logFileFullName = logFileName;
			this.instance = instanceName;
			this.watermarkFullFileName = watermarkFileName;
			this.blocksNeedReprocessing = new List<LogFileRange>();
			this.blocksProcessed = new SortedList<long, LogFileRange>();
			this.blocksProcessedLock = new object();
			this.OpenFileAndReadWatermark();
			this.FindUnprocessedHoles();
		}

		public List<LogFileRange> BlocksNeedProcessing
		{
			get
			{
				return this.blocksNeedReprocessing;
			}
		}

		public long ProcessedSize
		{
			get
			{
				return Interlocked.Read(ref this.processedBytes);
			}
		}

		public string WatermarkFileFullName
		{
			get
			{
				return this.watermarkFullFileName;
			}
		}

		public SortedList<long, LogFileRange> BlocksProcessed
		{
			get
			{
				return this.blocksProcessed;
			}
		}

		public string LogFileFullName
		{
			get
			{
				return this.logFileFullName;
			}
		}

		public bool IsDisposed
		{
			get
			{
				return this.disposed > 0;
			}
		}

		public void UpdateLastReaderParsedEndOffset(long newEndOffset)
		{
			if (this.lastReaderProcessedEndOffset < newEndOffset)
			{
				this.lastReaderProcessedEndOffset = newEndOffset;
			}
		}

		public bool ReaderHasBytesToParse()
		{
			long logFileSize = this.GetLogFileSize();
			if (logFileSize == 0L)
			{
				return false;
			}
			if (this.lastReaderProcessedEndOffset < this.lastCheckedEndOffsertBeforeHoles)
			{
				ServiceLogger.LogCommon(ServiceLogger.LogLevel.Error, "lastReaderProcessedEndOffset", string.Format("this.lastReaderProcessedEndOffset {0}, lastCheckedEndOffsertBeforeHoles {1}", this.lastReaderProcessedEndOffset, this.lastCheckedEndOffsertBeforeHoles), ServiceLogger.Component.WatermarkFile, this.instance, this.logFileFullName);
				Tools.DebugAssert(false, string.Format("this.lastReaderProcessedEndOffset {0}, lastCheckedEndOffsertBeforeHoles {1}", this.lastReaderProcessedEndOffset, this.lastCheckedEndOffsertBeforeHoles));
			}
			if (this.blocksNeedReprocessing.Count == 0 && this.lastCheckedEndOffsertBeforeHoles < this.lastReaderProcessedEndOffset)
			{
				if (this.inMemoryBatchCount == 0)
				{
					this.FindUnprocessedHoles();
				}
				else
				{
					ServiceLogger.LogCommon(ServiceLogger.LogLevel.Debug, "MemoryBatchCountNotZero", this.inMemoryBatchCount.ToString(), ServiceLogger.Component.WatermarkFile, this.instance, this.logFileFullName);
				}
			}
			return this.blocksNeedReprocessing.Count > 0 || logFileSize > this.lastReaderProcessedEndOffset;
		}

		public void InMemoryCountIncrease()
		{
			Interlocked.Increment(ref this.inMemoryBatchCount);
		}

		public void InMemoryCountDecrease()
		{
			Interlocked.Decrement(ref this.inMemoryBatchCount);
			Tools.DebugAssert(this.inMemoryBatchCount >= 0, "this.inMemoryBatchCount");
		}

		public LogFileRange GetBlockToReprocess()
		{
			if (this.blocksNeedReprocessing.Count > 0)
			{
				LogFileRange result = this.blocksNeedReprocessing[0];
				this.blocksNeedReprocessing.RemoveAt(0);
				return result;
			}
			return null;
		}

		public LogFileRange GetNewBlockToProcess()
		{
			long length = new FileInfo(this.logFileFullName).Length;
			if (length > this.lastReaderProcessedEndOffset)
			{
				this.TraceBlockNeedProcessing(this.lastReaderProcessedEndOffset, long.MaxValue);
				return new LogFileRange(this.lastReaderProcessedEndOffset, long.MaxValue, ProcessingStatus.NeedProcessing);
			}
			return null;
		}

		public void WriteWatermark(List<LogFileRange> ranges)
		{
			ArgumentValidator.ThrowIfNull("ranges", ranges);
			if (this.disposed > 0)
			{
				ServiceLogger.LogError(ServiceLogger.Component.WatermarkFile, (LogUploaderEventLogConstants.Message)3221231496U, string.Empty, this.instance, this.logFileFullName);
				return;
			}
			lock (this.watermarkFileLock)
			{
				if (this.disposed > 0)
				{
					ServiceLogger.LogError(ServiceLogger.Component.WatermarkFile, (LogUploaderEventLogConstants.Message)3221231496U, string.Empty, this.instance, this.logFileFullName);
					return;
				}
				if (this.streamWriter == null)
				{
					FileStream stream = File.Open(this.watermarkFullFileName, FileMode.Append, FileAccess.Write, FileShare.Read);
					this.streamWriter = new StreamWriter(stream);
				}
				string arg = DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss");
				foreach (LogFileRange logFileRange in ranges)
				{
					this.streamWriter.WriteLine("{0},{1},{2}", logFileRange.StartOffset, logFileRange.EndOffset, arg);
					this.UpdateBlocksProcessed(logFileRange.StartOffset, logFileRange.EndOffset);
				}
				this.streamWriter.Flush();
			}
			if (ExTraceGlobals.WriterTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				DateTime utcNow = DateTime.UtcNow;
				string message = string.Format("Watermark update time is {0} for log {1}", utcNow, this.watermarkFullFileName);
				ExTraceGlobals.WriterTracer.TraceDebug((long)this.GetHashCode(), message);
			}
		}

		public void Dispose()
		{
			if (Interlocked.CompareExchange(ref this.disposed, 1, 0) > 0)
			{
				return;
			}
			if (this.streamWriter != null)
			{
				lock (this.watermarkFileLock)
				{
					if (this.streamWriter != null)
					{
						this.streamWriter.Close();
						this.streamWriter = null;
					}
				}
			}
		}

		public bool IsLogCompleted()
		{
			bool flag = this.ReaderHasBytesToParse();
			ServiceLogger.LogCommon(ServiceLogger.LogLevel.Debug, "LogIsNotCompleted", string.Format("{0}, {1}, {2}, {3} ", new object[]
			{
				this.blocksNeedReprocessing.Count,
				this.lastCheckedEndOffsertBeforeHoles,
				this.lastReaderProcessedEndOffset,
				this.inMemoryBatchCount
			}), ServiceLogger.Component.WatermarkFile, this.instance, this.logFileFullName);
			return !flag && this.lastCheckedEndOffsertBeforeHoles >= this.GetLogFileSize();
		}

		public void CreateDoneFile()
		{
			string path = Path.ChangeExtension(this.watermarkFullFileName, "done");
			try
			{
				File.Open(path, FileMode.OpenOrCreate).Close();
			}
			catch (IOException ex)
			{
				if (!ex.Message.Contains("There is not enough space on the disk."))
				{
					throw;
				}
				ServiceLogger.LogError(ServiceLogger.Component.WatermarkFile, (LogUploaderEventLogConstants.Message)3221231487U, ex.Message, this.instance, this.logFileFullName);
			}
		}

		internal static string GetFileRangeFromWatermark(string line)
		{
			int num = line.LastIndexOf(",");
			int num2 = line.IndexOf(",");
			if (num == num2)
			{
				return line;
			}
			return line.Substring(0, num);
		}

		internal LogFileRange ProcessOneWatermark(string line)
		{
			ArgumentValidator.ThrowIfNull("line", line);
			try
			{
				string fileRangeFromWatermark = WatermarkFile.GetFileRangeFromWatermark(line);
				LogFileRange logFileRange = LogFileRange.Parse(fileRangeFromWatermark);
				logFileRange.ProcessingStatus = ProcessingStatus.CompletedProcessing;
				lock (this.blocksProcessedLock)
				{
					if (!this.blocksProcessed.ContainsKey(logFileRange.StartOffset))
					{
						this.AddRangeToProcessed(logFileRange);
						return logFileRange;
					}
					EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_OverlappingLogRangeInWatermarkFile, this.WatermarkFileFullName, new object[]
					{
						this.WatermarkFileFullName,
						logFileRange.StartOffset,
						logFileRange.EndOffset,
						logFileRange.StartOffset,
						this.blocksProcessed[logFileRange.StartOffset].EndOffset
					});
					string text = string.Format("There are overlapping log ranges in watermark file {0}: ({1}, {2}), ({3}, {4}).", new object[]
					{
						this.WatermarkFileFullName,
						logFileRange.StartOffset,
						logFileRange.EndOffset,
						logFileRange.StartOffset,
						this.blocksProcessed[logFileRange.StartOffset].EndOffset
					});
					if (Interlocked.CompareExchange(ref WatermarkFile.overlappingWatermarksInWatermarkFile, 1, 0) == 0)
					{
						EventNotificationItem.Publish(ExchangeComponent.Name, "OverlappingWatermarkRecordsInFile", null, text, ResultSeverityLevel.Error, false);
					}
					ServiceLogger.LogError(ServiceLogger.Component.WatermarkFile, (LogUploaderEventLogConstants.Message)3221231476U, text, this.instance, this.WatermarkFileFullName);
				}
			}
			catch (MalformedLogRangeLineException ex)
			{
				string text2 = string.Format("Failed to parse watermark from {0}: {1}", this.watermarkFullFileName, ex.Message);
				ExTraceGlobals.ReaderTracer.TraceError((long)this.GetHashCode(), text2);
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_WatermarkFileParseException, this.watermarkFullFileName, new object[]
				{
					this.watermarkFullFileName,
					ex.Message
				});
				if (Interlocked.CompareExchange(ref WatermarkFile.watermarkParseError, 1, 0) == 0)
				{
					EventNotificationItem.Publish(ExchangeComponent.Name, "MalformedWatermarkRecordError", null, text2, ResultSeverityLevel.Warning, false);
				}
				ServiceLogger.LogError(ServiceLogger.Component.WatermarkFile, (LogUploaderEventLogConstants.Message)3221231475U, ex.Message, this.instance, this.watermarkFullFileName);
			}
			return null;
		}

		internal void FindUnprocessedHoles()
		{
			if (ServiceLogger.ServiceLogLevel == ServiceLogger.LogLevel.Debug)
			{
				string message = string.Format("FindblocksNeedProcessing found {0} blocks processed for {1}", this.blocksProcessed.Count, this.LogFileFullName);
				ExTraceGlobals.ReaderTracer.TraceDebug((long)this.GetHashCode(), message);
			}
			Tools.DebugAssert(this.blocksNeedReprocessing.Count == 0, "this.blocksNeedProcessing.Count == 0");
			bool foundHole = false;
			Action<long, long> action = delegate(long s, long e)
			{
				foundHole = true;
				LogFileRange logFileRange2 = new LogFileRange(s, e, ProcessingStatus.NeedProcessing);
				this.TraceBlockNeedProcessing(logFileRange2.StartOffset, logFileRange2.EndOffset);
				this.blocksNeedReprocessing.Add(logFileRange2);
			};
			long num = 0L;
			lock (this.blocksProcessedLock)
			{
				int count = this.blocksProcessed.Count;
				this.lastCheckedEndOffsertBeforeHoles = 0L;
				for (int i = 0; i < count; i++)
				{
					LogFileRange logFileRange = this.blocksProcessed.Values[i];
					if (logFileRange.EndOffset < logFileRange.StartOffset)
					{
						ServiceLogger.LogCommon(ServiceLogger.LogLevel.Error, "Invalid watermark range", string.Format("{0},{1}", logFileRange.StartOffset, logFileRange.EndOffset), ServiceLogger.Component.WatermarkFile, "", "");
						Tools.DebugAssert(false, string.Format("detected invalid range {0},{1}", logFileRange.StartOffset, logFileRange.EndOffset));
					}
					else
					{
						if (num < logFileRange.StartOffset)
						{
							action(num, logFileRange.StartOffset);
						}
						num = Math.Max(num, logFileRange.EndOffset);
						if (!foundHole)
						{
							this.lastCheckedEndOffsertBeforeHoles = num;
						}
					}
				}
			}
			if (this.lastReaderProcessedEndOffset > num)
			{
				action(num, this.lastReaderProcessedEndOffset);
			}
		}

		internal void UpdateBlocksProcessed(long startOffset, long endOffset)
		{
			LogFileRange logFileRange = new LogFileRange(startOffset, endOffset, ProcessingStatus.CompletedProcessing);
			lock (this.blocksProcessedLock)
			{
				if (this.blocksProcessed.ContainsKey(startOffset))
				{
					string text;
					if (this.blocksProcessed[startOffset].EndOffset == endOffset)
					{
						text = string.Format("Tried to add an existing block ({0}, {1}) when updating in-memory watermarks for log {2}.", startOffset, endOffset, this.logFileFullName);
						ExTraceGlobals.WriterTracer.TraceError((long)this.GetHashCode(), text);
						EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_WatermarkFileDuplicateBlock, this.logFileFullName, new object[]
						{
							startOffset.ToString(),
							endOffset.ToString(),
							this.logFileFullName
						});
						ServiceLogger.LogError(ServiceLogger.Component.WatermarkFile, (LogUploaderEventLogConstants.Message)3221231474U, string.Format("startOffset={0};endOffset={1}", startOffset, endOffset), this.instance, this.logFileFullName);
					}
					else
					{
						text = string.Format("Tried to add an block ({0}, {1}) that overlaps with an existing block ({2}, {3}) in the in-memory watermarks for log {4}.", new object[]
						{
							startOffset,
							endOffset,
							startOffset,
							this.blocksProcessed[startOffset].EndOffset,
							this.logFileFullName
						});
						ExTraceGlobals.WriterTracer.TraceError((long)this.GetHashCode(), text);
						EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_WatermarkFileOverlappingBlock, null, new object[]
						{
							startOffset,
							endOffset,
							startOffset,
							this.blocksProcessed[startOffset].EndOffset,
							this.logFileFullName
						});
						ServiceLogger.LogError(ServiceLogger.Component.WatermarkFile, (LogUploaderEventLogConstants.Message)3221231479U, string.Format("startOffset={0};endOffset={1}", startOffset, endOffset), this.instance, this.logFileFullName);
					}
					if (Interlocked.CompareExchange(ref WatermarkFile.overlappingWatermarksInMemory, 1, 0) == 0)
					{
						EventNotificationItem.Publish(ExchangeComponent.Name, "OverlappingWatermarkRecordsInMemory", null, text, ResultSeverityLevel.Error, false);
					}
				}
				else
				{
					this.AddRangeToProcessed(logFileRange);
				}
			}
		}

		private void OpenFileAndReadWatermark()
		{
			if (!File.Exists(this.watermarkFullFileName))
			{
				return;
			}
			using (FileStream fileStream = File.Open(this.watermarkFullFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				using (StreamReader streamReader = new StreamReader(fileStream))
				{
					string line;
					while ((line = streamReader.ReadLine()) != null)
					{
						LogFileRange logFileRange = this.ProcessOneWatermark(line);
						if (logFileRange != null)
						{
							this.lastReaderProcessedEndOffset = Math.Max(this.lastReaderProcessedEndOffset, logFileRange.EndOffset);
						}
					}
				}
			}
		}

		private void TraceBlockNeedProcessing(long startOffset, long endOffset)
		{
			if (ServiceLogger.ServiceLogLevel == ServiceLogger.LogLevel.Debug)
			{
				string message = string.Format("FindblocksNeedProcessing for {0} add ({1}, {2})", Path.GetFileName(this.logFileFullName), startOffset, endOffset);
				ExTraceGlobals.ReaderTracer.TraceDebug((long)this.GetHashCode(), message);
				ServiceLogger.LogCommon(ServiceLogger.LogLevel.Debug, "FindblocksNeedProcessing", string.Format("{0},{1}", startOffset, endOffset), ServiceLogger.Component.WatermarkFile, this.instance, this.LogFileFullName);
			}
		}

		private void AddRangeToProcessed(LogFileRange logFileRange)
		{
			this.blocksProcessed.Add(logFileRange.StartOffset, logFileRange);
			Interlocked.Add(ref this.processedBytes, (long)logFileRange.Size);
		}

		private long GetLogFileSize()
		{
			long result;
			try
			{
				result = new FileInfo(this.LogFileFullName).Length;
			}
			catch (FileNotFoundException ex)
			{
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_FileDeletedWhenCheckingItsCompletion, this.LogFileFullName, new object[]
				{
					this.LogFileFullName
				});
				ServiceLogger.LogError(ServiceLogger.Component.WatermarkFile, (LogUploaderEventLogConstants.Message)2147486661U, ex.Message, this.instance, this.LogFileFullName);
				result = 0L;
			}
			return result;
		}

		public const string WatermarkFileExtension = "wmk";

		public const string DoneFileExtension = "done";

		public const string LogFileExtension = "log";

		private const string DateTimerFormatter = "yyyy-MM-ddTHH\\:mm\\:ss";

		private static int overlappingWatermarksInMemory;

		private static int overlappingWatermarksInWatermarkFile;

		private static int watermarkParseError;

		private readonly string instance;

		private readonly string watermarkFullFileName;

		private readonly string logFileFullName;

		private object blocksProcessedLock;

		private List<LogFileRange> blocksNeedReprocessing;

		private SortedList<long, LogFileRange> blocksProcessed;

		private int inMemoryBatchCount;

		private long processedBytes;

		private object watermarkFileLock = new object();

		private StreamWriter streamWriter;

		private long lastReaderProcessedEndOffset;

		private long lastCheckedEndOffsertBeforeHoles;

		private int disposed;
	}
}
